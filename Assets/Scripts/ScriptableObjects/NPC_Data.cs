using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;
    public string startingNode = "Start";
    public enum dialogPhase { start, casual, repeat, quest_taken, quest_taken2, quest_taken_return, quest_complete, quest_complete2, quest_complete_return }
    public dialogPhase currentPhase;

    public void OnEnable()
    {
        resetData();
    }
    public void resetData()
    {
        currentPhase = dialogPhase.start;
    }
}
