using CbsPromPost.Other;
using CbsPromPost.View;

namespace CbsPromPost;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var mutex = new Mutex(true, "TULA_CBS", out var createdNew);
        if (!createdNew)
        {
            MessageBox.Show(@"������� ������ ��������� ���������! ������������ �� ��������....", @"������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Core.Start();

        switch (Core.Config.Type)
        {
            case 3:
                Application.Run(new FormFlash());
                break;
            default:
                MessageBox.Show(@"����������� ��� ������!", @"������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
        }
    }
}