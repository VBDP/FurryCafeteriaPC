
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using VRCCoffeeSet;

namespace VRCCoffee
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Cup_Straw : UdonSharpBehaviour
    {
        public Cup_Coffee m_cup;
        public ParticleSystemRenderer m_particleRenderer;

        float m_progress = 0;
        float m_progressTime = 1;

        Vector3 m_defaultPos;
        Vector3 m_defaultRot;

        bool m_isInteracted = false;

        void Start()
        {
            m_defaultPos = transform.localPosition;
            m_defaultRot = transform.localEulerAngles;

            ((VRCPickup)GetComponent(typeof(VRCPickup))).pickupable = Networking.LocalPlayer.IsUserInVR();
        }

        void Update()
        {
            if (m_isInteracted)
            {
                m_progress += Time.deltaTime / m_progressTime;
                m_particleRenderer.material.SetFloat("_Progress", m_progress);

                if (m_progress >= m_progressTime)
                {
                    Interacted();
                    m_isInteracted = false;
                    m_progress = 0;
                    m_particleRenderer.gameObject.SetActive(false);
                }
            }
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if (value == false)
            {
                m_isInteracted = false;
                m_particleRenderer.gameObject.SetActive(false);
            }
        }
        public override void Interact() 
        {
            if (m_cup.m_pickup.IsHeld && !Networking.IsOwner(m_cup.gameObject)) return;
            m_isInteracted = true;
            m_progress = 0;
            m_particleRenderer.gameObject.SetActive(true);
        }

        void Interacted()
        {
            if (m_cup.m_pickup.IsHeld && !Networking.IsOwner(m_cup.gameObject)) return;
            m_cup.m_contentStir = 2;
            m_cup.SerializeVariables(true);
            m_cup.SendCustomNetworkEvent(NetworkEventTarget.All, "StartStir");
        }

        public override void OnPickup()
        {
            ((VRCPickup)GetComponent(typeof(VRCPickup))).Drop();
            transform.localPosition = m_defaultPos;
            transform.localEulerAngles = m_defaultRot;
        }
    }
}
