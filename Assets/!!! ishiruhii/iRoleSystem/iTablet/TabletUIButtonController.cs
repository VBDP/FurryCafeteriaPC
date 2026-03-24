using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TabletUIButtonController : UdonSharpBehaviour
{
    [Header("=== NETWORK STATE ===")]
    [UdonSynced] private bool syncedState;

    [Header("=== OBJECT TOGGLES (Synced) ===")]
    public GameObject[] objectToggles;

    [Header("=== ANIMATORS ===")]
    public Animator[] animators;

    [Header("INT PARAMETERS (Optional Exclusive)")]
    public string intParameterName;
    public int intValue = 1;
    public bool useIntParameter;
    public bool exclusiveIntMode; // Si es true, solo uno puede estar activo

    [Header("BOOL PARAMETERS")]
    public string boolParameterName;
    public bool useBoolParameter;

    [Header("=== UI COLOR SETTINGS ===")]
    public Image buttonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.red;

    [Header("=== TELEPORT (Optional) ===")]
    public Transform teleportTarget;

    // =========================

    public override void Interact()
    {
        Toggle();
    }

    // Llamar desde el botón UI con SendCustomEvent("UIButtonPressed")
    public void UIButtonPressed()
    {
        Toggle();
    }

    private void Toggle()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);

        syncedState = !syncedState;

        ApplyState();

        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        ApplyState();
    }

    private void ApplyState()
    {
        // ---- OBJECT TOGGLES ----
        if (objectToggles != null)
        {
            foreach (GameObject obj in objectToggles)
            {
                if (obj != null)
                    obj.SetActive(syncedState);
            }
        }

        // ---- ANIMATORS ----
        if (animators != null)
        {
            foreach (Animator anim in animators)
            {
                if (anim == null) continue;

                if (useBoolParameter && !string.IsNullOrEmpty(boolParameterName))
                {
                    anim.SetBool(boolParameterName, syncedState);
                }

                if (useIntParameter && !string.IsNullOrEmpty(intParameterName))
                {
                    if (syncedState)
                    {
                        anim.SetInteger(intParameterName, intValue);
                    }
                    else if (exclusiveIntMode)
                    {
                        anim.SetInteger(intParameterName, 0);
                    }
                }
            }
        }

        // ---- UI COLOR ----
        if (buttonImage != null)
        {
            buttonImage.color = syncedState ? onColor : offColor;
        }

        // ---- TELEPORT ----
        if (teleportTarget != null && syncedState)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player != null)
            {
                player.TeleportTo(
                    teleportTarget.position,
                    teleportTarget.rotation
                );
            }
        }
    }
}
