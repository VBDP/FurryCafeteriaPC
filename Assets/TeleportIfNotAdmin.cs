using Rinvo;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportIfNotAdmin : UdonSharpBehaviour
{
    public Transform teleportLocation;
    public Keypad keypad;          // Referencia al keypad
    public string[] allowList;     // Lista de nombres permitidos

    void Start()
    {
        if (keypad != null)
        {
            allowList = keypad.allowList;

            foreach (string allowedId in allowList)
            {
                Debug.Log("ID permitido: " + allowedId);
            }
        }
        else
        {
            Debug.LogError("Keypad no asignado en el inspector.");
        }
    }

    public void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log(player.displayName + " ha entrado en el trigger");

        bool isAllowed = false;

        foreach (string allowedId in allowList)
        {
            if (player.displayName == allowedId)
            {
                isAllowed = true;
                break;
            }
        }

        if (isAllowed)
        {
            Debug.Log(player.displayName + " está en la lista de permitidos. No se teletransporta.");
        }
        else
        {
            Debug.Log(player.displayName + " NO está en la lista. Se teletransporta.");
            player.TeleportTo(teleportLocation.position, teleportLocation.rotation);
        }
    }
}
