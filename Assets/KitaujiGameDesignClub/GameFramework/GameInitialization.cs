
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace KitaujiGameDesignClub.GameFramework
{
    public class GameInitialization : MonoBehaviour
    {
        public int loadScene;
        public int openingScene;


#if UNITY_EDITOR
        public bool textGame = true;
#endif

        /// <summary>
        /// 在这里游戏初始化（在这里要对各种所需的yaml进行读取尝试，如果读取失败，就生成一个有默认值的文件）
        /// </summary>
        [ContextMenu("游戏初始化")]
        public virtual void Awake()
        {
            
            //游戏视频设置（Editor一侧）初始化
            VideoSettingsInitialization();
            
            
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            OnDemandRendering.renderFrameInterval = 1;


            //读取游戏基础设置文件（如果文件不存在或者不合规，会重置）
            Settings.ReadSettings();
        
        Debug.Log("初始化方法Awake执行");
        }

        /// <summary>
        /// 在这里游戏初始化（在这里要对各种所需的yaml进行读取尝试，如果读取失败，就生成一个有默认值的文件）
        /// </summary>
        public virtual void Start()
        {
            //调整音量
            PublicAudioSource.publicAudioSource.UpdateMusicVolume();


            //加载场景
#if !UNITY_EDITOR
          SceneManager.LoadScene("Opening");

#else

            SceneManager.LoadScene(textGame ? loadScene : openingScene);
#endif
            
            
            Debug.Log("初始化方法Start执行");
        }


/// <summary>
/// 游戏视频设置（Editor一侧）初始化
/// </summary>
        private void VideoSettingsInitialization()
        {
            
           
#if UNITY_EDITOR
            //编辑器将显示设置设为默认值
            //standalone
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.defaultIsNativeResolution = true;
            PlayerSettings.allowFullscreenSwitch = true;
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3,false);
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4,false);
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10,false);
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9,true);
            PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers,false);
            //android
            PlayerSettings.Android.renderOutsideSafeArea = false;
            PlayerSettings.Android.fullscreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.Android.resizableWindow = false;
            PlayerSettings.Android.maxAspectRatio = 1.86f;//16:9
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.useAnimatedAutorotation = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
#endif
        }
    }
}