using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AmplitudeAnalyticsManager : IAnalyticsManager
{
    private const string ENDPOINT = "https://api2.amplitude.com/2/httpapi";

    private readonly string _apiKey;
    private readonly bool _debugMode;
    private readonly MonoBehaviour _runner;

    private bool _isInitialised;
    private string _deviceId;     // UUID ต่อเครื่อง persistent ผ่าน PlayerPrefs

    // ── Constructor ───────────────────────────────────────
    public AmplitudeAnalyticsManager(
        string apiKey,
        MonoBehaviour coroutineRunner,
        bool debugMode = false)
    {
        _apiKey = apiKey;
        _runner = coroutineRunner;
        _debugMode = debugMode;
    }

    // ── Lifecycle ─────────────────────────────────────────
    public Task InitialiseAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("[Amplitude] API Key ว่าง — กรุณากรอกใน Inspector");
            return Task.CompletedTask;
        }

        // สร้าง device_id แบบ persistent ต่อเครื่อง
        const string key = "amp_device_id";
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetString(key, Guid.NewGuid().ToString());
            PlayerPrefs.Save();
        }
        _deviceId = PlayerPrefs.GetString(key);

        _isInitialised = true;
        Debug.Log($"[Amplitude] Initialised. device_id={_deviceId}");
        return Task.CompletedTask;
    }

    // ─────────────────────────────────────────────────────
    // 1. QuestAccepted
    // ─────────────────────────────────────────────────────
    public void LogQuestAccepted(QuestAcceptedData data)
    {
        if (!Ready(AnalyticsEventNames.QuestAccepted)) return;

        Send(AnalyticsEventNames.QuestAccepted,
            new Dictionary<string, object>
            {
                    { AnalyticsParams.QuestName,          data.QuestName },
                    { AnalyticsParams.QuestType,          data.QuestType.ToString() },
                    { AnalyticsParams.QuestObjectiveType, data.QuestObjectiveType.ToString() },
                    { AnalyticsParams.PlayerLevel,        data.PlayerLevel },
                    { AnalyticsParams.ExpReward,          data.ExpReward },
                    { AnalyticsParams.GoldReward,         data.GoldReward }
            });
    }

    // ─────────────────────────────────────────────────────
    // 2. BossBattleEndGame
    // ─────────────────────────────────────────────────────
    public void LogBossBattleEndGame(BossBattleData data)
    {
        if (!Ready(AnalyticsEventNames.BossBattleEndGame)) return;

        Send(AnalyticsEventNames.BossBattleEndGame,
            new Dictionary<string, object>
            {
                    { AnalyticsParams.BossName,             data.BossName },
                    { AnalyticsParams.Result,               data.Result.ToString() },
                    { AnalyticsParams.PlayerLevel,          data.PlayerLevel },
                    { AnalyticsParams.PlayerAtkPoint,       data.PlayerAtkPoint },
                    { AnalyticsParams.PlayerHpPoint,        data.PlayerHpPoint },
                    { AnalyticsParams.PlayerMoveSpeedPoint, data.PlayerMoveSpeedPoint },
                    { AnalyticsParams.AttemptCount,         data.AttemptCount }
            });
    }

    // ─────────────────────────────────────────────────────
    // 3. TimeTrialQuestWinRate
    // ─────────────────────────────────────────────────────
    public void LogTimeTrialQuestWinRate(TimeTrialQuestData data)
    {
        if (!Ready(AnalyticsEventNames.TimeTrialQuestWinRate)) return;

        Send(AnalyticsEventNames.TimeTrialQuestWinRate,
            new Dictionary<string, object>
            {
                    { AnalyticsParams.QuestName,          data.QuestName },
                    { AnalyticsParams.QuestObjectiveType, data.QuestObjectiveType.ToString() },
                    { AnalyticsParams.QuestTimeLimit,     data.QuestTimeLimit },
                    { AnalyticsParams.QuestProgress,      data.QuestProgress },
                    { AnalyticsParams.QuestTimeLeft,      data.QuestTimeLeft },
                    { AnalyticsParams.QuestResult,        data.QuestResult }
            });
    }

    // ── Flush ─────────────────────────────────────────────
    // Amplitude HTTP API เป็น stateless — ไม่มี buffer
    public void Flush() =>
        Debug.Log("[Amplitude] Flush — stateless HTTP, ไม่มี queue.");

    // ─────────────────────────────────────────────────────
    // Core: Build JSON และส่ง HTTP POST
    // ─────────────────────────────────────────────────────
    private void Send(string eventType, Dictionary<string, object> props)
    {
        // สร้าง event_properties JSON
        var propParts = new List<string>();
        foreach (var kv in props)
        {
            string v = kv.Value switch
            {
                int i => i.ToString(),
                long l => l.ToString(),
                float f => f.ToString("G",
                               System.Globalization.CultureInfo.InvariantCulture),
                double d => d.ToString("G",
                               System.Globalization.CultureInfo.InvariantCulture),
                string s => $"\"{Esc(s)}\"",
                _ => $"\"{Esc(kv.Value.ToString())}\"",
            };
            propParts.Add($"\"{kv.Key}\":{v}");
        }

        long timeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Amplitude HTTP API v2 schema
        // https://www.docs.developers.amplitude.com/analytics/apis/http-v2-api/
        string json =
            "{" +
            $"\"api_key\":\"{_apiKey}\"," +
            "\"events\":[{" +
            $"\"device_id\":\"{_deviceId}\"," +
            $"\"event_type\":\"{eventType}\"," +
            $"\"time\":{timeMs}," +
            "\"event_properties\":{" +
            string.Join(",", propParts) +
            "}" +
            "}]" +
            "}";

        if (_debugMode)
            Debug.Log($"[Amplitude] Sending → {eventType}\n{json}");

        _runner.StartCoroutine(PostCoroutine(json, eventType));
    }

    private IEnumerator PostCoroutine(string json, string eventType)
    {
        byte[] body = Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(ENDPOINT, "POST");
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "*/*");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            // Amplitude คืน HTTP 200 พร้อม {"code":200} เมื่อสำเร็จ
            if (_debugMode)
                Debug.Log($"[Amplitude] '{eventType}' OK → {req.downloadHandler.text}");
            else
                Debug.Log($"[Amplitude] '{eventType}' sent.");
        }
        else
        {
            Debug.LogWarning(
                $"[Amplitude] '{eventType}' failed: {req.error} " +
                $"(HTTP {req.responseCode}) → {req.downloadHandler.text}");
        }
    }

    // ── Helpers ───────────────────────────────────────────
    private bool Ready(string name)
    {
        if (_isInitialised) return true;
        Debug.LogWarning($"[Amplitude] Skipping '{name}' — not initialised.");
        return false;
    }

    private static string Esc(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"")
         .Replace("\n", "\\n").Replace("\r", "\\r");
}
