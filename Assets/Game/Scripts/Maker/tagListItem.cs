using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace  Maker
{
    /// <summary>
    /// 卡牌编辑器中 tag 用的代码
    /// </summary>
    public class tagListItem : MonoBehaviour
    {
        public TMP_Text text;
        /// <summary>
        /// 移除按钮（仅移除显示）
        /// </summary>
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

}
