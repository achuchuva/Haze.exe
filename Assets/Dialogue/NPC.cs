using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isPlayerNearby;

    void Update()
    {
        if (isPlayerNearby && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (!DialogueManager.Instance.DialogueActive)
            {
                DialogueManager.Instance.HideInteractPrompt();
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
        {
            isPlayerNearby = true;
            DialogueManager.Instance.ShowInteractPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            DialogueManager.Instance.HideInteractPrompt();
        }
    }
}
