using UnityEngine;

public class Ending : MonoBehaviour
{
    public Dialogue endingDialogue;
    public GameObject dialoguePanel;
    public GameObject canvasImage;

    public void EndGame()
    {
        // Parent the dialogue panel to the canvas image to ensure visibility
        dialoguePanel.transform.SetParent(canvasImage.transform, false);
        DialogueManager.Instance.StartDialogue(endingDialogue);
        Invoke("HidePanel", 5f);
        Invoke("HandleGameEnd", 6f);
    }

    private void HidePanel()
    {
        dialoguePanel.SetActive(false);
    }

    private void HandleGameEnd()
    {
        LevelLoader.Instance.LoadMenu();
    }
}
