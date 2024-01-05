namespace CbsPromPost.View;

public sealed partial class FormYesNo : Form
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Color _foreColor;

    public FormYesNo(string text, Color backColor, Color foreColor, Size formSize)
    {
        InitializeComponent();

        labelInfo.Text = text;
        BackColor = backColor;
        Size = formSize;
        _foreColor = foreColor;
        _timer = new System.Windows.Forms.Timer();
    }

    private void FormInfo_Shown(object sender, EventArgs e)
    {
        _timer.Interval = 200;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (ActiveForm != this) Activate();
        labelInfo.ForeColor = labelInfo.ForeColor == _foreColor ? Color.Black : _foreColor;
    }

    private void FormInfo_FormClosing(object sender, FormClosingEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
    }

    private void ButtonNo_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.No;
        Close();
    }

    private void ButtonYes_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Yes;
        Close();
    }
}