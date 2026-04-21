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
            _session = Session.instance;

            if (_session == null)
            {
                Debug.LogWarning("Unable to find Session object. Cannot continue.");
                enabled = false;
            }
        }


        private void OnEnable()
        {
            _session?.onBlockBegin?.AddListener(BlockBeginHandler);
            _session?.onTrialBegin?.AddListener(TrialBeginHandler);
            _session?.onTrialEnd?.AddListener(TrialEndHandler);
            _session?.onBlockEnd?.AddListener(BlockEndHandler);
        }


        private void BlockBeginHandler(Block currentBlock)
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


        private void TrialBeginHandler(Trial trial)
        {
            if (trial.block.HasSetting(m_settingsKey))
            {
                TrialBegin();
            }
        }


        protected abstract void TrialBegin();


        private void TrialEndHandler(Trial trial)
        {
            if (trial.block.HasSetting(m_settingsKey))
            {
                TrialEnd();
            }
        }


        protected abstract void TrialEnd();


        private void BlockEndHandler(Block currentBlock)
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
            _session?.onBlockBegin?.RemoveListener(BlockBeginHandler);
            _session?.onTrialBegin?.RemoveListener(TrialBeginHandler);
            _session?.onTrialEnd?.RemoveListener(TrialEndHandler);
            _session?.onBlockEnd?.RemoveListener(BlockEndHandler);
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
            value = default;
            var currentBlock = _session.CurrentBlock;

            if (currentBlock == null)
            {
                Debug.LogWarning($"GetValue('{key}'): No current block available.");
                return false;
            }

            if (!currentBlock.HasSetting(key))
            {
                Debug.LogWarning($"GetValue('{key}'): Setting does not exist in current block.");
                return false;
            }

            object rawValue;
            try
            {
                rawValue = currentBlock.GetSetting<object>(key);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"GetValue('{key}'): Failed to retrieve setting - {e.Message}");
                return false;
            }

            if (rawValue == null)
            {
                Debug.LogWarning($"GetValue('{key}'): Setting value is null.");
                return false;
            }

            var targetType = typeof(T);

            if (!targetType.IsAssignableFrom(rawValue.GetType()))
            {
                Debug.LogWarning($"GetValue('{key}'): Setting is type {rawValue.GetType()} but we expected {targetType}.");
                return false;
            }

            try
            {
                value = (T) rawValue;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"GetValue('{key}'): Failed to convert setting to {targetType} - {e.Message}");
                return false;
            }

            return true;
        }
    }
}
