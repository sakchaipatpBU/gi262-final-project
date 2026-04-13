using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour
{
    public GameObject cheatPanel;
    public QuestData[] questDataLists;
    
    public PlayerCharacter player;
    public QuestManager questManager;

    private InputAction cheatAction;

    private void Start()
    {
        cheatAction = InputSystem.actions.FindAction("Cheat");

        if(cheatPanel != null ) cheatPanel.SetActive(false);
    }
    private void Update()
    {
        if (cheatAction.WasPressedThisFrame())
        {
            cheatPanel.SetActive(!cheatPanel.activeInHierarchy);
        }
    }

    private PlayerCharacter GetPlayer()
    {
        if (player != null) return player;

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
            player = playerObj.GetComponent<PlayerCharacter>();
        if (player == null)
            Debug.LogWarning("[QuestManager] Player not found — call Init() from InitializeScene.");
        return player;
    }
    private QuestManager GetQuestManager()
    {
        if (questManager != null) return questManager;

        questManager = QuestManager.Instance;
        return questManager;
    }

    public void ButtonAdd100Exp()
    {
        player = GetPlayer();
        player.AddExperience(100);
    }
    public void ButtonAdd500Exp()
    {
        player = GetPlayer();
        player.AddExperience(500);
    }
    public void ButtonAdd1000Exp()
    {
        player = GetPlayer();
        player.AddExperience(1000);
    }
    public void ButtonAddCompleteQuest(int index)
    {
        questManager = GetQuestManager();
        questManager.completedQuests.Add(questDataLists[index]);
    }

    public void ButtonResetAllStatus()
    {
        SaveGame.ClearAllData();
    }

    public void ButtonReloadScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonSaveGame()
    {
        player = GetPlayer();
        SaveGame.SavePlayerData(player);
    }
}
