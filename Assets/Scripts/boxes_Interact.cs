using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class boxes_Interact : MonoBehaviour
{
    public bool canInteract;
    public TextMeshProUGUI interactText;
    public DialogueRunner dialogue;

    public GameObject modelOff;
    public GameObject modelOn;

    public NPC_Data ralphData;
    public NPC_Data ryomaData;

    public string pcLockedNode = "PC_locked";
    public string pcUnlockedNode = "PC_unlocked";

    public bool knowPW = false;

    private void Start()
    {
        knowPW = false;
        if (modelOff != null) modelOff.SetActive(true);
        if (modelOn != null) modelOn.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!knowPW)
            {
                setText("Turn on PC");
            }
            else
            {
                setText("Check PC");
            }
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText("");
            canInteract = false;
        }
    }

    public void setText(string txt)
    {
        interactText.text = txt;
    }

    public void Update()
    {
        if (canInteract && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (dialogue != null && dialogue.IsDialogueRunning)
            {
                Debug.LogWarning("[PC_Interact] Dialogue is ALREADY running! Waiting for it to finish.");
                return;
            }
            setText("");
            bool knowsPassword = (ralphData != null) &&
                (ralphData.currentPhase == NPC_Data.dialogPhase.quest_taken2 ||
                 ralphData.currentPhase == NPC_Data.dialogPhase.repeat);

            if (knowsPassword && !knowPW)
            {
                LockPlayer();
                TurnOnPC();

                if (ryomaData != null)
                {
                    ryomaData.currentPhase = NPC_Data.dialogPhase.quest_taken_return;
                }

                if (dialogue != null)
                {
                    dialogue.StartDialogue(pcUnlockedNode);
                }

                StartPCDialogue(pcUnlockedNode);
            }
            else if (knowPW)
            {
                LockPlayer();
                StartPCDialogue(pcUnlockedNode);
            }
            else
            {
                LockPlayer();
                StartPCDialogue(pcLockedNode);
            }

            canInteract = false;
        }
    }

    private void StartPCDialogue(string nodeName)
    {
        if (dialogue != null)
        {
            // Ensure no ghost dialogues are active
            dialogue.Stop();

            Debug.Log($"[PC_Interact] Launching Dialogue Node: {nodeName}");
            dialogue.StartDialogue(nodeName);
        }
        else
        {
            Debug.LogError("[PC_Interact] DialogueRunner component reference is missing in Inspector!");
        }
    }
    private void LockPlayer()
    {
        if (GameController.instance != null)
        {
            GameController.instance.setPlayerMovement(false);
        }
    }
    [YarnCommand("unlockPlayerFromPC")]
    public void UnlockPlayer()
    {
        if (GameController.instance != null)
        {
            GameController.instance.setPlayerMovement(true);
        }
    }

    [YarnCommand("turnOnPC")]
    public void TurnOnPC()
    {
        knowPW = true;
        if (modelOff != null) modelOff.SetActive(false);
        if (modelOn != null) modelOn.SetActive(true);
        Debug.Log("[PC_Interact] PC Screen turned ON!");
    }
}
