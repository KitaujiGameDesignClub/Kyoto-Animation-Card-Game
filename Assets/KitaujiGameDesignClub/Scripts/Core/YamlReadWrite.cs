using System;
using System.Globalization;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KitaujiGameDesignClub.GameFramework
{
    /// <summary>
    /// 如果要修改FileName或是添加其他IO用变量，请继承
    /// </summary>
    public class YamlReadWrite
    {
        /// <summary>
        /// PC:Assets上一级的目录（结尾没有/） Andorid:Application.persistentDataPath
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

                return Path.GetFullPath(done);
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
                $"# Only for {Application.productName} with utf-8\n{profile.Note}\n\n{serializer.Serialize(content)}";

            StreamWriter streamWriter =
                new($"{GetFullDirectory(profile.Path)}/{profile.FileName}", false, Encoding.UTF8);

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
        public static void WriteString(DescribeFileIO profile, string content)
        {
           
            StreamWriter streamWriter =
                new($"{GetFullDirectory(profile.Path)}/{profile.FileName}", false, Encoding.UTF8);

            streamWriter.Write(content);
            streamWriter.Dispose();
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
                $"# Only for {Application.productName} with utf-8\n{profile.Note}\n\n{serializer.Serialize(content)}";

            StreamWriter streamWriter =
                new($"{GetFullDirectory(profile.Path)}/{profile.FileName}", false, Encoding.UTF8);

            streamWriter.Write(authenticContent);
             streamWriter.Dispose();
            streamWriter.Close();
        }
       

        /// <summary>
        /// 读取yaml
        /// </summary>
        /// <param name="yaml">IO</param>
        /// <param name="content">读取文件的内容（作为默认值）</param>
        /// <param name="createIfFileNotExit">如果文件不存在，则在返回默认值的时候顺便创建一个文件。文件内容为默认值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> ReadAsync<T>(DescribeFileIO yaml, T content, bool createIfFileNotExit = true)
        {
            var directory = GetFullDirectory(yaml.Path);
            var fileName = yaml.FileName;

            if (fileName.Substring(0, 1) == "*")
            {
                fileName = FindOneMatchedFile(fileName.Substring(1), directory);            
            }

            Deserializer deserializer = new();

            //存在的话就读取
            StreamReader streamReader = StreamReader.Null;


            //尝试yaml文件
            try
            {
                streamReader =
                    new StreamReader($"{directory}/{fileName}", Encoding.UTF8);


                var fileContent = deserializer.Deserialize<T>(await streamReader.ReadToEndAsync());
                streamReader.Dispose();
                streamReader.Close();

                Debug.Log($"成功加载：{directory}/{fileName}");
                return fileContent;
            }
            catch (Exception e)
            {
                //关闭之前的文件流，防止出现IOException: Sharing violation错误
                streamReader.Dispose();
                streamReader.Close();


                if (File.Exists($"{directory}/{yaml.FileName}"))
                {
                    //文件损坏，备份原文件先，然后弄个新的
                    File.Move($"{directory}/{yaml.FileName}",
                        $"{directory}/{yaml.FileName} - {System.DateTime.Now:yyyy-MM-dd-HH-mm-ss}.bak");
                    Debug.LogWarning($"{directory}中的“{yaml.FileName}”已损坏，此文件已备份，并创建了新文件。损坏原因：{e}");
                }
                else
                {
                    //不存在的话，初始化一个
                  if(createIfFileNotExit)  Debug.LogWarning($"{directory}中不存在“{yaml.FileName}”，已创建此文件。");
                    else Debug.LogWarning($"{directory}中不存在“{yaml.FileName}”");
                }


                if(createIfFileNotExit)
                {
                    await WriteAsync(yaml, content);
                 
                }
             
                return content;
            
               
            }
        }

        /// <summary>
        /// 读取yaml
        /// </summary>
        /// <param name="yaml">IO</param>
        /// <param name="defaultContent">读取文件的内容（作为默认值）</param>
        /// <param name="createIfFileNotExit">如果文件不存在，则在返回默认值的时候顺便创建一个文件。文件内容为默认值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Read<T>(DescribeFileIO yaml, T defaultContent, bool createIfFileNotExit = true) =>
            Read(yaml, defaultContent, out int e, createIfFileNotExit);



         /// <summary>
        /// 读取yaml
        /// </summary>
        /// <param name="yaml">IO</param>
        /// <param name="defaultContent">读取文件的内容（作为默认值）</param>
        /// <param name="errorCode">0表示一切正常。-1文件存在但损坏 -2文件不存在 </param>
        /// <param name="createIfFileNotExit">如果文件不存在，则在返回默认值的时候顺便创建一个文件。文件内容为默认值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Read<T>(DescribeFileIO yaml, T defaultContent, out int errorCode,bool createIfFileNotExit = true)
        {
            var directory = GetFullDirectory(yaml.Path);
            var fileName = yaml.FileName;

            if (fileName.Substring(0, 1) == "*")
            {
                fileName = FindOneMatchedFile(fileName.Substring(1), directory);            
            }

            Deserializer deserializer = new();

            //存在的话就读取
            StreamReader streamReader = StreamReader.Null;


            //尝试yaml文件
            try
            {
                streamReader = new StreamReader($"{directory}/{fileName}", Encoding.UTF8);

                var fileContent = deserializer.Deserialize<T>(streamReader.ReadToEnd());
                streamReader.Dispose();
                streamReader.Close();

                Debug.Log($"成功加载：{directory}/{fileName}");
                errorCode = 0;
                return fileContent;
            }
            catch (Exception e)
            {
                //关闭之前的文件流，防止出现IOException: Sharing violation错误
                streamReader.Dispose();
                streamReader.Close();

                if (File.Exists($"{directory}/{yaml.FileName}"))
                {
                    //文件损坏，备份原文件
                    File.Move($"{directory}/{yaml.FileName}",
                        $"{directory}/{yaml.FileName} - {System.DateTime.Now:yyyy-MM-dd-HH-mm-ss}.bak");
                    Debug.LogWarning($"{directory}中的“{yaml.FileName}”已损坏，此文件已备份。损坏原因：{e}");

                    errorCode = -1;
                }
                else
                {
                    //不存在
                  Debug.LogWarning($"{directory}中不存在“{yaml.FileName}”");


                    errorCode = -2;
                }


                if(createIfFileNotExit)
                {
                     Write(yaml, defaultContent);
                }
             
                return defaultContent;
            
               
            }
        }


        /// <summary>
        /// 获取完整的，绝对路径（并创建有关的文件夹）
        /// </summary>
        /// <param name="path">开头有-的话，就是绝对路径；反之就是从游戏根目录开始的相对路径</param>
        /// <returns></returns>
        static string GetFullDirectory(string path)
        {
            //实际路径（绝对路径）
            string actualPath;
            //给的路径有个-  说明是绝对路径
            if (path.Substring(0, 1).Equals("-"))
            {
                actualPath = path.Substring(1);
            }
            //开头没有-，是从根目录开始的相对路径
            else
            {
                actualPath = $"{UnityButNotAssets}/{path}";
            }


            //如果文件夹不存在，则创建
            if (!Directory.Exists(actualPath))
            {
                Directory.CreateDirectory(actualPath);
            }


            return actualPath;
        }

        /// <summary>
        /// 找到一个拓展名匹配的文件
        /// </summary>
        /// <param name="extension">".拓展名"的形式</param>
        /// <returns></returns>
        private static string FindOneMatchedFile(string extension,string findPath)
        {
            // 获取查找路径下所有的文件
            DirectoryInfo dirInfo = new DirectoryInfo(findPath);
            FileInfo[] files = dirInfo.GetFiles();

            foreach (var t in files)
            {
                if (t.Extension == extension)
                {
                    return t.Name;
                }
            }

            return string.Empty;

        }
    }
}