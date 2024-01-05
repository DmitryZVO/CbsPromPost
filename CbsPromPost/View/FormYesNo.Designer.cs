namespace CbsPromPost.View;

sealed partial class FormYesNo
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        labelInfo = new Label();
        buttonNo = new Button();
        buttonYes = new Button();
        SuspendLayout();
        // 
        // labelInfo
        // 
        labelInfo.Dock = DockStyle.Top;
        labelInfo.Font = new Font("Segoe UI", 29F, FontStyle.Regular, GraphicsUnit.Point);
        labelInfo.Location = new Point(0, 0);
        labelInfo.Name = "labelInfo";
        labelInfo.Size = new Size(798, 335);
        labelInfo.TabIndex = 0;
        labelInfo.Text = "ИНФОРМАЦИЯ";
        labelInfo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonNo
        // 
        buttonNo.Dock = DockStyle.Left;
        buttonNo.Font = new Font("Segoe UI", 30F, FontStyle.Regular, GraphicsUnit.Point);
        buttonNo.ForeColor = Color.FromArgb(64, 0, 0);
        buttonNo.Location = new Point(0, 335);
        buttonNo.Name = "buttonNo";
        buttonNo.Size = new Size(390, 63);
        buttonNo.TabIndex = 1;
        buttonNo.Text = "НЕТ";
        buttonNo.UseVisualStyleBackColor = true;
        buttonNo.Click += ButtonNo_Click;
        // 
        // buttonYes
        // 
        buttonYes.Dock = DockStyle.Right;
        buttonYes.Font = new Font("Segoe UI", 30F, FontStyle.Regular, GraphicsUnit.Point);
        buttonYes.ForeColor = Color.FromArgb(0, 64, 0);
        buttonYes.Location = new Point(408, 335);
        buttonYes.Name = "buttonYes";
        buttonYes.Size = new Size(390, 63);
        buttonYes.TabIndex = 2;
        buttonYes.Text = "ДА";
        buttonYes.UseVisualStyleBackColor = true;
        buttonYes.Click += ButtonYes_Click;
        // 
        // FormYesNo
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(798, 398);
        ControlBox = false;
        Controls.Add(buttonYes);
        Controls.Add(buttonNo);
        Controls.Add(labelInfo);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        MaximizeBox = false;
        MaximumSize = new Size(800, 400);
        MdiChildrenMinimizedAnchorBottom = false;
        MinimizeBox = false;
        MinimumSize = new Size(800, 400);
        Name = "FormYesNo";
        ShowIcon = false;
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterParent;
        TopMost = true;
        FormClosing += FormInfo_FormClosing;
        Shown += FormInfo_Shown;
        ResumeLayout(false);
    }

    #endregion

    private Label labelInfo;
    private Button buttonNo;
    private Button buttonYes;
}