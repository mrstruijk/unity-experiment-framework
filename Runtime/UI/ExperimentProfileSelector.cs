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
    public enum PathSelector
    {
        PersistentDataPath,
        StreamingAssets
    }

    public class ExperimentProfileSelector : MonoBehaviour
    {

        public FormElement dropdown;
        UIController uiController;
        string profileKey = "uxf_profile";

        public string profilePath { get; private set; }

        public PathSelector pathSelector = PathSelector.PersistentDataPath;

        void Awake()
        {
            uiController = GetComponentInParent<UIController>();

            if (pathSelector == PathSelector.PersistentDataPath)
            {
                profilePath = Application.persistentDataPath;
            }
            else if (pathSelector == PathSelector.StreamingAssets)
            {
                profilePath = Application.streamingAssetsPath;
                Debug.Log("Please note that there can only be 1 StreamingAssetsPath, which needs to be directly in 'Assets'");
            }
            else
            {
                Debug.LogWarning("No other path has been configured yet");
            }
        }


        void Start()
        {
            Populate();
        }


        public void Populate(bool retry = true)
        {
            if (!Directory.Exists(profilePath))
            {
                Utilities.UXFDebugLogWarning($"SettingsProfile path {profilePath} folder was moved or deleted! Creating a new one.");

                Directory.CreateDirectory(profilePath);
            }

            var profileNames = Directory.GetFiles(profilePath, uiController.settingsSearchPattern)
                .Select(f => Path.GetFileName(f))
                .ToList();

            if (profileNames.Count > 0)
            {
                if (PlayerPrefs.HasKey(profileKey))
                {
                    string profile = PlayerPrefs.GetString(profileKey);
                    if (profileNames.Contains(profile))
                    {
                        int profileIdx = profileNames.IndexOf(profile);
                        Tuple<int, IEnumerable<string>> data = new Tuple<int, IEnumerable<string>>(
                            profileIdx, profileNames
                        );

                        dropdown.SetContents(data); // pass tuple
                    }
                    else
                    {
                        dropdown.SetContents(profileNames);
                    }
                }
                else
                {
                    dropdown.SetContents(profileNames);
                }
            }
            else
            {
                string newName = uiController.settingsSearchPattern.Replace("*", "my_experiment");
                string newPath = Path.Combine(profilePath, newName);
                if (!File.Exists(newPath))
                {
                    File.WriteAllText(newPath, "{\n}");
                    Utilities.UXFDebugLogWarningFormat("No profiles found in {0} that match search pattern {1}. A blank one called {2} has been made for you.", profilePath, uiController.settingsSearchPattern, newName);
                    if (retry)
                        Populate(retry: false);
                }
            }
        }


        public void ShowFolder()
        {
            string path = profilePath;

            if (!Directory.Exists(path))
            {
                Utilities.UXFDebugLogWarningFormat("Directory does not exist: {0}", path);
                return;
            }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            string winPath = path.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer.exe", "/root," + winPath);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            System.Diagnostics.Process.Start("xdg-open", path);
#else
            Utilities.UXFDebugLogError("Cannot open directory unless on PC platform!");
#endif
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            string current = (string)dropdown.GetContents();
            PlayerPrefs.SetString(profileKey, current);
        }
    }
}
