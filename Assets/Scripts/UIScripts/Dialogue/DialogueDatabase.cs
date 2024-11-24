using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue System/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    public List<Dialogue> dialogues; // List of all dialogue assets

    public Dialogue GetDialogueByName(string dialogueName)
    {
        return dialogues.Find(dialogue => dialogue.name == dialogueName);
    }
    public IEnumerator PlayDialogue(Dialogue dialogue)
    {
        foreach (var line in dialogue.lines)
        {
            yield return LineCanvas.Bottom.DisplayLineAndWaitForClick(line.speaker, line.text, null);
        }
    }
}