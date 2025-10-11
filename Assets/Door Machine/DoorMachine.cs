using UnityEngine;

public class DoorMachine : MonoBehaviour
{
    public Dialogue fullDialogue;
    public Dialogue emptyDialogue;
    public NPC npc;
    bool used = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npc.dialogue = fullDialogue;
        npc.OnDialogueStart.AddListener(DialogueStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DialogueStart()
    {
        FindObjectOfType<DialogueManager>().OnDialogueEnd.AddListener(DialogueEnd);
    }

    void DialogueEnd()
    {
        FindObjectOfType<DialogueManager>().OnDialogueEnd.RemoveListener(DialogueEnd);
        npc.dialogue = emptyDialogue;
        if (used) return;
        used = true;
        // Give the player dooooors
        DoorManager.Instance.doorCount += 2;
        if (!DoorManager.Instance.receivedDoors)
        {
            WarningFlash.Instance.FlashWarning("Q", 200);
            DoorManager.Instance.receivedDoors = true;
        }
    }
}
