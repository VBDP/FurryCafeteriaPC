
using System.ComponentModel;
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

    public class U_SwimmingPlayerMovement : U_BasicPlayerMovement
    {
        private float underWaterWalkSpeed, underWaterRunSpeed, underWaterStrafeSpeed, underWaterJumpImpulse, underWaterGravity, swimmingVelocityMod, swimmingVelocityVerticalMod, waterJumpMod;

        private bool isInWater = false, isHeadUnderWater = false;

        public U_HashStudiosSwimmingSystem_Main main;


        void Start() { Initialize(); SetWaterInteractions(); }


        //constantly update player velocity, eye + head position, and if their feet are on ground (even if underwater at the same time)
        void Update() { UpdatePlayerParameters(); }


        //getters and setters, resetters, and specific toggles relevant to underwater movement functionalities for a player

        public bool AreFeetInWater() { return isInWater; }
        public bool IsHeadUnderWater() { return isHeadUnderWater; }
        public bool ToggleHeadIsUnderWater(bool value) { return isHeadUnderWater = value; }
        public void ToggleIsFeetInWater(bool value) { isInWater = value; }
        public void ResetToGroundedMovement() { ResetMovementParameters(); }
        public void SetToWaterMovement()
        {
            Debug.Log("Setting to water movement.");
            SetGravityStrength(underWaterGravity);
            SetRunSpeed(underWaterRunSpeed);
            SetWalkSpeed(underWaterWalkSpeed);
            SetStrafeSpeed(underWaterStrafeSpeed);
            SetJumpImpulse(underWaterJumpImpulse);
        }
        public void SetVelocity(Vector3 v) { player.SetVelocity(v); }
        public void SetWaterInteractions()
        {
            this.underWaterWalkSpeed = main.underWaterWalkSpeed;
            this.underWaterRunSpeed = main.underWaterRunSpeed;
            this.underWaterStrafeSpeed = main.underWaterStrafeSpeed;
            this.underWaterJumpImpulse = main.underWaterJumpImpulse;
            this.underWaterGravity = main.underWaterGravity;
            this.swimmingVelocityMod = main.swimmingVelocityMod;
            this.swimmingVelocityVerticalMod = main.swimmingVelocityVerticalMod;
            this.waterJumpMod = main.waterJumpMod;
        }
        public float GetVelocityMod() { return this.swimmingVelocityMod; }
        public float GetVerticalVelocityMod() { return this.swimmingVelocityVerticalMod; }
        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (Networking.LocalPlayer.IsValid() && Networking.LocalPlayer != null && IsHeadUnderWater()) Networking.LocalPlayer.SetVelocity(new Vector3(Networking.LocalPlayer.GetVelocity().x, Networking.LocalPlayer.GetVelocity().y + waterJumpMod, Networking.LocalPlayer.GetVelocity().z));
        }
    }
}