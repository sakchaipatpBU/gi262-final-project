using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentParent;  // Content ของ ScrollView
    public GameObject questSlotPrefab;

    private List<QuestData> allQuests = new List<QuestData>();

    private void Start()
    {
        LoadAllQuests();
        GenerateQuestListUI();
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
            Destroy(child.gameObject); // เคลียร์ของเก่า

        foreach (QuestData quest in allQuests)
        {
            GameObject slotObj = Instantiate(questSlotPrefab, contentParent);
            QuestSlotUI slot = slotObj.GetComponent<QuestSlotUI>();
            slot.SetQuestData(quest);
        }
    }
}
