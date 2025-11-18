using UnityEngine;
public enum QuestType
{
    Normal,
    TimeTrail
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public Sprite questIcon;
    public QuestType questType;

    [Header("Requirements")]
    public int playerLevel;
    public int combatScore;
    public bool isPrerequisiteQuest;
    public QuestData prerequisiteQuest;

    [Header("Objectives")]
    public QuestObjective objective;
    public float questTimeLimit;

    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    private void OnValidate()
    {
#if UNITY_EDITOR
        questName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
