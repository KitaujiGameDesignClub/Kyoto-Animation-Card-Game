using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KitaujiGameDesignClub.GameFramework.UI.InputFieldSearch
{
    public class Item : MonoBehaviour
    {
        /// <summary>
        /// 显示的文字
        /// </summary>
        [SerializeField] 
        TMP_Text label;
        /// <summary>
        /// 用于响应点击的按钮
        /// </summary>
        [SerializeField] private Button button;
        private InputFieldSearch inputFieldSearch;

     

        private void Start()
        {
            button.onClick.AddListener(delegate
            {
                inputFieldSearch.SelectCandidate(label.text);
            });
        }

        public void Initialization(string text,InputFieldSearch inputFieldSearch)
        {
            this.inputFieldSearch = inputFieldSearch;
            label.text = text;
        }
        
        
        
   
    }
}
