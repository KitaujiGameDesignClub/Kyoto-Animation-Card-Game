using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class tagListItem : MonoBehaviour
{
    public TMP_Text text;

    public UnityEvent<string> onRemove = new();
}
