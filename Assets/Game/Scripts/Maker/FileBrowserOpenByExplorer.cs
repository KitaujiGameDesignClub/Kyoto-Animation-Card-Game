using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileBrowserOpenByExplorer : MonoBehaviour
{
    public UnityEngine.UI.Text path;

    private void Awake()
    {
       var button =  GetComponent<Button>();

#if UNITY_STANDALONE || UNITY_EDITOR
        button.onClick.AddListener(delegate { Application.OpenURL($"file://{path.text}"); });

#else
    Destroy(button.gameObject);
#endif
    }


}
