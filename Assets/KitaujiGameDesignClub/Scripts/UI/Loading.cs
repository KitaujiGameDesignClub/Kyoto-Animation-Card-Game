using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using KitaujiGameDesignClub.GameFramework.Tools;
using Cysharp.Threading.Tasks;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    public class Loading : MonoBehaviour
    {
        public static void LoadScene(int sceneId)
        {

            loadScene(sceneId);
        }

        // Start is called before the first frame update
        static async UniTask loadScene(int sceneId)
        {
            SceneManager.LoadScene(3);

            await UniTask.DelayFrame(1);

            if (PublicAudioSource.publicAudioSource != null) PublicAudioSource.publicAudioSource.StopMusicPlaying();

            Settings.SaveSettings();
            GC.Collect();
            await Resources.UnloadUnusedAssets();
            GC.Collect();
            await Resources.UnloadUnusedAssets();

            await SceneManager.LoadSceneAsync(sceneId);
        }
    }
}