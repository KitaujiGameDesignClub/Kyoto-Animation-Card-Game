using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI gameUI;

    [SerializeField] GameObject BanInputLayer;
    [SerializeField] TMP_Text BanInputLayerText;

    private void Awake()
    {
        gameUI = this;
    }

    /// <summary>
    /// …Ë÷√ ‰»ÎΩ˚”√≤„
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="text"></param>
   public void SetBanInputLayer(bool enabled,string text)
    {
        BanInputLayer.SetActive(enabled);
        BanInputLayerText.text = text;
    }


}
