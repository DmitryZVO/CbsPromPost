namespace CbsPromPost.View;

sealed partial class FormFlash
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
        label1 = new Label();
        labelCount = new Label();
        buttonFinish = new Button();
        buttonPause = new Button();
        labelTime = new Label();
        labelUser = new Label();
        labelWork = new Label();
        labelName = new Label();
        labelDroneId = new Label();
        richTextBoxMain = new RichTextBox();
        buttonReset = new Button();
        button2 = new Button();
        button3 = new Button();
        labelComScanner = new Label();
        button4 = new Button();
        button5 = new Button();
        buttonImpulseRC = new Button();
        buttonWebCam = new Button();
        labelHex = new Label();
        labelFpl = new Label();
        labelComBeta = new Label();
        progressBar1 = new ProgressBar();
        buttonMsp = new Button();
        textBoxCli = new TextBox();
        labelDfu = new Label();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        label1.ForeColor = Color.Black;
        label1.Location = new Point(1151, 143);
        label1.Name = "label1";
        label1.Size = new Size(421, 82);
        label1.TabIndex = 17;
        label1.Text = "НОРМАТИВ: 10 мин.";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelCount
        // 
        labelCount.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        labelCount.ForeColor = Color.Black;
        labelCount.Location = new Point(12, 143);
        labelCount.Name = "labelCount";
        labelCount.Size = new Size(309, 82);
        labelCount.TabIndex = 16;
        labelCount.Text = "КОЛ-ВО: 0";
        labelCount.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonFinish
        // 
        buttonFinish.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        buttonFinish.ForeColor = Color.FromArgb(64, 0, 0);
        buttonFinish.Location = new Point(1342, 75);
        buttonFinish.Name = "buttonFinish";
        buttonFinish.Size = new Size(230, 61);
        buttonFinish.TabIndex = 15;
        buttonFinish.Text = "ЗАКОНЧИТЬ";
        buttonFinish.UseVisualStyleBackColor = true;
        // 
        // buttonPause
        // 
        buttonPause.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        buttonPause.ForeColor = Color.Purple;
        buttonPause.Location = new Point(12, 75);
        buttonPause.Name = "buttonPause";
        buttonPause.Size = new Size(230, 61);
        buttonPause.TabIndex = 14;
        buttonPause.Text = "ОТДЫХ";
        buttonPause.UseVisualStyleBackColor = true;
        // 
        // labelTime
        // 
        labelTime.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point);
        labelTime.ForeColor = Color.Black;
        labelTime.Location = new Point(327, 143);
        labelTime.Name = "labelTime";
        labelTime.Size = new Size(818, 82);
        labelTime.TabIndex = 13;
        labelTime.Text = "00:00:00";
        labelTime.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelUser
        // 
        labelUser.BorderStyle = BorderStyle.FixedSingle;
        labelUser.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        labelUser.ForeColor = Color.FromArgb(0, 0, 64);
        labelUser.Location = new Point(501, 75);
        labelUser.Name = "labelUser";
        labelUser.Size = new Size(835, 61);
        labelUser.TabIndex = 12;
        labelUser.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelWork
        // 
        labelWork.Font = new Font("Segoe UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
        labelWork.ForeColor = Color.FromArgb(0, 0, 64);
        labelWork.Location = new Point(12, 9);
        labelWork.Name = "labelWork";
        labelWork.Size = new Size(1170, 63);
        labelWork.TabIndex = 11;
        labelWork.Text = "ТИП РАБОТЫ";
        labelWork.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelName
        // 
        labelName.Font = new Font("Segoe UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
        labelName.ForeColor = Color.FromArgb(64, 0, 0);
        labelName.Location = new Point(1188, 9);
        labelName.Name = "labelName";
        labelName.Size = new Size(384, 63);
        labelName.TabIndex = 10;
        labelName.Text = "ПОСТ №";
        labelName.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelDroneId
        // 
        labelDroneId.BorderStyle = BorderStyle.FixedSingle;
        labelDroneId.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        labelDroneId.ForeColor = Color.FromArgb(0, 0, 64);
        labelDroneId.Location = new Point(248, 75);
        labelDroneId.Name = "labelDroneId";
        labelDroneId.Size = new Size(247, 61);
        labelDroneId.TabIndex = 18;
        labelDroneId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // richTextBoxMain
        // 
        richTextBoxMain.BackColor = Color.White;
        richTextBoxMain.BorderStyle = BorderStyle.FixedSingle;
        richTextBoxMain.Location = new Point(12, 265);
        richTextBoxMain.Name = "richTextBoxMain";
        richTextBoxMain.ReadOnly = true;
        richTextBoxMain.Size = new Size(1324, 589);
        richTextBoxMain.TabIndex = 20;
        richTextBoxMain.Text = "";
        // 
        // buttonReset
        // 
        buttonReset.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonReset.Location = new Point(1342, 329);
        buttonReset.Name = "buttonReset";
        buttonReset.Size = new Size(230, 38);
        buttonReset.TabIndex = 21;
        buttonReset.Text = "ПЕРЕЗАГРУЗКА";
        buttonReset.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button2.Location = new Point(1342, 407);
        button2.Name = "button2";
        button2.Size = new Size(230, 38);
        button2.TabIndex = 22;
        button2.Text = "button2";
        button2.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        button3.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button3.Location = new Point(1342, 451);
        button3.Name = "button3";
        button3.Size = new Size(230, 38);
        button3.TabIndex = 23;
        button3.Text = "button3";
        button3.UseVisualStyleBackColor = true;
        // 
        // labelComScanner
        // 
        labelComScanner.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelComScanner.ForeColor = Color.Black;
        labelComScanner.Location = new Point(1342, 234);
        labelComScanner.Name = "labelComScanner";
        labelComScanner.Size = new Size(230, 28);
        labelComScanner.TabIndex = 25;
        labelComScanner.Text = "СКАНЕР";
        labelComScanner.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // button4
        // 
        button4.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button4.Location = new Point(1342, 495);
        button4.Name = "button4";
        button4.Size = new Size(230, 38);
        button4.TabIndex = 26;
        button4.Text = "button4";
        button4.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        button5.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button5.Location = new Point(1342, 539);
        button5.Name = "button5";
        button5.Size = new Size(230, 38);
        button5.TabIndex = 27;
        button5.Text = "button5";
        button5.UseVisualStyleBackColor = true;
        // 
        // buttonImpulseRC
        // 
        buttonImpulseRC.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonImpulseRC.Location = new Point(1342, 785);
        buttonImpulseRC.Name = "buttonImpulseRC";
        buttonImpulseRC.Size = new Size(230, 38);
        buttonImpulseRC.TabIndex = 28;
        buttonImpulseRC.Text = "ДРАЙВЕРА ImpulseRC";
        buttonImpulseRC.UseVisualStyleBackColor = true;
        // 
        // buttonWebCam
        // 
        buttonWebCam.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonWebCam.Location = new Point(1342, 873);
        buttonWebCam.Name = "buttonWebCam";
        buttonWebCam.Size = new Size(230, 38);
        buttonWebCam.TabIndex = 29;
        buttonWebCam.Text = "ВИДЕО";
        buttonWebCam.UseVisualStyleBackColor = true;
        // 
        // labelHex
        // 
        labelHex.BackColor = Color.Bisque;
        labelHex.BorderStyle = BorderStyle.FixedSingle;
        labelHex.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        labelHex.ForeColor = Color.Black;
        labelHex.Location = new Point(12, 234);
        labelHex.Name = "labelHex";
        labelHex.Size = new Size(718, 28);
        labelHex.TabIndex = 30;
        labelHex.Text = "HEX";
        labelHex.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelFpl
        // 
        labelFpl.BackColor = Color.MistyRose;
        labelFpl.BorderStyle = BorderStyle.FixedSingle;
        labelFpl.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelFpl.ForeColor = Color.Black;
        labelFpl.Location = new Point(736, 234);
        labelFpl.Name = "labelFpl";
        labelFpl.Size = new Size(600, 28);
        labelFpl.TabIndex = 31;
        labelFpl.Text = "FPL";
        labelFpl.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelComBeta
        // 
        labelComBeta.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelComBeta.ForeColor = Color.Black;
        labelComBeta.Location = new Point(1342, 265);
        labelComBeta.Name = "labelComBeta";
        labelComBeta.Size = new Size(230, 28);
        labelComBeta.TabIndex = 33;
        labelComBeta.Text = "ПОЛЕТНИК";
        labelComBeta.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // progressBar1
        // 
        progressBar1.Location = new Point(12, 888);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new Size(1324, 23);
        progressBar1.TabIndex = 34;
        // 
        // buttonMsp
        // 
        buttonMsp.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonMsp.Location = new Point(1342, 829);
        buttonMsp.Name = "buttonMsp";
        buttonMsp.Size = new Size(230, 38);
        buttonMsp.TabIndex = 35;
        buttonMsp.Text = "ТЕЛЕМЕТРИЯ/КОНФИГ";
        buttonMsp.UseVisualStyleBackColor = true;
        // 
        // textBoxCli
        // 
        textBoxCli.Location = new Point(12, 860);
        textBoxCli.Name = "textBoxCli";
        textBoxCli.Size = new Size(1324, 23);
        textBoxCli.TabIndex = 37;
        // 
        // labelDfu
        // 
        labelDfu.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        labelDfu.ForeColor = Color.Black;
        labelDfu.Location = new Point(1342, 295);
        labelDfu.Name = "labelDfu";
        labelDfu.Size = new Size(230, 28);
        labelDfu.TabIndex = 38;
        labelDfu.Text = "ПОЛЕТНИК DFU";
        labelDfu.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // FormFlash
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1584, 923);
        Controls.Add(labelDfu);
        Controls.Add(textBoxCli);
        Controls.Add(buttonMsp);
        Controls.Add(progressBar1);
        Controls.Add(labelComBeta);
        Controls.Add(labelFpl);
        Controls.Add(labelHex);
        Controls.Add(buttonWebCam);
        Controls.Add(buttonImpulseRC);
        Controls.Add(button5);
        Controls.Add(button4);
        Controls.Add(labelComScanner);
        Controls.Add(button3);
        Controls.Add(button2);
        Controls.Add(buttonReset);
        Controls.Add(richTextBoxMain);
        Controls.Add(labelDroneId);
        Controls.Add(label1);
        Controls.Add(labelCount);
        Controls.Add(buttonFinish);
        Controls.Add(buttonPause);
        Controls.Add(labelTime);
        Controls.Add(labelUser);
        Controls.Add(labelWork);
        Controls.Add(labelName);
        MaximizeBox = false;
        Name = "FormFlash";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "[КБ ЦБС] Пост прошивки БПЛА";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private Label labelCount;
    private Button buttonFinish;
    private Button buttonPause;
    private Label labelTime;
    private Label labelUser;
    private Label labelWork;
    private Label labelName;
    private Label labelDroneId;
    private RichTextBox richTextBoxMain;
    private Button buttonReset;
    private Button button2;
    private Button button3;
    private Label labelComScanner;
    private Button button4;
    private Button button5;
    private Button buttonImpulseRC;
    private Button buttonWebCam;
    private Label labelHex;
    private Label labelFpl;
    private Label labelComBeta;
    private ProgressBar progressBar1;
    private Button buttonMsp;
    private TextBox textBoxCli;
    private Label labelDfu;
}