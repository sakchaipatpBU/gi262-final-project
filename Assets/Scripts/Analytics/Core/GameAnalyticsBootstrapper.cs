using UnityEngine;

public class GameAnalyticsBootstrapper : MonoBehaviour
{
    [Header("Amplitude")]
    [Tooltip("API Key จาก Amplitude → Settings → Projects → General")]
    [SerializeField] private string _amplitudeApiKey = "";

    [Header("Backend Toggles")]
    [SerializeField] private bool _useAmplitude = true;
    [SerializeField] private bool _useUnityAnalytics = true;

    [Header("Debug")]
    [Tooltip("true = log JSON ทุก event ลง Console\nปิดก่อน build release")]
    [SerializeField] private bool _debugMode = true;

    private void Awake()
    {
        GameAnalyticsService.Instance.Configure(
            useAmplitude: _useAmplitude,
            amplitudeApiKey: _amplitudeApiKey,
            debugMode: _debugMode,
            useUnityAnalytics: _useUnityAnalytics
        );
    }
}
