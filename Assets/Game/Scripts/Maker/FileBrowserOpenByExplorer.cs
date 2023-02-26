
using System.IO;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace Maker
{
    public class FileBrowserOpenByExplorer : MonoBehaviour
    {
        public UnityEngine.UI.InputField path;

        private void Awake()
        {
            var button = GetComponent<Button>();

#if UNITY_STANDALONE || UNITY_EDITOR
            button.onClick.AddListener(delegate { Application.OpenURL($"file://{path.text}"); Debug.Log(path.text);});

#else
    Destroy(button.gameObject);
#endif
        }
    }
}