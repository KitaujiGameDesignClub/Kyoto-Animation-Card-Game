using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KitaujiGameDesignClub.GameFramework.UI.InputFieldSearch
{
    public class InputFieldSearch : MonoBehaviour
    {
        
            
        private TMP_InputField inputField;
        
        /// <summary>
        /// 输入框内的内容
        /// </summary>
        public string text
        {
            get => inputField.text;
            set => inputField.text = value;
        }
        
        
        /// <summary>
        /// 所有的候选对象都会在这里生成
        /// </summary>
        [SerializeField] RectTransform candidates;


        /// <summary>
        /// 候选对象模板
        /// </summary>
        [SerializeField] Item itemTemplate;

        /// <summary>
        /// 所有的候选结果
        /// </summary>
        [SerializeField] private List<string> allOptionDatas = new();

        /// <summary>
        ///  所有合适的候选
        /// </summary>
        [SerializeField] private List<string> fittedOptionDatas = new();

        /// <summary>
        /// 记录dropdown的值是否发生变化
        /// </summary>
        private bool changed;

        private void Awake()
        {
           
            inputField = GetComponent<TMP_InputField>();


            inputField.onSelect.AddListener(delegate(string arg0) { Search(arg0);});


            inputField.onValueChanged.AddListener(delegate(string arg0)
            {
                if (candidates.childCount > 0) Search(arg0);
            });


            //候选选定了（一定是从0变成别的数）
        }

        /// <summary>
        /// 初始化所有搜索候选
        /// </summary>
        /// <param name="candidate"></param>
        public void Initialization(List<string> candidate)
        {
            for (int i = 0; i < candidate.Count; i++)
            {
                allOptionDatas.Add(new string(candidate[i]));
            }
        }

        public void Search(string fieldText)
        {
         RemoveAllItems();

            if (fieldText == "")
            {
                fittedOptionDatas = allOptionDatas;
            }
            else
            {
                //所有合适的候选
                fittedOptionDatas = new List<string>();
                for (int i = 0; i < allOptionDatas.Count; i++)
                {
                    //包含输入的字符，把整个optionDatas归类为合适的候选
                    if (allOptionDatas[i].Contains(fieldText))
                    {
                        fittedOptionDatas.Add(allOptionDatas[i]);
                    }
                }
            }
            
            
            //将适合的对象呈现出来
            for (int i = 0; i < fittedOptionDatas.Count; i++)
            {
               var item = Instantiate(itemTemplate.gameObject, candidates).GetComponent<Item>();
               item.Initialization(fittedOptionDatas[i],this);
             
            }
        }

        /// <summary>
        /// 选择了一个候选对象
        /// </summary>
        public void SelectCandidate(string content)
        {
            RemoveAllItems();
            inputField.text = content;
          
        }

/// <summary>
/// 移出所有候选对象，无论输入框内有何内容
/// </summary>
        public void RemoveAllItems()
        {
            for (int i = 0; i < candidates.childCount; i++)
            {
                Debug.Log(candidates.childCount);
             DestroyImmediate(candidates.GetChild(0).gameObject);  
            }
            
     
        }
    }
}