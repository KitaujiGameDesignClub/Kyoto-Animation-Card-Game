using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace KitaujiGameDesignClub.GameFramework.Tools
{
    /// <summary>
    /// 常用的一些工具
    /// </summary>
    public class CommonTools 
    {
        /// <summary>
        /// 去除非法字符
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static string CleanInvalid(string strIn)
        {
            return Regex.Replace(strIn, "[^\\w\\.@-]", " ");
        }
        
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

        /// <summary>
        /// 【PR标记点专用】将有好的时间线转化为电脑可以用的（视频帧数） 
        /// </summary>
        public static int ConvertFriendlyToReadable(int videoFps, string friendlyContent, int lag)
        {
            //00:00:00:00
            string[] fix = friendlyContent.Split(':');
            return int.Parse(fix[3]) + int.Parse(fix[2]) * videoFps + int.Parse(fix[1]) * 60 * videoFps + lag;
        }
        
    }
}
