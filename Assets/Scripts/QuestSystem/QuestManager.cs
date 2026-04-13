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
    public void Init(PlayerCharacter _player)
    {
        player = _player;
    }
    private PlayerCharacter GetPlayer()
    {
        if (player != null) return player;
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
            player = playerObj.GetComponent<PlayerCharacter>();
        if (player == null)
            Debug.LogWarning("[QuestManager] Player not found — call Init() from InitializeScene.");
        return player;
    }
    public bool HasActiveQuest()
    {
        if(currentQuest == null)
        {
            return false;
        }
        else
        {
            if(currentQuest.questData == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public void AcceptQuest(QuestData quest)
    {
        if (HasActiveQuest())
        {   
            Debug.LogWarning($"You already have an active quest: {currentQuest.questData.questName}. Cancel it first!");
            return;
        }

        PlayerCharacter p = GetPlayer();
        GameAnalyticsService.Instance.LogQuestAccepted(new QuestAcceptedData
        {
            QuestName = quest.questName,
            QuestType = quest.questType,
            QuestObjectiveType = quest.objective.type,
            PlayerLevel = p != null ? p.Level : 0,
            ExpReward = quest.expReward,
            GoldReward = quest.goldReward
        });

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
        PlayerCharacter p = GetPlayer();
        if (p == null)
        {
            Debug.LogWarning("[QuestManager] Cannot claim reward — player not found.");
            return;
        }
        QuestData completedQuest = currentQuest.questData;
        completedQuests.Add(completedQuest);
        p.AddExperience(currentQuest.questData.expReward);
        p.AddGold(currentQuest.questData.goldReward);
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

    public void ResetProgress()
    {
        if (currentQuest != null && currentQuest.questData != null && !currentQuest.isCompleted)
        {
            currentQuest.currentProgress = 0;
        }
    }
}
