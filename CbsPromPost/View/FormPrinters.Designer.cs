namespace CbsPromPost.View
{
    sealed partial class FormPrinters
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
            button1 = new Button();
            button2 = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 39F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(570, 90);
            button1.TabIndex = 0;
            button1.Text = "ПРИНТЕР 1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 39F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(12, 108);
            button2.Name = "button2";
            button2.Size = new Size(570, 90);
            button2.TabIndex = 1;
            button2.Text = "ПРИНТЕР 2";
            button2.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Font = new Font("Segoe UI", 39F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCancel.ForeColor = Color.Maroon;
            buttonCancel.Location = new Point(12, 204);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(570, 78);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "ОТМЕНА";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormPrinters
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(594, 294);
            ControlBox = false;
            Controls.Add(buttonCancel);
            Controls.Add(button2);
            Controls.Add(button1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MaximumSize = new Size(1000, 800);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "FormPrinters";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button buttonCancel;
    }
}