using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string name;

    [TextArea(2, 5)]
    public string[] lines;

    [Header("Audio")]
    public AudioClip[] dialogueSounds;
    [Range(1, 5)]
    public int frequencyLevel = 2; // Play sound every n characters
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 1.5f;
    public bool stopAudioSource = true;
}
