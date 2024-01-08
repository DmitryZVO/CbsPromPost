namespace CbsPromPost.View
{
    partial class FormDroneConfig
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
            pictureBoxMain = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxMain
            // 
            pictureBoxMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxMain.Location = new Point(12, 12);
            pictureBoxMain.Name = "pictureBoxMain";
            pictureBoxMain.Size = new Size(1200, 800);
            pictureBoxMain.TabIndex = 0;
            pictureBoxMain.TabStop = false;
            // 
            // FormDroneConfig
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1486, 823);
            Controls.Add(pictureBoxMain);
            Name = "FormDroneConfig";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "НАСТРОЙКА ИЗДЕЛИЯ";
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxMain;
    }
}