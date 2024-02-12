namespace CbsPromPost.View;

public partial class FormIdInfo : Form
{
    private string _id;
    public FormIdInfo(string id)
    {
        _id = id;
        InitializeComponent();

        Shown += FrmShown;
    }

    private void FrmShown(object? sender, EventArgs e)
    {
    }
}