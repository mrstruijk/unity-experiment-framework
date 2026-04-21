using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    /// <summary>
    ///     Derivatives of this class will need to implement the abstract methods for Trial & Block, for both Beginning and Stopping.
    ///     What the methods will do is up to them. This base class will just let them know:
    ///     1) Did a Block / Trial just begin or end?
    ///     2) Does this Block (or the entire Session) use the Setting that is referenced by the string `m_settingsKey`?
    ///     If this is the case, the corresponding abstract methods will be called.
    ///     See "SettingToUnityEventBool.cs" for implementation and explanation.
    /// </summary>
    public abstract class SettingsResponder : MonoBehaviour
    {
        [SerializeField] protected string m_settingsKey;

        protected Session _session;


        private void Awake()
        {
            _session = FindFirstObjectByType<Session>();

            if (_session == null)
            {
                Debug.LogWarning("Unable to find Session object. Cannot continue.");
                enabled = false;
            }
        }


        private void OnEnable()
        {
            _session?.onSessionBegin?.AddListener(CheckSettingExist);
            _session?.onBlockBegin?.AddListener(BlockBegin);
            _session?.onTrialBegin?.AddListener(TrialBegin);
            _session?.onTrialEnd?.AddListener(TrialEnd);
            _session?.onBlockEnd?.AddListener(BlockEnd);
        }


        private void CheckSettingExist(Session session)
        {
            if (_session.DoesSettingExist(m_settingsKey))
            {
                return;
            }

            Debug.LogWarning($"We're looking for {m_settingsKey}, which does not seem to be registered in this Session or in any of the Blocks. Please check that this is ok?");
        }


        private void BlockBegin(Block currentBlock)
        {
            if (currentBlock.HasSetting(m_settingsKey))
            {
                BlockBegin();
            }
            else
            {
                Debug.LogFormat($"I don't have setting {m_settingsKey} in this Block");
            }
        }


        protected abstract void BlockBegin();


        private void TrialBegin(Trial trial)
        {
            if (trial.block.HasSetting(m_settingsKey))
            {
                TrialBegin();
            }
        }


        protected abstract void TrialBegin();


        private void TrialEnd(Trial trial)
        {
            if (trial.block.HasSetting(m_settingsKey))
            {
                TrialEnd();
            }
        }


        protected abstract void TrialEnd();


        private void BlockEnd(Block currentBlock)
        {
            if (currentBlock.HasSetting(m_settingsKey))
            {
                BlockEnd();
            }
            else
            {
                Debug.Log($"I don't have setting {m_settingsKey} in this Block");
            }
        }


        protected abstract void BlockEnd();


        private void OnDisable()
        {
            _session?.onSessionBegin?.RemoveListener(CheckSettingExist);
            _session?.onBlockBegin?.RemoveListener(BlockBegin);
            _session?.onTrialBegin?.RemoveListener(TrialBegin);
            _session?.onTrialEnd?.RemoveListener(TrialEnd);
            _session?.onBlockEnd?.RemoveListener(BlockEnd);
        }


        /// <summary>
        ///     TODO: extend this to also work with Session settings
        ///     Use this to ask: is the Value of our Key of type X? If so, give me the Value in that correct type.
        ///     Usage example:
        ///     `if (!GetValue(m_settingsKey, out int value)) { return; }`
        ///     "If our setting is not of type int, do not continue. Otherwise, do continue, and our value (the actual setting) is now actually usable as an int"
        ///     Pass it other types (e.g. bool, string) to work with those instead.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected bool GetValue<T>(string key, out T value)
        {
            value = _session.CurrentBlock.GetSetting<T>(key);

            var type = typeof(T);

            if (value == null)
            {
                Debug.LogWarning($"Our value for '{key}' null!");

                return false;
            }

            if (value.GetType() != type)
            {
                Debug.LogWarning($"We were expecting '{key}' to be of type {type}. Cannot send this UnityEvent out");

                return false;
            }

            return true;
        }
    }
}
