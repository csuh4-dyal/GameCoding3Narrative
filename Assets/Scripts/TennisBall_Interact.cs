using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TennisBall_Interact : MonoBehaviour
{
    [Header("UI & Input")]
    public bool canInteract;
    public TextMeshProUGUI interactText;

    [Header("NPC Data References")]
    public NPC_Data ryomaData; 
    public NPC_Data ralphData;
    public NPC_Interact2 questDone;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setText("Pick up Tennis Balls");
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
            setText("");
            if (interactText != null) interactText.text = "";

            // Call your existing getYarnPhase method directly
            if (questDone != null)
            {
                questDone.getYarnPhase("quest_complete");
            }

            canInteract = false;
            Destroy(gameObject);
        }
    }
}