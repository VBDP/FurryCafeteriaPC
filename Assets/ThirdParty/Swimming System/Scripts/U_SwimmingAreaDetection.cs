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

    public class U_SwimmingAreaDetection : UdonSharpBehaviour
    {
        //only played when head is underneath water //play when player enters a water body, with volume dependant on player speed as they enter.
        private AudioSource waterEnterAudio;
        private AudioSource underWaterAudio;
        [SerializeField] private U_SwimmingPlayerMovement swimming;
        [SerializeField] private GameObject underWaterEffect;
        [SerializeField] private U_HashStudiosSwimmingSystem_Main main;
        private Vector3 swimmingVector;
        private Vector3 splashLocation;


        [Header("Used for testing purposes")]
        public GameObject debugSphere;
        private void Start()
        {
            SetAudioSources();
        }
        private void Update()
        {
            //Debug Mode: Allow user to test different swimming parameters in real time testing without having to reload.
            if (main.debugMode && swimming.AreFeetInWater())
            {
                swimming.SetWaterInteractions();
                swimming.SetToWaterMovement();
            }

            RaycastHit headHit, footHit;
            bool b = Physics.Raycast(swimming.followHeadPoint.transform.position, swimming.followHeadPoint.transform.TransformDirection(Vector3.up), out headHit, Mathf.Infinity, 1 << 5, QueryTriggerInteraction.Collide);
            bool c = Physics.Raycast(swimming.followFeetPoint.transform.position, swimming.followFeetPoint.transform.TransformDirection(Vector3.up), out footHit, Mathf.Infinity, 1 << 5, QueryTriggerInteraction.Collide);
            Transform t = headHit.transform;
            Transform u = footHit.transform;


            //Feet check

            //Raycast distance and floats are tricky so cast to an int instead for easy comparison. If 0, then their feet have hit the water.
            if (c && u != null && footHit.transform.name.Contains("Fluid Body Top Plane (FBTP)") && (int)footHit.distance == 0 && !waterEnterAudio.isPlaying)
            {
                splashLocation = footHit.point;
                WaterFeetEnter(splashLocation);
            }
            else if ((!c || u == null || u.name.Contains("Fluid Body Bottom Plane (FBBP)")) && !waterEnterAudio.isPlaying) WaterFeetExit();

            //Head check (IE, Is player fully submerged?)

            if (b && t != null && t.name.Contains("Fluid Body Top Plane (FBTP)") && !swimming.IsHeadUnderWater()) WaterHeadEnter();

            //If Player's head has exited water body or never entered a body to begin with.
            else if (!b || t == null || t.name.Contains("Fluid Body Bottom Plane (FBBP)")) WaterHeadExit();

            if (swimming.IsHeadUnderWater())
            {
                Vector3 velo = SwimVelocityCalc();
                swimming.SetVelocity((1f - Time.deltaTime) * swimming.GetVelocityMod() * velo);
            }
        }

        private Vector3 SwimVelocityCalc()
        {
            float deltaTimeStep = Time.deltaTime;
            float initVel = 0.0012f / deltaTimeStep;
            swimmingVector = SwimVectorCalc();
            Vector3 newVelocity = (Networking.LocalPlayer.GetVelocity() + Vector3.Scale(Vector3.Scale(swimmingVector, new Vector3(1f, 0.2f, 1f)).normalized, new Vector3(initVel, 0f, initVel)) + deltaTimeStep * swimmingVector.y * Vector3.up * swimming.GetVerticalVelocityMod());
            return newVelocity;
        }
        private Vector3 SwimVectorCalc()
        {
            Quaternion moveRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            Vector3 emulatedSwim = moveRot * swimming.GetMovementVector() * 2f * swimming.GetVerticalVelocityMod()
                    + (Input.GetButton("Jump") ? Vector3.up : Vector3.zero)
                    + (Input.GetKey(KeyCode.LeftControl) ? Vector3.down : Vector3.zero); // + (Networking.LocalPlayer.IsUserInVR() ? Input : Vector3.zero)
            return emulatedSwim;
        }

        //Feet and head functions.    
        private void WaterHeadEnter()
        {
            if (!underWaterAudio.isPlaying) //Only run if ambience was not already playing (if ambience is playing, then their head is in fact already submerged).
            {
                underWaterAudio.Play();
                swimming.ToggleHeadIsUnderWater(true);
                if (underWaterEffect != null) underWaterEffect.SetActive(true);
                Debug.Log("Player's head has entered a pool.");
            }
        }

        //Only run if head WAS in fact submerged before this call
        private void WaterHeadExit()
        {
            if (swimming.IsHeadUnderWater())
            {
                underWaterAudio.Stop();
                swimming.ToggleHeadIsUnderWater(false);
                if (underWaterEffect != null) underWaterEffect.SetActive(false);
                Debug.Log("Player's head has exited pool");
            }
        }

        //Only run if feet were NOT already in water before this call
        private void WaterFeetEnter(Vector3 splashPoint)
        {
            if (!swimming.AreFeetInWater())
            {
                swimming.ToggleIsFeetInWater(true);
                swimming.SetToWaterMovement();
                ChangeSplashPoint(splashLocation);
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayEnterWaterFeet"); //Everyone should be able to hear someone else jumping into a pool, so fire event across network.       
                Debug.Log("Player's feet have entered a pool.");
            }
        }

        //Only run if feet WERE in fact in water before this call
        private void WaterFeetExit()
        {
            if (swimming.AreFeetInWater())
            {
                swimming.ToggleIsFeetInWater(false);
                swimming.ResetToGroundedMovement();
                Debug.Log("Player's feet (and whole body) have exited a pool.");
            }
        }

        //A player above water should not hear underwater ambience when someone else is underwater. Only an underwater player should hear ambience. 
        //However, everyone should be able to hear when they or someone else jump into a pool, so this function is sent as a network event globally. 
        public void PlayEnterWaterFeet() { waterEnterAudio.Play(); }
        public void ChangeSplashPoint(Vector3 splashPoint) { waterEnterAudio.gameObject.transform.position = splashPoint; }
        private void SetAudioSources()
        {
            this.waterEnterAudio = main.waterEnterAudio;
            this.underWaterAudio = main.underWaterAudio;
        }


    }
}