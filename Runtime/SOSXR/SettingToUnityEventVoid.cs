using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    public class SettingToUnityEventVoid : SettingsResponder
    {
        [Space(10)]
        public UnityEvent OnBlockBegin;
        public UnityEvent OnTrialBegin;
        public UnityEvent OnTrialEnd;
        public UnityEvent OnBlockEnd;


        protected override void BlockBegin()
        {
            OnBlockBegin?.Invoke();
        }


        protected override void TrialBegin()
        {
            OnTrialBegin?.Invoke();
        }


        protected override void TrialEnd()
        {
            OnTrialEnd?.Invoke();
        }


        protected override void BlokEnd()
        {
            OnBlockEnd?.Invoke();
        }
    }
}