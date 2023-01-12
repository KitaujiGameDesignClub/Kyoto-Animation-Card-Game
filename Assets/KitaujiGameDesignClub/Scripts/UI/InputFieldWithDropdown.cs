using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    public class InputFieldWithDropdown : MonoBehaviour
    {
        public TMP_InputField inputField;
       [SerializeField] private TMP_Dropdown dropdown;
       [SerializeField] private Button dropdownButton;

        private List<string> allOptionDatas = new List<string>();


        public string text
        {
            get => inputField.text;
            set => inputField.text = value;
            
        }

        public bool interactable
        {
            get => inputField.interactable;
            set
            {
                inputField.interactable = value;
                dropdownButton.interactable = value;
            }
        }

        public List<TMP_Dropdown.OptionData> options => dropdown.options;


        public TMP_Dropdown.DropdownEvent  onDropdownValueChanged => dropdown.onValueChanged;

        public UnityEvent onDropdownValueChangedWithoutInt = new();

        public int DropdownValue => dropdown.value;

        //编辑器中会显示的：

        /// <summary>
        /// 如果想要筛选功能，请改为true
        /// </summary>
        [FormerlySerializedAs("supportSearch")] [Header("候选筛选功能")]
        public bool supportFilter = false;

        /// <summary>
        /// 禁用内容。被禁用的内容不会被选择进入输入框内
        /// </summary>
        [Header("被禁用的内容不会被选择")] public List<string> ban = new();

        private void Awake()
        {
            
          //  inputField = GetComponentInChildren<TMP_InputField>();
          //  dropdown = GetComponentInChildren<TMP_Dropdown>();
           // dropdownButton = GetComponentInChildren<Button>();
            //占位符也加入禁选列表内
            ban.Add("未搜索到结果，但仍可以使用");
            ban.Add("以下为候选结果：");
            ban.Add("以下为全部可用内容：");

            //选定某一项
            dropdown.onValueChanged.AddListener(delegate(int arg0)
            {
                //所选不在禁用列表里，才执行后续的操作
                if (ban == null)
                {
                    //没展开value变化的话，保留之前的内容
                    text = dropdown.IsExpanded ? dropdown.captionText.text : text;
                    text = Regex.Replace(text, "<.*?>", string.Empty);//去除富文本

                }
                
                if (!ban.Contains(dropdown.captionText.text))
                {
                    //没展开value变化的话，保留之前的内容
                    text = dropdown.IsExpanded ? dropdown.captionText.text : text;
                    text = Regex.Replace(text, "<.*?>", string.Empty);//去除富文本
                }

                onDropdownValueChangedWithoutInt.Invoke();
              
            });
            
            //打开下拉框，并按需进行搜索
            dropdownButton.onClick.AddListener(delegate
            {               
                    if (supportFilter) Search(inputField.text);
                    dropdown.SetValueWithoutNotify(0);//便于调用onValueChanged
                    dropdown.Show();              
              
            });
        }

        /// <summary>
        /// 将下拉栏内容更换
        /// </summary>
        /// <param name="optionDatas"></param>
        /// <param name="all">要作为此下拉列表的全部内容吗</param>
        public void ChangeOptionDatas(string[] optionDatas, bool all = true) =>
            ChangeOptionDatas(Tools.CommonTools.ListArrayConversion(optionDatas), all);
        
        /// <summary>
        /// 将下拉栏内容更换
        /// </summary>
        /// <param name="optionDatas"></param>
        /// <param name="all">要作为此下拉列表的全部内容吗</param>
        public void ChangeOptionDatas(List<TMP_Dropdown.OptionData> optionDatas, bool all = true)
        {
            List<string> s = new List<string>();
            for (int i = 0; i < optionDatas.Count; i++)
            {
                s.Add(optionDatas[i].text);
            }
            ChangeOptionDatas(s, all);
        }
          
        
        /// <summary>
        /// 将下拉栏内容更换
        /// </summary>
        /// <param name="optionDatas"></param>
        /// <param name="all">要作为此下拉列表的全部内容吗</param>
        public void ChangeOptionDatas(List<string> optionDatas, bool all = true)
        {
            dropdownButton.interactable = true;
            dropdown.ClearOptions();
          if(optionDatas != null)   dropdown.AddOptions(optionDatas);
            dropdown.RefreshShownValue();
            if (all) allOptionDatas = optionDatas;
        }

        
        /// <summary>
        /// 禁用下拉栏
        /// </summary>
        public void Ban()
        {
            dropdown.Hide();
            dropdownButton.interactable = false;
            allOptionDatas = null;
            dropdown.ClearOptions();
        }

        public void ClearOptions() => dropdown.ClearOptions();
       


        /// <summary>
        /// 编辑某一个候选的值
        /// </summary>
        /// <param name="newText"></param>
        public void EditOptionText(int index, string newText)
        {
            dropdown.options[index].text = newText;
            allOptionDatas[index] = newText;
        }

        /// <summary>
        /// 搜索所有候选，并更新出符合关键词的下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        public void Search(string keyword)
        {
            dropdown.Hide();

            //没有关键词（没输入内容），就正常显示所有的结果
            if (keyword == "")
            {
                ChangeOptionDatas(allOptionDatas);
                var head = new TMP_Dropdown.OptionData("以下为全部可用内容：");
                dropdown.options.Insert(0, head);
                return;
            }


            List<string> fitted = new();

            //将有这个关键词的内容作为适合的候选
            for (int i = 0; i < allOptionDatas.Count; i++)
            {
                if (allOptionDatas[i].Contains(keyword))
                {
                    fitted.Add(allOptionDatas[i]);
                }
            }

            //如果没有候选，加一个占位符，表明真的没有可用的
            if (fitted.Count == 0)
            {
                fitted.Add("未搜索到结果，但仍可以使用");
            }
            //如果有的话，加一个value=0的头，占位子
            else
            {
                fitted.Insert(0, "以下为候选结果：");
            }

            //提交上去
            ChangeOptionDatas(fitted, false);
        }

        public override string ToString()
        {
            return text;
        }
    }
}