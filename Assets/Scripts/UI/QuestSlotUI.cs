using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image icon;
    public TMP_Text questNameText;
    public TMP_Text requireAmountText;
    public TMP_Text requirementText;
    public TMP_Text rewardText;
    public Button acceptButton;
    public Button claimButton;
    public Button cancelButton;

    private QuestData questData;
    private PlayerCharacter playerCharacter;
    private void Awake()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
    }

    public void SetQuestData(QuestData data)
    {
        questData = data;
        if (questData == null) return;

        icon.sprite = questData.questIcon;
        questNameText.text = questData.questName;
        requireAmountText.text = $"{questData.objective.type} {questData.objective.requiredAmount} {questData.objective.targetName}";
        if(questData.objective.type == QuestObjectiveType.Talk)
        {
            requireAmountText.text = $"{questData.objective.type} to {questData.objective.targetName}";
        }
        if (questData.questType == QuestType.TimeTrail)
        {
            requireAmountText.text += $" , Time : {questData.questTimeLimit} Second";
        }
        requirementText.text = $"Requirements: Level {questData.playerLevel} , CP {questData.combatScore}";
        if(questData.prerequisiteQuest != null)
        {
            requirementText.text += $" Quest Require: {questData.prerequisiteQuest.name}";
        }
        rewardText.text = $"Reward: {questData.goldReward} Coins / {questData.expReward} EXP";

        // setup button
        acceptButton.onClick.AddListener(() => AcceptQuest());
        claimButton.onClick.AddListener(() => ClaimQuest());
        cancelButton.onClick.AddListener(() => CancelQuest());

        UpdateButtonState();
    }
    
    public void UpdateButtonState()
    {
        bool hasQuest = QuestManager.Instance.HasActiveQuest();
        
        if (!hasQuest)
        {
            if(playerCharacter == null)
            {
                playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
                Debug.Log("QuestSlotUI GameObject.Find(\"Player\").GetComponent<PlayerCharacter>();");
            }
            bool requirement = false;
            if (playerCharacter.Level >= questData.playerLevel &&
                playerCharacter.CombatScore >= questData.combatScore)
            {
                requirement = true;
            }
            if (requirement && questData.isPrerequisiteQuest)
            {
                requirement = CheckPrerequisiteQuest();
            }
            if (!requirement)
            {
                // Not reach Requirement
                acceptButton.gameObject.SetActive(false);
                claimButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                return;
            }

            // No quest → accept only
            acceptButton.gameObject.SetActive(true);
            claimButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
        }
        else
        {
            var currentQuest = QuestManager.Instance.currentQuest;
            if (currentQuest == null)
            {
                Debug.Log("Can NOT found current quest from QuestManager");
                return;
            }

            if (currentQuest.questData.questName == questData.questName)
            {
                if (currentQuest.isCompleted && !currentQuest.isClaimed)
                {
                    acceptButton.gameObject.SetActive(false);
                    claimButton.gameObject.SetActive(true);
                    cancelButton.gameObject.SetActive(false);
                }
                else if (!currentQuest.isCompleted)
                {
                    acceptButton.gameObject.SetActive(false);
                    claimButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(true);
                }
                else
                {
                    // claimed
                    acceptButton.gameObject.SetActive(false);
                    claimButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                }
            }
            else
            {
                // has other quest → close all button
                acceptButton.gameObject.SetActive(false);
                claimButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
            }
        }
    }
    private bool CheckPrerequisiteQuest()
    {
        if (QuestManager.Instance.completedQuests.Count == 0) return false;

        foreach (var quest in QuestManager.Instance.completedQuests)
        {
            if (quest == questData.prerequisiteQuest)
            {
                return true;
            }
        }
        return false;
    }

    private void AcceptQuest()
    {
        QuestManager.Instance.AcceptQuest(questData);
        QuestUIManager.Instance.UpdateAllQuestSlot();
    }

    private void ClaimQuest()
    {
        QuestManager.Instance.ClaimReward();
        QuestUIManager.Instance.UpdateAllQuestSlot();
    }

    private void CancelQuest()
    {
        QuestManager.Instance.CancelQuest();
        QuestUIManager.Instance.UpdateAllQuestSlot();
    }
}
