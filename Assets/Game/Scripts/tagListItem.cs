using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class tagListItem : MonoBehaviour
{
    public TMP_Text text;
    public Button button;
    
    public UnityEvent<string> onRemove = new();
    
    
    private void Awake()
    {
        //消除此tag
       button.onClick.AddListener(delegate
       {
           CardMaker.cardMaker.changeSignal.SetActive(true);
           onRemove.Invoke(text.text);
           Destroy(gameObject);
       });
    }

    public void Initialization(string text)
    {
        gameObject.SetActive(true);
        this.text.text = text;
    }
}
