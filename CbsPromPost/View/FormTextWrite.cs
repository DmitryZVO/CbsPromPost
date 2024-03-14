namespace CbsPromPost.View;

public sealed partial class FormTextWrite : Form
{
    public string Result = string.Empty;

    public FormTextWrite(string id)
    {
        InitializeComponent();

        Shown += FrmShown;

        Text = $@"ОПИСАНИЕ БРАКА ДЛЯ ИЗДЕЛИЯ {id}";

        buttonCancel.Click += ButtonCancel_Click;
        buttonSend.Click += ButtonSend_Click;
        button1.Click += ButtonNameSend;
        button2.Click += ButtonNameSend;
        button3.Click += ButtonNameSend;
        button4.Click += ButtonNameSend;
        button5.Click += ButtonNameSend;
        button6.Click += ButtonNameSend;
        button7.Click += ButtonNameSend;
        button8.Click += ButtonNameSend;
        button9.Click += ButtonNameSend;
        button10.Click += ButtonNameSend;
        button11.Click += ButtonNameSend;
        button12.Click += ButtonNameSend;
        button13.Click += ButtonNameSend;
        button14.Click += ButtonNameSend;
        button15.Click += ButtonNameSend;
        button16.Click += ButtonNameSend;
        button17.Click += ButtonNameSend;
        button18.Click += ButtonNameSend;
        button19.Click += ButtonNameSend;
        button20.Click += ButtonNameSend;
        button21.Click += ButtonNameSend;
        button22.Click += ButtonNameSend;
        button23.Click += ButtonNameSend;
        button24.Click += ButtonNameSend;
        button25.Click += ButtonNameSend;
        button26.Click += ButtonNameSend;
        button27.Click += ButtonNameSend;
        button28.Click += ButtonNameSend;
        button29.Click += ButtonNameSend;
        button30.Click += ButtonNameSend;
        button31.Click += ButtonNameSend;
        button32.Click += ButtonNameSend;
        button33.Click += ButtonNameSend;
        button34.Click += ButtonNameSend;
        button35.Click += ButtonNameSend;
        button36.Click += ButtonNameSend;
    }

    private void ButtonNameSend(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;

        Result = button.Name;
        Close();
    }

    private void ButtonSend_Click(object? sender, EventArgs e)
    {
        Result = richTextBoxMain.Text;
        Close();
    }

    private void ButtonCancel_Click(object? sender, EventArgs e)
    {
        Result = string.Empty;
        Close();
    }

    private void FrmShown(object? sender, EventArgs e)
    {
        richTextBoxMain.Clear();
    }
}