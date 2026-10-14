using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class Closet_Interact : MonoBehaviour
{
    [Header("UI & Input")]
    public bool canInteract;
    public TextMeshProUGUI interactText;
    public DialogueRunner dialogue;

    [Header("NPC Data & PC References")]
    public NPC_Data ryomaData;     
    public boxes_Interact pcInteract;

    [Header("Objects to Destroy/Spawn")]
    public GameObject closetLockObject; 
    public GameObject tennisBallObject; 

    [Header("Dialogue Nodes")]
    public string closetLockedNode = "Closet_Locked";
    public string closetUnlockedNode = "Closet_Unlocked";

    [Header("State")]
    public bool isUnlocked = false;

    private void Start()
    {
        // Hide tennis ball initially if it's already in scene
        if (tennisBallObject != null)
        {
            tennisBallObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if Ryoma is waiting for the balls (PC was solved) OR PC is on
            isUnlocked = CheckIfUnlocked();

            if (!isUnlocked)
            {
                setText("The closet is locked");
            }
            else
            {
                setText("Open the closet");
            }

            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setText("");
            canInteract = false;
        }
    }

    public void setText(string txt)
    {
        if (interactText != null)
        {
            interactText.text = txt;
        }
    }

    public void Update()
    {
        if (canInteract && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Re-verify unlock status in case PC was cleared while nearby
            isUnlocked = CheckIfUnlocked();

            setText("");

            // Lock player movement during dialogue
            if (GameController.instance != null)
            {
                GameController.instance.setPlayerMovement(false);
            }

            if (isUnlocked)
            {
                // Player knows code / PC is unlocked
                OpenAndDestroyCloset();

                if (dialogue != null)
                {
                    dialogue.StartDialogue(closetUnlockedNode);
                }
            }
            else
            {
                // Closet is still locked
                if (dialogue != null)
                {
                    dialogue.StartDialogue(closetLockedNode);
                }
            }

            canInteract = false;
        }
    }

    private bool CheckIfUnlocked()
    {
        // Unlocked if Ryoma's phase is quest_taken_return (PC turned on) 
        // or if pcInteract explicitly reports the screen is on
        bool ryomaReady = (ryomaData != null) &&
            (ryomaData.currentPhase == NPC_Data.dialogPhase.quest_taken_return ||
             ryomaData.currentPhase == NPC_Data.dialogPhase.quest_complete);

        bool pcReady = (pcInteract != null) && pcInteract.knowPW;

        return ryomaReady || pcReady;
    }

    public void OpenAndDestroyCloset()
    {
        // Enable Tennis Ball
        if (tennisBallObject != null)
        {
            tennisBallObject.SetActive(true);
            Debug.Log("[Closet_Interact] Tennis ball revealed!");
        }

        // Destroy Lock or Closet object
        if (closetLockObject != null)
        {
            Destroy(closetLockObject);
            Debug.Log("[Closet_Interact] Closet doors/lock destroyed!");
        }
        else
        {
            // Destroy this GameObject if no specific child target is provided
            Destroy(gameObject);
        }
        if (GameController.instance != null)
        {
            GameController.instance.setPlayerMovement(true);
        }
    }

    [YarnCommand("unlockPlayerFromCloset")]
    public void UnlockPlayer()
    {
        if (GameController.instance != null)
        {
            GameController.instance.setPlayerMovement(true);
        }
    }
}