using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KitaujiGameDesignClub.GameFramework.Tools
{

    [CreateAssetMenu(fileName = "BasicEvents.asset", menuName = "KitaujiGD/Events/Basic Events",order = 102)]
    public  class BasicEvents :ScriptableObject
    {
        public int gameScene;
        public int LoadScene;
        public int OpeningScene;

     
    
    
    
    


        public void ExitGame()
        {
            Settings.SaveSettings();
        
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    
    
        public void StartGame()
        {
            clickSound();
            SceneManager.LoadScene(LoadScene);//load这边已经停止音乐播放了
        }

        public  void clickSound()
        {
            if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.PlaySoundEffect(PublicAudioSource.AudioType.Click);
        }

        public  void OpenWeb()
        {
            clickSound();
            Application.OpenURL("https://kitaujigamedesign.top");
        }
    
        public  void ReturnToTitle()
        {
            if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.StopMusicPlaying();
            clickSound();
            SceneManager.LoadScene(OpeningScene);
            Settings.SaveSettings();
        }

        public void PlayAgain()
        {
            if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.StopMusicPlaying();
            clickSound();
            SceneManager.LoadScene(LoadScene);
            Settings.SaveSettings();
        }

        [ContextMenu("发送一个DEBUG信息（错误等级不定）")] 
        public void Debug()
        {
            Debug("EasterEggs");
        }


        public void Debug(string text = "EasterEggs")
        {
            if (text == "EasterEggs")
            {
                string content = String.Empty;
                switch (UnityEngine.Random.Range(0,5))
                {
                    case 0:
                        content = "この银河を统括する情报统合思念体によって造られた対有机生命体コンタクト用ヒューマノイド．インターフェース。それが、わたし";
                        break;
                
                    case 1:
                        content = "危机が迫るとしたら...まず、あなた";
                        break;
                
                    case 2:
                        content = "最初からわたししかいない";
                        break;
                
                    case 3:
                        content = "情报の伝えるのに齟齬が生じるかもしれない。でも、闻いて";
                        break;
                
                    case 4:
                        content = "それがわたしがここにいる理由、あなたがここにいる理由。信じて。";
                        break;
                }

                text = $"YUKI.N > {content} ";
            }
        
            UnityEngine.Debug.Log(text);

      
        }



    
    }
}
