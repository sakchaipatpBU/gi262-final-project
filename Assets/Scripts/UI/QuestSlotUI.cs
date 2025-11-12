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

    private void Start()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        UpdateButtonState();
    }

    public void SetQuestData(QuestData data)
    {
        questData = data;
        if (questData == null) return;

        icon.sprite = questData.questIcon;
        questNameText.text = questData.questName;
        requireAmountText.text = $"Amount: {questData.objective.requiredAmount}";
        requirementText.text = $"Requirements: Level {questData.playerLevel} , CP {questData.combatScore}";
        if(questData.prerequisiteQuest != null)
        {
            requirementText.text += $" Quest Require: {questData.prerequisiteQuest}";
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
        bool requirement = (playerCharacter.Level >= questData.playerLevel ||
            playerCharacter.CombatScore >= questData.combatScore);
        if(questData.isPrerequisiteQuest)
        {

        }


        if (!hasQuest)
        {
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

            if (currentQuest.questData == questData)
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
