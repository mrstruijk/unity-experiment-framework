using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    public class SettingToUnityEventInt : SettingsResponder
    {
        [SerializeField] private int m_value = -999;
        [Space(15)]
        public UnityEvent<int> OnBlockBegin;
        public UnityEvent<int> OnTrialBegin;
        public UnityEvent<int> OnTrialEnd;
        public UnityEvent<int> OnBlockEnd;


        public int Value => m_value;


        protected override void BlockBegin()
        {
            if (!GetValue(m_settingsKey, out int value))
            {
                return;
            }

            OnBlockBegin?.Invoke(value);

            m_value = value;
        }


        protected override void TrialBegin()
        {
            if (!GetValue(m_settingsKey, out int value))
            {
                return;
            }

            OnTrialBegin?.Invoke(value);
        }


        protected override void TrialEnd()
        {
            if (!GetValue(m_settingsKey, out int value))
            {
                return;
            }

            OnTrialEnd?.Invoke(value);
        }


        protected override void BlokEnd()
        {
            if (!GetValue(m_settingsKey, out int value))
            {
                return;
            }

            OnBlockEnd?.Invoke(value);

            m_value = -999;
        }
    }
}
