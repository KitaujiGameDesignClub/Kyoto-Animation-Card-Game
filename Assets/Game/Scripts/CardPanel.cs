using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core;
using System;

public class CardPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text cardName;
    public TMP_Text cv;
    public TMP_Text description;
    public SpriteRenderer image;

    public Sprite defaultImage;

    public void UpdateBasicInformation(string cardName,string cv, string description,Sprite image)
    {
        this.cardName.text = cardName ?? throw new ArgumentNullException(nameof(cardName));
        this.cv.text = cv ?? throw new ArgumentNullException(nameof(cv));
        this.description.text = description ?? throw new ArgumentNullException(nameof(description));
        this.image.sprite = image == null ? defaultImage : image;
    }
}
