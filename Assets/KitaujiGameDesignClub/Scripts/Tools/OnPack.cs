


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
            //内部版本号迭代（移动平台）
            PlayerSettings.Android.bundleVersionCode++;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; ;

        }

        
    }
}
#endif

