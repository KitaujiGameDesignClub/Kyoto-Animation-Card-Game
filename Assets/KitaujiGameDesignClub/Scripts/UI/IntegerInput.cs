using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    public class IntegerInput : MonoBehaviour
    {
        //为闭区间
        public int Max;
        public int Min;

        public TMP_InputField inputField;

        public int value => int.Parse(inputField.text);

        private void Start()
        {
            //检查在不在范围内
            inputField.onEndEdit.AddListener(delegate 
            {
                inputField.SetTextWithoutNotify(Mathf.Clamp(value, Min, Max).ToString());
            });
        }

        public void Button(int value)
        {
            var number = this.value + value;
            number = Mathf.Clamp(number, Min, Max);

            inputField.SetTextWithoutNotify(number.ToString());
        }


    }
}

