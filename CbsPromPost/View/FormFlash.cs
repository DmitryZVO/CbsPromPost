using CbsPromPost.Other;

namespace CbsPromPost.View;

public sealed partial class FormFlash : Form
{
    public FormFlash()
    {
        InitializeComponent();

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, прошивка и тестирование готовых изделий";
        Enabled = false;
    }
}