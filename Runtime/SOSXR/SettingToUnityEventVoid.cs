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
            OnBlockBegin?.Invoke(); // Invoking the UnityEvent to signal that the block has begun (no value is passed since we don't need the setting value here)
        }


        protected override void TrialBegin()
        {
            OnTrialBegin?.Invoke(); // Invoking the UnityEvent to signal that the trial has begun (no value is passed since we don't need the setting value here)
        }


        protected override void TrialEnd()
        {
            OnTrialEnd?.Invoke(); // Invoking the UnityEvent to signal that the trial has ended (no value is passed since we don't need the setting value here)
        }


        protected override void BlockEnd()
        {
            OnBlockEnd?.Invoke(); // Invoking the UnityEvent to signal that the block has ended (no value is passed since we don't need the setting value here)
        }
    }
}
