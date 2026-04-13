using System;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
public class UnityAnalyticsManager : IAnalyticsManager
{
    private bool _isInitialised;

    public async Task InitialiseAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            // Production: เรียกหลัง consent dialog เท่านั้น
            AnalyticsService.Instance.StartDataCollection();
            _isInitialised = true;
            Debug.Log("[Unity Analytics] Initialised.");
        }
        catch (Exception ex)
        {
            Debug.LogError("[Unity Analytics] Init failed: " + ex.Message);
        }
    }

    public void LogQuestAccepted(QuestAcceptedData d)
    {
        if (!Ready(AnalyticsEventNames.QuestAccepted)) return;
        AnalyticsService.Instance.RecordEvent(new QuestAcceptedEvent
        {
            QuestName = d.QuestName,
            QuestType = d.QuestType.ToString(),
            QuestObjectiveType = d.QuestObjectiveType.ToString(),
            PlayerLevel = d.PlayerLevel,
            ExpReward = d.ExpReward,
            GoldReward = d.GoldReward
        });
    }

    public void LogBossBattleEndGame(BossBattleData d)
    {
        if (!Ready(AnalyticsEventNames.BossBattleEndGame)) return;
        AnalyticsService.Instance.RecordEvent(new BossBattleEndGameEvent
        {
            BossName = d.BossName,
            Result = d.Result.ToString(),
            PlayerLevel = d.PlayerLevel,
            PlayerAtkPoint = d.PlayerAtkPoint,
            PlayerHpPoint = d.PlayerHpPoint,
            PlayerMoveSpeedPoint = d.PlayerMoveSpeedPoint,
            AttemptCount = d.AttemptCount
        });
    }

    public void LogTimeTrialQuestWinRate(TimeTrialQuestData d)
    {
        if (!Ready(AnalyticsEventNames.TimeTrialQuestWinRate)) return;
        AnalyticsService.Instance.RecordEvent(new TimeTrialQuestWinRateEvent
        {
            QuestName          = d.QuestName,
            QuestObjectiveType = d.QuestObjectiveType.ToString(),
            QuestTimeLimit     = d.QuestTimeLimit,
            QuestProgress      = d.QuestProgress,
            QuestTimeLeft      = d.QuestTimeLeft,
            QuestResult        = d.QuestResult
        });
    }

    public void Flush()
    {
        if (!Ready("Flush")) return;
        AnalyticsService.Instance.Flush();
    }

    private bool Ready(string name)
    {
        if (_isInitialised) return true;
        Debug.LogWarning($"[Unity Analytics] Skipping '{name}' — not initialised.");
        return false;
    }
}
