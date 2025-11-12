using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    void Update()
    {
        
    }

    public void GaveOver()
    {
        gameOverPanel.SetActive(true);

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
