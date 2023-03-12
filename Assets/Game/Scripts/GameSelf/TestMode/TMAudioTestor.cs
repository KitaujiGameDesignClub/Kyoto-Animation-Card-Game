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

    public void EnableAudioTestor(CardPanel cardPanel)
    {
        gameObject.SetActive(true);
        image.sprite = cardPanel.CoverImage;
        Name.text = $"{cardPanel.Profile.FriendlyCardName}\n<size=75%>{cardPanel.Profile.CardName}</size>";
        //��Ƶ��Դ���
        clips[0] = cardPanel.voiceDebut;       
        clips[1] = cardPanel.voiceAbility;
        clips[2] = cardPanel.voiceDefeat;
       
        clips[3] = cardPanel.voiceExit;
       
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
