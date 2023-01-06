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
    AudioSource audioSource;
    [SerializeField]private TMP_Text text;
    /// <summary>
    /// 英文标准名称（不含拓展名）
    /// </summary>
    public string VoiceName;
    [FormerlySerializedAs("audioFullFileName")] [HideInInspector]public string newAudioFullFileName  = string.Empty;


    /// <summary>
    /// 其他音频设置用的audiosource，同一个group中只能播放一个音频
    /// </summary>
    public audioSetting[] groups;

    public GameObject playingSignal;
    
    
    public string title;

    public UnityEvent<audioSetting> OnPrepareToSelectAudio = new();

    private void Start()
    {
       if(audioSource == null) audioSource = GetComponent<AudioSource>();
       if (audioSource.clip == null) clear();
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
        if(audioSource != null)  audioSource.Stop();
        playingSignal.SetActive(false);
    }

    public void clear()
    {
        if(audioSource != null)   audioSource.clip = null;
        newAudioFullFileName = string.Empty;
        text.text = $"{title}：\n<size=80%>无音频</size>";
        Stop();
    }

    public void select()
    {
        Stop();
       OnPrepareToSelectAudio.Invoke(this);
    }

    /// <summary>
    /// 选择好音频了，加载进来
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="fileFullPath"></param>
    public void AudioSelected(AudioClip audioClip,string fileFullPath)
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        newAudioFullFileName = fileFullPath;
        text.text = $"{title}：\n<size=80%>{Path.GetFileName(newAudioFullFileName)}</size>";
        CardMaker.cardMaker.changeSignal.SetActive(true);
    }
}
