using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class NPC_Interact : MonoBehaviour
{
    public TextMeshProUGUI interactText;
    public string interaction;
    public bool canInteract;
    public DialogueRunner dialogue;
    public NPC_Cam_Control camController;
    public NPC_Data npcData;

    public void Start()
    {
        //this behavior is a bit `static` compared to previous system design techniques
        //BUT we are making this more `hard-coded` in an attempt to standardize our NPC_Cam_Rig system
        camController = transform.GetComponentInChildren<NPC_Cam_Control>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            setText(interaction);
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            setText("");
            canInteract = false;
        }
    }

    //this is the setter method for our interact text
    public void setText(string txt)
    {
        interactText.text = txt;
    }

    public void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("You Talked to the NPC!");
                setText("");
                if (camController != null)
                {
                    camController.movePlayer(); // Moves player and locks movement via GameController
                }
                else
                {
                    // Fallback if camController isn't used
                    if (GameController.instance != null)
                    {
                        GameController.instance.setPlayerMovement(false);
                    }
                }
                InMemoryVariableStorage vData = Object.FindAnyObjectByType<InMemoryVariableStorage>();
                if (vData != null && npcData != null)
                {
                    string phaseString = npcData.currentPhase.ToString().ToLower();
                    vData.SetValue("$dialogPhase", phaseString);
                }
                dialogue.StartDialogue(npcData.startingNode);
                canInteract = false;
            }
        }
    }

    //this is the `set` of the current phase of our YS dialog pulled from our npcData variable
    [YarnCommand("dialogPhaseSetter")]
    public void setYarnPhase()
    {
        InMemoryVariableStorage vData = GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
        vData.SetValue("$dialogPhase", npcData.currentPhase.ToString());
    }

    //this is the `get` of the current phase MADE in our YS dialog based on what options we've selected.
    [YarnCommand("dialogPhaseGetter")]
    public void getYarnPhase(string phase)
    {
        if (phase == "casual")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.casual;
        }
        if (phase == "quest_taken")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.quest_taken;
        }
        if (phase == "quest_taken2")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.quest_taken2;
        }
        if (phase == "quest_taken_return")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.quest_taken_return;
        }
        if (phase == "quest_complete")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.quest_complete;
        }
        if (phase == "quest_complete_return")
        {
            npcData.currentPhase = NPC_Data.dialogPhase.quest_complete_return;
        }
    }

    public void OnApplicationQuit()
    {
        npcData.resetData();
    }
}
