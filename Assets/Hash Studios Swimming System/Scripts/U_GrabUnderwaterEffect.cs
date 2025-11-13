
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HashStudiosSwimmingSystem.Scripts
{
    /// <summary>
    /// Copyright (c) 2024 Hash Studios LLC. All rights reserved.
    /// This code is the sole property of Hash Studios LLC and is protected by copyright laws.
    /// It may not be used, reproduced, or distributed without the express written permission of Hash Studios LLC.
    ///
    /// Any unauthorized use of this code is strictly prohibited and may result in legal action.
    ///
    /// This copyright notice must be included in any copies or reproductions of this code.
    ///
    /// NOTE: This copyright notice does not apply to any dependencies or
    /// external libraries used in this code, such as UdonSharp, VRC SDK, or VRChat.
    /// The copyright and licensing terms of these dependencies apply to their respective code.
    /// Hash Studios LLC is not claiming any rights to these dependencies.
    /// </summary>
    public class U_GrabUnderwaterEffect : UdonSharpBehaviour
    {
        [Header("This script will ensure the material/shader in the parent prefab will be used in the proper child objects.\n\n" + "Leave this gameobject disabled in the editor.")]

        [SerializeField] private U_HashStudiosSwimmingSystem_Main swimmingPrefab;
        [SerializeField] private MeshRenderer underwaterEffectMat;

        void Start()
        {
            if (swimmingPrefab.underwaterVisualEffect != null) underwaterEffectMat.material = swimmingPrefab.underwaterVisualEffect;
        }
    }
}