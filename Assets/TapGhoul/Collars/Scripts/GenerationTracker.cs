using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace TapGhoul.Collars
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class GenerationTracker : UdonSharpBehaviour
    {
        [UdonSynced, HideInInspector] public short generation = 0;
    }
}