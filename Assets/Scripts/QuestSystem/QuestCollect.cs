using UnityEngine;

public class QuestCollect : MonoBehaviour
{
    public string itemName;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QuestManager.Instance.ReportProgress(itemName, QuestObjectiveType.Collect);

            //TO-DO sound effect

            Destroy(gameObject, 0.5f);
        }
    }
}
