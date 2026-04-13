using TMPro;
using UnityEngine;

public class QuestTimeTrailUI : MonoBehaviour
{
    [SerializeField] private float questTime;
    [SerializeField] private bool isStart = false;
    public bool isInit = false;
    
    public TMP_Text timeCountingText;

    private QuestProgress currentQuest;
    public void Init(QuestProgress _currentQuest)
    {
        currentQuest = _currentQuest;
        questTime = currentQuest.questData.questTimeLimit;
        isStart = true;
        isInit = true;
    }

    private void Update()
    {
        if(!isInit) return;

        if(currentQuest.isCompleted)
        {
            SendAnalytics("Success");
        }
        if (GameManager.Instance.isGameOver)
        {
            SendAnalytics("Fail");
        }

        if (!isStart) return;
        questTime -= Time.deltaTime;
        if(questTime <= 0)
        {
            isStart = false;
            questTime = 0;
            GameManager.Instance.GaveOver();
        }
        if(questTime > 10)
        {
            timeCountingText.text = questTime.ToString("0");
        }
        else
        {
            timeCountingText.text = questTime.ToString("0.00");
        }
    }

    private void SendAnalytics(string result)
    {
        GameAnalyticsService.Instance.LogTimeTrialQuestWinRate(new TimeTrialQuestData
        {
            QuestName = currentQuest.questData.questName,
            QuestObjectiveType = currentQuest.questData.objective.type,
            QuestTimeLimit = currentQuest.questData.questTimeLimit,
            QuestProgress = ((float)currentQuest.currentProgress 
                            / (float)currentQuest.questData.objective.requiredAmount) 
                            * 100,
            QuestTimeLeft = questTime,
            QuestResult = result
        });
        Destroy(gameObject);
    }
}
