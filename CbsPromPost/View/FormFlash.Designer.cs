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
        pictureBoxMain = new PictureBox();
        richTextBoxMain = new RichTextBox();
        button1 = new Button();
        button2 = new Button();
        button3 = new Button();
        comboBoxScanner = new ComboBox();
        label2 = new Label();
        button4 = new Button();
        button5 = new Button();
        button6 = new Button();
        button7 = new Button();
        labelHex = new Label();
        labelFpl = new Label();
        ((System.ComponentModel.ISupportInitialize)pictureBoxMain).BeginInit();
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
        // pictureBoxMain
        // 
        pictureBoxMain.BackColor = Color.Black;
        pictureBoxMain.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxMain.Location = new Point(12, 234);
        pictureBoxMain.Name = "pictureBoxMain";
        pictureBoxMain.Size = new Size(901, 611);
        pictureBoxMain.TabIndex = 19;
        pictureBoxMain.TabStop = false;
        // 
        // richTextBoxMain
        // 
        richTextBoxMain.BackColor = Color.White;
        richTextBoxMain.BorderStyle = BorderStyle.FixedSingle;
        richTextBoxMain.Location = new Point(919, 234);
        richTextBoxMain.Name = "richTextBoxMain";
        richTextBoxMain.ReadOnly = true;
        richTextBoxMain.Size = new Size(417, 611);
        richTextBoxMain.TabIndex = 20;
        richTextBoxMain.Text = "";
        // 
        // button1
        // 
        button1.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button1.Location = new Point(1342, 354);
        button1.Name = "button1";
        button1.Size = new Size(230, 61);
        button1.TabIndex = 21;
        button1.Text = "КОНТРОЛЛЕР";
        button1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button2.Location = new Point(1342, 421);
        button2.Name = "button2";
        button2.Size = new Size(230, 61);
        button2.TabIndex = 22;
        button2.Text = "ImpulseRC";
        button2.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        button3.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button3.Location = new Point(1342, 488);
        button3.Name = "button3";
        button3.Size = new Size(230, 61);
        button3.TabIndex = 23;
        button3.Text = "Betaflight";
        button3.UseVisualStyleBackColor = true;
        // 
        // comboBoxScanner
        // 
        comboBoxScanner.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxScanner.Font = new Font("Segoe UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
        comboBoxScanner.FormattingEnabled = true;
        comboBoxScanner.Location = new Point(1342, 300);
        comboBoxScanner.Name = "comboBoxScanner";
        comboBoxScanner.Size = new Size(230, 48);
        comboBoxScanner.TabIndex = 24;
        // 
        // label2
        // 
        label2.Font = new Font("Segoe UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
        label2.ForeColor = Color.Black;
        label2.Location = new Point(1342, 234);
        label2.Name = "label2";
        label2.Size = new Size(230, 63);
        label2.TabIndex = 25;
        label2.Text = "СКАНЕР";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // button4
        // 
        button4.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button4.Location = new Point(1342, 555);
        button4.Name = "button4";
        button4.Size = new Size(230, 61);
        button4.TabIndex = 26;
        button4.Text = "FPL";
        button4.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        button5.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button5.Location = new Point(1342, 622);
        button5.Name = "button5";
        button5.Size = new Size(230, 61);
        button5.TabIndex = 27;
        button5.Text = "HEX";
        button5.UseVisualStyleBackColor = true;
        // 
        // button6
        // 
        button6.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button6.Location = new Point(1342, 689);
        button6.Name = "button6";
        button6.Size = new Size(230, 61);
        button6.TabIndex = 28;
        button6.Text = "HEX";
        button6.UseVisualStyleBackColor = true;
        // 
        // button7
        // 
        button7.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        button7.Location = new Point(1342, 756);
        button7.Name = "button7";
        button7.Size = new Size(230, 61);
        button7.TabIndex = 29;
        button7.Text = "HEX";
        button7.UseVisualStyleBackColor = true;
        // 
        // labelHex
        // 
        labelHex.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        labelHex.ForeColor = Color.Black;
        labelHex.Location = new Point(12, 848);
        labelHex.Name = "labelHex";
        labelHex.Size = new Size(1560, 32);
        labelHex.TabIndex = 30;
        labelHex.Text = "HEX";
        labelHex.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelFpl
        // 
        labelFpl.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        labelFpl.ForeColor = Color.Black;
        labelFpl.Location = new Point(12, 880);
        labelFpl.Name = "labelFpl";
        labelFpl.Size = new Size(1560, 32);
        labelFpl.TabIndex = 31;
        labelFpl.Text = "FPL";
        labelFpl.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // FormFlash
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1584, 921);
        Controls.Add(labelFpl);
        Controls.Add(labelHex);
        Controls.Add(button7);
        Controls.Add(button6);
        Controls.Add(button5);
        Controls.Add(button4);
        Controls.Add(label2);
        Controls.Add(comboBoxScanner);
        Controls.Add(button3);
        Controls.Add(button2);
        Controls.Add(button1);
        Controls.Add(richTextBoxMain);
        Controls.Add(pictureBoxMain);
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
        ((System.ComponentModel.ISupportInitialize)pictureBoxMain).EndInit();
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
    private PictureBox pictureBoxMain;
    private RichTextBox richTextBoxMain;
    private Button button1;
    private Button button2;
    private Button button3;
    private ComboBox comboBoxScanner;
    private Label label2;
    private Button button4;
    private Button button5;
    private Button button6;
    private Button button7;
    private Label labelHex;
    private Label labelFpl;
}