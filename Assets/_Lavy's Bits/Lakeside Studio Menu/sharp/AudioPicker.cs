
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioPicker : UdonSharpBehaviour
{
    public int whichSound;
    public GameObject[] bgSound;

    public RectTransform pos;
    public RectTransform dot;

    public override void Interact()
    {
        for (int i = 0; i < bgSound.Length; i++)
        {
            bgSound[i].SetActive(false);
        }
        bgSound[whichSound].SetActive(true);
        dot.position = pos.position;
    }
}
