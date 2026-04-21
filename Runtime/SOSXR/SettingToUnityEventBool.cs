using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    /// <summary>
    ///     Use this one for when you want to use a bool Setting that's registered with UXF.
    ///     If "our" Setting (as noted by the string `m_settingsKey` in the base-class is registered to this Block (or the Session),
    ///     the UnityEvents will be invoked, passing along the value of the bool setting.
    /// </summary>
    public class SettingToUnityEventBool : SettingsResponder
    {
        [SerializeField] private bool m_value; // Just to for visuals in the Inspector


        public UnityEvent<bool> OnBlockBegin; // These UnityEvents will get invoked at the moments described there, but only if our Setting is registered for this Block (or Session)
        public UnityEvent<bool> OnTrialBegin; // Use the UnityEvents either with their value directly (in the dropdown in the Inspector they are then shown as "Dynamic"), discard the value, or mix-n-match.
        public UnityEvent<bool> OnTrialEnd;
        public UnityEvent<bool> OnBlockEnd;

        public bool Value => m_value; // In case you want to grab the settings value from another place.


        protected override void BlockBegin()
        {
            if (!GetValue(m_settingsKey, out bool value)) // "If our Setting is not of a bool-type, do not continue. Otherwise, give me the actual bool value, so I can use it like a bool"
            {
                return;
            }

            OnBlockBegin?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting. In the bool's case, that can be True or Falso

            m_value = value; // For visual reference, and if other scripts want to access the value.
        }


        protected override void TrialBegin()
        {
            if (!GetValue(m_settingsKey, out bool value))
            {
                return;
            }

            OnTrialBegin?.Invoke(value);
        }


        protected override void TrialEnd()
        {
            if (!GetValue(m_settingsKey, out bool value))
            {
                return;
            }

            OnTrialEnd?.Invoke(value);
        }


        protected override void BlockEnd()
        {
            if (!GetValue(m_settingsKey, out bool value))
            {
                return;
            }

            OnBlockEnd?.Invoke(value);

            m_value = false; // "reset the value". TBH this makes little sense with a bool, but with an int / string / etc you can reset the value to a clear "not-real" value (eg. -999 or "Hello_World"), to indicate that the value is/should no longer be used.
        }
    }
}
