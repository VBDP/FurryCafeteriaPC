
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.Udon.Common.Interfaces;

public class ResetObjects : UdonSharpBehaviour
{
    public VRCObjectSync[] syncObjects;
    
    [Header("Lista de usuarios permitidos (además del Master)")]
    public string[] allowedPlayers;

    public override void Interact()
    {
        if (IsPlayerAllowed(Networking.LocalPlayer))
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(RespawnAllObjects));
        }
    }

    public void RespawnAllObjects()
    {
        if (syncObjects == null) return;

        int count = syncObjects.Length;
        for (int i = 0; i < count; i++)
        {
            VRCObjectSync _syncedObj = syncObjects[i];
            if (_syncedObj != null)
            {
                // Solo el dueño actual del objeto ejecuta el Respawn
                // Esto asegura que la sincronización de la física y posición sea autoritativa
                if (Networking.GetOwner(_syncedObj.gameObject) == Networking.LocalPlayer)
                {
                    _syncedObj.Respawn();
                }
            }
        }
    }

    private bool IsPlayerAllowed(VRCPlayerApi player)
    {
        if (player == null) return false;

        // El Master (dueño de la sala) siempre tiene permiso
        if (player.isMaster) return true;

        // Comprobar la lista de nombres permitidos
        if (allowedPlayers != null)
        {
            string localName = player.displayName;
            for (int i = 0; i < allowedPlayers.Length; i++)
            {
                if (allowedPlayers[i] != null && allowedPlayers[i].ToLower() == localName.ToLower())
                {
                    return true;
                }
            }
        }

        return false;
    }
}

