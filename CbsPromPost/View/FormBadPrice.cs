using CbsPromPost.Model;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using Color = System.Drawing.Color;

namespace CbsPromPost.View;

public sealed partial class FormBadPrice : Form
{
    public int Dec { get; private set; } // Сумма штрафа
    public int Inc { get; private set; } // Сумма оплаты
    public List<string> DecUserName { get; private set; } = new(); // ФИО сотрудника для штрафа

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
        comboBox2.SelectedIndexChanged += NameChange;
        comboBox3.SelectedIndexChanged += NameChange;
        comboBox4.SelectedIndexChanged += NameChange;
        comboBox5.SelectedIndexChanged += NameChange;
    }

    private void NameChange(object? sender, EventArgs e)
    {
        var c1 = comboBox1.SelectedItem?.ToString() ?? string.Empty;
        var c2 = comboBox2.SelectedItem?.ToString() ?? string.Empty;
        var c3 = comboBox3.SelectedItem?.ToString() ?? string.Empty;
        var c4 = comboBox4.SelectedItem?.ToString() ?? string.Empty;
        var c5 = comboBox5.SelectedItem?.ToString() ?? string.Empty;
        if (DecUserName.FindAll(x => x.Equals(c1)).Count == 0) DecUserName.Add(c1);
        if (DecUserName.FindAll(x => x.Equals(c2)).Count == 0) DecUserName.Add(c2);
        if (DecUserName.FindAll(x => x.Equals(c3)).Count == 0) DecUserName.Add(c3);
        if (DecUserName.FindAll(x => x.Equals(c4)).Count == 0) DecUserName.Add(c4);
        if (DecUserName.FindAll(x => x.Equals(c5)).Count == 0) DecUserName.Add(c5);

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
        comboBox1.Items.Add(string.Empty);
        comboBox2.Items.Add(string.Empty);
        comboBox3.Items.Add(string.Empty);
        comboBox4.Items.Add(string.Empty);
        comboBox5.Items.Add(string.Empty);
        foreach (var u in users)
        {
            comboBox1.Items.Add(u.Name);
            comboBox2.Items.Add(u.Name);
            comboBox3.Items.Add(u.Name);
            comboBox4.Items.Add(u.Name);
            comboBox5.Items.Add(u.Name);
        }

        comboBox1.SelectedIndex = 0;
        comboBox2.SelectedIndex = 0;
        comboBox3.SelectedIndex = 0;
        comboBox4.SelectedIndex = 0;
        comboBox5.SelectedIndex = 0;

        RefreshButtons();
    }
}