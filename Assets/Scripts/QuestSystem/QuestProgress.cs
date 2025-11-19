using System;
using UnityEngine;

[Serializable]
public class QuestProgress
{
    public QuestData questData;
    public int currentProgress;
    public bool isCompleted;
    public bool isClaimed;

    public QuestProgress(QuestData data)
    {
        questData = data;
        currentProgress = 0;
        isCompleted = false;
        isClaimed = false;
    }

    public void AddProgress(string _targetName, QuestObjectiveType type)
    {
        if (questData == null) return;
        if (isCompleted) return;

        var objective = questData.objective;
        if (objective.type == type && objective.targetName == _targetName)
        {
            currentProgress++;
            if (currentProgress >= objective.requiredAmount)
                currentProgress = objective.requiredAmount;

            Debug.Log($"Quest {questData.questName} progress: ({currentProgress}/{objective.requiredAmount})");
        }

        CheckIfCompleted();
    }

    public void CheckIfCompleted()
    {
        if (currentProgress < questData.objective.requiredAmount) return; // -> 1 objective
        isCompleted = true;
        Debug.Log($"Quest '{questData.questName}' completed!");
    }
}
