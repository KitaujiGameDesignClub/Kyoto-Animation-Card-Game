using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMAudioTestor : MonoBehaviour
{
    public Button[] playAudioButtons = new Button[4];
    public Image image;
    public TMP_Text Name;
    public AudioSource audioSource;

    private AudioClip[] clips = new AudioClip[4];

    private void Start()
    {
   
        //注册点击按钮播放音频的事件。不需要对clip=null进行验证
       
        
    }

    public void EnableAudioTestor(CardPanel cardPanel)
    {
        gameObject.SetActive(true);
        image.sprite = cardPanel.cardStateInGame.CoverImage;
        Name.text = $"{cardPanel.cardStateInGame.profile.FriendlyCardName}\n<size=75%>{cardPanel.cardStateInGame.profile.CardName}</size>";
        //音频资源添加
        clips[0] = cardPanel.cardStateInGame.voiceDebut;       
        clips[1] = cardPanel.cardStateInGame.voiceAbility;
        clips[2] = cardPanel.cardStateInGame.voiceDefeat;
       
        clips[3] = cardPanel.cardStateInGame.voiceExit;
       
        //对于不存在的音频资源，禁用按钮的交互
        for (int i = 0; i < 4; i++)
        {

            var clip = clips[i];
            playAudioButtons[i].onClick.AddListener(delegate
            {
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            });

            playAudioButtons[i].interactable = clips[i] != null;
        }
       
    }
}
