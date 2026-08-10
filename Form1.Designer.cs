namespace paintMachine
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnPaint = new Button();
            lblStatus = new Label();
            cmbColors = new ComboBox();
            pnlColorPreview = new Panel();
            pbServo1 = new ProgressBar();
            pbServo2 = new ProgressBar();
            label1 = new Label();
            lblServo1 = new Label();
            lblServo2 = new Label();
            dgvHistory = new DataGridView();
            label2 = new Label();
            pnlServo1Led = new Panel();
            pnlServo2Led = new Panel();
            pnlComLed = new Panel();
            txtBoxTank1 = new TextBox();
            txtBoxTank2 = new TextBox();
            btnTankAdd1 = new Button();
            btnTankAdd2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            SuspendLayout();
            // 
            // btnPaint
            // 
            btnPaint.BackColor = Color.FromArgb(37, 99, 235);
            btnPaint.FlatStyle = FlatStyle.Flat;
            btnPaint.Font = new Font("Tahoma", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnPaint.ForeColor = Color.White;
            btnPaint.Location = new Point(929, 366);
            btnPaint.Name = "btnPaint";
            btnPaint.Size = new Size(287, 82);
            btnPaint.TabIndex = 0;
            btnPaint.Text = "Boya Yap";
            btnPaint.UseVisualStyleBackColor = false;
            btnPaint.Click += btnPaint_Click_1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 9);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(230, 34);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Bağlantı durumu:";
            // 
            // cmbColors
            // 
            cmbColors.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColors.FormattingEnabled = true;
            cmbColors.Location = new Point(929, 76);
            cmbColors.Name = "cmbColors";
            cmbColors.Size = new Size(287, 41);
            cmbColors.TabIndex = 2;
            cmbColors.SelectedIndexChanged += cmbColors_SelectedIndexChanged;
            // 
            // pnlColorPreview
            // 
            pnlColorPreview.Location = new Point(929, 208);
            pnlColorPreview.Name = "pnlColorPreview";
            pnlColorPreview.Size = new Size(287, 103);
            pnlColorPreview.TabIndex = 3;
            // 
            // pbServo1
            // 
            pbServo1.Location = new Point(12, 159);
            pbServo1.Maximum = 1000;
            pbServo1.Name = "pbServo1";
            pbServo1.Size = new Size(337, 59);
            pbServo1.Style = ProgressBarStyle.Continuous;
            pbServo1.TabIndex = 4;
            // 
            // pbServo2
            // 
            pbServo2.Location = new Point(12, 323);
            pbServo2.Maximum = 1000;
            pbServo2.Name = "pbServo2";
            pbServo2.Size = new Size(337, 59);
            pbServo2.Style = ProgressBarStyle.Continuous;
            pbServo2.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(109, 101);
            label1.Name = "label1";
            label1.Size = new Size(133, 34);
            label1.TabIndex = 6;
            label1.Text = "Hazneler:";
            // 
            // lblServo1
            // 
            lblServo1.AutoSize = true;
            lblServo1.Location = new Point(355, 172);
            lblServo1.Name = "lblServo1";
            lblServo1.Size = new Size(127, 34);
            lblServo1.TabIndex = 7;
            lblServo1.Text = "lblServo1";
            // 
            // lblServo2
            // 
            lblServo2.AutoSize = true;
            lblServo2.Location = new Point(355, 339);
            lblServo2.Name = "lblServo2";
            lblServo2.Size = new Size(127, 34);
            lblServo2.TabIndex = 8;
            lblServo2.Text = "lblServo2";
            // 
            // dgvHistory
            // 
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Location = new Point(12, 487);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.RowHeadersWidth = 51;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.Size = new Size(1268, 249);
            dgvHistory.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 450);
            label2.Name = "label2";
            label2.Size = new Size(191, 34);
            label2.TabIndex = 10;
            label2.Text = "Boya Geçmişi:";
            // 
            // pnlServo1Led
            // 
            pnlServo1Led.Location = new Point(588, 178);
            pnlServo1Led.Name = "pnlServo1Led";
            pnlServo1Led.Size = new Size(121, 28);
            pnlServo1Led.TabIndex = 11;
            // 
            // pnlServo2Led
            // 
            pnlServo2Led.Location = new Point(588, 345);
            pnlServo2Led.Name = "pnlServo2Led";
            pnlServo2Led.Size = new Size(121, 28);
            pnlServo2Led.TabIndex = 12;
            // 
            // pnlComLed
            // 
            pnlComLed.Location = new Point(248, 12);
            pnlComLed.Name = "pnlComLed";
            pnlComLed.Size = new Size(31, 31);
            pnlComLed.TabIndex = 12;
            // 
            // txtBoxTank1
            // 
            txtBoxTank1.Location = new Point(12, 224);
            txtBoxTank1.Name = "txtBoxTank1";
            txtBoxTank1.Size = new Size(166, 40);
            txtBoxTank1.TabIndex = 13;
            // 
            // txtBoxTank2
            // 
            txtBoxTank2.Location = new Point(12, 388);
            txtBoxTank2.Name = "txtBoxTank2";
            txtBoxTank2.Size = new Size(166, 40);
            txtBoxTank2.TabIndex = 14;
            // 
            // btnTankAdd1
            // 
            btnTankAdd1.BackColor = Color.FromArgb(226, 232, 240);
            btnTankAdd1.FlatStyle = FlatStyle.Flat;
            btnTankAdd1.ForeColor = Color.FromArgb(15, 23, 42);
            btnTankAdd1.Location = new Point(184, 224);
            btnTankAdd1.Name = "btnTankAdd1";
            btnTankAdd1.Size = new Size(127, 40);
            btnTankAdd1.TabIndex = 15;
            btnTankAdd1.Text = "Ekle";
            btnTankAdd1.UseVisualStyleBackColor = false;
            btnTankAdd1.Click += btnTankAdd1_Click;
            // 
            // btnTankAdd2
            // 
            btnTankAdd2.BackColor = Color.FromArgb(226, 232, 240);
            btnTankAdd2.FlatStyle = FlatStyle.Flat;
            btnTankAdd2.ForeColor = Color.FromArgb(15, 23, 42);
            btnTankAdd2.Location = new Point(184, 388);
            btnTankAdd2.Name = "btnTankAdd2";
            btnTankAdd2.Size = new Size(127, 40);
            btnTankAdd2.TabIndex = 16;
            btnTankAdd2.Text = "Ekle";
            btnTankAdd2.UseVisualStyleBackColor = false;
            btnTankAdd2.Click += btnTankAdd2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(15F, 33F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1292, 748);
            Controls.Add(btnTankAdd2);
            Controls.Add(btnTankAdd1);
            Controls.Add(txtBoxTank2);
            Controls.Add(txtBoxTank1);
            Controls.Add(pnlComLed);
            Controls.Add(pnlServo2Led);
            Controls.Add(pnlServo1Led);
            Controls.Add(label2);
            Controls.Add(dgvHistory);
            Controls.Add(lblServo2);
            Controls.Add(lblServo1);
            Controls.Add(label1);
            Controls.Add(pbServo2);
            Controls.Add(pbServo1);
            Controls.Add(pnlColorPreview);
            Controls.Add(cmbColors);
            Controls.Add(lblStatus);
            Controls.Add(btnPaint);
            Font = new Font("Tahoma", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 162);
            Margin = new Padding(6, 5, 6, 5);
            Name = "Form1";
            Text = "Boya Makinesi";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnPaint;
        private Label lblStatus;
        private ComboBox cmbColors;
        private Panel pnlColorPreview;
        private ProgressBar pbServo1;
        private ProgressBar pbServo2;
        private Label label1;
        private Label lblServo1;
        private Label lblServo2;
        private DataGridView dgvHistory;
        private Label label2;
        private Panel pnlServo1Led;
        private Panel pnlServo2Led;
        private Panel pnlComLed;
        private TextBox txtBoxTank1;
        private TextBox txtBoxTank2;
        private Button btnTankAdd1;
        private Button btnTankAdd2;
    }
}
