using Newtonsoft.Json;
using SocketIOClient.Packgers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Websocket.Client;
using Debug = UnityEngine.Debug;

namespace SocketIOClient.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient
    {
        private readonly IObserver<ResponseMessage> _messageReceivedObserver;
        private readonly IObserver<DisconnectionInfo> _disconnectionHappened;
        
        public WebSocketClient(SocketIO io, PackgeManager parser)
        {
            _parser = parser;
            _io = io;
            _messageReceivedObserver = new MessageReceivedObserver(_parser, _io);
            _disconnectionHappened = new DisconnectionHappenedObserver(_io);
        }

        readonly PackgeManager _parser;
        readonly SocketIO _io;
        WebsocketClient _client;
        
        
        public async Task ConnectAsync(Uri uri, WebSocketConnectionOptions options)
        {
            _client = new WebsocketClient(uri)
            {
                IsReconnectionEnabled = false,
                ReconnectTimeout = options.ConnectionTimeout
            };
            // _client.MessageReceived.Subscribe()
            _client.MessageReceived.Subscribe(_messageReceivedObserver);
            _client.DisconnectionHappened.Subscribe(_disconnectionHappened);

            await _client.Start();
        }

        public async Task SendMessageAsync(string text)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Faild to emit, websocket is not connected yet.");
            }
            await _client.SendInstant(text);
        }

        public async Task SendMessageAsync(byte[] bytes)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Faild to emit, websocket is not connected yet.");
            }
            await _client.SendInstant(bytes);
        }

        public async Task DisconnectAsync()
        {
            await _client.Stop(WebSocketCloseStatus.NormalClosure, nameof(WebSocketCloseStatus.NormalClosure));
        }
    }

    public class DisconnectionHappenedObserver : IObserver<DisconnectionInfo>
    {
        readonly SocketIO _io;

        public DisconnectionHappenedObserver(SocketIO io)
        {
            _io = io;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DisconnectionInfo disconnectionInfo)
        {
            string reason = null;
            switch (disconnectionInfo.Type)
            {
                case DisconnectionType.Exit:
                    break;
                case DisconnectionType.Lost:
                    reason = "transport close";
                    break;
                case DisconnectionType.NoMessageReceived:
                    break;
                case DisconnectionType.Error:
                    break;
                case DisconnectionType.ByUser:
                    break;
                case DisconnectionType.ByServer:
                    break;
                default:
                    break;
            }
            if (reason != null)
            {
                _io.InvokeDisconnect(reason);
            }
        }
    }
    
    public class MessageReceivedObserver : IObserver<ResponseMessage>
    {
        private PackgeManager _parser;
        private SocketIO _io;

        public MessageReceivedObserver(PackgeManager parser, SocketIO io)
        {
            _parser = parser;
            _io = io;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Debug.LogWarning(error);
        }

        public void OnNext(ResponseMessage message)
        {
            if (message.MessageType == WebSocketMessageType.Text)
            {
                _parser.Unpack(message.Text);
                Debug.Log(message.Text);
            }
            else if (message.MessageType == WebSocketMessageType.Binary)
            {
                _io.InvokeBytesReceived(message.Binary.Skip(1).ToArray());
            }

        }
    }

}
