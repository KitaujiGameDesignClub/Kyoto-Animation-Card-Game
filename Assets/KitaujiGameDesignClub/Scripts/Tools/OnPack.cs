


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace KitaujiGameDesignClub.GameFramework.Tools
{
    public class OnPack :IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
    
        //打包时：
        public void OnPreprocessBuild(BuildReport report)
        {
            //游戏视频设置初始化
            VideoSettingsInitialization();
            //内部版本号迭代（移动平台）
            PlayerSettings.Android.bundleVersionCode++;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; ;

        }


        #region 内部方法
        /// <summary>
        /// 游戏视频设置初始化
        /// </summary>
        private void VideoSettingsInitialization()
        {
            
            
            //编辑器将显示设置设为默认值
            //standalone
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.defaultScreenHeight = 1280;
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
        }
    

        #endregion
    }
}
#endif

