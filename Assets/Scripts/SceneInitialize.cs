using UnityEngine;

public class InitializeScene : MonoBehaviour
{
    PlayerCharacter playerCharacter;

    private void Start()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        if (playerCharacter != null)
        {
            QuestManager.Instance.Init(playerCharacter);
        }
        else
        {
            Debug.Log("InitializeScene Can NOT find Player");
        }
    }
}
