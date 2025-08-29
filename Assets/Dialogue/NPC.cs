using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isPlayerNearby;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.gameObject.activeInHierarchy)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
            }
            else
            {
                DialogueManager.Instance.NextLine();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
