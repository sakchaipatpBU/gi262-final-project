using UnityEngine;

public class InitializeScene : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    private void Start()
    {
        if(playerCharacter == null)
        {
            playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        }
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
