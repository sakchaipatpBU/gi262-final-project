using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameAnalyticsService : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────
    private static GameAnalyticsService _instance;
    public static GameAnalyticsService Instance
    {
        get
        {
            if (_instance) return _instance;
            var go = new GameObject("[GameAnalyticsService]");
            _instance = go.AddComponent<GameAnalyticsService>();
            DontDestroyOnLoad(go);
            return _instance;
        }
    }

    private readonly List<IAnalyticsManager> _backends = new();
    private bool _isReady;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        foreach (var b in _backends) b.Flush();
    }

    // ── Configure (เรียกจาก Bootstrapper) ─────────────────
    public async void Configure(
        bool useAmplitude,
        string amplitudeApiKey,
        bool debugMode,
        bool useUnityAnalytics)
    {
        _backends.Clear();
        _isReady = false;

        var tasks = new List<Task>();

        if (useAmplitude)
        {
            var amp = new AmplitudeAnalyticsManager(
                    apiKey: amplitudeApiKey,
                    coroutineRunner: this,
                    debugMode: debugMode
                );
            _backends.Add(amp);
            tasks.Add(amp.InitialiseAsync());
        }

        if (useUnityAnalytics)
        {
            var unity = new UnityAnalyticsManager();
            _backends.Add(unity);
            tasks.Add(unity.InitialiseAsync());
        }

        await Task.WhenAll(tasks);
        _isReady = true;
        Debug.Log($"[GameAnalyticsService] Ready. Backends: {_backends.Count}");
    }

    // ── Public API ────────────────────────────────────────
    public void LogQuestAccepted(QuestAcceptedData data)
    {
        if (!WarnIfNotReady(AnalyticsEventNames.QuestAccepted)) return;
        foreach (var b in _backends) b.LogQuestAccepted(data);
    }

    public void LogBossBattleEndGame(BossBattleData data)
    {
        if (!WarnIfNotReady(AnalyticsEventNames.BossBattleEndGame)) return;
        foreach (var b in _backends) b.LogBossBattleEndGame(data);
    }

    public void LogTimeTrialQuestWinRate(TimeTrialQuestData data)
    {
        if (!WarnIfNotReady(AnalyticsEventNames.TimeTrialQuestWinRate)) return;
        foreach (var b in _backends) b.LogTimeTrialQuestWinRate(data);
    }

    private bool WarnIfNotReady(string name)
    {
        if (_isReady) return true;
        Debug.LogWarning($"[GameAnalyticsService] '{name}' dropped — not ready.");
        return false;
    }
}
