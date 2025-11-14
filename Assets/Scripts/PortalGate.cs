using UnityEngine;

public class PortalGate : MonoBehaviour
{
    public GameObject selectMapUI;


    private void Start()
    {
        selectMapUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            selectMapUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(selectMapUI != null)
            selectMapUI.SetActive(false);
        }
    }
}

