using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using AppApi.Repository.Contract;
using Newtonsoft.Json;

namespace AppApi.Handlers
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var username = context.Request.Query["username"];
            //if (string.IsNullOrEmpty(username))
            //{
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    return;
            //}

            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();

            _sockets.TryAdd(socketId, currentSocket);

            //ne moze ovako sa while(true)
            while (true)
            {
                var timer = new Timer(async (state) =>
                {
                    foreach (var socket in _sockets)
                    {
                        if (socket.Value.State != WebSocketState.Open)
                        {
                            continue;
                        }

                        await SendStringAsync(socket.Value, username, ct);
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            }

            WebSocket dummy;
            _sockets.TryRemove(socketId, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }

        private static Task SendStringAsync(WebSocket socket, string username, CancellationToken ct = default(CancellationToken))
        {
            return socket.SendAsync(Encoding.UTF8.GetBytes("test1"), WebSocketMessageType.Text, true, ct);
        }

        private async Task<string> AuthenticateWebSocketConnection(HttpContext context)
        {
            var buffer = new byte[1024];
            var result = await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var message = Encoding.UTF8.GetString(buffer, 0, result);

            // Parse the message and extract the username
            var payload = JsonConvert.DeserializeObject<WebSocketAuthPayload>(message);
            return payload?.Username;
        }
    }

}

