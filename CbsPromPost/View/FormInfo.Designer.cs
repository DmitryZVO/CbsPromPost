namespace CbsPromPost.View;

sealed partial class FormInfo
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
        SuspendLayout();
        // 
        // labelInfo
        // 
        labelInfo.Dock = DockStyle.Fill;
        labelInfo.Font = new Font("Segoe UI", 29F, FontStyle.Regular, GraphicsUnit.Point);
        labelInfo.Location = new Point(0, 0);
        labelInfo.Name = "labelInfo";
        labelInfo.Size = new Size(594, 294);
        labelInfo.TabIndex = 0;
        labelInfo.Text = "ИНФОРМАЦИЯ";
        labelInfo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // FormInfo
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(594, 294);
        ControlBox = false;
        Controls.Add(labelInfo);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        MaximizeBox = false;
        MaximumSize = new Size(1000, 800);
        MdiChildrenMinimizedAnchorBottom = false;
        MinimizeBox = false;
        Name = "FormInfo";
        ShowIcon = false;
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        FormClosing += FormInfo_FormClosing;
        Shown += FormInfo_Shown;
        ResumeLayout(false);
    }

    #endregion

    private Label labelInfo;
}