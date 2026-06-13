
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

    public class U_BasicPlayerMovement : UdonSharpBehaviour
    {
        /*This class is the base script that will interface with a players general movement parameters.
        It is is at the core of their swimming movement which will extend to this script and use additional physics parameters and functionalities for swimming.
        Default walk speed = 2
        Default run speed = 4
        Default strafe speed is 2
        Default jump impulse is 3
        default gravity strength = 1*/
        protected bool isOnGround = true;
        protected VRCPlayerApi player;
        private float defaultWalkSpeed, defaultRunSpeed, defaultStrafeSpeed, defaultJumpImpulse, defaultGravity, playerVelocityMagnitude, currentWalkSpeed, currentRunSpeed, currentStrafeSpeed, currentJumpImpulse, currentGravity, playerHeight;
        private Vector3 inputMovementVector, headPosition, playerVelocityVector, feetPosition;
        public GameObject followHeadPoint, followFeetPoint; // invisible game objects representative of the player's head and feet


        //Initialize basic movement parameters
        protected void Initialize()
        {
            player = Networking.LocalPlayer;
            if (player == null || !player.IsValid()) this.gameObject.SetActive(false);
            defaultWalkSpeed = player.GetWalkSpeed();
            defaultRunSpeed = player.GetRunSpeed();
            playerVelocityVector = player.GetVelocity();
            defaultGravity = player.GetGravityStrength();
            defaultStrafeSpeed = player.GetStrafeSpeed();
            defaultJumpImpulse = 3; //for some reason, GetJumpImpulse() was returning 0. So this had to be hard coded.
            isOnGround = player.IsPlayerGrounded();
            currentWalkSpeed = defaultWalkSpeed;
            currentRunSpeed = defaultRunSpeed;
            currentStrafeSpeed = defaultStrafeSpeed;
            currentJumpImpulse = defaultJumpImpulse;
            currentGravity = defaultGravity;
        }

        protected void UpdatePlayerParameters()
        {
            if (player == null || !player.IsValid()) return;
            playerVelocityMagnitude = player.GetVelocity().magnitude;
            headPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            feetPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin).position;
            followHeadPoint.transform.position = headPosition;
            followFeetPoint.transform.position = feetPosition;
            playerVelocityVector = player.GetVelocity();
            isOnGround = player.IsPlayerGrounded();
            playerHeight = player.GetAvatarEyeHeightAsMeters();
        }

        //overridden functions to detect input values.
        public override void InputMoveVertical(float value, UdonInputEventArgs args) { inputMovementVector.z = value; }
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args) { inputMovementVector.x = value; }

        public Vector3 GetMovementVector() { Debug.Log("Original unclamped movement vector: " + inputMovementVector); return Vector3.ClampMagnitude(inputMovementVector, 1f); }

        //setters and reseters for inherited water movement script extending to this basic movement parameters script.

        //Reset movement parameters
        private void ResetWalkSpeed() { currentWalkSpeed = defaultWalkSpeed; player.SetWalkSpeed(currentWalkSpeed); }
        private void ResetRunSpeed() { currentRunSpeed = defaultRunSpeed; player.SetRunSpeed(currentRunSpeed); }
        private void ResetGravityStrength() { currentGravity = defaultGravity; player.SetGravityStrength(currentGravity); }
        private void ResetStrafeSpeed() { currentStrafeSpeed = defaultStrafeSpeed; player.SetStrafeSpeed(currentStrafeSpeed); }
        private void ResetJumpImpulse() { currentJumpImpulse = defaultJumpImpulse; player.SetJumpImpulse(currentJumpImpulse); }

        protected void ResetMovementParameters()
        {
            ResetRunSpeed();
            ResetWalkSpeed();
            ResetStrafeSpeed();
            ResetJumpImpulse();
            ResetGravityStrength();
            Debug.Log("Resetted to default land movement.");
        }

        protected void SetWalkSpeed(float speed) { currentWalkSpeed = speed; player.SetWalkSpeed(currentWalkSpeed); }
        protected void SetRunSpeed(float speed) { currentRunSpeed = speed; player.SetRunSpeed(currentWalkSpeed); }
        protected void SetStrafeSpeed(float speed) { currentStrafeSpeed = speed; player.SetStrafeSpeed(currentStrafeSpeed); }
        protected void SetJumpImpulse(float impulse) { currentJumpImpulse = impulse; player.SetJumpImpulse(currentJumpImpulse); }
        protected void SetGravityStrength(float strength) { currentGravity = strength; player.SetGravityStrength(currentGravity); }
    }
}