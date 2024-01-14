using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CbsPromPost.Other;

public class RelayPower
{
    public bool Alive { get; private set; } // Статус реле
    private readonly string _ip = Core.Config.RelayPowerIp;
    private const int Port = 8283;
    private int _k1;
    private int _k2;

    public class StateRelay
    {
        public int K1;
        public int K2;
    }

    public async void StartAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

            var currState = UpdateState();
            if (currState == null)
            {
                Alive = false;
            }
            else
            {
                Alive = true;
                _k1 = currState.K1;
                _k2 = currState.K2;
            }
        }
    }

    public void SetValues(int k1, int k2)
    {
        if (_k1 == k1 && _k2 == k2) return; // Значения уже установленны

        var udp = new UdpClient();
        var snd = Encoding.ASCII.GetBytes("admin admin k1=" + k1.ToString("0") + " k2=" + k2.ToString("0") + " ");
        udp.Send(snd, snd.Length, _ip, Port);
        udp.Close();
        udp.Dispose();
    }

    private StateRelay? UpdateState()
    {
        try
        {
            var ret = new StateRelay();
            var http = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(1000) 
            };
            var result0 = http.GetAsync(@"http://" + _ip + @"/pstat.xml").Result;
            if (result0.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var ss = result0.Content.ReadAsStringAsync().Result;
            ss = ss.Replace("<response>", "");
            ss = ss.Replace("</response>", "");
            ss = ss.Replace("<rl0string>", "");
            ss = ss.Replace("<rl1string>", "");
            ss = ss.Replace("</rl1string>", ",");
            ss = ss.Replace("</rl0string>", ",");
            var spl = ss.Split(',');
            ret.K1 = Convert.ToInt32(spl[0]);
            ret.K2 = Convert.ToInt32(spl[1]);
            return ret;
        }
        catch
        {
            return null;
        }
    }

}