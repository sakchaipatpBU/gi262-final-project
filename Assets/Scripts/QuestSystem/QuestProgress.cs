using System;
using UnityEngine;

[Serializable]
public class QuestProgress
{
    public QuestData questData;
    public int[] currentProgress;
    public bool isCompleted;
    public bool isClaimed;

    public QuestProgress(QuestData data)
    {
        questData = data;
        currentProgress = new int[data.objectives.Length];
        isCompleted = false;
        isClaimed = false;
    }

    public void AddProgress(string _targetName, QuestObjectiveType type)
    {
        if (isCompleted) return;

        for (int i = 0; i < questData.objectives.Length; i++)
        {
            var obj = questData.objectives[i];
            if (obj.type == type && obj.targetName == _targetName)
            {
                currentProgress[i]++;
                if (currentProgress[i] >= obj.requiredAmount)
                    currentProgress[i] = obj.requiredAmount;

                Debug.Log($"Quest {questData.questName} progress: ({currentProgress[i]}/{obj.requiredAmount})");
            }
        }

        CheckIfCompleted();
    }

    public void CheckIfCompleted()
    {
        for (int i = 0; i < questData.objectives.Length; i++)
        {
            if (currentProgress[i] < questData.objectives[i].requiredAmount)
                return;
        }

        isCompleted = true;
        Debug.Log($"Quest '{questData.questName}' completed!");
    }
}
