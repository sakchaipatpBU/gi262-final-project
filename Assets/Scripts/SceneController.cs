using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public PlayerCharacter player;
    public string scene1;
    public string scene2;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
    }
    public void LoadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
