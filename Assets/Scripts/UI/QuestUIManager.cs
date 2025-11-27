using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentParent;  // Content in ScrollView
    public GameObject questSlotPrefab;
    public GameObject questBoardPanel; // parent of all Quest Board UI
    public GameObject questTrackingUIPanel;
    public GameObject questTimeTrailUIPanel;
    public QuestProgress currentQuest;
    public QuestTrackingUI questTrackingUI;
    public QuestTimeTrailUI questTimeTrailUI;

    private List<QuestData> allQuests = new List<QuestData>();
    private List<QuestSlotUI> allQuestsOnBoard = new List<QuestSlotUI>();

    public QuestBoardNPC questBoardNPC;
    private InputAction questAction;
    [SerializeField] private bool isDisplay = false;
    [SerializeField] private bool canDisplay = false;

    private static QuestUIManager instance;
    public static QuestUIManager Instance {  get { return instance; } }
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        questAction = InputSystem.actions.FindAction("Interact");

    }
    public void Init(Transform _contentParent, GameObject _questBoardPanel, GameObject _questTrackingUIPanel
        , GameObject _questTimeTrailUIPanel, QuestTrackingUI _questTrackingUI, QuestTimeTrailUI _questTimeTrailUI
        , QuestBoardNPC _questBoardNPC)
    {
        contentParent = _contentParent;
        questBoardPanel = _questBoardPanel;
        questTrackingUIPanel = _questTrackingUIPanel;
        if (_questTimeTrailUIPanel != null)
        {
            questTimeTrailUIPanel = _questTimeTrailUIPanel;
        }
        questTrackingUI = _questTrackingUI;
        if (_questTimeTrailUI != null)
        {
            questTimeTrailUI = _questTimeTrailUI;
        }
        if (_questBoardNPC != null)
        {
            questBoardNPC = _questBoardNPC;
        }

        LoadAllQuests();
        GenerateQuestListUI();
        questBoardPanel.SetActive(false);
        if (questTimeTrailUI != null)
        {
            questTimeTrailUIPanel.SetActive(false);
        }
    }
    private void Start()
    {
        LoadAllQuests();
        GenerateQuestListUI();
        questBoardPanel.SetActive(false);
        if(questTimeTrailUI != null)
        {
            questTimeTrailUIPanel.SetActive(false);
        }
    }
    private void Update()
    {
        if(QuestManager.Instance.HasActiveQuest())
        {
            currentQuest = QuestManager.Instance.currentQuest;
            questTrackingUI.Initialize(currentQuest);
            questTrackingUIPanel.SetActive(true);

            if(currentQuest.questData.questType == QuestType.TimeTrail
                && questTimeTrailUI != null)
            {
                if (!questTimeTrailUI.isInit)
                {
                    questTimeTrailUI.Init(currentQuest);
                    questTimeTrailUIPanel.SetActive(true);
                }
                if (currentQuest.isCompleted)
                {
                    questTimeTrailUIPanel.SetActive(false);
                }
            }
        }
        else
        {
            questTrackingUIPanel.SetActive(false);

            if (questTimeTrailUI != null)
            {
                questTimeTrailUIPanel.SetActive(false);
            }
        }

        if(questBoardNPC == null) return;
        canDisplay = questBoardNPC.canOpenQuestBoardUI;
        if (questAction.triggered)
        {
            if(canDisplay && !isDisplay)
            {
                isDisplay = true;
                questBoardPanel.SetActive(true);
                UpdateAllQuestSlot();
                SoundManager.Instance.PlaySFX("Click_UI", 0.3f);

            }
            else if (canDisplay && isDisplay)
            {
                isDisplay = false;
                questBoardPanel.SetActive(false);
                SoundManager.Instance.PlaySFX("Click_UI", 0.3f);

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
    public void CloseBoardQuest()
    {
        if (isDisplay)
        {
            SoundManager.Instance.PlaySFX("Click_UI", 0.3f);
        }
        isDisplay = false;
        if (questBoardPanel != null)
        questBoardPanel.SetActive(false);
    }

    public void OnExitQuestBoardButtonClicked()
    {
        isDisplay = false;
    }

    public void CancelQuestByQuestTrackingUI()
    {
        currentQuest = null;
        QuestManager.Instance.CancelQuest();
    }
}
