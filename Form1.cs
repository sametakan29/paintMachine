using System;
using System.Drawing;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace paintMachine
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;

        // Depo Kapasiteleri (ml)
        private double _servo1TankMl = 1000.0;
        private double _servo2TankMl = 1000.0;

        // 1 ms başına dökülen miktar (1000 ms = 10 ml -> 0.01 ml/ms)
        private const double MlPerMs = 0.01;
        private const double MsPerMl = 50.0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetLeds();
            InitSerialPort();

            // 🎯 KRİTİK DÜZELTME: ProgressBar sınırlarını DB'den veri yüklemeden ÖNCE ayarlıyoruz!
            pbServo1.Minimum = 0; pbServo1.Maximum = 1000;
            pbServo2.Minimum = 0; pbServo2.Maximum = 1000;

            cmbColors.SelectedIndexChanged += cmbColors_SelectedIndexChanged;

            // Artık DB yüklendiğinde ProgressBar Maximum değeri 1000 olduğu için hata vermeyecek
            LoadDataFromDb();

            if (cmbColors.Items.Count > 0)
            {
                cmbColors.SelectedIndex = 0;
                cmbColors_SelectedIndexChanged(cmbColors, EventArgs.Empty);
            }
        }

        // 🔌 Arka Plan Seri Port Bağlantısı (Tek ve Timeout Ayarlı Metot)
        private void InitSerialPort()
        {
            try
            {
                // Doğrudan COM7 portunu hedefliyoruz
                _serialPort = new SerialPort("COM7", 9600);
                _serialPort.WriteTimeout = 1000;
                _serialPort.ReadTimeout = 1000;

                _serialPort.Open();

                // COM7 başarıyla açıldı -> Yeşil LED
                pnlComLed.BackColor = Color.LimeGreen;
            }
            catch
            {
                // COM7 takılı değilse veya erişilemiyorsa (Örn: Arduino sökükse) -> Kırmızı LED
                pnlComLed.BackColor = Color.Red;
            }
        }

        private void ResetLeds()
        {
            pnlServo1Led.BackColor = Color.LightGray;
            pnlServo2Led.BackColor = Color.LightGray;
        }

        // 🗄️ Veritabanından Renk ve Geçmiş Yükleme
        private void LoadDataFromDb()
        {
            try
            {
                var recipes = DbHelper.GetRecipes();
                cmbColors.DataSource = recipes;

                dgvHistory.DataSource = DbHelper.GetLogs();

                // 🎯 DB'den Depo Miktarlarını Yüklüyoruz
                var (s1, s2) = DbHelper.GetTankLevels();
                _servo1TankMl = s1;
                _servo2TankMl = s2;
                UpdateTankUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Bağlantı Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbColors_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbColors.SelectedItem is ColorRecipe selectedRecipe)
            {
                if (!string.IsNullOrEmpty(selectedRecipe.HexCode))
                {
                    try
                    {
                        pnlColorPreview.BackColor = ColorTranslator.FromHtml(selectedRecipe.HexCode!);
                    }
                    catch
                    {
                        pnlColorPreview.BackColor = Color.Gray;
                    }
                }
                else
                {
                    pnlColorPreview.BackColor = Color.Gray;
                }
            }
        }

        // 🛢️ Depo Miktarlarını Arayüze Yansıtma
        private void UpdateTankUI()
        {
            lblServo1.Text = $"Servo 1: {_servo1TankMl:F1} ml";
            lblServo2.Text = $"Servo 2: {_servo2TankMl:F1} ml";

            // Tank kapasitelerini (0 - Maximum) arasında güvenli tutuyoruz
            int max1 = pbServo1.Maximum > 0 ? pbServo1.Maximum : 1000;
            int max2 = pbServo2.Maximum > 0 ? pbServo2.Maximum : 1000;

            pbServo1.Value = Math.Max(0, Math.Min(max1, (int)_servo1TankMl));
            pbServo2.Value = Math.Max(0, Math.Min(max2, (int)_servo2TankMl));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        // 🚀 Boya Yap Buton Tıklaması
        private void btnPaint_Click_1(object sender, EventArgs e)
        {
            if (!(cmbColors.SelectedItem is ColorRecipe recipe))
            {
                MessageBox.Show("Lütfen bir renk seçin!");
                return;
            }

            bool isSerialPortReady = _serialPort != null && _serialPort.IsOpen;

            if (!isSerialPortReady)
            {
                DialogResult result = MessageBox.Show(
                    "Arduino bağlı değil! Yine de test modunda devam edilsin mi?",
                    "Donanım Uyarısı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No) return;
            }

            btnPaint.Enabled = false;

            int servo1DurationMs = (int)(recipe.Servo1Ml * MsPerMl);
            int servo2DurationMs = (int)(recipe.Servo2Ml * MsPerMl);

            // Seri port üzerinden haberleşme (Timeout korumalı)
            if (isSerialPortReady)
            {
                try
                {
                    _serialPort!.WriteLine($"{servo1DurationMs},{servo2DurationMs}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Port Hatası: " + ex.Message);
                }
            }

            // Depodan Düşme ve UI Güncelleme
            _servo1TankMl = Math.Max(0, _servo1TankMl - recipe.Servo1Ml);
            _servo2TankMl = Math.Max(0, _servo2TankMl - recipe.Servo2Ml);
            UpdateTankUI();

            // PostgreSQL Kayıt ve Depo Güncelleme
            try
            {
                // 1. Log kaydını ekle
                DbHelper.AddLog(recipe.Id, recipe.ColorName ?? "", recipe.Servo1Ml, recipe.Servo2Ml);

                // 2. Depodaki yeni seviyeyi PostgreSQL'e kaydet
                DbHelper.UpdateTankLevels(_servo1TankMl, _servo2TankMl);

                // 3. Geçmiş tablosunu yenile
                dgvHistory.DataSource = DbHelper.GetLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Kayıt Hatası: " + ex.Message);
            }

            // Arayüzü dondurmayan Timer Simülasyonu
            RunLedSimulation(servo1DurationMs, servo2DurationMs);
        }

        // ⏱️ Donmasız LED Animasyonu
        private void RunLedSimulation(int s1Ms, int s2Ms)
        {
            if (s1Ms <= 0 && s2Ms <= 0)
            {
                btnPaint.Enabled = true;
                return;
            }

            if (s1Ms > 0)
            {
                pnlServo1Led.BackColor = Color.LimeGreen;
                var timer1 = new System.Windows.Forms.Timer { Interval = s1Ms };
                timer1.Tick += (s, args) =>
                {
                    pnlServo1Led.BackColor = Color.LightGray;
                    timer1.Stop();
                    timer1.Dispose();

                    if (s2Ms > 0)
                    {
                        pnlServo2Led.BackColor = Color.LimeGreen;
                        var timer2 = new System.Windows.Forms.Timer { Interval = s2Ms };
                        timer2.Tick += (s2, args2) =>
                        {
                            pnlServo2Led.BackColor = Color.LightGray;
                            timer2.Stop();
                            timer2.Dispose();
                            btnPaint.Enabled = true;
                        };
                        timer2.Start();
                    }
                    else
                    {
                        btnPaint.Enabled = true;
                    }
                };
                timer1.Start();
            }
            else if (s2Ms > 0)
            {
                pnlServo2Led.BackColor = Color.LimeGreen;
                var timer2 = new System.Windows.Forms.Timer { Interval = s2Ms };
                timer2.Tick += (s2, args2) =>
                {
                    pnlServo2Led.BackColor = Color.LightGray;
                    timer2.Stop();
                    timer2.Dispose();
                    btnPaint.Enabled = true;
                };
                timer2.Start();
            }
        }

        // 🛢️ 1. HAZNEYE BOYA EKLEME BUTONU
        // 🛢️ 1. HAZNEYE BOYA EKLEME BUTONU
        private void btnTankAdd1_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtBoxTank1.Text, out double addAmount) && addAmount > 0)
            {
                // 1. Depo zaten tamamen dolu mu?
                if (_servo1TankMl >= 1000.0)
                {
                    MessageBox.Show("1. Hazne zaten tamamen dolu (1000 ml)!", "Depo Dolu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Ne kadar boş yerimiz var?
                double availableSpace = 1000.0 - _servo1TankMl;

                // Eklenen miktarı boş yere göre belirle
                double actualAdded = Math.Min(addAmount, availableSpace);

                _servo1TankMl += actualAdded;

                try
                {
                    // Veritabanını ve Arayüzü Güncelle
                    DbHelper.UpdateTankLevels(_servo1TankMl, _servo2TankMl);
                    UpdateTankUI();
                    txtBoxTank1.Clear();

                    // Kullanıcıyı kapasite aşımı durumuna göre bilgilendir
                    if (addAmount > availableSpace)
                    {
                        MessageBox.Show(
                            $"Haznede yalnızca {availableSpace:F1} ml boş yer vardı.\nTaşma olmaması için hazneye {actualAdded:F1} ml eklendi ve depo dolduruldu (1000 ml).",
                            "Kapasite Sınırı Aşıldı",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                    else
                    {
                        MessageBox.Show($"1. Hazneye {actualAdded:F1} ml boya eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Depo güncellenirken DB hatası: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen 1. Hazne için geçerli ve pozitif bir sayı girin!", "Geçersiz Miktar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 🛢️ 2. HAZNEYE BOYA EKLEME BUTONU
        private void btnTankAdd2_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtBoxTank2.Text, out double addAmount) && addAmount > 0)
            {
                // 1. Depo zaten tamamen dolu mu?
                if (_servo2TankMl >= 1000.0)
                {
                    MessageBox.Show("2. Hazne zaten tamamen dolu (1000 ml)!", "Depo Dolu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Ne kadar boş yerimiz var?
                double availableSpace = 1000.0 - _servo2TankMl;

                // Eklenen miktarı boş yere göre belirle
                double actualAdded = Math.Min(addAmount, availableSpace);

                _servo2TankMl += actualAdded;

                try
                {
                    // Veritabanını ve Arayüzü Güncelle
                    DbHelper.UpdateTankLevels(_servo1TankMl, _servo2TankMl);
                    UpdateTankUI();
                    txtBoxTank2.Clear();

                    // Kullanıcıyı kapasite aşımı durumuna göre bilgilendir
                    if (addAmount > availableSpace)
                    {
                        MessageBox.Show(
                            $"Haznede yalnızca {availableSpace:F1} ml boş yer vardı.\nTaşma olmaması için hazneye {actualAdded:F1} ml eklendi ve depo dolduruldu (1000 ml).",
                            "Kapasite Sınırı Aşıldı",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                    else
                    {
                        MessageBox.Show($"2. Hazneye {actualAdded:F1} ml boya eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Depo güncellenirken DB hatası: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen 2. Hazne için geçerli ve pozitif bir sayı girin!", "Geçersiz Miktar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}