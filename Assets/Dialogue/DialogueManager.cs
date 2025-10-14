using UnityEngine;
using TMPro; // For TextMeshPro UI
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public GameObject interactPromptPanel;

    [Header("Settings")]
    [Range(0.01f, 0.2f)]
    [Tooltip("Seconds per character when typing out dialogue")]
    public float typeSpeed = 0.05f; // Seconds per character
    public AudioSource dialogueSound;

    public bool DialogueActive { get; private set; }
    public UnityEvent OnDialogueEnd;

    private string[] lines;
    private int index;
    private Coroutine typingCoroutine;
    private bool isTyping;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
        interactPromptPanel.SetActive(false);
        DialogueActive = false;
    }

    void Update()
    {
        if (DialogueActive && isTyping)
        {
            if (!dialogueSound.isPlaying)
            {
                dialogueSound.Play();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Disable a bunch of stuff
        FindObjectOfType<DoorPlacer>().Disabled = true;
        FindObjectOfType<DoorPlacer>().ExitPlacementMode();
        FindObjectOfType<Minimap>().Locked = true;
        FindObjectOfType<Minimap>().DisableMinimap();
        FindObjectOfType<Minimap>().minimapPrompt.SetActive(false);
        FindObjectOfType<FirstPersonMovement>().Disable();
        FindObjectOfType<FirstPersonLook>().Disabled = true;

        nameText.text = dialogue.name;
        lines = dialogue.lines;
        index = 0;
        dialoguePanel.SetActive(true);
        DialogueActive = true;
        ShowLine();
    }

    public void NextLine()
    {
        if (isTyping)
        {
            // If still typing, instantly show full line
            FinishTyping();
        }
        else
        {
            // Otherwise go to next
            if (index < lines.Length - 1)
            {
                index++;
                ShowLine();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void ShowInteractPrompt()
    {
        interactPromptPanel.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        interactPromptPanel.SetActive(false);
    }

    private void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(lines[index]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        float timer = 0f;
        int charIndex = 0;

        while (charIndex < line.Length)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= typeSpeed)
            {
                timer -= typeSpeed;
                dialogueText.text += line[charIndex];
                charIndex++;
            }

            yield return new WaitForFixedUpdate();
        }

        isTyping = false;
    }

    private void FinishTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = lines[index]; // Show full line instantly
        isTyping = false;
    }

    private void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
        dialoguePanel.SetActive(false);
        DialogueActive = false;

        // Re-enable all the stuff
        FindObjectOfType<DoorPlacer>().Disabled = false;
        FindObjectOfType<Minimap>().Locked = false;
        FindObjectOfType<Minimap>().minimapPrompt.SetActive(true);
        FindObjectOfType<FirstPersonMovement>().Enable();
        FindObjectOfType<FirstPersonLook>().Disabled = false;
    }
}
