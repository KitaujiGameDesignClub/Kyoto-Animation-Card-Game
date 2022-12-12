using KitaujiGameDesignClub.GameFramework.Tools;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    /// <summary>
    /// 负责读取并应用基础设置（UI部分）
    /// </summary>
    public class ApplyUISettings : MonoBehaviour
    {
        //视频设置
        public TMP_Dropdown fullScreenMode;
        public TMP_Dropdown resolution;

        public TMP_Dropdown antiAliasing;
        public LeanToggle sync;

        public LeanToggle dithering;

        //音频设置
        public Slider MusicVolSlider;

        public Slider EffectVolSlider;

        //调试模式
        public LeanToggle showFps;
        public LeanToggle showConsole;


        // Start is called before the first frame update
        void Start()
        {
            //注册事件，滑条更新音量
            MusicVolSlider.onValueChanged.AddListener(delegate(float arg0)
            {
                Settings.BasicSettingsContent.MusicVolume = arg0;
                if (PublicAudioSource.publicAudioSource != null)
                    PublicAudioSource.publicAudioSource.UpdateMusicVolume();
            });
            EffectVolSlider.onValueChanged.AddListener(delegate(float arg0)
            {
                Settings.BasicSettingsContent.SoundEffectVolume = arg0;
            });
        }


        /// <summary>
        /// 从内存中读取并应用设置，并修改设置界面
        /// </summary>
        [ContextMenu("从内存中读取设置，并修改设置界面")]
        public void ReadSettingsFromMemoryAndApplyToSettingsPage()
        {
            //android禁用组件
#if UNITY_ANDROID
             fullScreenMode.interactable = false;
            resolution.interactable = false;
#endif
           
            
            
            //视频设置
            fullScreenMode.value = (int)Settings.BasicSettingsContent.fullscreenMode == 1 ? 0:1;


//本机所有分辨率
            var machineResoultion = Screen.resolutions;
            /*
//内存中为0或者是超过了当前目前的分辨率，则将游戏的分辨率设置为比较安全的720p（仅仅是把设置文件改了）
            if (Settings.BasicSettingsContent.resolution.width == 0 |
                Settings.BasicSettingsContent.resolution.width > Screen.currentResolution.width)
            {
              //  Screen.SetResolution(1280, 720, fullScreenMode.value == 1,60);
                Settings.BasicSettingsContent.resolution = new Resolution()
                {
                    height = 1280,
                    refreshRate = 60,
                    width = 720,
                };
                

            }
            //内存中是合法的分辨率，则应用
            else
            {
              Screen.SetResolution(Settings.BasicSettingsContent.resolution.width,Settings.BasicSettingsContent.resolution.height,fullScreenMode.value == 1,60);
            }
            */

            //设置下拉框的初始值
            resolution.value = 0;
            for (int i = 0; i < machineResoultion.Length; i++)
            {
                //如果是16：9@60Hz，则视为可用分辨率，加到下拉框里
                if (Mathf.Abs((float)machineResoultion[i].width / machineResoultion[i].height - 1.77f) <= 0.1f && machineResoultion[i].refreshRate == 60)
                {
                    //此分辨率添加到下拉框
                    resolution.options.Add(new TMP_Dropdown.OptionData(
                        $"{machineResoultion[i].width.ToString()} x {machineResoultion[i].height.ToString()}"));
                }
            }

            antiAliasing.value = (int)Settings.BasicSettingsContent.antiAliasing;
            sync.Set(Settings.BasicSettingsContent.sync);
            dithering.Set(Settings.BasicSettingsContent.dithering);
            MusicVolSlider.value = Settings.BasicSettingsContent.MusicVolume;
            EffectVolSlider.value = Settings.BasicSettingsContent.SoundEffectVolume;
            showConsole.Set(Settings.BasicSettingsContent.showConsole);
            showFps.Set(Settings.BasicSettingsContent.showFps);
        }

        [ContextMenu("从文件中读取设置,并修改设置界面")]
        public void ReadSettingsFromFileAndApplyToSettingsPage()
        {
            Settings.ReadSettings();
            ReadSettingsFromMemoryAndApplyToSettingsPage();
        }


        [ContextMenu("写入文件并应用设置")]
        public void WriteAndApplySettings()
        {
            Settings.BasicSettingsContent.dithering = dithering;
            if (fullScreenMode.value == 0)
            {
                Settings.BasicSettingsContent.fullscreenMode = FullScreenMode.FullScreenWindow;
            }
            else
            {
                Settings.BasicSettingsContent.fullscreenMode = FullScreenMode.Windowed;
            }
           
            if (resolution.value != 0)
            {
                var res = resolution.options[resolution.value].text.Split(" x ");
                Settings.BasicSettingsContent.resolution = new Resolution()
                {
                    height = int.Parse(res[0]),
                    width = int.Parse(res[1]),
                    refreshRate = 60,
                };
                
            }

            Settings.BasicSettingsContent.sync = sync.On;
            Settings.BasicSettingsContent.antiAliasing = antiAliasing.value; //其他场景相机自动读取
            //相机一起修改了
            
            Settings.BasicSettingsContent.showConsole = showConsole.On; //此设置适用于游戏场景。进入游戏场景时自动读取此设置，并应用
            Settings.BasicSettingsContent.showFps = showFps.On; //此设置适用于游戏场景。进入游戏场景时自动读取此设置，并应用
            Settings.BasicSettingsContent.MusicVolume = MusicVolSlider.value; //此设置有onValueChanged事件直接修改音量。其他场景会自动读取
            Settings.BasicSettingsContent.SoundEffectVolume =
                EffectVolSlider.value; //此设置有onValueChanged事件直接修改音量。其他场景会自动读取

            //写入yaml 
            YamlReadWrite.Write(Settings.BasicSettingIO, Settings.BasicSettingsContent);
            //应用设置
            Settings.ApplySettings();
        }
        
        
        public void InitializeSettings()
        {
            Settings.InitializeSettings();
          
        
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
    
    
   
}