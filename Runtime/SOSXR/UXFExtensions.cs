using System;
using System.Collections.Generic;
using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    public static class UXFExtensions
    {
        public static bool IsInitialised(this Session session)
        {
            if (session.blocks == null || session.blocks.Count == 0)
            {
                Debug.LogWarning("Session has not yet been initialised: we infer this, because there are currently no Blocks registered with the SessioN");

                return false;
            }

            return true;
        }


        public static Trial GetCurrentTrialInBlock(this Block block)
        {
            if (!block.session.IsInitialised())
            {
                return null;
            }

            if (block.session.currentTrialNum == 0 || block.session.CurrentTrial == null || block.session.CurrentTrial.number == 0)
            {
                return null;
            }

            var currentTrial = block.session.CurrentTrial;

            if (currentTrial == null)
            {
                return null;
            }

            foreach (var trial in block.trials)
            {
                if (trial == currentTrial)
                {
                    return trial;
                }
            }

            return null;
        }


        public static int GetCurrentRelativeTrialNumber(this Block block)
        {
            var currentTrial = GetCurrentTrialInBlock(block);
            return currentTrial != null ? currentTrial.numberInBlock : -1;
        }


        public static Trial GetFirstTrialInBlock(this Block block)
        {
            return block.firstTrial;
        }


        public static Trial GetLastTrialInBlock(this Block block)
        {
            return block.lastTrial;
        }


        public static bool IsFirstTrialInBlock(this Trial trial)
        {
            return trial == trial.block?.firstTrial;
        }


        public static bool IsLastTrial(this Session session)
        {
            if (!session.IsInitialised())
            {
                return false;
            }

            if (session.currentTrialNum == 0)
            {
                return false;
            }

            if (session.CurrentTrial == null)
            {
                return false;
            }

            return session.CurrentTrial == session.LastTrial;
        }


        public static bool IsLastTrialInBlock(this Trial trial)
        {
            return trial == trial.block?.lastTrial;
        }


        public static bool TrialInProgress(this Session session)
        {
            if (!session.IsInitialised())
            {
                return false;
            }

            if (session.currentTrialNum == 0 || session.CurrentTrial == null || session.CurrentTrial.number == 0)
            {
                return false;
            }

            foreach (var trial in session.Trials)
            {
                if (trial.status == TrialStatus.InProgress)
                {
                    return true;
                }
            }

            return false;
        }


        #region Settings

        /// <summary>
        ///     Add a KVP to the Settings, and check with the Session whether this specific key is already registered to be Logged: if not, it will be automatically added to the "Settings To Log" list.
        ///     You can turn off the auto-logging (pass 'logSettings = false'), for those settings that do not need to be logged.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValueStored(this Settings settings, string key, object value, bool logSettings = true)
        {
            settings.SetValue(key, value);

            if (!logSettings)
            {
                return;
            }

            var settingsParent = settings.GetParent();

            if (settingsParent == null)
            {
                Debug.LogWarningFormat("Settings parent is null, cannot determine Session for auto-logging of key {0}!", key);

                return;
            }

            Session session;

            // We need to somehow get the Session, but we don't know what we're getting from the GetParent, since that's the interface, and not the concrete type.
            if (settingsParent.GetType() == typeof(Session))
            {
                session = settingsParent as Session;
            }
            else if (settingsParent.GetType() == typeof(Block))
            {
                var block = settingsParent as Block;
                session = block?.session;
            }
            else
            {
                var trial = settingsParent as Trial;
                session = trial?.session;
            }

            if (session == null)
            {
                Debug.LogWarningFormat("Session is null, we cannot add the {key} to the 'Settings to Log' list!");

                return;
            }

            // This is why we needed the Session! So we can look at it's `settingsToLog` list, and add our key if it was not yet added.
            // This way, our Setting gets logged to the `trial_results.csv` file.
            if (!session.settingsToLog.Contains(key))
            {
                session.settingsToLog.Add(key);
            }
        }


        /// <summary>
        ///     Get this Block Setting dictionary.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSettings(this Block block)
        {
            return block.settings.baseDict;
        }


        /// <summary>
        ///     Returns the value of a specific setting on a specific Block
        /// </summary>
        /// <param name="block"></param>
        /// <param name="key"></param>
        /// <returns>
        ///     WARNING: Returns default(T) on conversion failure. For value types (int, bool, float),
        ///     this means 0, false, or 0.0f which may be indistinguishable from valid data.
        /// </returns>
        public static T GetSetting<T>(this Block block, string key)
        {
            if (!block.HasSetting(key))
                return default;

            var value = block.settings.baseDict[key];

            if (value is T typedValue)
                return typedValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T)); // Is used to attempt to convert functionally equivalent types (long > int | int64 > int32 | etc)
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not convert {value?.GetType()} to {typeof(T)}: {e.Message}");
                return default;
            }
        }


        /// <summary>
        ///     Returns the value of a specific setting on a specific Session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns>
        ///     WARNING: Returns default(T) on conversion failure. For value types (int, bool, float),
        ///     this means 0, false, or 0.0f which may be indistinguishable from valid data.
        /// </returns>
        public static T GetSetting<T>(this Session session, string key)
        {
            if (!session.HasSetting(key))
                return default;

            var value = session.settings.baseDict[key];

            if (value is T typedValue)
                return typedValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T)); // Is used to attempt to convert functionally equivalent types (long > int | int64 > int32 | etc)
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not convert {value?.GetType()} to {typeof(T)}: {e.Message}");
                return default;
            }
        }



        /// <summary>
        ///     Get the Session Setting dictionary.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSessionSettings(this Session session)
        {
            return session.settings.baseDict;
        }


        public static bool HasSetting(this Session session, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("HasSetting called with null or empty key.");

                return false;
            }

            var hasSetting = session.settings.ContainsKey(key);

            if (hasSetting)
            {
                return true;
            }

            Debug.LogWarningFormat($"Key {key} has not been found in this Session");

            return false;
        }


        public static bool HasSetting(this Block block, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("HasSetting called with null or empty key.");

                return false;
            }

            var hasSetting = block.settings.ContainsKey(key);

            if (hasSetting)
            {
                return true;
            }

            Debug.LogWarningFormat($"Key {key} has not been found in this Block");

            return false;
        }


        public static bool DoesAnyBlockHaveSetting(this Session session, string key)
        {
            foreach (var block in session.blocks)
            {
                if (block.settings.ContainsKey(key))
                {
                    return true;
                }
            }

            Debug.LogWarningFormat($"Key {key} has not been found in any of the Blocks");

            return false;
        }


        /// <summary>
        ///     Looks through the Session and all registered Blocks to see if a Setting exists.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DoesSettingExist(this Session session, string key)
        {
            if (session == null)
            {
                Debug.LogErrorFormat("Session is null");

                return false;
            }

            if (session.settings == null)
            {
                Debug.LogErrorFormat("Session settings is null");

                return false;
            }

            var sessionHasSetting = session.settings.ContainsKey(key);

            var blocksHaveSetting = false;

            foreach (var block in session.blocks)
            {
                if (block.settings.ContainsKey(key))
                {
                    blocksHaveSetting = true;
                }
            }

            if (sessionHasSetting || blocksHaveSetting)
            {
                return true;
            }

            Debug.LogWarningFormat($"Key {key} has not been found in any of the Blocks or the Session");

            return false;
        }

        #endregion
    }
}
