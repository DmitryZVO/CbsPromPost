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
        buttonLoadBinImage = new Button();
        buttonLoadFpl = new Button();
        labelComScanner = new Label();
        buttonClearFlash = new Button();
        buttonWriteBinImage = new Button();
        buttonImpulseRC = new Button();
        buttonWebCam = new Button();
        labelHex = new Label();
        labelFpl = new Label();
        labelComBeta = new Label();
        progressBarMain = new ProgressBar();
        buttonDroneConfig = new Button();
        textBoxCli = new TextBox();
        labelDfu = new Label();
        button1 = new Button();
        button2 = new Button();
        button6 = new Button();
        button7 = new Button();
        button8 = new Button();
        button9 = new Button();
        button10 = new Button();
        button11 = new Button();
        button12 = new Button();
        button3 = new Button();
        buttonWriteFpl = new Button();
        groupBoxButtons = new GroupBox();
        groupBoxCli = new GroupBox();
        buttonFullFlash = new Button();
        groupBoxButtons.SuspendLayout();
        groupBoxCli.SuspendLayout();
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
        buttonFinish.Location = new Point(1331, 75);
        buttonFinish.Name = "buttonFinish";
        buttonFinish.Size = new Size(241, 61);
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
        labelUser.Size = new Size(824, 61);
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
        richTextBoxMain.Size = new Size(1313, 534);
        richTextBoxMain.TabIndex = 20;
        richTextBoxMain.Text = "";
        // 
        // buttonReset
        // 
        buttonReset.BackColor = Color.White;
        buttonReset.FlatStyle = FlatStyle.Flat;
        buttonReset.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        buttonReset.Location = new Point(1331, 234);
        buttonReset.Name = "buttonReset";
        buttonReset.Size = new Size(243, 28);
        buttonReset.TabIndex = 21;
        buttonReset.Text = "ПЕРЕЗАГРУЗКА";
        buttonReset.UseVisualStyleBackColor = false;
        // 
        // buttonLoadBinImage
        // 
        buttonLoadBinImage.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonLoadBinImage.Location = new Point(6, 222);
        buttonLoadBinImage.Name = "buttonLoadBinImage";
        buttonLoadBinImage.Size = new Size(230, 38);
        buttonLoadBinImage.TabIndex = 22;
        buttonLoadBinImage.Text = "СКАЧАТЬ RAW HEX";
        buttonLoadBinImage.UseVisualStyleBackColor = true;
        // 
        // buttonLoadFpl
        // 
        buttonLoadFpl.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonLoadFpl.Location = new Point(6, 266);
        buttonLoadFpl.Name = "buttonLoadFpl";
        buttonLoadFpl.Size = new Size(230, 38);
        buttonLoadFpl.TabIndex = 23;
        buttonLoadFpl.Text = "СКАЧАТЬ FPL";
        buttonLoadFpl.UseVisualStyleBackColor = true;
        // 
        // labelComScanner
        // 
        labelComScanner.BorderStyle = BorderStyle.FixedSingle;
        labelComScanner.FlatStyle = FlatStyle.Flat;
        labelComScanner.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelComScanner.ForeColor = Color.Black;
        labelComScanner.Location = new Point(1331, 265);
        labelComScanner.Name = "labelComScanner";
        labelComScanner.Size = new Size(243, 28);
        labelComScanner.TabIndex = 25;
        labelComScanner.Text = "СКАНЕР";
        labelComScanner.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonClearFlash
        // 
        buttonClearFlash.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonClearFlash.Location = new Point(6, 310);
        buttonClearFlash.Name = "buttonClearFlash";
        buttonClearFlash.Size = new Size(230, 38);
        buttonClearFlash.TabIndex = 26;
        buttonClearFlash.Text = "ОЧИСТИТЬ FLASH";
        buttonClearFlash.UseVisualStyleBackColor = true;
        // 
        // buttonWriteBinImage
        // 
        buttonWriteBinImage.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonWriteBinImage.Location = new Point(6, 354);
        buttonWriteBinImage.Name = "buttonWriteBinImage";
        buttonWriteBinImage.Size = new Size(230, 38);
        buttonWriteBinImage.TabIndex = 27;
        buttonWriteBinImage.Text = "ЗАЛИТЬ RAW HEX";
        buttonWriteBinImage.UseVisualStyleBackColor = true;
        // 
        // buttonImpulseRC
        // 
        buttonImpulseRC.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonImpulseRC.Location = new Point(6, 442);
        buttonImpulseRC.Name = "buttonImpulseRC";
        buttonImpulseRC.Size = new Size(230, 38);
        buttonImpulseRC.TabIndex = 28;
        buttonImpulseRC.Text = "ДРАЙВЕРА ImpulseRC";
        buttonImpulseRC.UseVisualStyleBackColor = true;
        // 
        // buttonWebCam
        // 
        buttonWebCam.BackColor = Color.White;
        buttonWebCam.FlatStyle = FlatStyle.Flat;
        buttonWebCam.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        buttonWebCam.Location = new Point(1333, 883);
        buttonWebCam.Name = "buttonWebCam";
        buttonWebCam.Size = new Size(241, 28);
        buttonWebCam.TabIndex = 29;
        buttonWebCam.Text = "ВИДЕО";
        buttonWebCam.UseVisualStyleBackColor = false;
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
        labelFpl.Size = new Size(589, 28);
        labelFpl.TabIndex = 31;
        labelFpl.Text = "FPL";
        labelFpl.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelComBeta
        // 
        labelComBeta.BorderStyle = BorderStyle.FixedSingle;
        labelComBeta.FlatStyle = FlatStyle.Flat;
        labelComBeta.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelComBeta.ForeColor = Color.Black;
        labelComBeta.Location = new Point(1331, 296);
        labelComBeta.Name = "labelComBeta";
        labelComBeta.Size = new Size(243, 28);
        labelComBeta.TabIndex = 33;
        labelComBeta.Text = "ПОЛЕТНИК";
        labelComBeta.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // progressBarMain
        // 
        progressBarMain.Location = new Point(6, 75);
        progressBarMain.Name = "progressBarMain";
        progressBarMain.Size = new Size(1301, 23);
        progressBarMain.TabIndex = 34;
        // 
        // buttonMsp
        // 
        buttonDroneConfig.BackColor = Color.White;
        buttonDroneConfig.FlatStyle = FlatStyle.Flat;
        buttonDroneConfig.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        buttonDroneConfig.Location = new Point(1333, 849);
        buttonDroneConfig.Name = "buttonDroneConfig";
        buttonDroneConfig.Size = new Size(241, 28);
        buttonDroneConfig.TabIndex = 35;
        buttonDroneConfig.Text = "ТЕЛЕМЕТРИЯ/КОНФИГ";
        buttonDroneConfig.UseVisualStyleBackColor = false;
        // 
        // textBoxCli
        // 
        textBoxCli.Location = new Point(6, 47);
        textBoxCli.Name = "textBoxCli";
        textBoxCli.Size = new Size(1301, 23);
        textBoxCli.TabIndex = 37;
        // 
        // labelDfu
        // 
        labelDfu.BorderStyle = BorderStyle.FixedSingle;
        labelDfu.FlatStyle = FlatStyle.Flat;
        labelDfu.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        labelDfu.ForeColor = Color.Black;
        labelDfu.Location = new Point(1331, 327);
        labelDfu.Name = "labelDfu";
        labelDfu.Size = new Size(243, 28);
        labelDfu.TabIndex = 38;
        labelDfu.Text = "ПОЛЕТНИК DFU";
        labelDfu.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // button1
        // 
        button1.BackColor = Color.Honeydew;
        button1.Location = new Point(96, 16);
        button1.Name = "button1";
        button1.Size = new Size(39, 25);
        button1.TabIndex = 39;
        button1.Text = "#";
        button1.UseVisualStyleBackColor = false;
        button1.Click += Button1_Click;
        // 
        // button2
        // 
        button2.Location = new Point(141, 16);
        button2.Name = "button2";
        button2.Size = new Size(59, 25);
        button2.TabIndex = 40;
        button2.Text = "help";
        button2.UseVisualStyleBackColor = true;
        button2.Click += Button2_Click;
        // 
        // button6
        // 
        button6.Location = new Point(206, 16);
        button6.Name = "button6";
        button6.Size = new Size(59, 25);
        button6.TabIndex = 41;
        button6.Text = "version";
        button6.UseVisualStyleBackColor = true;
        button6.Click += Button6_Click;
        // 
        // button7
        // 
        button7.BackColor = Color.MistyRose;
        button7.Location = new Point(1262, 16);
        button7.Name = "button7";
        button7.Size = new Size(45, 25);
        button7.TabIndex = 42;
        button7.Text = "exit";
        button7.UseVisualStyleBackColor = false;
        button7.Click += Button7_Click;
        // 
        // button8
        // 
        button8.Location = new Point(267, 16);
        button8.Name = "button8";
        button8.Size = new Size(59, 25);
        button8.TabIndex = 43;
        button8.Text = "status";
        button8.UseVisualStyleBackColor = true;
        button8.Click += Button8_Click;
        // 
        // button9
        // 
        button9.Location = new Point(332, 16);
        button9.Name = "button9";
        button9.Size = new Size(59, 25);
        button9.TabIndex = 44;
        button9.Text = "bl";
        button9.UseVisualStyleBackColor = true;
        button9.Click += Button9_Click;
        // 
        // button10
        // 
        button10.Location = new Point(397, 16);
        button10.Name = "button10";
        button10.Size = new Size(59, 25);
        button10.TabIndex = 45;
        button10.Text = "dump";
        button10.UseVisualStyleBackColor = true;
        button10.Click += Button10_Click;
        // 
        // button11
        // 
        button11.Location = new Point(1197, 16);
        button11.Name = "button11";
        button11.Size = new Size(59, 25);
        button11.TabIndex = 46;
        button11.Text = "save";
        button11.UseVisualStyleBackColor = true;
        button11.Click += Button11_Click;
        // 
        // button12
        // 
        button12.BackColor = Color.White;
        button12.Location = new Point(6, 16);
        button12.Name = "button12";
        button12.Size = new Size(84, 25);
        button12.TabIndex = 47;
        button12.Text = "ОЧИСТИТЬ";
        button12.UseVisualStyleBackColor = false;
        button12.Click += Button12_Click;
        // 
        // button3
        // 
        button3.Location = new Point(462, 16);
        button3.Name = "button3";
        button3.Size = new Size(70, 25);
        button3.TabIndex = 48;
        button3.Text = "get name";
        button3.UseVisualStyleBackColor = true;
        button3.Click += Button3_Click;
        // 
        // buttonWriteFpl
        // 
        buttonWriteFpl.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonWriteFpl.Location = new Point(6, 398);
        buttonWriteFpl.Name = "buttonWriteFpl";
        buttonWriteFpl.Size = new Size(230, 38);
        buttonWriteFpl.TabIndex = 49;
        buttonWriteFpl.Text = "ЗАЛИТЬ FPL";
        buttonWriteFpl.UseVisualStyleBackColor = true;
        // 
        // groupBoxButtons
        // 
        groupBoxButtons.Controls.Add(buttonFullFlash);
        groupBoxButtons.Controls.Add(buttonWriteFpl);
        groupBoxButtons.Controls.Add(buttonLoadBinImage);
        groupBoxButtons.Controls.Add(buttonLoadFpl);
        groupBoxButtons.Controls.Add(buttonClearFlash);
        groupBoxButtons.Controls.Add(buttonWriteBinImage);
        groupBoxButtons.Controls.Add(buttonImpulseRC);
        groupBoxButtons.Location = new Point(1331, 355);
        groupBoxButtons.Name = "groupBoxButtons";
        groupBoxButtons.Size = new Size(243, 486);
        groupBoxButtons.TabIndex = 50;
        groupBoxButtons.TabStop = false;
        // 
        // groupBoxCli
        // 
        groupBoxCli.Controls.Add(button12);
        groupBoxCli.Controls.Add(progressBarMain);
        groupBoxCli.Controls.Add(textBoxCli);
        groupBoxCli.Controls.Add(button3);
        groupBoxCli.Controls.Add(button1);
        groupBoxCli.Controls.Add(button2);
        groupBoxCli.Controls.Add(button6);
        groupBoxCli.Controls.Add(button11);
        groupBoxCli.Controls.Add(button7);
        groupBoxCli.Controls.Add(button10);
        groupBoxCli.Controls.Add(button8);
        groupBoxCli.Controls.Add(button9);
        groupBoxCli.Location = new Point(12, 805);
        groupBoxCli.Name = "groupBoxCli";
        groupBoxCli.Size = new Size(1313, 106);
        groupBoxCli.TabIndex = 51;
        groupBoxCli.TabStop = false;
        // 
        // buttonFullFlash
        // 
        buttonFullFlash.BackColor = Color.FromArgb(192, 255, 192);
        buttonFullFlash.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        buttonFullFlash.Location = new Point(7, 17);
        buttonFullFlash.Name = "buttonFullFlash";
        buttonFullFlash.Size = new Size(230, 38);
        buttonFullFlash.TabIndex = 51;
        buttonFullFlash.Text = "СТАНДАРТИЗИРОВАТЬ";
        buttonFullFlash.UseVisualStyleBackColor = false;
        // 
        // FormFlash
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1584, 923);
        Controls.Add(groupBoxCli);
        Controls.Add(buttonReset);
        Controls.Add(groupBoxButtons);
        Controls.Add(labelComScanner);
        Controls.Add(labelComBeta);
        Controls.Add(labelDfu);
        Controls.Add(buttonWebCam);
        Controls.Add(buttonDroneConfig);
        Controls.Add(labelFpl);
        Controls.Add(labelHex);
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
        groupBoxButtons.ResumeLayout(false);
        groupBoxCli.ResumeLayout(false);
        groupBoxCli.PerformLayout();
        ResumeLayout(false);
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
    private Button buttonLoadBinImage;
    private Button buttonLoadFpl;
    private Label labelComScanner;
    private Button buttonClearFlash;
    private Button buttonWriteBinImage;
    private Button buttonImpulseRC;
    private Button buttonWebCam;
    private Label labelHex;
    private Label labelFpl;
    private Label labelComBeta;
    private ProgressBar progressBarMain;
    private Button buttonDroneConfig;
    private TextBox textBoxCli;
    private Label labelDfu;
    private Button button1;
    private Button button2;
    private Button button6;
    private Button button7;
    private Button button8;
    private Button button9;
    private Button button10;
    private Button button11;
    private Button button12;
    private Button button3;
    private Button buttonWriteFpl;
    private GroupBox groupBoxButtons;
    private GroupBox groupBoxCli;
    private Button buttonFullFlash;
}