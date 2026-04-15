using UnityEngine;


[CreateAssetMenu(menuName = "SOSXR/UXF/ExperimentSettings", fileName = "ExperimentSettings")]
public class ExperimentSettings : ScriptableObject
{
    [SerializeField][Min(1)] private int m_blocksAmount = 5;

    [SerializeField] private bool m_shuffleBlocks = true;

    [SerializeField][Min(1)] private int m_trialsPerBlock = 2;

    [Tooltip("If set to less than 0, stopping a trial will NOT automatically start a new one.")]
    [SerializeField][Min(-1)] private int m_startNextTrialDelayMS = 250;

    public int BlocksAmount => m_blocksAmount;
    public bool ShuffleBlocks => m_shuffleBlocks;
    public int TrialsPerBlock => m_trialsPerBlock;
    public int StartNextTrialDelayMS => m_startNextTrialDelayMS;
}
