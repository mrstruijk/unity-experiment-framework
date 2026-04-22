using System;
using System.Collections.Generic;
using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    public static class UXFExtensions
    {
        private static void ValidateKey(string key, string paramName)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Value cannot be null or empty.", paramName);
            }
        }

        /// <summary>
        /// Determines whether the session has been initialised by UXF.
        /// </summary>
        /// <param name="session">The session to inspect.</param>
        /// <returns><c>true</c> when the session has been initialised; otherwise, <c>false</c>.</returns>
        public static bool IsInitialised(this Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            return session.hasInitialised;
        }


        /// <summary>
        /// Gets the session's current trial when it belongs to the specified block.
        /// </summary>
        /// <param name="block">The block to compare against the session's current trial.</param>
        /// <returns>The current trial for the block, or <c>null</c> when no trial in this block is active.</returns>
        public static Trial GetCurrentTrialInBlock(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var session = block.session;

            if (session == null || !session.IsInitialised())
            {
                return null;
            }

            var currentTrial = session.CurrentTrial;

            if (session.currentTrialNum == 0 || currentTrial == null || currentTrial.number == 0)
            {
                return null;
            }

            return currentTrial.block == block ? currentTrial : null;
        }


        /// <summary>
        /// Gets the active trial number relative to the specified block.
        /// </summary>
        /// <param name="block">The block whose current relative trial number should be returned.</param>
        /// <returns>The current 1-based trial number within the block, or <c>-1</c> when no trial in the block is active.</returns>
        public static int GetCurrentRelativeTrialNumber(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var currentTrial = GetCurrentTrialInBlock(block);
            return currentTrial != null ? currentTrial.numberInBlock : -1;
        }


        /// <summary>
        /// Gets the first trial in the specified block.
        /// </summary>
        /// <param name="block">The block to inspect.</param>
        /// <returns>The first trial in the block, or <c>null</c> when the block has no trials.</returns>
        public static Trial GetFirstTrialInBlock(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.firstTrial;
        }


        /// <summary>
        /// Gets the last trial in the specified block.
        /// </summary>
        /// <param name="block">The block to inspect.</param>
        /// <returns>The last trial in the block, or <c>null</c> when the block has no trials.</returns>
        public static Trial GetLastTrialInBlock(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.lastTrial;
        }


        /// <summary>
        /// Determines whether the specified trial is the first trial in its block.
        /// </summary>
        /// <param name="trial">The trial to inspect.</param>
        /// <returns><c>true</c> when the trial is the first trial in its block; otherwise, <c>false</c>.</returns>
        public static bool IsFirstTrialInBlock(this Trial trial)
        {
            if (trial == null)
                throw new ArgumentNullException(nameof(trial));

            return trial == trial.block?.firstTrial;
        }


        /// <summary>
        /// Determines whether the session's current trial is the final trial in the session.
        /// </summary>
        /// <param name="session">The session to inspect.</param>
        /// <returns><c>true</c> when the current trial is the last trial in the session; otherwise, <c>false</c>.</returns>
        public static bool IsLastTrial(this Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

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


        /// <summary>
        /// Determines whether the specified trial is the last trial in its block.
        /// </summary>
        /// <param name="trial">The trial to inspect.</param>
        /// <returns><c>true</c> when the trial is the last trial in its block; otherwise, <c>false</c>.</returns>
        public static bool IsLastTrialInBlock(this Trial trial)
        {
            if (trial == null)
                throw new ArgumentNullException(nameof(trial));

            return trial == trial.block?.lastTrial;
        }


        /// <summary>
        /// Determines whether any trial in the session is currently marked as in progress.
        /// </summary>
        /// <param name="session">The session to inspect.</param>
        /// <returns><c>true</c> when a trial is in progress; otherwise, <c>false</c>.</returns>
        public static bool TrialInProgress(this Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

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
        /// Adds a key/value pair to the settings and optionally registers the key for session logging.
        /// </summary>
        /// <param name="settings">The settings collection to update.</param>
        /// <param name="key">The setting key to store.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        /// <param name="logSettings">When <c>true</c>, adds the key to the session's settings-to-log list if needed.</param>
        /// <remarks>
        /// Auto-logging can be disabled for settings that should not be written to the behavioural output.
        /// </remarks>
        public static void SetValueStored(this Settings settings, string key, object value, bool logSettings = true)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            ValidateKey(key, nameof(key));

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
                Debug.LogWarning($"Session is null, we cannot add the {key} to the 'Settings to Log' list!");

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
        /// Gets the local settings dictionary for a block.
        /// </summary>
        /// <param name="block">The block whose local settings should be returned.</param>
        /// <returns>The block's underlying local settings dictionary.</returns>
        public static Dictionary<string, object> GetSettings(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.settings.baseDict;
        }


        /// <summary>
        /// Returns the value of a specific setting for a block using UXF's cascading settings lookup.
        /// </summary>
        /// <param name="block">The block whose setting should be read.</param>
        /// <param name="key">The setting key to retrieve.</param>
        /// <returns>
        /// The converted setting value, or <c>default(T)</c> when the setting does not exist or conversion fails.
        /// </returns>
        public static T GetSetting<T>(this Block block, string key)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));
            ValidateKey(key, nameof(key));

            if (!block.HasSetting(key))
                return default;

            var value = block.settings.GetObject(key);

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
        /// Returns the value of a specific setting on a session.
        /// </summary>
        /// <param name="session">The session whose setting should be read.</param>
        /// <param name="key">The setting key to retrieve.</param>
        /// <returns>
        /// The converted setting value, or <c>default(T)</c> when the setting does not exist or conversion fails.
        /// </returns>
        public static T GetSetting<T>(this Session session, string key)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            ValidateKey(key, nameof(key));

            if (!session.HasSetting(key))
                return default;

            var value = session.settings.GetObject(key);

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
        /// Gets the local settings dictionary for a session.
        /// </summary>
        /// <param name="session">The session whose local settings should be returned.</param>
        /// <returns>The session's underlying local settings dictionary.</returns>
        public static Dictionary<string, object> GetSessionSettings(this Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            return session.settings.baseDict;
        }


        /// <summary>
        /// Determines whether a setting exists in the session settings hierarchy.
        /// </summary>
        /// <param name="session">The session to inspect.</param>
        /// <param name="key">The setting key to look up.</param>
        /// <returns><c>true</c> when the key exists; otherwise, <c>false</c>.</returns>
        public static bool HasSetting(this Session session, string key)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

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


        /// <summary>
        /// Determines whether a setting exists in the block settings hierarchy.
        /// </summary>
        /// <param name="block">The block to inspect.</param>
        /// <param name="key">The setting key to look up.</param>
        /// <returns><c>true</c> when the key exists; otherwise, <c>false</c>.</returns>
        public static bool HasSetting(this Block block, string key)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

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


        /// <summary>
        /// Determines whether any block in the session contains the specified setting key.
        /// </summary>
        /// <param name="session">The session whose blocks should be inspected.</param>
        /// <param name="key">The setting key to look up.</param>
        /// <returns><c>true</c> when any block reports the setting key; otherwise, <c>false</c>.</returns>
        public static bool DoesAnyBlockHaveSetting(this Session session, string key)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("DoesAnyBlockHaveSetting called with null or empty key.");

                return false;
            }

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
        /// Looks through the session and all registered blocks to determine whether a setting exists.
        /// </summary>
        /// <param name="session">The session to inspect.</param>
        /// <param name="key">The setting key to look up.</param>
        /// <returns><c>true</c> when the key exists on the session or any block; otherwise, <c>false</c>.</returns>
        public static bool DoesSettingExist(this Session session, string key)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("DoesSettingExist called with null or empty key.");

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
