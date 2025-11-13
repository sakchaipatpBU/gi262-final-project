using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public Sprite questIcon;

    //public string questID;
    //[TextArea] public string questDescription;

    [Header("Requirements")]
    public int playerLevel;
    public int combatScore;
    public bool isPrerequisiteQuest;
    public QuestData prerequisiteQuest;

    [Header("Objectives")]
    //public QuestObjective[] objectives; // old
    public QuestObjective objective;    // new

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
