using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Android.OS;
using Android.Util;
using IdleBallBotClient;
using Newtonsoft.Json;
using SharedLibrary;

namespace SharedLibrary;

public class WebSocketHandler
{
    private WebSocket webSocket;
    
    public WebSocketHandler(WebSocket _webSocket)
    {
        webSocket = _webSocket;
        Receive();
        //Send(JsonConvert.SerializeObject(new Package("Echo", "Hej med dig kage")));
    }
    
    public async void Receive()
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024 * 1000];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Package? package = null;
                try
                {
                    package = JsonConvert.DeserializeObject<Package>(message);
                }
                catch (Exception e)
                {
                    
                }
                
                if (package != null)
                {
                    
                    switch (package.Action)
                    {
                        case "Echo":
                            Log.Debug("Echo", package.Message);
                            Send(JsonConvert.SerializeObject(new Package("Echo", package.Message)));
                            break;
                        case "Screenshot":
                            Log.Debug("Screenshot", "Taking screenshot");
                            byte[] imageSource = await TakeScreenshotAsync();
                            Send(JsonConvert.SerializeObject(new Package("Screenshot", Convert.ToBase64String(imageSource))));
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
    }

    public async Task<byte[]> TakeScreenshotAsync()
    {
        if (Screenshot.Default.IsCaptureSupported)
        {
            

            IScreenshotResult screen = await Screenshot.CaptureAsync();

            using MemoryStream memoryStream = new();
            await screen.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        return null;
    }

    public async void Send(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}