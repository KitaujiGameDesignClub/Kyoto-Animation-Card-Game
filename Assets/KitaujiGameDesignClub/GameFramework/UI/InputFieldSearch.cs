using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldSearch : MonoBehaviour
{



    public string text
    {
        get => inputField.text;
        set => inputField.text = value;
    }

    /// <summary>
    /// 提供下拉栏，选择候选结果
    /// </summary>
    private TMP_Dropdown dropdown;
    ///并确定最终值的地方
    private TMP_InputField inputField;


    /// <summary>
    /// 所有的候选结果
    /// </summary>
   [SerializeField] private  List<TMP_Dropdown.OptionData> allOptionDatas = new();
    /// <summary>
    ///  所有合适的候选
    /// </summary>
   [SerializeField] private  List<TMP_Dropdown.OptionData> fittedOptionDatas = new();

    /// <summary>
    /// 记录dropdown的值是否发生变化
    /// </summary>
    private bool changed;
    
    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        dropdown = GetComponentInChildren<TMP_Dropdown>();

        dropdown.ClearOptions();

        inputField.onSelect.AddListener(delegate (string arg0) 
        {
            dropdown.Show();

        });


        inputField.onValueChanged.AddListener(Search);

     

      

        //候选选定了（一定是从0变成别的数）
       
        

      
    }

    /// <summary>
    /// 初始化所有搜索候选
    /// </summary>
    /// <param name="candidate"></param>
    public void Initialization(List<string> candidate)
    {
       
        dropdown.ClearOptions();
        
        for (int i = 0; i < candidate.Count; i++)
        {
            allOptionDatas.Add(new TMP_Dropdown.OptionData(candidate[i]));
            
        }
      
    }

    public void Search(string fieldText)
    {
       
        dropdown.ClearOptions();

        if (fieldText == "")
        {
            fittedOptionDatas = allOptionDatas;
          
        }
        else
        {
            //所有合适的候选
            fittedOptionDatas = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < allOptionDatas.Count; i++)
            {
                //包含输入的字符，把整个optionDatas归类为合适的候选
                if (allOptionDatas[i].text.Contains(fieldText))
                {
                    fittedOptionDatas.Add(allOptionDatas[i]);
                }
            }
      }


        //候选筛选完成，把候选结果传递给dropdown
       
        dropdown.AddOptions(fittedOptionDatas);
        
        dropdown.RefreshShownValue();
       
      
    }
}
