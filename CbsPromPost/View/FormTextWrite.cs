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