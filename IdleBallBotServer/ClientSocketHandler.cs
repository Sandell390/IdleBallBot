using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary;

namespace SharedLibrary;

public class ClientWebSocketHandler
{
    private WebSocket webSocket;
    
    bool hasSentSymmetricKey = false;
    public ClientWebSocketHandler(WebSocket _webSocket)
    {
        webSocket = _webSocket;
        Receive();
        Send(JsonConvert.SerializeObject(new Package("Echo", "Hej med dig kage")));
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
                            Console.WriteLine(package.Message);
                            Thread.Sleep(1000);
                            Send(JsonConvert.SerializeObject(new Package("Screenshot", package.Message)));
                            break;
                        case "Screenshot":
                            Console.WriteLine(package.Message);
                            File.WriteAllBytes("image.png",Convert.FromBase64String(package.Message));
                            Thread.Sleep(10000);
                            Send(JsonConvert.SerializeObject(new Package("Screenshot", "Screenshot")));
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
    
    public async void Send(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}