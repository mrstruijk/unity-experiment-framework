using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    /// <summary>
    ///     Use this one for when you want to use a string Setting that's registered with UXF.
    ///     If "our" Setting (as noted by the string `m_settingsKey` in the base-class is registered to this Block (or the Session),
    ///     the UnityEvents will be invoked, passing along the value of the string setting.
    /// </summary>
    public class SettingToUnityEventString : SettingsResponder
    {
        [SerializeField] private string m_value; // Just to for visuals in the Inspector. Value is set to null when reset (indicating "no value should be used")
        [Space(15)]
        public UnityEvent<string> OnBlockBegin; // These UnityEvents will get invoked at the moments described there, but only if our Setting is registered for this Block (or Session)
        public UnityEvent<string> OnTrialBegin; // Use the UnityEvents either with their value directly (in the dropdown in the Inspector they are then shown as "Dynamic"), discard the value, or mix-n-match.
        public UnityEvent<string> OnTrialEnd;
        public UnityEvent<string> OnBlockEnd;


        public string Value => m_value; // In case you want to grab the settings value from another place.


        protected override void BlockBegin()
        {
            if (!GetValue(m_settingsKey, out string value)) // "If our Setting is not of a string-type, do not continue. Otherwise, give me the actual string value, so I can use it like a string"
            {
                return;
            }

            OnBlockBegin?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting. In the string's case, that can be any string value

            m_value = value; // For visual reference, and if other scripts want to access the value.
        }


        protected override void TrialBegin()
        {
            if (!GetValue(m_settingsKey, out string value)) // "If our Setting is not of a string-type, do not continue. Otherwise, give me the actual string value"
            {
                return;
            }

            OnTrialBegin?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting
        }


        protected override void TrialEnd()
        {
            if (!GetValue(m_settingsKey, out string value)) // "If our Setting is not of a string-type, do not continue. Otherwise, give me the actual string value"
            {
                return;
            }

            OnTrialEnd?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting
        }


        protected override void BlockEnd()
        {
            if (!GetValue(m_settingsKey, out string value)) // "If our Setting is not of a string-type, do not continue. Otherwise, give me the actual string value"
            {
                return;
            }

            OnBlockEnd?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting

            m_value = null; // "reset the value" to null to indicate that the value is/should no longer be used.
        }
    }
}
