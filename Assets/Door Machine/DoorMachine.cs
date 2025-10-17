using UnityEngine;

public class DoorMachine : MonoBehaviour
{
    public Dialogue fullDialogue;
    public Dialogue emptyDialogue;
    public int doorAmount = 6;
    public NPC npc;
    bool used = false;
    public AudioSource dispenseDoor;

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
        dispenseDoor.Play();
        // Give the player dooooors
        DoorManager.Instance.doorCount += doorAmount;
        if (!DoorManager.Instance.receivedDoors)
        {
            WarningFlash.Instance.FlashWarning("Q", 200);
            DoorManager.Instance.receivedDoors = true;
        }
    }
}
