using System;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

// ── Typed Event Classes ───────────────────────────────────
public class QuestAcceptedEvent : Unity.Services.Analytics.Event
{
    public QuestAcceptedEvent() : base(AnalyticsEventNames.QuestAccepted) { }
    public string QuestName { set => SetParameter(AnalyticsParams.QuestName, value); }
    public string QuestType { set => SetParameter(AnalyticsParams.QuestType, value); }
    public string QuestObjectiveType { set => SetParameter(AnalyticsParams.QuestObjectiveType, value); }
    public int PlayerLevel { set => SetParameter(AnalyticsParams.PlayerLevel, value); }
    public int ExpReward { set => SetParameter(AnalyticsParams.ExpReward, value); }
    public int GoldReward { set => SetParameter(AnalyticsParams.GoldReward, value); }
}

public class BossBattleEndGameEvent : Unity.Services.Analytics.Event
{
    public BossBattleEndGameEvent() : base(AnalyticsEventNames.BossBattleEndGame) { }
    public string BossName { set => SetParameter(AnalyticsParams.BossName, value); }
    public string Result { set => SetParameter(AnalyticsParams.Result, value); }
    public int PlayerLevel { set => SetParameter(AnalyticsParams.PlayerLevel, value); }
    public int PlayerAtkPoint { set => SetParameter(AnalyticsParams.PlayerAtkPoint, value); }
    public int PlayerHpPoint { set => SetParameter(AnalyticsParams.PlayerHpPoint, value); }
    public int PlayerMoveSpeedPoint { set => SetParameter(AnalyticsParams.PlayerMoveSpeedPoint, value); }
    public int AttemptCount { set => SetParameter(AnalyticsParams.AttemptCount, value); }
}

public class TimeTrialQuestWinRateEvent : Unity.Services.Analytics.Event
{
    public TimeTrialQuestWinRateEvent() : base(AnalyticsEventNames.TimeTrialQuestWinRate) { }
    public string QuestName { set => SetParameter(AnalyticsParams.QuestName, value); }
    public float QuestTimeLimit { set => SetParameter(AnalyticsParams.QuestTimeLimit, value); }
    public string QuestObjectiveType { set => SetParameter(AnalyticsParams.QuestObjectiveType, value); }
    public float QuestSuccessTimeLeft { set => SetParameter(AnalyticsParams.QuestSuccessTimeLeft, value); }
}
