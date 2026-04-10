using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisableAudioSources : UdonSharpBehaviour
{
    public AudioSource targetAudio;

    private bool originalState;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (targetAudio != null)
        {
            // Guardar el estado REAL antes de cambiarlo
            originalState = targetAudio.enabled;

            // Desactivar siempre al entrar
            targetAudio.enabled = false;
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (targetAudio != null)
        {
            // Restaurar exactamente como estaba
            targetAudio.enabled = originalState;
        }
    }
}