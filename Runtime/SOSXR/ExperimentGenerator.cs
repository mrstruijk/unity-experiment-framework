using UnityEngine;
using UXF;


namespace SOSXR.UXF
{
    public class ExperimentGenerator : MonoBehaviour
    {
        [SerializeField] private ExperimentSettings m_settings;
        [SerializeField] private bool m_autoGenerateSession = true;

        private void Start()
        {
            var session = Session.instance;

            if (session == null)
            {
                return;
            }

            if (m_autoGenerateSession == true)
            {
                session.onSessionBegin.AddListener(GenerateSession);
            }
        }

        /// <summary>
        ///     This gets called from the OnSessionBegin event on the [UXF_Rig] GameObject.
        ///     Set it in the Inspector of the Session component.
        /// </summary>
        /// <param name="session"></param>
        public void GenerateSession(Session session)
        {
            Debug.LogFormat($"Creating {m_settings.BlocksAmount} blocks with {m_settings.TrialsPerBlock} trials each");

            var startingBlock = 1;
            var endingBlock = m_settings.BlocksAmount;

            session.settings.SetValue("sesh", 10); // You can set Settings on the Session-level: automatically logged to `settings.json`.

            for (var i = startingBlock; i <= endingBlock; i++)
            {
                var block = session.CreateBlock(m_settings.TrialsPerBlock);

                var isFirstHalf = i <= (endingBlock + startingBlock) / 2;
                var isEvenBlock = i % 2 == 0;

                // this is how we can set values to the Blocks, bool in this case.
                block.settings.SetValueStored("example_isBlockInFirstHalf", isFirstHalf); // this auto-logs itself in the `trial_results.json`, because it registers itself to the "Settings To Log" list.

                if (isEvenBlock)
                {
                    block.settings.SetValueStored("example_isBlockEven", "isEven");
                }
                else
                {
                    block.settings.SetValueStored("example_isBlockEven", "isUneven");
                }

                Debug.Log(i);
            }

            if (m_settings.ShuffleBlocks)
            {
                session.blocks.Shuffle();
                Debug.Log("Shuffled blocks to new order");
            }

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
            var session = Session.instance;

            if (session == null)
            {
                return;
            }

            if (m_autoGenerateSession == true)
            {
                session.onSessionBegin.RemoveListener(GenerateSession);
            }
        }
    }
}
