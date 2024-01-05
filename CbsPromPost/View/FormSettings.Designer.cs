namespace CbsPromPost.View;

partial class FormSettings
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
        components = new System.ComponentModel.Container();
        buttonExit = new Button();
        label1 = new Label();
        buttonEngRestart = new Button();
        buttonEngShutdown = new Button();
        buttonEngClose = new Button();
        buttonEngCmd = new Button();
        buttonEngControlPanel = new Button();
        buttonEngConfig = new Button();
        buttonEngExplorer = new Button();
        buttonEngKillExplorer = new Button();
        buttonEngAutoLogin = new Button();
        buttonEngTaskManager = new Button();
        buttonEngTaskSheduler = new Button();
        buttonEngPowerShell = new Button();
        buttonEngUpdate = new Button();
        label2 = new Label();
        label3 = new Label();
        buttonEngShell = new Button();
        timerFPS = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        // 
        // buttonExit
        // 
        buttonExit.BackColor = Color.FromArgb(255, 192, 192);
        buttonExit.FlatStyle = FlatStyle.Flat;
        buttonExit.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        buttonExit.Location = new Point(708, 12);
        buttonExit.Name = "buttonExit";
        buttonExit.Size = new Size(40, 40);
        buttonExit.TabIndex = 0;
        buttonExit.Text = "X";
        buttonExit.UseVisualStyleBackColor = false;
        buttonExit.Click += ButtonExit_Click;
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 18.75F, FontStyle.Bold, GraphicsUnit.Point);
        label1.Location = new Point(12, 12);
        label1.Name = "label1";
        label1.Size = new Size(692, 40);
        label1.TabIndex = 1;
        label1.Text = "НАСТРОЙКИ СИСТЕМЫ";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // buttonEngRestart
        // 
        buttonEngRestart.BackColor = Color.FromArgb(255, 255, 192);
        buttonEngRestart.FlatStyle = FlatStyle.Flat;
        buttonEngRestart.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngRestart.Location = new Point(10, 64);
        buttonEngRestart.Name = "buttonEngRestart";
        buttonEngRestart.Size = new Size(180, 40);
        buttonEngRestart.TabIndex = 2;
        buttonEngRestart.Text = "ПЕРЕЗАГРУЗИТЬ";
        buttonEngRestart.UseVisualStyleBackColor = false;
        buttonEngRestart.Click += ButtonEngRestart_Click;
        // 
        // buttonEngShutdown
        // 
        buttonEngShutdown.BackColor = Color.FromArgb(255, 192, 192);
        buttonEngShutdown.FlatStyle = FlatStyle.Flat;
        buttonEngShutdown.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngShutdown.Location = new Point(196, 64);
        buttonEngShutdown.Name = "buttonEngShutdown";
        buttonEngShutdown.Size = new Size(180, 40);
        buttonEngShutdown.TabIndex = 3;
        buttonEngShutdown.Text = "ВЫКЛЮЧИТЬ";
        buttonEngShutdown.UseVisualStyleBackColor = false;
        buttonEngShutdown.Click += ButtonEngShutdown_Click;
        // 
        // buttonEngClose
        // 
        buttonEngClose.BackColor = Color.FromArgb(192, 255, 255);
        buttonEngClose.FlatStyle = FlatStyle.Flat;
        buttonEngClose.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngClose.Location = new Point(382, 64);
        buttonEngClose.Name = "buttonEngClose";
        buttonEngClose.Size = new Size(180, 40);
        buttonEngClose.TabIndex = 4;
        buttonEngClose.Text = "ЗАКРЫТЬ";
        buttonEngClose.UseVisualStyleBackColor = false;
        buttonEngClose.Click += ButtonEngClose_Click;
        // 
        // buttonEngCmd
        // 
        buttonEngCmd.BackColor = Color.Silver;
        buttonEngCmd.FlatStyle = FlatStyle.Flat;
        buttonEngCmd.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngCmd.Location = new Point(568, 64);
        buttonEngCmd.Name = "buttonEngCmd";
        buttonEngCmd.Size = new Size(180, 40);
        buttonEngCmd.TabIndex = 5;
        buttonEngCmd.Text = "CMD";
        buttonEngCmd.UseVisualStyleBackColor = false;
        buttonEngCmd.Click += ButtonEngCmd_Click;
        // 
        // buttonEngControlPanel
        // 
        buttonEngControlPanel.BackColor = Color.White;
        buttonEngControlPanel.FlatStyle = FlatStyle.Flat;
        buttonEngControlPanel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngControlPanel.Location = new Point(10, 110);
        buttonEngControlPanel.Name = "buttonEngControlPanel";
        buttonEngControlPanel.Size = new Size(180, 40);
        buttonEngControlPanel.TabIndex = 6;
        buttonEngControlPanel.Text = "ПАНЕЛЬ УПРАВЛЕНИЯ";
        buttonEngControlPanel.UseVisualStyleBackColor = false;
        buttonEngControlPanel.Click += ButtonEngControlPanel_Click;
        // 
        // buttonEngConfig
        // 
        buttonEngConfig.BackColor = Color.White;
        buttonEngConfig.FlatStyle = FlatStyle.Flat;
        buttonEngConfig.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngConfig.Location = new Point(196, 110);
        buttonEngConfig.Name = "buttonEngConfig";
        buttonEngConfig.Size = new Size(180, 40);
        buttonEngConfig.TabIndex = 7;
        buttonEngConfig.Text = "WINDOWS CONFIG";
        buttonEngConfig.UseVisualStyleBackColor = false;
        buttonEngConfig.Click += ButtonEngConfig_Click;
        // 
        // buttonEngExplorer
        // 
        buttonEngExplorer.BackColor = Color.White;
        buttonEngExplorer.FlatStyle = FlatStyle.Flat;
        buttonEngExplorer.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngExplorer.Location = new Point(382, 110);
        buttonEngExplorer.Name = "buttonEngExplorer";
        buttonEngExplorer.Size = new Size(180, 40);
        buttonEngExplorer.TabIndex = 8;
        buttonEngExplorer.Text = "EXPLORER";
        buttonEngExplorer.UseVisualStyleBackColor = false;
        buttonEngExplorer.Click += ButtonEngExplorer_Click;
        // 
        // buttonEngKillExplorer
        // 
        buttonEngKillExplorer.BackColor = Color.White;
        buttonEngKillExplorer.FlatStyle = FlatStyle.Flat;
        buttonEngKillExplorer.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngKillExplorer.Location = new Point(568, 110);
        buttonEngKillExplorer.Name = "buttonEngKillExplorer";
        buttonEngKillExplorer.Size = new Size(180, 40);
        buttonEngKillExplorer.TabIndex = 9;
        buttonEngKillExplorer.Text = "УБИТЬ EXPLORER";
        buttonEngKillExplorer.UseVisualStyleBackColor = false;
        buttonEngKillExplorer.Click += ButtonEngKillExplorer_Click;
        // 
        // buttonEngAutoLogin
        // 
        buttonEngAutoLogin.BackColor = Color.White;
        buttonEngAutoLogin.FlatStyle = FlatStyle.Flat;
        buttonEngAutoLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngAutoLogin.Location = new Point(10, 156);
        buttonEngAutoLogin.Name = "buttonEngAutoLogin";
        buttonEngAutoLogin.Size = new Size(180, 40);
        buttonEngAutoLogin.TabIndex = 10;
        buttonEngAutoLogin.Text = "АВТОВХОД";
        buttonEngAutoLogin.UseVisualStyleBackColor = false;
        buttonEngAutoLogin.Click += ButtonEngAutoLogin_Click;
        // 
        // buttonEngTaskManager
        // 
        buttonEngTaskManager.BackColor = Color.White;
        buttonEngTaskManager.FlatStyle = FlatStyle.Flat;
        buttonEngTaskManager.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngTaskManager.Location = new Point(196, 156);
        buttonEngTaskManager.Name = "buttonEngTaskManager";
        buttonEngTaskManager.Size = new Size(180, 40);
        buttonEngTaskManager.TabIndex = 11;
        buttonEngTaskManager.Text = "ДИСПЕТЧЕР ЗАДАЧ";
        buttonEngTaskManager.UseVisualStyleBackColor = false;
        buttonEngTaskManager.Click += ButtonEngTaskManager_Click;
        // 
        // buttonEngTaskSheduler
        // 
        buttonEngTaskSheduler.BackColor = Color.White;
        buttonEngTaskSheduler.FlatStyle = FlatStyle.Flat;
        buttonEngTaskSheduler.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngTaskSheduler.Location = new Point(382, 156);
        buttonEngTaskSheduler.Name = "buttonEngTaskSheduler";
        buttonEngTaskSheduler.Size = new Size(180, 40);
        buttonEngTaskSheduler.TabIndex = 12;
        buttonEngTaskSheduler.Text = "TASK SHEDULER";
        buttonEngTaskSheduler.UseVisualStyleBackColor = false;
        buttonEngTaskSheduler.Click += ButtonEngTaskSheduler_Click;
        // 
        // buttonEngPowerShell
        // 
        buttonEngPowerShell.BackColor = Color.White;
        buttonEngPowerShell.FlatStyle = FlatStyle.Flat;
        buttonEngPowerShell.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngPowerShell.Location = new Point(568, 156);
        buttonEngPowerShell.Name = "buttonEngPowerShell";
        buttonEngPowerShell.Size = new Size(180, 40);
        buttonEngPowerShell.TabIndex = 13;
        buttonEngPowerShell.Text = "POWER SHELL";
        buttonEngPowerShell.UseVisualStyleBackColor = false;
        buttonEngPowerShell.Click += ButtonEngPowerShell_Click;
        // 
        // buttonEngUpdate
        // 
        buttonEngUpdate.BackColor = Color.White;
        buttonEngUpdate.FlatStyle = FlatStyle.Flat;
        buttonEngUpdate.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngUpdate.Location = new Point(568, 202);
        buttonEngUpdate.Name = "buttonEngUpdate";
        buttonEngUpdate.Size = new Size(180, 40);
        buttonEngUpdate.TabIndex = 14;
        buttonEngUpdate.Text = "ВКЛ";
        buttonEngUpdate.UseVisualStyleBackColor = false;
        buttonEngUpdate.Click += ButtonEngUpdate_Click;
        // 
        // label2
        // 
        label2.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        label2.Location = new Point(196, 202);
        label2.Name = "label2";
        label2.Size = new Size(366, 40);
        label2.TabIndex = 15;
        label2.Text = "СЛУЖБА ОБНОВЛЕНИЙ:";
        label2.TextAlign = ContentAlignment.MiddleRight;
        // 
        // label3
        // 
        label3.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        label3.Location = new Point(196, 248);
        label3.Name = "label3";
        label3.Size = new Size(366, 40);
        label3.TabIndex = 17;
        label3.Text = "РАБОЧИЙ СТОЛ:";
        label3.TextAlign = ContentAlignment.MiddleRight;
        // 
        // buttonEngShell
        // 
        buttonEngShell.BackColor = Color.White;
        buttonEngShell.FlatStyle = FlatStyle.Flat;
        buttonEngShell.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        buttonEngShell.Location = new Point(568, 248);
        buttonEngShell.Name = "buttonEngShell";
        buttonEngShell.Size = new Size(180, 40);
        buttonEngShell.TabIndex = 16;
        buttonEngShell.Text = "EXPLORER";
        buttonEngShell.UseVisualStyleBackColor = false;
        buttonEngShell.Click += ButtonEngShell_Click;
        // 
        // timerFPS
        // 
        timerFPS.Interval = 500;
        timerFPS.Tick += TimerFPS_Tick;
        // 
        // FormSettings
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(760, 300);
        ControlBox = false;
        Controls.Add(label3);
        Controls.Add(buttonEngShell);
        Controls.Add(label2);
        Controls.Add(buttonEngUpdate);
        Controls.Add(buttonEngPowerShell);
        Controls.Add(buttonEngTaskSheduler);
        Controls.Add(buttonEngTaskManager);
        Controls.Add(buttonEngAutoLogin);
        Controls.Add(buttonEngKillExplorer);
        Controls.Add(buttonEngExplorer);
        Controls.Add(buttonEngConfig);
        Controls.Add(buttonEngControlPanel);
        Controls.Add(buttonEngCmd);
        Controls.Add(buttonEngClose);
        Controls.Add(buttonEngShutdown);
        Controls.Add(buttonEngRestart);
        Controls.Add(label1);
        Controls.Add(buttonExit);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        MaximizeBox = false;
        MaximumSize = new Size(770, 310);
        MinimizeBox = false;
        MinimumSize = new Size(770, 310);
        Name = "FormSettings";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        FormClosing += FormSettings_FormClosing;
        Shown += FormSettings_Shown;
        ResumeLayout(false);
    }

    #endregion

    private Button buttonExit;
    private Label label1;
    private Button buttonEngRestart;
    private Button buttonEngShutdown;
    private Button buttonEngClose;
    private Button buttonEngCmd;
    private Button buttonEngControlPanel;
    private Button buttonEngConfig;
    private Button buttonEngExplorer;
    private Button buttonEngKillExplorer;
    private Button buttonEngAutoLogin;
    private Button buttonEngTaskManager;
    private Button buttonEngTaskSheduler;
    private Button buttonEngPowerShell;
    private Button buttonEngUpdate;
    private Label label2;
    private Label label3;
    private Button buttonEngShell;
    private System.Windows.Forms.Timer timerFPS;
}