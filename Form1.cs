using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer; //oluşabilecek belirsizliği çözen alias tanımlaması

namespace paintMachine
{
    public partial class Form1 : Form
    {
        //==========================================
        //ALANLAR VE BAŞLATMA
        //==========================================

        //Donanım (Seri Port) ve İş Mantığı (Tank Hesaplama) servis nesnelerimiz
        private readonly SerialService _serialService = new();
        private readonly TankManager _tankManager = new();

        public Form1()
        {
            InitializeComponent();
        }

        // Form ilk açıldığında otomatik olarak tetiklenen olay (Load)
        private void Form1_Load(object sender, EventArgs e)
        {
            InitHardware();   // Arduino ile seri port bağlantısını dener
            LoadDataFromDb(); // Veritabanından reçeteleri, tank seviyelerini ve geçmiş logları yükler
        }

        //Arduino bağlantısını başlatır ve arayüzdeki COM LED'ini günceller
        private void InitHardware()
        {
            bool isConnected = _serialService.Connect("COM7");
            pnlComLed.BackColor = isConnected ? Color.LimeGreen : Color.Red; // Bağlıysa Yeşil, değilse Kırmızı yanar
        }

        //Form kapatılırken seri port bağlantısını güvenli bir şekilde sonlandırır
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serialService.Disconnect();
        }

        //==========================================
        //VERİTABANI VE ARAYÜZ GÜNCELLEME METODLARI
        //==========================================

        // Veritabanındaki tüm verileri çekip arayüz bileşenlerine (ComboBox, DataGridView, ProgressBar) bağlar
        private void LoadDataFromDb()
        {
            try
            {
                // 1. Veritabanından mevcut tank seviyelerini çekip iş modeline aktarır
                var (s1, s2) = DbHelper.GetTankLevels();
                _tankManager.SetLevels(s1, s2);

                // 2. Tank seviyelerini arayüzdeki progress bar ve label'lara yansıtır
                UpdateTankUI();

                // 3. Geçmiş boyama loglarını DataGridView tablosuna yükler
                dgvHistory.DataSource = DbHelper.GetLogs();

                // 4. Renk reçetelerini seçim kutusuna (ComboBox) bağlar
                cmbColors.DataSource = DbHelper.GetRecipes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Bağlantı Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tank seviyelerini ekran üzerindeki ProgressBar ve metin etiketlerinde günceller
        private void UpdateTankUI()
        {
            lblServo1.Text = $"{_tankManager.Servo1TankMl:F1} ml servo1:";
            lblServo2.Text = $"{_tankManager.Servo2TankMl:F1} ml servo2:";

            pbServo1.Value = (int)_tankManager.Servo1TankMl;
            pbServo2.Value = (int)_tankManager.Servo2TankMl;
        }

        // ==========================================
        // KULLANICI ETKİLEŞİMİ VE OLAYLAR (EVENTS)
        // ==========================================

        // ComboBox'tan bir renk seçildiğinde önizleme kutusunun arka plan rengini günceller
        private void cmbColors_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // C# Pattern Matching ile seçili nesnenin tip kontrolü ve cast işlemi tek adımda yapılır
            if (cmbColors.SelectedItem is ColorRecipe recipe && !string.IsNullOrEmpty(recipe.HexCode))
            {
                try { pnlColorPreview.BackColor = ColorTranslator.FromHtml(recipe.HexCode); }
                catch { pnlColorPreview.BackColor = Color.Gray; }
            }
            else
            {
                pnlColorPreview.BackColor = Color.Gray;
            }
        }

        //Boyama Başlat butonuna tıklandığında boyama sürecini yürüt
        private void btnPaint_Click_1(object sender, EventArgs e)
        {
            if (cmbColors.SelectedItem is not ColorRecipe recipe)
            {
                MessageBox.Show("Lütfen bir renk seçin!");
                return;
            }

            // Donanım bağlı değilse kullanıcıya test modunda devam edip etmeyeceği sorulur
            if (!_serialService.IsConnected)
            {
                var result = MessageBox.Show(
                    "Arduino bağlı değil! Yine de test modunda devam edilsin mi?",
                    "Donanım Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No) return;
            }

            //İşlem sürerken butonun tekrar tıklanmasını önlemek için devre dışı bırakılır
            btnPaint.Enabled = false;

            //Reçetedeki ml miktarına göre servoların basılı tutulma süreleri hesaplanır (10ml -> 1000ms)
            var (s1Ms, s2Ms) = _tankManager.CalculateDurations(recipe.Servo1Ml, recipe.Servo2Ml);

            //Arduino bağlıysa komut seri port üzerinden gönderilir
            if (_serialService.IsConnected)
            {
                if (!_serialService.SendDurations(s1Ms, s2Ms))
                    MessageBox.Show("Port üzerinden veri gönderilemedi!");
            }

            // Depodaki kalan boya seviyesi eksiltilir ve ekran güncellenir
            _tankManager.ConsumePaint(recipe.Servo1Ml, recipe.Servo2Ml);
            UpdateTankUI();

            // Veritabanına geçmiş log kaydı ve güncel tank seviyeleri yazılır
            try
            {
                DbHelper.AddLog(recipe.Id, recipe.ColorName ?? "", recipe.Servo1Ml, recipe.Servo2Ml);
                DbHelper.UpdateTankLevels(_tankManager.Servo1TankMl, _tankManager.Servo2TankMl);
                dgvHistory.DataSource = DbHelper.GetLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Kayıt Hatası: " + ex.Message);
            }

            // Ekranda servoların döküm anını gösteren LED simülasyonu başlatılır
            RunLedSimulation(s1Ms, s2Ms);
        }

        //1. Depoya boya ekleme butonunun olayı
        private void btnTankAdd1_Click(object sender, EventArgs e)
        {
            HandleTankAddition(txtBoxTank1, 1);
        }

        //2. Depoya boya ekleme butonunun olayı
        private void btnTankAdd2_Click(object sender, EventArgs e)
        {
            HandleTankAddition(txtBoxTank2, 2);
        }

        //==========================================
        //YARDIMCI VE SİMÜLASYON METODLARI
        //==========================================

        // Depolara boya ekleme işlemlerini yürüten ortak yardımcı metot
        private void HandleTankAddition(TextBox txtBox, int tankIndex)
        {
            if (double.TryParse(txtBox.Text, out double addAmount) && addAmount > 0)
            {
                double current = tankIndex == 1 ? _tankManager.Servo1TankMl : _tankManager.Servo2TankMl;
                if (current >= TankManager.MaxCapacity)
                {
                    MessageBox.Show($"{tankIndex}. Hazne zaten tamamen dolu!", "Depo Dolu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var (actualAdded, overflow) = tankIndex == 1
                    ? _tankManager.AddPaintToTank1(addAmount)
                    : _tankManager.AddPaintToTank2(addAmount);

                try
                {
                    DbHelper.UpdateTankLevels(_tankManager.Servo1TankMl, _tankManager.Servo2TankMl);
                    UpdateTankUI();
                    txtBox.Clear();

                    if (overflow)
                        MessageBox.Show($"Kapasite aşıldı! Yalnızca {actualAdded:F1} ml eklendi ve depo dolduruldu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MessageBox.Show($"{tankIndex}. Hazneye {actualAdded:F1} ml boya eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB Güncelleme Hatası: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show($"Lütfen {tankIndex}. Hazne için geçerli bir miktar girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Arduino döküm yaparken arayüzde hangi servonun aktif olduğunu gösteren LED simülasyonu
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
                var t1 = new Timer { Interval = s1Ms };
                t1.Tick += (s, a) =>
                {
                    pnlServo1Led.BackColor = Color.LightGray;
                    t1.Stop();
                    t1.Dispose();

                    if (s2Ms > 0)
                    {
                        // Arduino servolar arasında 200ms beklediği için simülasyona da 200ms aralık ekleniyor
                        var gapTimer = new Timer { Interval = 200 };
                        gapTimer.Tick += (gs, ga) =>
                        {
                            gapTimer.Stop();
                            gapTimer.Dispose();
                            StartSecondLedTimer(s2Ms);
                        };
                        gapTimer.Start();
                    }
                    else
                        btnPaint.Enabled = true;
                };
                t1.Start();
            }
            else if (s2Ms > 0)
            {
                StartSecondLedTimer(s2Ms);
            }
        }

        //2. Servonun LED simülasyonunu başlatır
        private void StartSecondLedTimer(int s2Ms)
        {
            pnlServo2Led.BackColor = Color.LimeGreen;
            var t2 = new Timer { Interval = s2Ms };
            t2.Tick += (s, a) =>
            {
                pnlServo2Led.BackColor = Color.LightGray;
                t2.Stop();
                t2.Dispose();
                btnPaint.Enabled = true; // Tüm dökümler bittiğinde boyama butonunu tekrar aktif eder
            };
            t2.Start();
        }
    }
}