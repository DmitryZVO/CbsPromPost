namespace CbsPromPost.View;

sealed partial class FormFlyRecord
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
        labelTime = new Label();
        labelUser = new Label();
        labelWork = new Label();
        labelName = new Label();
        labelDroneId = new Label();
        pictureBoxCamFpv = new PictureBox();
        buttonBadDrone = new Button();
        buttonOkDrone = new Button();
        checkBoxNotMy = new CheckBox();
        pictureBoxCamBox = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBoxCamFpv).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxCamBox).BeginInit();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        label1.ForeColor = Color.Black;
        label1.Location = new Point(1129, 143);
        label1.Name = "label1";
        label1.Size = new Size(443, 82);
        label1.TabIndex = 17;
        label1.Text = "НОРМАТИВ: 10 мин.";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelCount
        // 
        labelCount.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        labelCount.ForeColor = Color.Black;
        labelCount.Location = new Point(12, 143);
        labelCount.Name = "labelCount";
        labelCount.Size = new Size(354, 82);
        labelCount.TabIndex = 16;
        labelCount.Text = "КОЛ-ВО: 0";
        labelCount.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonFinish
        // 
        buttonFinish.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        buttonFinish.ForeColor = Color.FromArgb(64, 0, 0);
        buttonFinish.Location = new Point(1331, 73);
        buttonFinish.Name = "buttonFinish";
        buttonFinish.Size = new Size(241, 61);
        buttonFinish.TabIndex = 15;
        buttonFinish.Text = "ЗАКОНЧИТЬ";
        buttonFinish.UseVisualStyleBackColor = true;
        // 
        // labelTime
        // 
        labelTime.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point);
        labelTime.ForeColor = Color.Black;
        labelTime.Location = new Point(372, 143);
        labelTime.Name = "labelTime";
        labelTime.Size = new Size(584, 82);
        labelTime.TabIndex = 13;
        labelTime.Text = "00:00:00";
        labelTime.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelUser
        // 
        labelUser.BorderStyle = BorderStyle.FixedSingle;
        labelUser.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
        labelUser.ForeColor = Color.FromArgb(0, 0, 64);
        labelUser.Location = new Point(494, 73);
        labelUser.Name = "labelUser";
        labelUser.Size = new Size(831, 61);
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
        labelDroneId.Location = new Point(12, 73);
        labelDroneId.Name = "labelDroneId";
        labelDroneId.Size = new Size(232, 63);
        labelDroneId.TabIndex = 18;
        labelDroneId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pictureBoxCamFpv
        // 
        pictureBoxCamFpv.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxCamFpv.Location = new Point(12, 228);
        pictureBoxCamFpv.Name = "pictureBoxCamFpv";
        pictureBoxCamFpv.Size = new Size(775, 582);
        pictureBoxCamFpv.TabIndex = 53;
        pictureBoxCamFpv.TabStop = false;
        // 
        // buttonBadDrone
        // 
        buttonBadDrone.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        buttonBadDrone.ForeColor = Color.Maroon;
        buttonBadDrone.Location = new Point(372, 73);
        buttonBadDrone.Name = "buttonBadDrone";
        buttonBadDrone.Size = new Size(116, 63);
        buttonBadDrone.TabIndex = 54;
        buttonBadDrone.Text = "БРАК";
        buttonBadDrone.UseVisualStyleBackColor = true;
        // 
        // buttonOkDrone
        // 
        buttonOkDrone.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        buttonOkDrone.ForeColor = Color.FromArgb(0, 64, 64);
        buttonOkDrone.Location = new Point(250, 73);
        buttonOkDrone.Name = "buttonOkDrone";
        buttonOkDrone.Size = new Size(116, 63);
        buttonOkDrone.TabIndex = 55;
        buttonOkDrone.Text = "ОК";
        buttonOkDrone.UseVisualStyleBackColor = true;
        // 
        // checkBoxNotMy
        // 
        checkBoxNotMy.AutoSize = true;
        checkBoxNotMy.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
        checkBoxNotMy.Location = new Point(962, 173);
        checkBoxNotMy.Name = "checkBoxNotMy";
        checkBoxNotMy.Size = new Size(196, 29);
        checkBoxNotMy.TabIndex = 56;
        checkBoxNotMy.Text = "ЧУЖОЕ ИЗДЕЛИЕ";
        checkBoxNotMy.UseVisualStyleBackColor = true;
        // 
        // pictureBoxCamBox
        // 
        pictureBoxCamBox.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxCamBox.Location = new Point(797, 228);
        pictureBoxCamBox.Name = "pictureBoxCamBox";
        pictureBoxCamBox.Size = new Size(775, 582);
        pictureBoxCamBox.TabIndex = 57;
        pictureBoxCamBox.TabStop = false;
        // 
        // FormFlyRecord
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1584, 961);
        Controls.Add(pictureBoxCamBox);
        Controls.Add(checkBoxNotMy);
        Controls.Add(buttonOkDrone);
        Controls.Add(buttonBadDrone);
        Controls.Add(pictureBoxCamFpv);
        Controls.Add(labelDroneId);
        Controls.Add(label1);
        Controls.Add(labelCount);
        Controls.Add(buttonFinish);
        Controls.Add(labelTime);
        Controls.Add(labelUser);
        Controls.Add(labelWork);
        Controls.Add(labelName);
        MaximizeBox = false;
        Name = "FormFlyRecord";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "[КБ ЦБС] Пост финальных полетов БПЛА";
        ((System.ComponentModel.ISupportInitialize)pictureBoxCamFpv).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxCamBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private Label labelCount;
    private Button buttonFinish;
    private Label labelTime;
    private Label labelUser;
    private Label labelWork;
    private Label labelName;
    private Label labelDroneId;
    private PictureBox pictureBoxCamFpv;
    private Button buttonBadDrone;
    private Button buttonOkDrone;
    private CheckBox checkBoxNotMy;
    private PictureBox pictureBoxCamBox;
}