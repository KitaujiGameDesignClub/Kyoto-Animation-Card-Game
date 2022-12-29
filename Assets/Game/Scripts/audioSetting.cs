using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class audioSetting : MonoBehaviour
{
   private AudioSource audioSource;
    [SerializeField]private TMP_Text text;
    [HideInInspector]public string audioFullFileName;


    /// <summary>
    /// 其他音频设置用的audiosource，同一个group中只能播放一个音频
    /// </summary>
    public audioSetting[] groups;

    public GameObject playingSignal;
    
    
    public string title;

    public UnityEvent<audioSetting> OnPrepareToSelectAudio = new();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Stop();
    }

    public void playAudio()
    {
        if (audioSource.isPlaying)
        {
            Stop();
        }
        else if(audioSource.clip != null)
        {
            audioSource.Play();
            playingSignal.SetActive(true);

            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].Stop();
            }
        }
        
        
    }

    public void Stop()
    {
        audioSource.Stop();
        playingSignal.SetActive(false);
    }

    public void clear()
    {
        audioSource.clip = null;
        audioFullFileName = String.Empty;
        text.text = $"{title}：无音频";
        Stop();
    }

    public void select()
    {
        Stop();
       OnPrepareToSelectAudio.Invoke(this);
    }

    public void AudioSelected(AudioClip audioClip,string fileFullPath)
    {
        audioSource.clip = audioClip;
        audioFullFileName = fileFullPath;
        text.text = $"{title}：{Path.GetFileName(audioFullFileName)}";
        
    }
}
