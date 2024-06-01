using CbsPromPost.Model;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using Color = System.Drawing.Color;

namespace CbsPromPost.View;

public sealed partial class FormBadPrice : Form
{
    public int Dec { get; private set; } // Сумма штрафа
    public int Inc { get; private set; } // Сумма оплаты
    public string DecUserName { get; private set; } = string.Empty; // ФИО сотрудника для штрафа

    public FormBadPrice(string id)
    {
        InitializeComponent();

        Shown += FrmShown;

        Text = $@"СТОИМОСТЬ РЕМОНТА ИЗДЕЛИЯ {id}";

        buttonCancel.Click += ButtonCancel_Click;
        buttonSend.Click += ButtonSend_Click;

        buttonD0.Click += ButtonD0;
        buttonD1.Click += ButtonD1;
        buttonD2.Click += ButtonD2;
        buttonD3.Click += ButtonD3;
        buttonD4.Click += ButtonD4;
        buttonD5.Click += ButtonD5;
        buttonD6.Click += ButtonD6;

        buttonI0.Click += ButtonI0;
        buttonI1.Click += ButtonI1;
        buttonI2.Click += ButtonI2;
        buttonI3.Click += ButtonI3;
        buttonI4.Click += ButtonI4;
        buttonI5.Click += ButtonI5;
        buttonI6.Click += ButtonI6;

        comboBox1.SelectedIndexChanged += NameChange;
    }

    private void NameChange(object? sender, EventArgs e)
    {
        DecUserName = comboBox1.SelectedItem?.ToString() ?? string.Empty;
        RefreshButtons();
    }

    private void ButtonD0(object? sender, EventArgs e)
    {
        Dec = 0;
        RefreshButtons();
    }

    private void ButtonD1(object? sender, EventArgs e)
    {
        Dec = 100;
        RefreshButtons();
    }
    private void ButtonD2(object? sender, EventArgs e)
    {
        Dec = 200;
        RefreshButtons();
    }
    private void ButtonD3(object? sender, EventArgs e)
    {
        Dec = 300;
        RefreshButtons();
    }
    private void ButtonD4(object? sender, EventArgs e)
    {
        Dec = 500;
        RefreshButtons();
    }
    private void ButtonD5(object? sender, EventArgs e)
    {
        Dec = 750;
        RefreshButtons();
    }
    private void ButtonD6(object? sender, EventArgs e)
    {
        Dec = 1000;
        RefreshButtons();
    }

    private void ButtonI0(object? sender, EventArgs e)
    {
        Inc = 0;
        RefreshButtons();
    }

    private void ButtonI1(object? sender, EventArgs e)
    {
        Inc = 50;
        RefreshButtons();
    }
    private void ButtonI2(object? sender, EventArgs e)
    {
        Inc = 100;
        RefreshButtons();
    }
    private void ButtonI3(object? sender, EventArgs e)
    {
        Inc = 150;
        RefreshButtons();
    }
    private void ButtonI4(object? sender, EventArgs e)
    {
        Inc = 200;
        RefreshButtons();
    }
    private void ButtonI5(object? sender, EventArgs e)
    {
        Inc = 250;
        RefreshButtons();
    }
    private void ButtonI6(object? sender, EventArgs e)
    {
        Inc = 300;
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        buttonD0.BackColor = Color.White;
        buttonD1.BackColor = Color.White;
        buttonD2.BackColor = Color.White;
        buttonD3.BackColor = Color.White;
        buttonD4.BackColor = Color.White;
        buttonD5.BackColor = Color.White;
        buttonD6.BackColor = Color.White;

        switch (Dec)
        {
            case 0:
                buttonD0.BackColor = Color.Yellow;
                break;
            case 100:
                buttonD1.BackColor = Color.Yellow;
                break;
            case 200:
                buttonD2.BackColor = Color.Yellow;
                break;
            case 300:
                buttonD3.BackColor = Color.Yellow;
                break;
            case 500:
                buttonD4.BackColor = Color.Yellow;
                break;
            case 750:
                buttonD5.BackColor = Color.Yellow;
                break;
            case 1000:
                buttonD6.BackColor = Color.Yellow;
                break;
        }

        buttonI0.BackColor = Color.White;
        buttonI1.BackColor = Color.White;
        buttonI2.BackColor = Color.White;
        buttonI3.BackColor = Color.White;
        buttonI4.BackColor = Color.White;
        buttonI5.BackColor = Color.White;
        buttonI6.BackColor = Color.White;

        switch (Inc)
        {
            case 0:
                buttonI0.BackColor = Color.Yellow;
                break;
            case 50:
                buttonI1.BackColor = Color.Yellow;
                break;
            case 100:
                buttonI2.BackColor = Color.Yellow;
                break;
            case 150:
                buttonI3.BackColor = Color.Yellow;
                break;
            case 200:
                buttonI4.BackColor = Color.Yellow;
                break;
            case 250:
                buttonI5.BackColor = Color.Yellow;
                break;
            case 300:
                buttonI6.BackColor = Color.Yellow;
                break;
        }
    }

    private void ButtonSend_Click(object? sender, EventArgs e)
    {
        var f = new FormYesNo(@"ДАННЫЕ ВЕРНЫ?", Color.LightGreen, Color.DarkRed, new Size(200, 100));
        if (f.ShowDialog(this) != DialogResult.Yes) return;

        DialogResult = DialogResult.OK;
        Close();
    }

    private void ButtonCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Abort;
        Close();
    }

    private async void FrmShown(object? sender, EventArgs e)
    {
        await Core.IoC.Services.GetRequiredService<Users>().UpdateAsync(default);
        var users = Core.IoC.Services.GetRequiredService<Users>().Items.FindAll(x => x.State == 0 && !x.Name.Equals(string.Empty));
        foreach (var u in users)
        {
            comboBox1.Items.Add(u.Name);
        }

        if (users.Count > 0) comboBox1.SelectedIndex = 0;

        RefreshButtons();
    }
}