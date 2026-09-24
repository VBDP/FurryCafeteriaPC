
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDK3.Persistence;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
#endif

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class ToggleButton : UdonSharpBehaviour
{
    [SerializeField] private Image iconOff;
    [SerializeField] private Image iconOn;
    public bool isOn = true;
    [SerializeField] private GameObject[] onObjects;
    [SerializeField] private GameObject[] offObjects;
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

    public void _ToggleOn()
    {
        isOn = true;
        _UpdateObjects();
        SaveState();
    }

    public void _ToggleOff()
    {
        isOn = false;
        _UpdateObjects();
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

        // Null checks before accessing objects
        if (onObjects != null)
        {
            foreach (GameObject onObject in onObjects)
            {
                if (onObject != null)
                {
                    onObject.SetActive(isOn);
                }
            }
        }

        if (offObjects != null)
        {
            foreach (GameObject offObject in offObjects)
            {
                if (offObject != null)
                {
                    offObject.SetActive(!isOn);
                }
            }
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    public void AddChairs()
    {
        offObjects = new GameObject[0];

        VRCStation[] objects = FindObjectsOfType<VRCStation>();
        onObjects = new GameObject[objects.Length];
        for (int i = 0; i < onObjects.Length; i++)
        {
            onObjects[i] = objects[i].gameObject;
        }
    }

    public void AddPost()
    {
        offObjects = new GameObject[0];

        PostProcessVolume[] objects = FindObjectsOfType<PostProcessVolume>();
        onObjects = new GameObject[objects.Length];
        for (int i = 0; i < onObjects.Length; i++)
        {
            onObjects[i] = objects[i].gameObject;
        }
    }

    public void AddSfx()
    {
        offObjects = new GameObject[0];

        AudioSource[] objects = FindObjectsOfType<AudioSource>();
        onObjects = new GameObject[objects.Length];
        for (int i = 0; i < onObjects.Length; i++)
        {
            onObjects[i] = objects[i].gameObject;
        }
    }

    public void AddParticles()
    {
        offObjects = new GameObject[0];

        ParticleSystem[] objects = FindObjectsOfType<ParticleSystem>();
        onObjects = new GameObject[objects.Length];
        for (int i = 0; i < onObjects.Length; i++)
        {
            onObjects[i] = objects[i].gameObject;
        }
    }
#endif
}
