using SharedLibrary;
using System.Net;

Console.Title = "Server";
List<ClientWebSocketHandler> clientHandlers = new List<ClientWebSocketHandler>();
var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls("http://192.168.0.11:6666");
var app = builder.Build();
app.UseWebSockets();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
        {
            clientHandlers.Add(new ClientWebSocketHandler(webSocket));
            while (true)
            {

            }
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});
await app.RunAsync();
