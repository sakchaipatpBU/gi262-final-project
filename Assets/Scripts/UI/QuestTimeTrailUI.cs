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
        if(!isStart || GameManager.Instance.isGameOver 
            || currentQuest.isCompleted) return;
        questTime -= Time.deltaTime;
        if(questTime <= 0)
        {
            questTime = 0;
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
}
