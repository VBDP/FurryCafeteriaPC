using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisableAudioSources : UdonSharpBehaviour
{
    public AudioSource targetAudio;
    public AudioSource targetAudio2;
    public AudioSource targetAudio3;

    private bool originalState;
    private bool originalState2;
    private bool originalState3;

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

        if (targetAudio2 != null)
        {
            originalState2 = targetAudio2.enabled;
            targetAudio2.enabled = false;
        }

        if (targetAudio3 != null)
        {
            originalState3 = targetAudio3.enabled;
            targetAudio3.enabled = false;
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