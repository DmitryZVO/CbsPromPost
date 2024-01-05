using System.Diagnostics;
using CbsPromPost.Model;
using CbsPromPost.Other;
using Microsoft.Win32;

namespace CbsPromPost.View;

public partial class FormSettings : Form
{
    public FormSettings()
    {
        InitializeComponent();
    }

    private void ButtonExit_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ButtonEngRestart_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(@"ВЫ УВЕРЕНЫ?", @"ПЕРЕЗАГРУЗКА", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) != DialogResult.Yes) return;
        Core.Config.Save();
        Process.Start("shutdown", "/r /f /t 0");
    }

    private void ButtonEngShutdown_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(@"ВЫ УВЕРЕНЫ?", @"ВЫКЛЮЧЕНИЕ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) != DialogResult.Yes) return;
        Core.Config.Save();
        Process.Start("shutdown", "/s /f /t 0");
    }

    private void ButtonEngClose_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(@"УВЕРЕНЫ?", @"ЗАКРЫТЬ ПРИЛОЖЕНИЕ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) != DialogResult.Yes) return;
        Owner.Close();
    }

    private void ButtonEngCmd_Click(object sender, EventArgs e)
    {
        Process.Start("cmd.exe");
    }

    private void ButtonEngControlPanel_Click(object sender, EventArgs e)
    {
        Process.Start("control");
    }

    private void ButtonEngConfig_Click(object sender, EventArgs e)
    {
        Process.Start("cmd", "/C start ms-settings:");
    }

    private void ButtonEngExplorer_Click(object sender, EventArgs e)
    {
        Process.Start("explorer");
    }

    private void ButtonEngKillExplorer_Click(object sender, EventArgs e)
    {
        Process.Start("taskkill", "/f /im explorer.exe");
    }

    private void ButtonEngAutoLogin_Click(object sender, EventArgs e)
    {
        Process.Start("cmd", "/C netplwiz");
    }

    private void ButtonEngTaskManager_Click(object sender, EventArgs e)
    {
        Process.Start("taskmgr");
    }

    private void ButtonEngTaskSheduler_Click(object sender, EventArgs e)
    {
        Process.Start("cmd", "/C taskschd.msc /s");
    }

    private void ButtonEngPowerShell_Click(object sender, EventArgs e)
    {
        Process.Start("powershell");
    }

    private void ButtonEngUpdate_Click(object sender, EventArgs e)
    {
        Core.Config.ServiceUpdateDisable = !Core.Config.ServiceUpdateDisable;
    }

    private void ButtonEngShell_Click(object sender, EventArgs e)
    {
        var shell = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
        if (shell == null) return;

        var exe = shell.GetValue("Shell")?.ToString() ?? "";
        if (exe.Equals("explorer.exe"))
        {
            shell.SetValue("Shell", "cmd /C " + "\"" + Application.StartupPath + Application.ProductName + ".exe\"");
        }
        else
        {
            shell.SetValue("Shell", "explorer.exe");
        }
    }

    private void TimerFPS_Tick(object sender, EventArgs e)
    {
        var update = false;
        try { update = Station.ServiceInfo("Центр обновления Windows") == System.ServiceProcess.ServiceControllerStatus.Running; }
        catch
        {
            // ignored
        }

        try { update = Station.ServiceInfo("Windows Update") == System.ServiceProcess.ServiceControllerStatus.Running; }
        catch
        {
            // ignored
        }

        buttonEngUpdate.BackColor = update ? Color.LightGreen : Color.LightPink;
        buttonEngUpdate.Text = update ? "РАБОТАЕТ" : "ВЫКЛЮЧЕНА";

        switch (Core.Config.ServiceUpdateDisable)
        {
            case true when update:
                try { Station.ServiceStop("Центр обновления Windows"); }
                catch
                {
                    // ignored
                }

                try { Station.ServiceStop("Windows Update"); }
                catch
                {
                    // ignored
                }

                break;
            case false when !update:
                try { Station.ServiceStart("Центр обновления Windows"); }
                catch
                {
                    // ignored
                }

                try { Station.ServiceStart("Windows Update"); }
                catch
                {
                    // ignored
                }

                break;
        }

        try
        {
            var shell = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon",
                true);
            if (shell == null) return;

            var exe = shell.GetValue("Shell")?.ToString() ?? "";
            buttonEngShell.BackColor = exe.Equals("explorer.exe") ? Color.LightPink : Color.LightGreen;
            buttonEngShell.Text = exe.Equals("explorer.exe") ? "EXPLORER" : "ЗАМЕНЕН";
        }
        catch
        {
            // ignored
        }
    }

    private void FormSettings_Shown(object sender, EventArgs e)
    {
        timerFPS.Start();
    }

    private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
    {
        timerFPS.Stop();
    }
}