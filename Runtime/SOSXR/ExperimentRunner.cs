using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    [RequireComponent(typeof(Session))]
    public class ExperimentRunner : MonoBehaviour
    {
        // [SerializeField] private ExperimentSettings m_settings;

        private Session _session;


        private void Awake()
        {
            _session = GetComponent<Session>();
        }


        [ContextMenu(nameof(StartTrial))]
        public void StartTrial()
        {
            if (!_session.IsInitialised())
            {
                return;
            }

            if (_session.IsLastTrial())
            {
                Debug.LogError("Something is wrong. This should not be possible");

                return;
            }

            if (_session.TrialInProgress())
            {
                Debug.LogWarning("We are already in an active Trial. Stop that Trial before starting this new one");

                return;
            }

            _session.BeginNextTrial(); // By itself, this always needs to be at the start, since otherwise the "CurrentTrial" / "currentTrialNum" are either not initialized, or at 0 (and UXF is NON-ZERO-INDEXED!). There's now some other methods to check for this, but it's good to know that quite a few UXF operations won't be possible until at least one Trial has started.

            if (_session.CurrentBlock.GetRelativeTrial(1) == _session.CurrentTrial)
            {
                StartedBlock();
            }

            //Debug.Log($"Starting Trial {_session.currentTrialNum}/{_session.LastTrial.number} (total) / {_session.CurrentBlock.GetCurrentTrialInBlock().numberInBlock}/{_session.CurrentBlock.trials.Count} (relative) of Block {_session.currentBlockNum}/{_session.blocks.Count}");
        }


        private void StartedBlock()
        {
            Debug.Log($"Starting Block {_session.currentBlockNum}");

            var example_isBlockEven = _session.CurrentBlock.GetSetting<bool>("example_isBlockEven");
            if (example_isBlockEven)
            {
                Debug.Log("Example getting stored values: This is an even block");
            }
            else
            {
                Debug.Log("Example getting stored values: not an even block");
            }

            foreach (var blockSetting in _session.CurrentBlock.GetSettings())
            {
                Debug.Log($"Our Block has {blockSetting.Key}:{blockSetting.Value}");
            }
        }


        [ContextMenu(nameof(StopTrial))]
        public void StopTrial()
        {
            if (!_session.IsInitialised())
            {
                return;
            }

            if (!_session.TrialInProgress())
            {
                Debug.LogWarning("There is currently no Trial in progress, so we cannot stop a Trial either. Start a new Trial first.");

                return;
            }

            _session.EndCurrentTrial();

            //Debug.Log($"Stopping Trial {_session.currentTrialNum}/{_session.LastTrial.number} (total) / {_session.CurrentBlock.GetCurrentTrialInBlock().numberInBlock}/{_session.CurrentBlock.trials.Count} (relative) of Block {_session.currentBlockNum}/{_session.blocks.Count}");

            if (_session.CurrentTrial.IsLastTrialInBlock())
            {
                BlockEnded();
            }

            if (_session.CurrentTrial == _session.LastTrial)
            {
                StopSession();

                return;
            }
            var StartNextTrialDelayMS = _session.GetSetting<int>("StartNextTrialDelayMS");

            if (StartNextTrialDelayMS >= 0)
            {
                var delaySecs = StartNextTrialDelayMS / 1000;

                Debug.Log($"Auto-starting next Trial in {delaySecs} seconds");

                Invoke(nameof(StartTrial), delaySecs);
            }
        }


        private void BlockEnded()
        {
            Debug.Log("That was the last Trial in this Block!");
        }


        private void StopSession()
        {
            Debug.Log("That was the last Trial in our last Block, will be stopping this Session");

            _session.End();

            // This in turn SHOULD invoke the Session to call the OnSessionEnd event, where I can hook a method that gracefully handles fade to black, scene cleanup, etc.
        }


        [ContextMenu(nameof(StopSessionEarly))]
        public void StopSessionEarly()
        {
            if (_session.IsInitialised())
            {
                var currentTrial = _session.CurrentTrial.number;
                var remaining = _session.LastTrial.number - currentTrial;

                Debug.LogWarningFormat($"We're not done yet! We're bailing out of {remaining} Trials!");

                for (var i = currentTrial; i < _session.LastTrial.number; i++)
                {
                    _session.endAfterLastTrial = true;
                    _session.EndCurrentTrial();
                    _session.BeginNextTrialSafe();
                    _session.EndCurrentTrial();
                }

                StopSession();
            }
        }
    }
}
