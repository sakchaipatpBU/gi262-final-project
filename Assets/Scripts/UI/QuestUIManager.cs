using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentParent;  // Content in ScrollView
    public GameObject questSlotPrefab;
    public GameObject questBoard;


    private List<QuestData> allQuests = new List<QuestData>();
    private List<QuestSlotUI> allQuestsOnBoard = new List<QuestSlotUI>();

    private PlayerCharacter playerCharacter;
    private InputAction questAction;
    [SerializeField] private bool isDisplay = false;
    public bool canDisplay = false;

    private static QuestUIManager instance;
    public static QuestUIManager Instance {  get { return instance; } }
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        questAction = InputSystem.actions.FindAction("Interact");
        LoadAllQuests();
        GenerateQuestListUI();
        questBoard.SetActive(false);
    }
    private void Update()
    {

        if (questAction.triggered)
        {
            if(canDisplay && !isDisplay)
            {
                isDisplay = true;
                UpdateAllQuestSlot();
                questBoard.SetActive(true);
            }
            else if (canDisplay && isDisplay)
            {
                isDisplay = false;
                questBoard.SetActive(false);
            }
        }
    }
    private void LoadAllQuests()
    {
        allQuests.Clear();
        QuestData[] loadedQuests = Resources.LoadAll<QuestData>("Quests");
        allQuests.AddRange(loadedQuests);

        Debug.Log($"Loaded {allQuests.Count} quests from Resources/Quests");
    }

    private void GenerateQuestListUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        allQuestsOnBoard.Clear();
        foreach (QuestData quest in allQuests)
        {
            GameObject slotObj = Instantiate(questSlotPrefab, contentParent);
            QuestSlotUI slot = slotObj.GetComponent<QuestSlotUI>();
            slot.SetQuestData(quest);
            allQuestsOnBoard.Add(slot);
        }
    }
    public void UpdateAllQuestSlot()
    {
        if (allQuestsOnBoard.Count == 0) return;

        foreach (QuestSlotUI quest in allQuestsOnBoard)
        {
            quest.UpdateButtonState();
        }
    }

    public void OnExitQuestBoardButtonClicked()
    {
        isDisplay = false;
    }
}
