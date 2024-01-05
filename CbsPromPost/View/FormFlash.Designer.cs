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
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        label1.ForeColor = Color.Black;
        label1.Location = new Point(1151, 167);
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
        labelCount.Location = new Point(12, 167);
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
        buttonFinish.Location = new Point(1342, 99);
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
        buttonPause.Location = new Point(12, 99);
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
        labelTime.Location = new Point(327, 167);
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
        labelUser.Location = new Point(501, 99);
        labelUser.Name = "labelUser";
        labelUser.Size = new Size(835, 61);
        labelUser.TabIndex = 12;
        labelUser.Text = "ФИО";
        labelUser.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelWork
        // 
        labelWork.Font = new Font("Segoe UI", 32F, FontStyle.Regular, GraphicsUnit.Point);
        labelWork.ForeColor = Color.FromArgb(0, 0, 64);
        labelWork.Location = new Point(12, 9);
        labelWork.Name = "labelWork";
        labelWork.Size = new Size(1170, 83);
        labelWork.TabIndex = 11;
        labelWork.Text = "ТИП РАБОТЫ";
        labelWork.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelName
        // 
        labelName.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point);
        labelName.ForeColor = Color.FromArgb(64, 0, 0);
        labelName.Location = new Point(1188, 9);
        labelName.Name = "labelName";
        labelName.Size = new Size(384, 84);
        labelName.TabIndex = 10;
        labelName.Text = "ПОСТ №";
        labelName.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // labelDroneId
        // 
        labelDroneId.BorderStyle = BorderStyle.FixedSingle;
        labelDroneId.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point);
        labelDroneId.ForeColor = Color.FromArgb(0, 0, 64);
        labelDroneId.Location = new Point(248, 99);
        labelDroneId.Name = "labelDroneId";
        labelDroneId.Size = new Size(247, 61);
        labelDroneId.TabIndex = 18;
        labelDroneId.Text = "ИЗДЕЛИЕ";
        labelDroneId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // FormFlash
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1584, 921);
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
        Text = "[КБ ЦБС] Пост прошивки БПЛА";
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
}