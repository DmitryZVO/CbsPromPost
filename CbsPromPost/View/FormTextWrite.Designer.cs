namespace CbsPromPost.View
{
    sealed partial class FormTextWrite
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
            richTextBoxMain = new RichTextBox();
            buttonCancel = new Button();
            buttonSend = new Button();
            SuspendLayout();
            // 
            // richTextBoxMain
            // 
            richTextBoxMain.Dock = DockStyle.Bottom;
            richTextBoxMain.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBoxMain.Location = new Point(0, 57);
            richTextBoxMain.Name = "richTextBoxMain";
            richTextBoxMain.Size = new Size(800, 393);
            richTextBoxMain.TabIndex = 0;
            richTextBoxMain.Text = "";
            // 
            // buttonCancel
            // 
            buttonCancel.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCancel.ForeColor = Color.Maroon;
            buttonCancel.Location = new Point(12, 12);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(183, 39);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "ОТМЕНИТЬ";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSend
            // 
            buttonSend.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            buttonSend.ForeColor = Color.FromArgb(0, 64, 0);
            buttonSend.Location = new Point(605, 12);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(183, 39);
            buttonSend.TabIndex = 2;
            buttonSend.Text = "ПОДТВЕРДИТЬ";
            buttonSend.UseVisualStyleBackColor = true;
            // 
            // FormTextWrite
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonSend);
            Controls.Add(buttonCancel);
            Controls.Add(richTextBoxMain);
            Name = "FormTextWrite";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormIdInfo";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBoxMain;
        private Button buttonCancel;
        private Button buttonSend;
    }
}