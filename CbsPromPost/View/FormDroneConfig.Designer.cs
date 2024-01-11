namespace CbsPromPost.View;

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
        pictureBox3d = new PictureBox();
        pictureBox2d = new PictureBox();
        splitContainer1 = new SplitContainer();
        buttonAcelCalibrate = new Button();
        buttonInverseAll = new Button();
        button1010 = new Button();
        buttonD3Inv = new Button();
        buttonD1Inv = new Button();
        buttonD2Inv = new Button();
        buttonD4Inv = new Button();
        buttonMotors1500 = new Button();
        buttonMotors1250 = new Button();
        buttonMotors1100 = new Button();
        buttonMotors1000 = new Button();
        trackBarMotors = new TrackBar();
        trackBarD4 = new TrackBar();
        trackBarD3 = new TrackBar();
        trackBarD2 = new TrackBar();
        trackBarD1 = new TrackBar();
        ((System.ComponentModel.ISupportInitialize)pictureBox3d).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2d).BeginInit();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)trackBarMotors).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD1).BeginInit();
        SuspendLayout();
        // 
        // pictureBox3d
        // 
        pictureBox3d.BorderStyle = BorderStyle.FixedSingle;
        pictureBox3d.Dock = DockStyle.Fill;
        pictureBox3d.Location = new Point(0, 0);
        pictureBox3d.Name = "pictureBox3d";
        pictureBox3d.Size = new Size(803, 800);
        pictureBox3d.TabIndex = 0;
        pictureBox3d.TabStop = false;
        // 
        // pictureBox2d
        // 
        pictureBox2d.BorderStyle = BorderStyle.FixedSingle;
        pictureBox2d.Dock = DockStyle.Fill;
        pictureBox2d.Location = new Point(0, 0);
        pictureBox2d.Name = "pictureBox2d";
        pictureBox2d.Size = new Size(803, 800);
        pictureBox2d.TabIndex = 1;
        pictureBox2d.TabStop = false;
        // 
        // splitContainer1
        // 
        splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        splitContainer1.Location = new Point(12, 12);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(pictureBox3d);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(buttonAcelCalibrate);
        splitContainer1.Panel2.Controls.Add(buttonInverseAll);
        splitContainer1.Panel2.Controls.Add(button1010);
        splitContainer1.Panel2.Controls.Add(buttonD3Inv);
        splitContainer1.Panel2.Controls.Add(buttonD1Inv);
        splitContainer1.Panel2.Controls.Add(buttonD2Inv);
        splitContainer1.Panel2.Controls.Add(buttonD4Inv);
        splitContainer1.Panel2.Controls.Add(buttonMotors1500);
        splitContainer1.Panel2.Controls.Add(buttonMotors1250);
        splitContainer1.Panel2.Controls.Add(buttonMotors1100);
        splitContainer1.Panel2.Controls.Add(buttonMotors1000);
        splitContainer1.Panel2.Controls.Add(trackBarMotors);
        splitContainer1.Panel2.Controls.Add(trackBarD4);
        splitContainer1.Panel2.Controls.Add(trackBarD3);
        splitContainer1.Panel2.Controls.Add(trackBarD2);
        splitContainer1.Panel2.Controls.Add(trackBarD1);
        splitContainer1.Panel2.Controls.Add(pictureBox2d);
        splitContainer1.Size = new Size(1610, 800);
        splitContainer1.SplitterDistance = 803;
        splitContainer1.TabIndex = 2;
        splitContainer1.TabStop = false;
        // 
        // buttonAcelCalibrate
        // 
        buttonAcelCalibrate.BackColor = Color.White;
        buttonAcelCalibrate.FlatStyle = FlatStyle.Flat;
        buttonAcelCalibrate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonAcelCalibrate.Location = new Point(306, 561);
        buttonAcelCalibrate.Name = "buttonAcelCalibrate";
        buttonAcelCalibrate.Size = new Size(189, 36);
        buttonAcelCalibrate.TabIndex = 17;
        buttonAcelCalibrate.TabStop = false;
        buttonAcelCalibrate.Text = "КАЛИБРОВКА УРОВНЯ";
        buttonAcelCalibrate.UseVisualStyleBackColor = false;
        // 
        // buttonInverseAll
        // 
        buttonInverseAll.BackColor = SystemColors.InactiveBorder;
        buttonInverseAll.FlatStyle = FlatStyle.Flat;
        buttonInverseAll.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
        buttonInverseAll.Location = new Point(306, 603);
        buttonInverseAll.Name = "buttonInverseAll";
        buttonInverseAll.Size = new Size(189, 36);
        buttonInverseAll.TabIndex = 18;
        buttonInverseAll.TabStop = false;
        buttonInverseAll.Text = "ИНВЕРСИЯ ВСЕХ ДВИГАТЕЛЕЙ";
        buttonInverseAll.UseVisualStyleBackColor = false;
        // 
        // button1010
        // 
        button1010.BackColor = Color.Honeydew;
        button1010.FlatStyle = FlatStyle.Flat;
        button1010.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        button1010.Location = new Point(306, 519);
        button1010.Name = "button1010";
        button1010.Size = new Size(59, 36);
        button1010.TabIndex = 17;
        button1010.TabStop = false;
        button1010.Text = "1020";
        button1010.UseVisualStyleBackColor = false;
        // 
        // buttonD3Inv
        // 
        buttonD3Inv.BackColor = SystemColors.InactiveBorder;
        buttonD3Inv.FlatStyle = FlatStyle.Flat;
        buttonD3Inv.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonD3Inv.Location = new Point(47, 410);
        buttonD3Inv.Name = "buttonD3Inv";
        buttonD3Inv.Size = new Size(59, 36);
        buttonD3Inv.TabIndex = 16;
        buttonD3Inv.TabStop = false;
        buttonD3Inv.Text = "ИНВ";
        buttonD3Inv.UseVisualStyleBackColor = false;
        // 
        // buttonD1Inv
        // 
        buttonD1Inv.BackColor = SystemColors.InactiveBorder;
        buttonD1Inv.FlatStyle = FlatStyle.Flat;
        buttonD1Inv.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonD1Inv.Location = new Point(701, 410);
        buttonD1Inv.Name = "buttonD1Inv";
        buttonD1Inv.Size = new Size(59, 36);
        buttonD1Inv.TabIndex = 15;
        buttonD1Inv.TabStop = false;
        buttonD1Inv.Text = "ИНВ";
        buttonD1Inv.UseVisualStyleBackColor = false;
        // 
        // buttonD2Inv
        // 
        buttonD2Inv.BackColor = SystemColors.InactiveBorder;
        buttonD2Inv.FlatStyle = FlatStyle.Flat;
        buttonD2Inv.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonD2Inv.Location = new Point(677, 69);
        buttonD2Inv.Name = "buttonD2Inv";
        buttonD2Inv.Size = new Size(59, 36);
        buttonD2Inv.TabIndex = 14;
        buttonD2Inv.TabStop = false;
        buttonD2Inv.Text = "ИНВ";
        buttonD2Inv.UseVisualStyleBackColor = false;
        // 
        // buttonD4Inv
        // 
        buttonD4Inv.BackColor = SystemColors.InactiveBorder;
        buttonD4Inv.FlatStyle = FlatStyle.Flat;
        buttonD4Inv.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonD4Inv.Location = new Point(70, 69);
        buttonD4Inv.Name = "buttonD4Inv";
        buttonD4Inv.Size = new Size(59, 36);
        buttonD4Inv.TabIndex = 13;
        buttonD4Inv.TabStop = false;
        buttonD4Inv.Text = "ИНВ";
        buttonD4Inv.UseVisualStyleBackColor = false;
        // 
        // buttonMotors1500
        // 
        buttonMotors1500.BackColor = Color.LightSalmon;
        buttonMotors1500.FlatStyle = FlatStyle.Flat;
        buttonMotors1500.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonMotors1500.Location = new Point(501, 519);
        buttonMotors1500.Name = "buttonMotors1500";
        buttonMotors1500.Size = new Size(59, 36);
        buttonMotors1500.TabIndex = 11;
        buttonMotors1500.TabStop = false;
        buttonMotors1500.Text = "1500";
        buttonMotors1500.UseVisualStyleBackColor = false;
        // 
        // buttonMotors1250
        // 
        buttonMotors1250.BackColor = Color.PeachPuff;
        buttonMotors1250.FlatStyle = FlatStyle.Flat;
        buttonMotors1250.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonMotors1250.Location = new Point(436, 519);
        buttonMotors1250.Name = "buttonMotors1250";
        buttonMotors1250.Size = new Size(59, 36);
        buttonMotors1250.TabIndex = 10;
        buttonMotors1250.TabStop = false;
        buttonMotors1250.Text = "1250";
        buttonMotors1250.UseVisualStyleBackColor = false;
        // 
        // buttonMotors1100
        // 
        buttonMotors1100.BackColor = Color.OldLace;
        buttonMotors1100.FlatStyle = FlatStyle.Flat;
        buttonMotors1100.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        buttonMotors1100.Location = new Point(371, 519);
        buttonMotors1100.Name = "buttonMotors1100";
        buttonMotors1100.Size = new Size(59, 36);
        buttonMotors1100.TabIndex = 9;
        buttonMotors1100.TabStop = false;
        buttonMotors1100.Text = "1100";
        buttonMotors1100.UseVisualStyleBackColor = false;
        // 
        // buttonMotors1000
        // 
        buttonMotors1000.BackColor = Color.White;
        buttonMotors1000.FlatStyle = FlatStyle.Flat;
        buttonMotors1000.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        buttonMotors1000.Location = new Point(241, 519);
        buttonMotors1000.Name = "buttonMotors1000";
        buttonMotors1000.Size = new Size(59, 36);
        buttonMotors1000.TabIndex = 8;
        buttonMotors1000.TabStop = false;
        buttonMotors1000.Text = "СТОП";
        buttonMotors1000.UseVisualStyleBackColor = false;
        // 
        // trackBarMotors
        // 
        trackBarMotors.AutoSize = false;
        trackBarMotors.LargeChange = 50;
        trackBarMotors.Location = new Point(326, 473);
        trackBarMotors.Maximum = 2000;
        trackBarMotors.Minimum = 1000;
        trackBarMotors.Name = "trackBarMotors";
        trackBarMotors.Size = new Size(147, 44);
        trackBarMotors.SmallChange = 10;
        trackBarMotors.TabIndex = 7;
        trackBarMotors.TickStyle = TickStyle.Both;
        trackBarMotors.Value = 1000;
        // 
        // trackBarD4
        // 
        trackBarD4.AutoSize = false;
        trackBarD4.LargeChange = 50;
        trackBarD4.Location = new Point(135, 20);
        trackBarD4.Maximum = 2000;
        trackBarD4.Minimum = 1000;
        trackBarD4.Name = "trackBarD4";
        trackBarD4.Orientation = Orientation.Vertical;
        trackBarD4.Size = new Size(43, 135);
        trackBarD4.SmallChange = 10;
        trackBarD4.TabIndex = 5;
        trackBarD4.TickStyle = TickStyle.Both;
        trackBarD4.Value = 1000;
        // 
        // trackBarD3
        // 
        trackBarD3.AutoSize = false;
        trackBarD3.LargeChange = 50;
        trackBarD3.Location = new Point(112, 359);
        trackBarD3.Maximum = 2000;
        trackBarD3.Minimum = 1000;
        trackBarD3.Name = "trackBarD3";
        trackBarD3.Orientation = Orientation.Vertical;
        trackBarD3.Size = new Size(43, 135);
        trackBarD3.SmallChange = 10;
        trackBarD3.TabIndex = 4;
        trackBarD3.TickStyle = TickStyle.Both;
        trackBarD3.Value = 1000;
        // 
        // trackBarD2
        // 
        trackBarD2.AutoSize = false;
        trackBarD2.LargeChange = 50;
        trackBarD2.Location = new Point(628, 20);
        trackBarD2.Maximum = 2000;
        trackBarD2.Minimum = 1000;
        trackBarD2.Name = "trackBarD2";
        trackBarD2.Orientation = Orientation.Vertical;
        trackBarD2.Size = new Size(43, 135);
        trackBarD2.SmallChange = 10;
        trackBarD2.TabIndex = 3;
        trackBarD2.TickStyle = TickStyle.Both;
        trackBarD2.Value = 1000;
        // 
        // trackBarD1
        // 
        trackBarD1.AutoSize = false;
        trackBarD1.LargeChange = 50;
        trackBarD1.Location = new Point(652, 359);
        trackBarD1.Maximum = 2000;
        trackBarD1.Minimum = 1000;
        trackBarD1.Name = "trackBarD1";
        trackBarD1.Orientation = Orientation.Vertical;
        trackBarD1.Size = new Size(43, 135);
        trackBarD1.SmallChange = 10;
        trackBarD1.TabIndex = 2;
        trackBarD1.TickStyle = TickStyle.Both;
        trackBarD1.Value = 1000;
        // 
        // FormDroneConfig
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1634, 821);
        Controls.Add(splitContainer1);
        MaximizeBox = false;
        Name = "FormDroneConfig";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "НАСТРОЙКА ИЗДЕЛИЯ";
        ((System.ComponentModel.ISupportInitialize)pictureBox3d).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2d).EndInit();
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)trackBarMotors).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD4).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD3).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD2).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarD1).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private PictureBox pictureBox3d;
    private PictureBox pictureBox2d;
    private SplitContainer splitContainer1;
    private TrackBar trackBarD1;
    private TrackBar trackBarD2;
    private TrackBar trackBarD3;
    private TrackBar trackBarD4;
    private TrackBar trackBarMotors;
    private Button buttonMotors1500;
    private Button buttonMotors1250;
    private Button buttonMotors1100;
    private Button buttonMotors1000;
    private Button buttonD2Inv;
    private Button buttonD4Inv;
    private Button buttonD3Inv;
    private Button buttonD1Inv;
    private Button buttonAcelCalibrate;
    private Button button1010;
    private Button buttonInverseAll;
}