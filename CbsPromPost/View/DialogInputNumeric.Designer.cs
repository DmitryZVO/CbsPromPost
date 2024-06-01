namespace CbsPromPost.View;

sealed partial class DialogInputNumeric
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
        textBoxMain = new TextBox();
        button1 = new Button();
        textBoxPrefix = new TextBox();
        SuspendLayout();
        // 
        // textBoxMain
        // 
        textBoxMain.Location = new Point(65, 12);
        textBoxMain.Name = "textBoxMain";
        textBoxMain.Size = new Size(140, 23);
        textBoxMain.TabIndex = 0;
        textBoxMain.TextAlign = HorizontalAlignment.Center;
        textBoxMain.KeyDown += TextBoxMain_KeyDown;
        // 
        // button1
        // 
        button1.Location = new Point(211, 12);
        button1.Name = "button1";
        button1.Size = new Size(91, 23);
        button1.TabIndex = 1;
        button1.Text = "ПРИМЕНИТЬ";
        button1.UseVisualStyleBackColor = true;
        button1.Click += Button1_Click;
        // 
        // textBoxPrefix
        // 
        textBoxPrefix.Enabled = false;
        textBoxPrefix.Location = new Point(12, 12);
        textBoxPrefix.Name = "textBoxPrefix";
        textBoxPrefix.ReadOnly = true;
        textBoxPrefix.Size = new Size(47, 23);
        textBoxPrefix.TabIndex = 2;
        textBoxPrefix.TextAlign = HorizontalAlignment.Center;
        // 
        // DialogInputNumeric
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(314, 47);
        Controls.Add(textBoxPrefix);
        Controls.Add(button1);
        Controls.Add(textBoxMain);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DialogInputNumeric";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "ВВЕДИТЕ ДАННЫЕ";
        Shown += DialogInputString_Shown;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox textBoxMain;
    private Button button1;
    private TextBox textBoxPrefix;
}