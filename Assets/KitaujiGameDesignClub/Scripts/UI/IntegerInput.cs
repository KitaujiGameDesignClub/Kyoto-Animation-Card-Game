using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    public class IntegerInput : MonoBehaviour
    {
        //Ϊ������
        public int Max;
        public int Min;

        public TMP_InputField inputField;

        public int value => int.Parse(inputField.text);

        public UnityEvent<int> onEndEdit;

        private void Start()
        {
            //��������
            inputField.onEndEdit.AddListener(delegate 
            {
                var value = Mathf.Clamp(this.value, Min, Max);
                inputField.text =(value.ToString());
                onEndEdit.Invoke(value);
            });
        }

        /// <summary>
        /// ��ť���Ӽ�����
        /// </summary>
        /// <param name="value"></param>
        public void Button(int value)
        {
            var number = this.value + value;
            number = Mathf.Clamp(number, Min, Max);

            inputField.text = (number.ToString());
        }


    }
}

