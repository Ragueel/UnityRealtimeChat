using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private SocketIOClientMono _clientMono;
        [SerializeField] private InputField _textField;
        
        public void OnSendClick()
        {
            if (_textField.text.Trim().Length == 0)
            {
                return;
            }
            _clientMono.SendChatMessage(new ChatMessage
            {
                Message = _textField.text,
                Author = Data.Name
            });
            _textField.text = "";
        }
    }
}