using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class audioSetting : MonoBehaviour
{
   private AudioSource audioSource;
    [SerializeField]private TMP_Text text;
    [FormerlySerializedAs("audioFullFileName")] [HideInInspector]public string newAudioFullFileName  = string.Empty;


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
        newAudioFullFileName = string.Empty;
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
        newAudioFullFileName = fileFullPath;
        text.text = $"{title}：{Path.GetFileName(newAudioFullFileName)}";
        CardMaker.cardMaker.changeSignal.SetActive(true);
    }
}
