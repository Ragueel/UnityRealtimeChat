using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SocketIOClientMono : MonoBehaviour
{
    [Header("Chat")]
    [SerializeField] private Transform _chatMessagesParentTransform;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private GameObject _chatMessagePrefab;
    
    [Header("Room")]
    [SerializeField] private Transform _roomUserParentTransform;
    [SerializeField] private GameObject _roomUserPrefab;
    
    private SocketIO _client;
    private Queue<ChatMessage> _chatMessages = new Queue<ChatMessage>();
    private JoinChatObject _joinedUser;
    private Queue<RoomUser> _roomUsers = new Queue<RoomUser>();
    private async void Start()
    {
        // Ideally it should be moved to separate method
        // where it would be initialized with some parameters
        _joinedUser = new JoinChatObject
        {
            Username = Data.Name,
            Room = "room_1"
        };
        _client = new SocketIO("http://localhost:5010");
        
        // Subscribe to connection event to adjust emit events
        _client.OnConnected += OnConnected;
    
        await _client.ConnectAsync();
    }

    private async void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected");
        
        // When new messages appear
        _client.On("messages", HandleNewMessage);
        
        // Current users in the room
        _client.On("usersCount", HandleUsersInRoomChanged);
        
        // Join is called to connect to the room and add new user to some sort of database
        await _client.EmitAsync("join", response =>
        {
            print("Hello");
        }, JsonUtility.ToJson(_joinedUser));
    }

    private void HandleUsersInRoomChanged(SocketIOResponse obj)
    {
        // Lock users queue so no modifications will appear
        lock (_roomUsers)
        {
            List<RoomUser> roomUsers = obj.GetValue<List<RoomUser>>();
            _roomUsers.Clear();
            
            
            foreach (RoomUser roomUser in roomUsers)
            {
                _roomUsers.Enqueue(roomUser);
            }
        }
    }

    private void Update()
    {
        // Add new chat messages
        lock (_chatMessages)
        {
            if (_chatMessages.Count > 0)
            {
                ChatMessage chatMessage = _chatMessages.Dequeue();
                AddChatMessageToUI(chatMessage);
            }
        }

        // Update current userslist
        lock (_roomUsers)
        {
            if (_roomUsers.Count > 0)
            {
                for (int i = 0; i < _roomUserParentTransform.childCount; i++)
                {
                    Destroy(_roomUserParentTransform.GetChild(i));
                }

                for (int i = 0; i < _roomUsers.Count; i++)
                {
                    RoomUser roomUser = _roomUsers.Dequeue();
                    AddUserToRoomList(roomUser);   
                }
            }
        }
        
        // Scroll to the bottom
        _scrollbar.value = 0;
    }

    private void HandleNewMessage(SocketIOResponse response)
    {
        ChatMessage chatMessage = response.GetValue<ChatMessage>();
        
        // Lock new message and enqueue it so we could use it from main thread and add it to UI
        lock (_chatMessages)
        {
            _chatMessages.Enqueue(chatMessage);
        }
    }

    /// <summary>
    /// Creates UI gameobject from prefab and assigns text
    /// </summary>
    /// <param name="chatMessage"></param>
    private void AddChatMessageToUI(ChatMessage chatMessage)
    {
        GameObject chatMessageObject = Instantiate(_chatMessagePrefab, _chatMessagesParentTransform);
        chatMessageObject.GetComponent<Text>().text = chatMessage.Author;
        chatMessageObject.transform.GetChild(0).GetComponent<Text>().text = chatMessage.Message;
    }

    /// <summary>
    /// Creates UI for users currently in the room
    /// </summary>
    /// <param name="roomUser"></param>
    private void AddUserToRoomList(RoomUser roomUser)
    {
        GameObject chatMessageObject = Instantiate(_roomUserPrefab, _roomUserParentTransform);
        chatMessageObject.GetComponent<Text>().text = roomUser.Username;
    }
    
    public async void SendChatMessage(ChatMessage chatMessage)
    {
        if (_client.Connected)
        { 
            await _client.EmitAsync("chatMessage", OnMessageSent, JsonUtility.ToJson(chatMessage));
        }
        else
        {
            Debug.LogWarning("Not connected can't send message");
        }
    }

    /// <summary>
    /// Release connections
    /// </summary>
    private async void OnDestroy()
    {
        await _client.DisconnectAsync();
    }

    private void OnMessageSent(SocketIOResponse obj)
    {
        Debug.Log(obj.GetValue<string>());
    }
}

public static class Data
{
    public static string Name = "Chad";
}