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
        SetupUI();
    }

    void Update()
    {
        if (currentQuest != null && currentQuest.questData != null)
        {
            progresText.text = $"{currentQuest.currentProgress}/{currentQuest.questData.objective.requiredAmount}";
        }

    }
    void SetupUI()
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
    }
}
