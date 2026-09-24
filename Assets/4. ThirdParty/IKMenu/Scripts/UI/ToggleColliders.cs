
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.Persistence;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class ToggleColliders : UdonSharpBehaviour
{
    [SerializeField] private Image iconOff;
    [SerializeField] private Image iconOn;
    public bool isOn = true;
    [SerializeField] private Collider[] onObjects;
    [SerializeField] private Collider[] offObjects;
    private AudioSource audioSource;

    [Header("Persistencia de VRChat")]
    [Tooltip("La clave única registrada en tu PlayerData Profile para guardar esta opción.")]
    public string saveKey;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _UpdateObjects();
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (player != null && player.isLocal && !string.IsNullOrEmpty(saveKey))
        {
            bool savedValue;
            if (PlayerData.TryGetBool(player, saveKey, out savedValue))
            {
                isOn = savedValue;
                _UpdateObjects();
            }
        }
    }

    public void _ToggleChange()
    {
        isOn = !isOn;
        _UpdateObjects();
        audioSource.Play();
        SaveState();
    }

    private void SaveState()
    {
        if (!string.IsNullOrEmpty(saveKey))
        {
            PlayerData.SetBool(saveKey, isOn);
        }
    }

    private void _UpdateObjects()
    {
        Color baseColor = iconOn.color;
        baseColor.a = isOn ? 1.0f : 0.25f;
        iconOn.color = baseColor;

        baseColor = iconOff.color;
        baseColor.a = isOn ? 0.25f : 1.0f;
        iconOff.color = baseColor;

        if (onObjects != null)
        {
            foreach (Collider onObject in onObjects)
            {
                if (onObject != null)
                {
                    onObject.enabled = isOn;
                }
            }
        }

        if (offObjects != null)
        {
            foreach (Collider offObject in offObjects)
            {
                if (offObject != null)
                {
                    offObject.enabled = !isOn;
                }
            }
        }
    }
}

