using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HP.Omnicept;
using HP.Omnicept.Messaging.Messages;

namespace ReverbOmnicept
{
    class Program
    {
        static void Main(string[] args)
        {
            HostHeartRateData().Wait();
        }

        static async Task HostHeartRateData()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Listening for WebSocket connections...");
            while (true)
            {
                var context = await listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("WebSocket request received.");
                    await HandleWebSocketRequest(context);
                }
                else
                {
                    Console.WriteLine("Non-WebSocket request received, ignoring.");
                }
            }
        }

        static async Task HandleWebSocketRequest(HttpListenerContext context)
        {
            var webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;

            while (webSocket.State == WebSocketState.Open)
            {
                HeartRate hr = GetHeartRateFromOmnicept();
                int heartRate = Convert.ToInt32(hr.Rate);
                string heartRateString = heartRate.ToString();

                var buffer = Encoding.UTF8.GetBytes(heartRateString);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                Console.Write("Heart Rate: ");
                if (heartRate > 120)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(heartRate);
                    Console.ResetColor();
                }
                else if (heartRate > 100)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(heartRate);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(heartRate);
                }

                string fileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                using (var logFile = File.AppendText(fileName))
                {
                    logFile.WriteLine("[{0}] Heart Rate: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), heartRate);
                }

                Thread.Sleep(1000);
            }
        }

        public static HeartRate GetHeartRateFromOmnicept()
        {
            int heartRate = 0;
            HeartRate hr = null;
            try
            {
                heartRate = Convert.ToInt32(hr.Rate.ToString() + " BPM");
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving heart rate data: " + ex.Message);
            }

            return hr;

        }
    }
}