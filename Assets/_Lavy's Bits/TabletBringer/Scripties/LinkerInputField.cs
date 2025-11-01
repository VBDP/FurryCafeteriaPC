
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LinkerInputField : UdonSharpBehaviour
{
    InputField infld;
    public string linkToShare;
    void Start()
    {
        infld = GetComponent<InputField>();

        infld.text = linkToShare;
    }

    public void _OnTextEnd()
    {
        infld.text = linkToShare;
    }
}
