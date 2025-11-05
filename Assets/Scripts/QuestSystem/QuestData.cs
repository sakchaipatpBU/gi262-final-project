using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    //public string questID;
    public string questName;
    //[TextArea] public string questDescription;

    [Header("Objectives")]
    public QuestObjective[] objectives;

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
