using System.ComponentModel;
using CbsPromPost.Model;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;

namespace CbsPromPost.View;

public partial class FormIdInfo : Form
{
    private readonly string _id;
    private readonly BindingList<DataGridMainRow> _dataGridMainData = new();

    public FormIdInfo(string id)
    {
        _id = id;
        InitializeComponent();

        dataGridViewMain.DataSource = new BindingSource { DataSource = _dataGridMainData };
        dataGridViewMain.AllowUserToAddRows = false;
        dataGridViewMain.AllowUserToDeleteRows = false;
        dataGridViewMain.AllowUserToResizeRows = false;
        dataGridViewMain.AllowUserToOrderColumns = false;
        dataGridViewMain.RowHeadersVisible = false;
        dataGridViewMain.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
        dataGridViewMain.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewMain.DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, };
        dataGridViewMain.Columns["UserName"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        dataGridViewMain.Columns["WorkName"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dataGridViewMain.Columns["WorkStart"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        dataGridViewMain.Columns["WorkEnd"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        dataGridViewMain.Columns["WorkTime"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        dataGridViewMain.Columns["DroneId"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        dataGridViewMain.SelectionMode = DataGridViewSelectionMode.CellSelect;
        dataGridViewMain.SetDoubleBuffered();

        Shown += FrmShown;
    }

    private void FrmShown(object? sender, EventArgs e)
    {
        _ = UpdateUsersValues();
    }

    private async Task UpdateUsersValues()
    {
        var newValues = new List<DataGridMainRow>();
        var works = Core.IoC.Services.GetRequiredService<Works>();
        var values = await Server.GetHistoryForId(_id, default);
        foreach (var p in values)
        {
            var time = Math.Max((p.TimeEnd - p.TimeStart).TotalSeconds, 1);
            var work = works.Get(p.Work.Id);
            var data = new DataGridMainRow
            {
                UserName = p.User.Name,
                DroneId = p.DroneId,
                WorkStart = p.TimeStart.ToString("yyyy.MM.dd HH:mm:ss"),
                WorkEnd = p.TimeEnd.ToString("yyyy.MM.dd HH:mm:ss"),
                WorkName = work.Name,
                WorkTime = time.ToSecTime(),
                Start = p.TimeStart.Ticks,
            };
            newValues.Add(data);
        }

        bool delete;
        do
        {
            delete = false;
            foreach (var r in _dataGridMainData)
            {
                if (newValues.Exists(x => x.Start.Equals(r.Start))) continue;
                _dataGridMainData.Remove(r);
                delete = true;
                break;
            }
        } while (delete);

        foreach (var n in newValues)
        {
            var r = _dataGridMainData.ToList().FindIndex(x => x.Start.Equals(n.Start));
            if (r < 0)
            {
                _dataGridMainData.Add(n);
            }
            else
            {
                _dataGridMainData[r] = n;
            }
        }

        dataGridViewMain.Refresh();
    }

    public class DataGridMainRow
    {
        [DisplayName("НАЧАЛО")] public string WorkStart { get; set; } = string.Empty;
        [DisplayName("КОНЕЦ")] public string WorkEnd { get; set; } = string.Empty;
        [DisplayName("СОТРУДНИК")] public string UserName { get; set; } = string.Empty;
        [DisplayName("ВИД РАБОТЫ")] public string WorkName { get; set; } = string.Empty;
        [DisplayName("ЗАТРАЧЕНО")] public string WorkTime { get; set; } = string.Empty;
        [DisplayName("ИЗДЕЛИЕ")] public string DroneId { get; set; } = string.Empty;
        [Browsable(false)] public long Start { get; set; }
    }
}