using UnityEngine;
using System.Threading.Tasks;

public interface IAnalyticsManager
{
    Task InitialiseAsync();
    void LogQuestAccepted(QuestAcceptedData data);
    void LogBossBattleEndGame(BossBattleData data);
    void LogTimeTrialQuestWinRate(TimeTrialQuestData data);
    void Flush();
}

public enum BattleResult
{
    Win, Lose
}

public struct QuestAcceptedData
{
    public string QuestName;
    public QuestType QuestType;
    public QuestObjectiveType QuestObjectiveType;
    public int PlayerLevel;
    public int ExpReward;
    public int GoldReward;
}

public struct BossBattleData
{
    public string BossName;
    public BattleResult Result;
    public int PlayerLevel;
    public int PlayerAtkPoint;
    public int PlayerHpPoint;
    public int PlayerMoveSpeedPoint;
    public int AttemptCount;
}

public struct TimeTrialQuestData
{
    public string QuestName;
    public float QuestTimeLimit;
    public QuestObjectiveType QuestObjectiveType;
    public float QuestSuccessTimeLeft; // 0 = fail
}