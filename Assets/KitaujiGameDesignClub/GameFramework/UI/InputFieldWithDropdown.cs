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

   public string text
   {
      get => inputField.text;
      set => inputField.text = value;
   }

   private void Awake()
   {
      inputField = GetComponentInChildren<TMP_InputField>();
      dropdown = GetComponentInChildren<TMP_Dropdown>();
      dropdownButton = GetComponentInChildren<Button>();
      
      dropdown.onValueChanged.AddListener(delegate(int arg0) { text = dropdown.captionText.text;  });
      dropdownButton.onClick.AddListener(delegate { dropdown.Show(); });
   }

   /// <summary>
   /// 将下拉栏内容更换
   /// </summary>
   /// <param name="optionDatas"></param>
   public void ChangeOptionDatas(List<string> optionDatas)
   {
      dropdown.ClearOptions();
      dropdown.AddOptions(optionDatas);
      dropdown.RefreshShownValue();
   }
}
