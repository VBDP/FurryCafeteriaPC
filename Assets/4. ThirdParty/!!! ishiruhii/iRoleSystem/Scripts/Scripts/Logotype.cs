using UnityEngine;
using System.Collections.Generic;

public class Logotype : MonoBehaviour
{
    public Sprite banner;

    [Header("AUTO DETECTADOS")]
    public List<GameObject> detectedObjects = new List<GameObject>();
    public List<bool> detectedStates = new List<bool>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyDetectedState();
    }
#endif

    public void AutoDetect()
    {
        detectedObjects.Clear();
        detectedStates.Clear();

        string[] rootNames =
        {
            "[x] Ranks Givers",
            "[x] ObjectsManager",
            "[x] Private Zones",
            "[x] Prison Zones",
            "iRoleRankDisplay"
        };

        foreach (Transform child in transform)
        {
            for (int i = 0; i < rootNames.Length; i++)
            {
                if (child.name == rootNames[i])
                {
                    AddObject(child.gameObject);

                    // Estos detectan hijos directos
                    if (child.name == "[x] Ranks Givers" ||
                        child.name == "[x] ObjectsManager" ||
                        child.name == "[x] Private Zones" ||
                        child.name == "[x] Prison Zones")
                    {
                        foreach (Transform sub in child)
                        {
                            AddObject(sub.gameObject);
                        }
                    }

                    break;
                }
            }
        }
    }

    void AddObject(GameObject obj)
    {
        if (obj == null)
            return;

        if (!detectedObjects.Contains(obj))
        {
            detectedObjects.Add(obj);
            detectedStates.Add(obj.activeSelf);
        }
    }

    public void ApplyDetectedState()
    {
        if (detectedObjects == null || detectedStates == null)
            return;

        int length = Mathf.Min(detectedObjects.Count, detectedStates.Count);

        for (int i = 0; i < length; i++)
        {
            if (detectedObjects[i] != null)
                detectedObjects[i].SetActive(detectedStates[i]);
        }
    }
}

