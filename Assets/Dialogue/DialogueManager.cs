using UnityEngine;
using TMPro; // For TextMeshPro UI
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    private string[] lines;
    private int index;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        lines = dialogue.lines;
        index = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    public void NextLine()
    {
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

    private void ShowLine()
    {
        dialogueText.text = lines[index];
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
