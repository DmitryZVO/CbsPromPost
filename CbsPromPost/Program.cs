using CbsPromPost.Other;
using CbsPromPost.View;

namespace CbsPromPost;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var mutex = new Mutex(true, "TULA_CBS_POST", out var createdNew);
        if (!createdNew)
        {
            MessageBox.Show(@"Запущен другой экземпляр программы! Продолженние не возможно....", @"ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Core.Start();

        switch (Core.Config.Type)
        {
            case 3:
                Application.Run(new FormFlash());
                break;
            case 4:
                Application.Run(new FormFlyRecord());
                break;
            case 14:
                Application.Run(new FormBadDrone());
                break;
            default:
                MessageBox.Show(@"Неизвестный тип работы!", @"ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
        }
    }
}