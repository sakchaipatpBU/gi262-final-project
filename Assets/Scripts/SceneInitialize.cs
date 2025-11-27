using UnityEngine;

public class InitializeScene : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    [Header("QuestUIManager")]
    public Transform contentParent;  // Content in ScrollView
    public GameObject questBoardPanel; // parent of all Quest Board UI
    public GameObject questTrackingUIPanel;
    public GameObject questTimeTrailUIPanel;
    public QuestTrackingUI questTrackingUI;
    public QuestTimeTrailUI questTimeTrailUI;
    public QuestBoardNPC questBoardNPC;


    private void Awake()
    {
        if (playerCharacter == null)
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

        QuestUIManager.Instance.Init(contentParent, questBoardPanel, questTrackingUIPanel
            , questTimeTrailUIPanel, questTrackingUI, questTimeTrailUI, questBoardNPC);
    }
    
}
