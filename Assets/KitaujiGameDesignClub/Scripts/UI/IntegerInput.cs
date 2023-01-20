using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    public class IntegerInput : MonoBehaviour
    {
        //Ϊ������
        public int Max;
        public int Min;

        public TMP_InputField inputField;

        public int value => int.Parse(inputField.text);

        private void Start()
        {
            //����ڲ��ڷ�Χ��
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

