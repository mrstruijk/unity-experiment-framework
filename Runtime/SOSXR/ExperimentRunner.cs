using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    public class ExperimentRunner : MonoBehaviour
    {
        private Session _session;


        private void Awake()
        {
            _session = Session.instance;
        }


        [ContextMenu(nameof(StartTrial))]
        public void StartTrial()
        {
            if (_session == null)
            {
                Debug.LogError("Cannot start a Trial if the Session is null");

                return;
            }

            if (!_session.hasInitialised)
            {
                Debug.LogWarning("Cannot start a Trial if the Session is not yet initialised");

                return;
            }

            if (_session.IsLastTrial())
            {
                Debug.LogWarning("Something is wrong. This should not be possible");

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
        }


        private void StartedBlock()
        {
            Debug.Log($"Starting Block {_session.currentBlockNum}");

            // This is really just an example to show which Settings are stored for this current Block.
            foreach (var blockSetting in _session.CurrentBlock.GetSettings())
            {
                Debug.Log($"Our Block has {blockSetting.Key}:{blockSetting.Value}");
            }
        }


        [ContextMenu(nameof(StopTrial))]
        public void StopTrial()
        {
            if (_session == null)
            {
                Debug.LogError("Cannot stop a Trial if the Session is null");

                return;
            }

            if (!_session.hasInitialised)
            {
                Debug.LogWarning("Cannot stop a Trial if the Session is not yet initialised");

                return;
            }

            if (!_session.TrialInProgress())
            {
                Debug.LogWarning("There is currently no Trial in progress, so we cannot stop a Trial either. Start a new Trial first.");

                return;
            }

            _session.EndCurrentTrial();

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
                CancelInvoke(nameof(StartTrial));

                var delaySecs = StartNextTrialDelayMS / 1000f;

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

            _session?.End();

            // This in turn SHOULD invoke the Session to call the OnSessionEnd event, where I can hook a method that gracefully handles fade to black, scene cleanup, etc.
        }


        [ContextMenu(nameof(StopSessionEarly))]
        public void StopSessionEarly()
        {
            if (_session == null)
            {
                Debug.LogError("Cannot stop a Session early if the Session is null");

                return;
            }

            if (!_session.hasInitialised)
            {
                Debug.LogWarning("Cannot stop a Session early if the Session is not yet initialised");

                return;
            }

            CancelInvoke(nameof(StartTrial));

            if (_session.TrialInProgress())
            {
                _session.EndCurrentTrial();
            }

            var currentTrial = _session.CurrentTrial.number;
            var remaining = _session.LastTrial.number - currentTrial;

            Debug.LogWarningFormat($"We're not done yet! We're bailing out of {remaining} Trials!");

            _session.endAfterLastTrial = true;
            _session.End();
        }
    }
}
