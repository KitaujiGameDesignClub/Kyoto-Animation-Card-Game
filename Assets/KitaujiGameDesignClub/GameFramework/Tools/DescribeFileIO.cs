using System;
using UnityEngine;

namespace KitaujiGameDesignClub.GameFramework.Tools
{
    /// <summary>
    /// 定义文件的读写路径（yamL：顺便还能加个注释）
    /// </summary>
    [Serializable]
    public struct DescribeFileIO
    {
        /// <summary>
        /// 文件名（含拓展名）
        /// </summary>
        public string FileName;
        /// <summary>
        /// 从根目录开始的相对路径（Android：从presistentData开始，以-开头的话，为绝对路径）
        /// </summary>
        public string Path;
        /// <summary>
        /// 注释（需要写入#符号）
        /// </summary>
        public string Note;

        

        /// <summary>
        /// 定义文件的读写路径（yamL：顺便还能加个注释）
        /// </summary>
        /// <param name="fileName">保存的文件名（含拓展名）</param>
        /// <param name="path">从根目录开始的相对路径（移动平台&WebGL：从presistentData开始，以-开头的话，为绝对路径），开头结尾不能有“ / ”</param>
        /// <param name="note">注释（需要写入#符号）</param>
        public DescribeFileIO(string fileName ="Default.yaml",string path = "saves",string note = null)
        {
            FileName = fileName;
            Path = path;
            Note = note;
        }

        public string pathWithFile()
        {
            return $"{Path}/{FileName}";
        }
    }

    /// <summary>
    /// 游戏基本的设置（为了兼容旧游戏）
    /// </summary>
    [Serializable]
    public struct BasicSettings
    {
        //音频设置
        public float MusicVolume;
        public float SoundEffectVolume;
       
        //视频设置
        public FullScreenMode fullscreenMode ;
        public Resolution resolution;
        public bool sync;
        public int antiAliasing;
        public bool dithering;
        
        //调试模式
        public bool showFps;
        public bool showConsole;
        
        
        /// <summary>
        /// 初始化（不用任何变量，直接初始化得了）
        /// </summary>
        /// <param name="musicVolume"></param>
        /// <param name="soundEffectVolume"></param>
        public BasicSettings(float musicVolume,float soundEffectVolume)
        {
            //默认设置
            MusicVolume = musicVolume;
            SoundEffectVolume = soundEffectVolume;
            fullscreenMode  = FullScreenMode.Windowed;
            sync = false;
            resolution = new Resolution()
            {
                height = 720,
                width = 1280,
                refreshRate = 60
            };
            antiAliasing = 1;
            dithering = false;
            showConsole = true;
            showFps = true;
        }
    }
}