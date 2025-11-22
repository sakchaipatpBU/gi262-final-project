using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameOver = false;
    public GameObject gameOverPanel;
    public PlayerCharacter player;


    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void GaveOver()
    {
        QuestManager.Instance.ResetProgress();
        gameOverPanel.SetActive(true);
        player.gameObject.GetComponent<PlayerController>().enabled = false;
        player.enabled = false;
        isGameOver = true;
    }

    public void OnSaveClicked()
    {
        SaveGame.SavePlayerData(player);
    }
    public void OnResetClicked()
    {
        SaveGame.ClearAllData();
    }
}
