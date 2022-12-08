using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KitaujiGameDesignClub.GameFramework.Tools
{
    /// <summary>
    /// 如果要修改FileName或是添加其他IO用变量，请继承
    /// </summary>
    public class YamlReadWrite
    {
        /// <summary>
        /// Assets上一级的目录（结尾没有/）
        /// </summary>
        /// <returns></returns>
        public static string UnityButNotAssets
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            get
            {
                string[] raw = Application.dataPath.Split("/");

                string done = string.Empty;
                for (int i = 1; i < raw.Length - 1; i++)
                {
                    done = $"{done}/{raw[i]}";
                }

                return done;
            }

#else
        get
        {
            return Application.persistentDataPath;
        }


#endif
        }


        /// <summary>
        /// 在规定的文件夹中写yaml文件
        /// </summary>
        /// <param name="profile">yamlIO文件设置</param>
        /// <param name="content">写入的内容</param>
        /// <typeparam name="T"></typeparam>
        public static async UniTask WriteAsync<T>(DescribeFileIO profile, T content)
        {
            Serializer serializer = new Serializer();
          
            //得到最终呈现在文件中的文本内容
            string authenticContent =
                $"# Only for {Application.productName}\n{profile.Note}\n\n{serializer.Serialize(content)}";

            StreamWriter streamWriter =
             new($"{GetFullPath(profile.Path)}/{profile.FileName}", false, Encoding.UTF8);

            await streamWriter.WriteAsync(authenticContent);
           await streamWriter.DisposeAsync();
            streamWriter.Close();

        }


        /// <summary>
        /// 在规定的文件夹中写yaml文件
        /// </summary>
        /// <param name="profile">yamlIO文件设置</param>
        /// <param name="content">写入的内容</param>
        /// <typeparam name="T"></typeparam>
        public static void Write<T>(DescribeFileIO profile, T content)
        {
           
            
            Serializer serializer = new Serializer();

          
            
            //得到最终呈现在文件中的文本内容
            string authenticContent =
                $"# Only for {Application.productName}\n{profile.Note}\n\n{serializer.Serialize(content)}";


            StreamWriter streamWriter =
                new StreamWriter($"{GetFullPath(profile.Path)}/{profile.FileName}", false, Encoding.UTF8);
            
            streamWriter.Write(authenticContent);
            streamWriter.Dispose();
            streamWriter.Close();
           
        }

        /// <summary>
        /// 读取yaml
        /// </summary>
        /// <param name="yaml"></param>
        /// <param name="content">读取文件的内容（作为默认值）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T>  ReadAsync<T>(DescribeFileIO yaml, T content)
        {
            Deserializer deserializer = new();

            //存在的话就读取
            StreamReader streamReader = StreamReader.Null;


            //尝试yaml文件
            try
            {
                streamReader =
                    new StreamReader($"{GetFullPath(yaml.Path)}/{yaml.FileName}", Encoding.UTF8);


                var fileContent = deserializer.Deserialize<T>(await streamReader.ReadToEndAsync());
                streamReader.Dispose();
                streamReader.Close();

              
                return fileContent;
            }
            catch (Exception)
            {
                //关闭之前的文件流，防止出现IOException: Sharing violation错误
                streamReader.Dispose();
                streamReader.Close();

                //不存在的话，初始化一个
                Debug.Log($"{yaml.Path}中不存在合规的{yaml.FileName}，已经初始化此文件");
                DescribeFileIO newFile = new DescribeFileIO(yaml.FileName, yaml.Path, yaml.Note);
               await WriteAsync(newFile, content);
                return content;
            }
        }


        /// <summary>
        /// 读取yaml
        /// </summary>
        /// <param name="yaml"></param>
        /// <param name="content">读取文件的内容（作为默认值）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Read<T>(DescribeFileIO yaml, T content)
        {
            Deserializer deserializer = new();

            //存在的话就读取
            StreamReader streamReader = StreamReader.Null;


            //尝试yaml文件
            try
            {
                streamReader =
                    new StreamReader($"{GetFullPath(yaml.Path)}/{yaml.FileName}", Encoding.UTF8);


                var fileContent = deserializer.Deserialize<T>(streamReader.ReadToEnd());
                streamReader.Dispose();
                streamReader.Close();
              
                Debug.Log($"成功加载：{yaml.Path}/{yaml.FileName}");
                return fileContent;
            }
            catch (Exception)
            {
                //关闭之前的文件流，防止出现IOException: Sharing violation错误
                streamReader.Dispose();
                streamReader.Close();

                //不存在的话，初始化一个
                Debug.Log($"{yaml.Path}中不存在合规的{yaml.FileName}，已经初始化此文件");
                DescribeFileIO newFile = new DescribeFileIO(yaml.FileName, yaml.Path, yaml.Note);
                Write(newFile, content);
                return content;
            }
        }

        
/// <summary>
/// 获取完整的，绝对路径（并创建有关的文件夹）
/// </summary>
/// <param name="path">开头有-的话，就是绝对路径；反之就是从游戏根目录开始的相对路径</param>
/// <returns></returns>
        static string GetFullPath(string path)
        {
            //实际路径（绝对路径）
            string actualPath  = String.Empty;
            //给的路径有个-  说明是绝对路径
            if (path.Substring(0, 1).Equals("-"))
            {
               
                actualPath = path.Substring(1);
            }
            //开头没有-，是从根目录开始的相对路径
            else
            {
               
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                actualPath = $"{UnityButNotAssets}/{path}";
                
#else
                actualPath = $"{Application.persistentDataPath}/{path}";
#endif
                
            }

          
            //如果文件夹不存在，则创建
            if (!Directory.Exists(actualPath))
            {
                Directory.CreateDirectory( actualPath);
            }

           
            return actualPath;
        }

       
    }
}