using System;
using System.Diagnostics;
using System.IO;
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
            GetHeartRateData().Wait();
        }

        static async Task GetHeartRateData()
        {
            using (var webSocket = new ClientWebSocket())
            {
                await webSocket.ConnectAsync(new Uri("ws://localhost:8080"), CancellationToken.None);

                while (webSocket.State == WebSocketState.Open)
                {
                    int heartRate = GetHeartRateFromOmnicept();
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
        }

        private static int GetHeartRateFromOmnicept(HeartRate hr)
        {
            int heartRate = 0;
            try
            {

                heartRate = Convert.ToInt32(hr.Rate.ToString() + " BPM");
            }
            catch (Exception ex)
            {
                // Log the error message
                Console.WriteLine("Error retrieving heart rate data: " + ex.Message);
            }

            return heartRate;
        }
    }
}