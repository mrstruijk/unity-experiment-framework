using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    public class SettingToUnityEventString : SettingsResponder
    {
        [SerializeField] private string m_value;
        [Space(15)]
        public UnityEvent<string> OnBlockBegin;
        public UnityEvent<string> OnTrialBegin;
        public UnityEvent<string> OnTrialEnd;
        public UnityEvent<string> OnBlockEnd;


        public string Value => m_value;


        protected override void BlockBegin()
        {
            if (!GetValue(m_settingsKey, out string value))
            {
                return;
            }

            OnBlockBegin?.Invoke(value);

            m_value = value;
        }


        protected override void TrialBegin()
        {
            if (!GetValue(m_settingsKey, out string value))
            {
                return;
            }

            OnTrialBegin?.Invoke(value);
        }


        protected override void TrialEnd()
        {
            if (!GetValue(m_settingsKey, out string value))
            {
                return;
            }

            OnTrialEnd?.Invoke(value);
        }


        protected override void BlokEnd()
        {
            if (!GetValue(m_settingsKey, out string value))
            {
                return;
            }

            OnBlockEnd?.Invoke(value);

            m_value = null;
        }
    }
}
