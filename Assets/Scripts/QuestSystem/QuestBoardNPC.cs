using UnityEngine;

public class QuestBoardNPC : MonoBehaviour
{
    public bool canOpenQuestBoardUI = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpenQuestBoardUI = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpenQuestBoardUI = false;
            QuestUIManager.Instance.CloseBoardQuest();
        }
    }
}
