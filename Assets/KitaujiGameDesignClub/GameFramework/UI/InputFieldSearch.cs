using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldSearch : MonoBehaviour
{
  [Header("提供下拉栏，选择候选结果")]
   [SerializeField] private TMP_Dropdown dropdown;
    [Header("玩家输入，并确定最终值的地方")]
    [SerializeField] private TMP_InputField inputField;

    public string text
    {
        get => inputField.text;
        set => inputField.text = value;
    }
    
    /// <summary>
    /// 所有的候选结果
    /// </summary>
    private  List<TMP_Dropdown.OptionData> allOptionDatas = new();
    /// <summary>
    ///  所有合适的候选
    /// </summary>
    private  List<TMP_Dropdown.OptionData> fittedOptionDatas = new();

    /// <summary>
    /// 记录dropdown的值是否发生变化
    /// </summary>
    private bool changed;
    
    private void Awake()
    {
        dropdown.gameObject.SetActive(false);
        dropdown.ClearOptions();

        //输入框被选定时，显示候选下拉栏
      inputField.onSelect.AddListener(delegate(string arg0)
      {
          Search(arg0);//进行一次搜索，使候选内容与目前的输入内容一致
          Debug.Log(arg0);
          dropdown.gameObject.SetActive(true);
        
      });
      
         //输入文本，进行搜索，并更新下拉栏
        inputField.onValueChanged.AddListener(Search);
      
        //输入框失去焦点，关闭搜索框
        inputField.onEndEdit.AddListener(delegate(string arg0)
        {
            dropdown.gameObject.SetActive(false);

            //如果是0的话，说明没选择候选，清空输入栏
            if (dropdown.value == 0)
            {
                inputField.text = string.Empty;
            }
        });
        
        
        //候选选定了（一定是从0变成别的数）
        dropdown.onValueChanged.AddListener(delegate(int arg0)
        {
            inputField.text = fittedOptionDatas[arg0].text;
        });
    }

    private void Start()
    {
        dropdown.value = 0;
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
        if (fittedOptionDatas.Count != 0)
        {
            fittedOptionDatas.Insert(0,new TMP_Dropdown.OptionData("下拉搜索..."));
        }
        else
        {
            fittedOptionDatas.Insert(0,new TMP_Dropdown.OptionData("无任何候选结果"));
        }
        dropdown.options = fittedOptionDatas;
        dropdown.value = 0;

    }
}
