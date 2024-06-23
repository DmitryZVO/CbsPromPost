namespace CbsPromPost.View;

sealed partial class FormOtk
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
        labelCount = new Label();
        buttonFinish = new Button();
        labelUser = new Label();
        labelName = new Label();
        labelDroneId = new Label();
        pictureBoxMain = new PictureBox();
        buttonBadDrone = new Button();
        numericUpDownFocus = new NumericUpDown();
        label1 = new Label();
        ((System.ComponentModel.ISupportInitialize)pictureBoxMain).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownFocus).BeginInit();
        SuspendLayout();
        // 
        // labelCount
        // 
        labelCount.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        labelCount.ForeColor = Color.Black;
        labelCount.Location = new Point(433, 12);
        labelCount.Name = "labelCount";
        labelCount.Size = new Size(184, 42);
        labelCount.TabIndex = 16;
        labelCount.Text = "КОЛ-ВО: 0";
        labelCount.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonFinish
        // 
        buttonFinish.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        buttonFinish.ForeColor = Color.FromArgb(64, 0, 0);
        buttonFinish.Location = new Point(1262, 12);
        buttonFinish.Name = "buttonFinish";
        buttonFinish.Size = new Size(216, 42);
        buttonFinish.TabIndex = 15;
        buttonFinish.Text = "ЗАКОНЧИТЬ";
        buttonFinish.UseVisualStyleBackColor = true;
        // 
        // labelUser
        // 
        labelUser.BorderStyle = BorderStyle.FixedSingle;
        labelUser.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelUser.ForeColor = Color.FromArgb(0, 0, 64);
        labelUser.Location = new Point(623, 12);
        labelUser.Name = "labelUser";
        labelUser.Size = new Size(633, 42);
        labelUser.TabIndex = 12;
        labelUser.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelName
        // 
        labelName.Font = new Font("Segoe UI", 22F, FontStyle.Regular, GraphicsUnit.Point);
        labelName.ForeColor = Color.FromArgb(64, 0, 0);
        labelName.Location = new Point(12, 12);
        labelName.Name = "labelName";
        labelName.Size = new Size(237, 42);
        labelName.TabIndex = 10;
        labelName.Text = "ПОСТ №";
        labelName.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelDroneId
        // 
        labelDroneId.BorderStyle = BorderStyle.FixedSingle;
        labelDroneId.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
        labelDroneId.ForeColor = Color.FromArgb(0, 0, 64);
        labelDroneId.Location = new Point(1592, 12);
        labelDroneId.Name = "labelDroneId";
        labelDroneId.Size = new Size(172, 42);
        labelDroneId.TabIndex = 18;
        labelDroneId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pictureBoxMain
        // 
        pictureBoxMain.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxMain.Location = new Point(12, 60);
        pictureBoxMain.Name = "pictureBoxMain";
        pictureBoxMain.Size = new Size(1860, 889);
        pictureBoxMain.TabIndex = 53;
        pictureBoxMain.TabStop = false;
        // 
        // buttonBadDrone
        // 
        buttonBadDrone.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        buttonBadDrone.ForeColor = Color.Maroon;
        buttonBadDrone.Location = new Point(1770, 12);
        buttonBadDrone.Name = "buttonBadDrone";
        buttonBadDrone.Size = new Size(102, 42);
        buttonBadDrone.TabIndex = 54;
        buttonBadDrone.Text = "БРАК";
        buttonBadDrone.UseVisualStyleBackColor = true;
        // 
        // numericUpDownFocus
        // 
        numericUpDownFocus.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
        numericUpDownFocus.Increment = new decimal(new int[] { 5, 0, 0, 0 });
        numericUpDownFocus.Location = new Point(125, 64);
        numericUpDownFocus.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
        numericUpDownFocus.Name = "numericUpDownFocus";
        numericUpDownFocus.ReadOnly = true;
        numericUpDownFocus.Size = new Size(65, 39);
        numericUpDownFocus.TabIndex = 56;
        numericUpDownFocus.TabStop = false;
        numericUpDownFocus.TextAlign = HorizontalAlignment.Center;
        numericUpDownFocus.UpDownAlign = LeftRightAlignment.Left;
        numericUpDownFocus.Value = new decimal(new int[] { 255, 0, 0, 0 });
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point);
        label1.Location = new Point(16, 64);
        label1.Name = "label1";
        label1.Size = new Size(108, 39);
        label1.TabIndex = 57;
        label1.Text = "ФОКУС:";
        // 
        // FormOtk
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1884, 961);
        Controls.Add(label1);
        Controls.Add(numericUpDownFocus);
        Controls.Add(buttonBadDrone);
        Controls.Add(pictureBoxMain);
        Controls.Add(labelDroneId);
        Controls.Add(labelCount);
        Controls.Add(buttonFinish);
        Controls.Add(labelUser);
        Controls.Add(labelName);
        MaximizeBox = false;
        Name = "FormOtk";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "[КБ ЦБС] Пост ОТК";
        ((System.ComponentModel.ISupportInitialize)pictureBoxMain).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownFocus).EndInit();
        ResumeLayout(false);
    }

    #endregion
    private Label labelCount;
    private Button buttonFinish;
    private Label labelUser;
    private Label labelName;
    private Label labelDroneId;
    private PictureBox pictureBoxMain;
    private Button buttonBadDrone;
    private NumericUpDown numericUpDownFocus;
    private Label label1;
}