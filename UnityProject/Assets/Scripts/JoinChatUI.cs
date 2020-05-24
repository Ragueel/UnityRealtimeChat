using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinChatUI : MonoBehaviour
{
    [SerializeField] private InputField _nameField;
    [SerializeField] private Canvas _canvas;


    public void OnJoinClick()
    {
        if (_nameField.text.Length == 0)
        {
            return;
        }

        Data.Name = _nameField.text;
        _canvas.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}