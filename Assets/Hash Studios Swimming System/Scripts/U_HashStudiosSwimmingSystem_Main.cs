
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

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

    public class U_HashStudiosSwimmingSystem_Main : UdonSharpBehaviour
    {
        public Material underwaterVisualEffect;
        [Header("Adjust for Comfort.\n")]
        public float underWaterWalkSpeed;
        public float underWaterRunSpeed;
        public float underWaterStrafeSpeed;
        public float underWaterJumpImpulse;
        public float underWaterGravity;
        public float swimmingVelocityMod;
        public float swimmingVelocityVerticalMod;
        public AudioSource waterEnterAudio;
        public AudioSource underWaterAudio;
        [Header("\nUse this modifier if you want a player to more easily 'jump' in the water. \n\nThis may make it easier for a player who is in a pool to force themselves out. (Optional)\n")]
        public float waterJumpMod;
        [Header("\nCheck Debug Mode boolean if you are actively experimenting with swimming values in inspector\n\nThis allows swimming values to update repeatedly instead of only once per scene load.\n\nOtherwise, leave boolean as unchecked.")]
        public bool debugMode;

        void Start()
        {

        }

    }
}