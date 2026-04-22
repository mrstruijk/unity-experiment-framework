using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    public class ExperimentGenerator : MonoBehaviour
    {
        [SerializeField] private bool m_autoGenerateSession = true;

        private Session _session;


        private void Awake()
        {
            _session = Session.instance;
        }


        private void OnEnable()
        {
            if (!m_autoGenerateSession)
            {
                return;
            }

            if (_session == null)
            {
                Debug.LogWarning($"Could not find Session. Cannot automatically add the {nameof(GenerateSession)} method to the OnSessionBegin event of the Session");

                return;
            }

            _session.onSessionBegin.AddListener(GenerateSession);
        }


        /// <summary>
        ///     This gets called from the OnSessionBegin event on the [UXF_Rig] GameObject.
        ///     Set it in the Inspector of the Session component if you set the m_autoGenerateSession to false.
        /// </summary>
        /// <param name="session"></param>
        public void GenerateSession(Session session)
        {
            var startingBlock = 1;
            var endingBlock = session.GetSetting<int>("BlocksAmount");
            var trialsPerBlock = session.GetSetting<int>("TrialsPerBlock");
            var shuffleBlocks = session.GetSetting<bool>("ShuffleBlocks");

            if (endingBlock <= 0)
            {
                Debug.LogError("BlocksAmount must be > 0");
                return;
            }

            if (trialsPerBlock <= 0)
            {
                Debug.LogError("TrialsPerBlock must be > 0");
                return;
            }

            Debug.LogFormat($"Creating {endingBlock} blocks with {trialsPerBlock} trials each");

            for (var i = startingBlock; i <= endingBlock; i++)
            {
                var block = session.CreateBlock(trialsPerBlock);

                // this is how we can set values to the Blocks, bool in this case.
                var isFirstHalf = i <= (endingBlock + startingBlock) / 2;
                block.settings.SetValueStored("example_isBlockInFirstHalf", isFirstHalf); // this auto-logs itself in the `trial_results.json`, because it registers itself to the "Settings To Log" list.

                var isEvenBlock = i % 2 == 0;
                block.settings.SetValueStored("example_isBlockEven", isEvenBlock);
            }

            if (shuffleBlocks)
            {
                session.blocks.Shuffle();
                Debug.Log("Shuffled blocks to new order");
            }

            // Warning: Debug logging block settings - avoid logging sensitive values in production
            foreach (var block in session.blocks)
            {
                foreach (var setting in block.GetSettings())
                {
                    Debug.Log($"Our Block {block.number} has {setting.Key}:{setting.Value}");
                }
            }
        }


        private void OnDisable()
        {
            if (m_autoGenerateSession == true)
            {
                _session?.onSessionBegin.RemoveListener(GenerateSession);
            }
        }
    }
}
