using UnityEngine;

[System.Serializable]
public class ChatMessage
{
    [SerializeField] private string message;
    [SerializeField] private string author;
    public string Message
    {
        get => message;
        set => message = value;
    }

    public string Author
    {
        get => author;
        set => author = value;
    }

}