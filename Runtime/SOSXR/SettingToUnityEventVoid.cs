using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.UXF
{
    /// <summary>
    ///     Use this one for when you want to use a Setting that's registered with UXF but you don't need to pass the actual value anywhere.
    ///     This version doesn't pass the setting's value through the UnityEvents - it simply invokes them to signal that a block/trial has begun or ended.
    ///     If "our" Setting (as noted by the string `m_settingsKey` in the base-class is registered to this Block (or the Session),
    ///     the UnityEvents will be invoked at the appropriate moments.
    /// </summary>
    public class SettingToUnityEventVoid : SettingsResponder
    {
        [Space(10)]
        public UnityEvent OnBlockBegin; // These UnityEvents will get invoked at the moments described there, but only if our Setting is registered for this Block (or Session)
        public UnityEvent OnTrialBegin; // Use the UnityEvents to trigger actions without needing the actual setting value (they just signal that the event occurred)
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


        protected override void BlockEnd()
        {
            OnBlockEnd?.Invoke();
        }
    }
}
