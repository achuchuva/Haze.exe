using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isPlayerNearby;
    public UnityEvent OnDialogueStart;
    public GameObject[] removedHaze;

    void Update()
    {
        if (isPlayerNearby && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (!DialogueManager.Instance.DialogueActive)
            {
                DialogueManager.Instance.HideInteractPrompt();
                DialogueManager.Instance.StartDialogue(dialogue);
                OnDialogueStart?.Invoke();
                DialogueManager.Instance.OnDialogueEnd.AddListener(DialogueEnd);
            }
            else
            {
                DialogueManager.Instance.NextLine();
            }
        }
    }

    public void DialogueEnd()
    {
        isPlayerNearby = false;
        DialogueManager.Instance.OnDialogueEnd.RemoveListener(DialogueEnd);

        foreach (GameObject haze in removedHaze)
        {
            haze.SetActive(false);
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
