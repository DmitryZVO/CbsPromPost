using System.IO.Ports;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public class SerialScanners
{
    private class SerialText
    {
        public string Text { get; init; } = string.Empty;
        public string Com { get; init; } = string.Empty;
    }

    private readonly List<string> _aliveSerials = new();
    private readonly List<string> _blackListSerials = new();
    private readonly Channel<SerialText> _pipe;
    private readonly ILogger<SerialScanners> _logger;
    private bool _alive;

    public event Action<string, string> OnReadValue = delegate { };
    public event Action<bool> OnAliveChange = delegate { };
    public bool IsAlive() => _alive;

    public SerialScanners(ILogger<SerialScanners> logger)
    {
        _pipe = Channel.CreateBounded<SerialText>(100);
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ReadFromChannelAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

            var allSerials = SerialPort.GetPortNames(); // список всех доступных COM-портов

            lock (_aliveSerials)
            {
                foreach (var current in allSerials)
                {
                    var running = _aliveSerials.Any(serial => current.Equals(serial));
                    if (running) continue;

                    if (_blackListSerials.Contains(current)) continue; // этот COM-порт в блеклисте

                    WriteFromSerialToChannelAsync(current, cancellationToken);
                }

                var newState = _aliveSerials.Any();
                if (newState != _alive) OnAliveChange(newState);
                _alive = newState;
            }
        }
    }

    private async void WriteFromSerialToChannelAsync(string name, CancellationToken cancellationToken)
    {
        lock (_aliveSerials)
        {
            _aliveSerials.Add(name);
        }

        var port = new SerialPort(name, 115200, Parity.None, 8, StopBits.One)
        {
            ReadTimeout = 1000,
            WriteTimeout = 1000,
        };

        try
        {
            port.Open();
            while (!cancellationToken.IsCancellationRequested) // читаем данные, пока порт открыт
            {
                await Task.Delay(100, cancellationToken);

                var bytes = port.BytesToRead;
                if (bytes <= 0) continue; // Читать нечего

                var data = new byte[bytes];
                port.Read(data, 0, bytes);

                var text = new SerialText()
                {
                    Text = Encoding.UTF8.GetString(data).Replace("\r", "").Trim(),
                    Com = name
                };
                await _pipe.Writer.WriteAsync(
                text, cancellationToken);
                _logger.LogInformation("SERIAL_{Serial}_READ: {Txt}", text.Com, text.Text);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Equals("Параметр задан неверно.") && !_blackListSerials.Contains(name)) // Добавляем в блек-лист порты, которые не являются сканерами
            {
                _blackListSerials.Add(name);
            }

            _logger.LogInformation("SERIAL_EXEPTION_{Serial}: {Ex}", name, ex.Message);
        }
        port.Close();
        port.Dispose();

        lock (_aliveSerials)
        {
            _aliveSerials.RemoveAll(x => x.Equals(name));
        }
        await Task.Delay(5000, cancellationToken);
    }

    private async void ReadFromChannelAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var st = await _pipe.Reader.ReadAsync(cancellationToken);
            OnReadValue.Invoke(st.Com, st.Text);
        }
    }
}