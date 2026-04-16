using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UXF.UI
{
    public class LocalDirectorySelector : MonoBehaviour
    {

        public FormElement inputField;
        UIController uiController;
        string dataLocKey = "uxf_datalocation";

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            uiController = GetComponentInParent<UIController>();
        }


        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            if (PlayerPrefs.HasKey(dataLocKey))
            {
                string loc = PlayerPrefs.GetString(dataLocKey);
                if (Directory.Exists(loc))
                {
                    inputField.SetContents(loc);
                }
            }
        }

        public void SelectFolder()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            string current = (string)inputField.GetContents();
            if (string.IsNullOrEmpty(current) || !Directory.Exists(current))
            {
                current = Application.streamingAssetsPath;
            }

            try
            {
                string[] selected = SFB.StandaloneFileBrowser.OpenFolderPanel("Select data directory", current, false);
                if (selected != null && selected.Length > 0)
                    inputField.SetContents(selected[0]);
            }
            catch (System.DllNotFoundException)
            {
                Utilities.UXFDebugLogWarning(
                    "Native file browser not available (Apple Silicon incompatibility). " +
                    "Opening folder in system file explorer. Please copy your desired path and paste it into the text field.");

                OpenFolderInExplorer(current);
            }
            catch (System.Exception ex)
            {
                Utilities.UXFDebugLogErrorFormat("Failed to open folder selection dialog: {0}", ex.Message);
            }
#else
            Utilities.UXFDebugLogError("Cannot select directory unless on PC platform!");
#endif
        }


        public static void OpenFolderInExplorer(string path)
        {
            if (!Directory.Exists(path))
            {
                path = Application.streamingAssetsPath;
            }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            string winPath = path.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer.exe", "/root," + winPath);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            System.Diagnostics.Process.Start("xdg-open", path);
#endif
        }


        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            string current = (string)inputField.GetContents();
            if (Directory.Exists(current))
            {
                PlayerPrefs.SetString(dataLocKey, current);
            }
        }

    }

}
