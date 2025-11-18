using UnityEngine;
using UnityEngine.InputSystem;

public class QuestTalk : MonoBehaviour
{
    public string npcName;
    [SerializeField] private bool canTalk = false;
    [SerializeField] private bool isTalk = false;
    private InputAction talkAction;

    void Start()
    {
        talkAction = InputSystem.actions.FindAction("Interact");
    }
    private void Update()
    {
        if (!canTalk || isTalk) return;
        if (talkAction.triggered)
        {
            isTalk = true;
            QuestManager.Instance.ReportProgress(npcName, QuestObjectiveType.Talk);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = false;
        }
    }
}
