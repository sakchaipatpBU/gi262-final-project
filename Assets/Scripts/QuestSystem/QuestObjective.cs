using UnityEngine;

public enum QuestObjectiveType
{
    Kill,
    Collect,
    Talk
}

[System.Serializable]
public class QuestObjective
{
    //public string description; //add-on
    public QuestObjectiveType type;
    public string targetName;
    public int requiredAmount = 1;
}
