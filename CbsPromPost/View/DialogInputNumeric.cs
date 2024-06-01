namespace CbsPromPost.View;

public sealed partial class DialogInputNumeric : Form
{
    public long Value { get; set; }
    private readonly long _min;
    private readonly long _max;
    public DialogInputNumeric(string prefix, long min, long max, long value)
    {
        _min = min;
        _max = max;
        Value = value;
        InitializeComponent();

        textBoxPrefix.Text = prefix;
        DialogResult = DialogResult.Cancel;
        Text = $@"ВВЕДИТЕ ЗНАЧЕНИЕ [{_min:0}..{_max:0}]";
        textBoxMain.Text = Value.ToString("0");
    }

    private void DialogInputString_Shown(object sender, EventArgs e)
    {
        textBoxMain.Focus();
    }

    private void TextBoxMain_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
            return;
        }
        if (e.KeyCode != Keys.Enter) return;
        ExitAsync();
    }

    private async void ExitAsync()
    {
        if (long.TryParse(textBoxMain.Text, out var value))
        {
            Value = Math.Min(Math.Max(_min, value), _max);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            for (var i = 0; i < 10; i++)
            {
                textBoxMain.BackColor = textBoxMain.BackColor == Color.WhiteSmoke ? Color.Pink : Color.WhiteSmoke;
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
            textBoxMain.BackColor = Color.WhiteSmoke;
        }
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        ExitAsync();
    }
}