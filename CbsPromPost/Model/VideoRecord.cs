using CbsPromPost.Other;
using OpenCvSharp;

namespace CbsPromPost.Model;

public class VideoRecord
{
    public const int MinimalSecondAfterFiles = 5;
    public bool Writing { get; private set; }

    private DateTime _lastFrameTime = DateTime.MinValue;
    private VideoWriter _wr = new(string.Empty, -1, 0, new OpenCvSharp.Size());

    public void ChangeWriting(bool write)
    {
        lock (_wr)
        {
            Writing = write;
            if (Writing) return;
            if (!_wr.IsDisposed) _wr.Dispose();
        }
    }

    public void FrameAdd(string id, Mat mat)
    {
        if (!Directory.Exists(Core.Config.DirRecords)) Directory.CreateDirectory(Core.Config.DirRecords);
        if (id.Equals(string.Empty)) return;
        var path = Core.Config.DirRecords + id;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        if (!Writing) return;
        if (mat.Sum().Val0 > 75000000) return; // Проверка на синий цвет
        lock (_wr)
        {
            if ((DateTime.Now - _lastFrameTime).TotalSeconds > MinimalSecondAfterFiles) // Обновляем файл записи
            {
                if (!_wr.IsDisposed) _wr.Dispose();

                _wr = new VideoWriter(path + "\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss_fff") + ".avi",
                    FourCC.FromString("XVID"),
                    20, new OpenCvSharp.Size(640, 480));
            }
            _wr.Write(mat);
            _lastFrameTime = DateTime.Now;
        }
    }
}