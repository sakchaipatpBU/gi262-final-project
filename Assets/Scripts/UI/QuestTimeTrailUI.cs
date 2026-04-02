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

        if(GameManager.Instance.isGameOver
            || currentQuest.isCompleted)
        {
            SendAnalytics();
        }

        if(!isStart) return;
        questTime -= Time.deltaTime;
        if(questTime <= 0)
        {
            questTime = 0;
            SendAnalytics();
            isStart = false;
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

    private void SendAnalytics()
    {
        GameAnalyticsService.Instance.LogTimeTrialQuestWinRate(new TimeTrialQuestData
        {
            QuestName = currentQuest.questData.questName,
            QuestTimeLimit = currentQuest.questData.questTimeLimit,
            QuestObjectiveType = currentQuest.questData.objective.type,
            QuestSuccessTimeLeft = questTime
        });
        Destroy(gameObject);
    }
}
