using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTrackingUI : MonoBehaviour
{
    public QuestProgress currentQuest;

    public TMP_Text questNameText;
    public TMP_Text progresText;
    public Image progressBarImage;


    public void Initialize(QuestProgress _currentQuest)
    {
        currentQuest = _currentQuest;
        UpdateQuestTrackingUI();
    }

    void Update()
    {
        if (currentQuest != null && currentQuest.questData != null)
        {
            UpdateQuestTrackingUI();
        }

    }
    void UpdateQuestTrackingUI()
    {
        questNameText.text = currentQuest.questData.questName;
        progresText.text = $"{currentQuest.currentProgress}/{currentQuest.questData.objective.requiredAmount}";
        progressBarImage.fillAmount = (float)currentQuest.currentProgress /
            (float)currentQuest.questData.objective.requiredAmount;
    }

    public void OnCancelQuestButtonClicked()
    {
        currentQuest = null;
        QuestUIManager.Instance.CancelQuestByQuestTrackingUI();
        SoundManager.Instance.PlaySFX("Click_UI", 0.3f);

    }
}
