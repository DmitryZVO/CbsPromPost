namespace CbsPromPost.View;

public sealed partial class FormPrinters : Form
{
    public int Printer = -1;

    public FormPrinters()
    {
        InitializeComponent();

        button1.Click += Printer1;
        button2.Click += Printer2;
        buttonCancel.Click += Cancel;
        DialogResult = DialogResult.Cancel;
    }

    private void Printer1(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Printer = 1;
        Close();
    }

    private void Printer2(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Printer = 2;
        Close();
    }

    private void Cancel(object? sender, EventArgs e)
    {
        Printer = -1;
        Close();
    }
}