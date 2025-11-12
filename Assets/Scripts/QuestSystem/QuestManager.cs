using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestProgress currentQuest; // only 1 quest can be accept
    public List<QuestData> completedQuests = new List<QuestData>();
    private PlayerCharacter player;

    private static QuestManager instance;
    public static QuestManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerCharacter>();
    }

    public bool HasActiveQuest()
    {
        return currentQuest != null && !currentQuest.isCompleted && !currentQuest.isClaimed;
    }

    public void AcceptQuest(QuestData quest)
    {
        if (HasActiveQuest())
        {
            Debug.LogWarning($"You already have an active quest: {currentQuest.questData.questName}. Cancel it first!");
            return;
        }

        currentQuest = new QuestProgress(quest);
        Debug.Log($"Accepted quest: {quest.questName}");
    }

    public void ReportProgress(string targetName, QuestObjectiveType type)
    {
        if (currentQuest == null) return;

        currentQuest.AddProgress(targetName, type);
    }

    public void ClaimReward()
    {
        if (currentQuest == null)
        {
            Debug.Log("No quest to claim reward for.");
            return;
        }

        if (!currentQuest.isCompleted)
        {
            Debug.Log("Quest not completed yet!");
            return;
        }

        if (currentQuest.isClaimed)
        {
            Debug.Log("Reward already claimed!");
            return;
        }
        QuestData completedQuest = currentQuest.questData;
        completedQuests.Add(completedQuest);
        player.AddExperience(currentQuest.questData.expReward);
        player.AddGold(currentQuest.questData.goldReward);
        currentQuest.isClaimed = true;

        Debug.Log($"Claimed reward: +{currentQuest.questData.expReward} EXP, +{currentQuest.questData.goldReward} Gold");

        // finish quest -> clear
        currentQuest = null;
    }

    public void CancelQuest()
    {
        if (currentQuest == null)
        {
            Debug.Log("No active quest to cancel.");
            return;
        }

        Debug.Log($"Canceled quest: {currentQuest.questData.questName}");
        currentQuest = null;
    }
}
