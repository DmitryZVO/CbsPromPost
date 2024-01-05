using Timer = System.Windows.Forms.Timer;

namespace CbsPromPost.View;

public sealed partial class FormInfo : Form
{
    private readonly Timer _timer;
    private readonly Color _foreColor;
    private readonly DateTime _start;
    private readonly double _wait;

    public FormInfo(string text, Color backColor, Color foreColor, double msWait, Size formSize)
    {
        InitializeComponent();

        labelInfo.Text = text;
        BackColor = backColor;
        Size = formSize;
        _foreColor = foreColor;
        _start = DateTime.Now;
        _wait = msWait;
        _timer = new Timer();
    }

    private void FormInfo_Shown(object sender, EventArgs e)
    {
        _timer.Interval = 200;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        labelInfo.ForeColor = labelInfo.ForeColor == _foreColor ? Color.Black : _foreColor;
        if ((DateTime.Now - _start).TotalMilliseconds > _wait) Close();
    }

    private void FormInfo_FormClosing(object sender, FormClosingEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
    }
}