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
   
        //ע������ť������Ƶ���¼�������Ҫ��clip=null������֤
       
        
    }

    public void EnableAudioTestor(CardPanel cardPanel)
    {
        gameObject.SetActive(true);
        image.sprite = cardPanel.cardStateInGame.CoverImage;
        Name.text = $"{cardPanel.cardStateInGame.profile.FriendlyCardName}\n<size=75%>{cardPanel.cardStateInGame.profile.CardName}</size>";
        //��Ƶ��Դ���
        clips[0] = cardPanel.cardStateInGame.voiceDebut;       
        clips[1] = cardPanel.cardStateInGame.voiceAbility;
        clips[2] = cardPanel.cardStateInGame.voiceDefeat;
       
        clips[3] = cardPanel.cardStateInGame.voiceExit;
       
        //���ڲ����ڵ���Ƶ��Դ�����ð�ť�Ľ���
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
