using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldWithDropdown : MonoBehaviour
{
   private TMP_InputField inputField;
   private TMP_Dropdown dropdown;
   private Button dropdownButton;

    private List<string> allOptionDatas;


   public string text
   {
      get => inputField.text;
      set => inputField.text = value;
   }

    //编辑器中会显示的：

    /// <summary>
    /// 如果想要下拉栏的内容与输入栏内容相匹配，请改为true
    /// </summary>
    public bool supportSearch = false;

    private void Awake()
   {
      inputField = GetComponentInChildren<TMP_InputField>();
      dropdown = GetComponentInChildren<TMP_Dropdown>();
      dropdownButton = GetComponentInChildren<Button>();




      
      dropdown.onValueChanged.AddListener(delegate(int arg0) 
      { 
          text = dropdown.IsExpanded ? dropdown.captionText.text:text;
      });
      dropdownButton.onClick.AddListener(delegate {

          dropdown.value = -1;
         

          if (supportSearch) Search(inputField.text);

          dropdown.Show();
      });



       
   }

    /// <summary>
    /// 将下拉栏内容更换
    /// </summary>
    /// <param name="optionDatas"></param>
    /// <param name="all">要作为此下拉列表的全部内容吗</param>
    public void ChangeOptionDatas(List<string> optionDatas,bool all = true)
   {
      dropdown.ClearOptions();
      dropdown.AddOptions(optionDatas);
      dropdown.RefreshShownValue();
        if (all) allOptionDatas = optionDatas;
   }

    /// <summary>
    /// 搜索所有候选，并更新出符合关键词的下拉列表
    /// </summary>
    /// <param name="keyword"></param>
    public void Search(string keyword)
    {
        //没有关键词（没输入内容），就正常显示所有的结果
        if(keyword == "")
        {
            ChangeOptionDatas(allOptionDatas);
            return;
        }


        dropdown.Hide();
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
        if (fitted.Count == 0) fitted.Add("未搜索到结果，但仍可以使用");

        //提交上去
        ChangeOptionDatas(fitted,false);

    }

}
