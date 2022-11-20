using System.Collections.Generic;
using UnityEngine;

namespace KitaujiGameDesignClub.GameFramework.Tools
{
    public class CommonTools 
    {
        /// <summary>
        /// 将数组转化为列表list
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ListArrayConversion<T>(T[] array)
        {
            var list = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }
        
        /// <summary>
        /// 将列表list转化为数组
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] ListArrayConversion<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            
            var array = new T[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = list[i];
            }

            return array;
        }
    }
}
