using UnityEngine;
using UnityEngine.Events;

namespace SOSXR.UXF
{
    /// <summary>
    ///     Use this one for when you want to use a int Setting that's registered with UXF.
    ///     If "our" Setting (as noted by the string `m_settingsKey` in the base-class is registered to this Block (or the Session),
    ///     the UnityEvents will be invoked, passing along the value of the int setting.
    /// </summary>
    public class SettingToUnityEventInt : SettingsResponder
    {
        [SerializeField] private int m_value = -999; // Just to for visuals in the Inspector. Default value of -999 indicates "no value has been set yet" (acts as a reset sentinel value)
        [Space(15)]
        public UnityEvent<int> OnBlockBegin; // These UnityEvents will get invoked at the moments described there, but only if our Setting is registered for this Block (or Session)
        public UnityEvent<int> OnTrialBegin; // Use the UnityEvents either with their value directly (in the dropdown in the Inspector they are then shown as "Dynamic"), discard the value, or mix-n-match.
        public UnityEvent<int> OnTrialEnd;
        public UnityEvent<int> OnBlockEnd;


        public int Value => m_value; // In case you want to grab the settings value from another place.


        protected override void BlockBegin()
        {
            if (!GetValue(m_settingsKey, out int value)) // "If our Setting is not of an int-type, do not continue. Otherwise, give me the actual int value, so I can use it like an int"
            {
                return;
            }

            OnBlockBegin?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting. In the int's case, that can be any integer value

            m_value = value; // For visual reference, and if other scripts want to access the value.
        }


        protected override void TrialBegin()
        {
            if (!GetValue(m_settingsKey, out int value)) // "If our Setting is not of an int-type, do not continue. Otherwise, give me the actual int value"
            {
                return;
            }

            OnTrialBegin?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting
        }


        protected override void TrialEnd()
        {
            if (!GetValue(m_settingsKey, out int value)) // "If our Setting is not of an int-type, do not continue. Otherwise, give me the actual int value"
            {
                return;
            }

            OnTrialEnd?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting
        }


        protected override void BlockEnd()
        {
            if (!GetValue(m_settingsKey, out int value)) // "If our Setting is not of an int-type, do not continue. Otherwise, give me the actual int value"
            {
                return;
            }

            OnBlockEnd?.Invoke(value); // Invoking the UnityEvent, sending along the value of our Setting

            m_value = -999; // "reset the value" to -999 (our sentinel value) to indicate that the value is/should no longer be used.
        }
    }
}
