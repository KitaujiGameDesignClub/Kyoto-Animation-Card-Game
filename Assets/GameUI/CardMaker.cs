using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class CardMaker : MonoBehaviour
{
    public SimpleFileBrowser.FileBrowser.OnSuccess OnSuccessLoadOrCreateManifest;
    public SimpleFileBrowser.FileBrowser.OnCancel OnManifestLoadOrCreateCancel;


    private void Start()
    {
        FileBrowser.HideDialog(true);
    }



    public void CreateBundle()
    {
        
       
    }

    public void ShowEditor()
    {
        
    }
}
