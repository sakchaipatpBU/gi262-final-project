using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image icon;
    public TMP_Text questNameText;
    public TMP_Text requirementText;
    public TMP_Text rewardText;
    public Button acceptButton;
    public Button claimButton;
    public Button cancelButton;

    private QuestData questData;

    private void Start()
    {
        UpdateButtonState();
    }

    public void SetQuestData(QuestData data)
    {
        questData = data;

        // ตั้งค่าข้อมูล UI
        if (questData == null) return;

        questNameText.text = questData.questName;
        requirementText.text = BuildRequirementString(questData);
        rewardText.text = $"Reward: {questData.expReward} EXP / {questData.goldReward} Gold";

        // icon ถ้ามี
        if (icon != null)
        {
            var sprite = Resources.Load<Sprite>($"QuestIcons/{questData.questName}");
            if (sprite) icon.sprite = sprite;
        }

        // ตั้งค่าปุ่ม
        acceptButton.onClick.AddListener(() => AcceptQuest());
        claimButton.onClick.AddListener(() => ClaimQuest());
        cancelButton.onClick.AddListener(() => CancelQuest());

        UpdateButtonState();
    }
    private string BuildRequirementString(QuestData quest)
    {
        string req = "";
        foreach (var obj in quest.objectives)
        {
            req += $"- {obj.type} {obj.targetName} x{obj.requiredAmount}\n";
        }
        return req.TrimEnd('\n');
    }
    private void UpdateButtonState()
    {
        bool hasQuest = QuestManager.Instance.HasActiveQuest();

        if (!hasQuest)
        {
            // ยังไม่มีเควส → ปุ่มรับเควสเท่านั้น
            acceptButton.gameObject.SetActive(true);
            claimButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
        }
        else
        {
            var currentQuest = QuestManager.Instance.currentQuest;
            if (currentQuest == null) return;

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
                    // เคลมแล้ว
                    acceptButton.gameObject.SetActive(false);
                    claimButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                }
            }
            else
            {
                // มีเควสอื่นอยู่ → ปุ่มทั้งหมดปิด
                acceptButton.gameObject.SetActive(false);
                claimButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
            }
        }
    }
    private void AcceptQuest()
    {
        QuestManager.Instance.AcceptQuest(questData);
        UpdateButtonState();
    }

    private void ClaimQuest()
    {
        QuestManager.Instance.ClaimReward();
        UpdateButtonState();
    }

    private void CancelQuest()
    {
        QuestManager.Instance.CancelQuest();
        UpdateButtonState();
    }
}
