using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;
}
