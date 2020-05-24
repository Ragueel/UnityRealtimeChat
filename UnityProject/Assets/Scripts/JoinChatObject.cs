using UnityEngine;

[System.Serializable]
public class JoinChatObject
{
    [SerializeField] private string username;
    [SerializeField] private string room;

    public string Username
    {
        get => username;
        set => username = value;
    }

    public string Room
    {
        get => room;
        set => room = value;
    }
}