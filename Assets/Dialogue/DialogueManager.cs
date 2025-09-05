using UnityEngine;
using TMPro; // For TextMeshPro UI
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject interactPromptPanel;

    [Header("Settings")]
    [Range(0.01f, 0.2f)]
    [Tooltip("Seconds per character when typing out dialogue")]
    public float typeSpeed = 0.05f; // Seconds per character

    public bool DialogueActive { get; private set; }

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

    public void StartDialogue(Dialogue dialogue)
    {
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
        dialoguePanel.SetActive(false);
        DialogueActive = false;
    }
}
