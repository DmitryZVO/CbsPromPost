using CbsPromPost.Other;

namespace CbsPromPost.View;

public sealed partial class FormFlash : Form
{
    public FormFlash()
    {
        InitializeComponent();

        Text = $@"[�� ���] ���� �{Core.Config.PostNumber:0}, �������� � ������������ ������� �������";
        Enabled = false;
    }
}