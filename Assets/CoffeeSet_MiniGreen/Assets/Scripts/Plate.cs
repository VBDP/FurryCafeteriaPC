
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Plate")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Plate : UdonSharpBehaviour
    {
        [Header("DO NOT Change variables\n")]
        public MainController m_controller;
        public VariableHolder m_variable;
        public VRC_Pickup m_pickup;
        public Transform m_posDeposit;
        public TypeCup m_type;
        public byte m_index;
        public byte m_skinIndex = 0;
        public byte m_matIndex = 0;
        public AudioSource m_audio;

        // Transform Data
        Vector3 defaultPos;
        Quaternion defaultRot;

        [HideInInspector] public bool m_isHeld { get => m_variable.m_info[2] == 1; set => m_variable.m_info[2] = (byte)(value ? 1 : 0); }
        [HideInInspector] public float m_yDeposit { get => m_variable.m_infoFloat[0]; set => m_variable.m_infoFloat[0] = value; }
        [HideInInspector] float mL_yDeposit;
        /// <summary> -1_No cup, 0-_Index of cup </summary>
        [HideInInspector] public int m_indexTarget { get => m_variable.m_info[0]-1; set => m_variable.m_info[0] = (byte)(value+1); }
        /// <summary> -1_None, 0-_Index of Ring </summary>
        [HideInInspector] public int m_stateRing { get => m_variable.m_info[1]-1; set => m_variable.m_info[1] = (byte)(value+1); }
        int ml_stateRing = -1;

        void Start()
        {
            defaultPos = transform.position;
            defaultRot = transform.rotation;
            SetRing();
        }

        public void UpdateData()
        {
            if (m_stateRing != ml_stateRing)
            {
                ml_stateRing = m_stateRing;
                Debug.Log(this.name + " Ring : " + m_stateRing);
                SetRing();
            }

            if (mL_yDeposit != m_yDeposit)
            {
                mL_yDeposit = m_yDeposit;
                Vector3 currentAngle = m_posDeposit.rotation.eulerAngles;
                m_posDeposit.rotation = Quaternion.Euler(currentAngle.x, m_yDeposit, currentAngle.z);
                m_posDeposit.localEulerAngles = new Vector3(0, m_posDeposit.localEulerAngles.y, 0);
            }

            if (m_indexTarget > -1)
            {
                if (!m_isHeld) GetCup(m_indexTarget).SetPickup(true);
                else GetCup(m_indexTarget).SetPickup(Networking.IsOwner(gameObject));
            }
        }
        
        public override void OnPickup()
        {
            m_isHeld = true;

            if (m_indexTarget > -1)
            {
                // Check state of deposit of cup for safe if not synced
                Cup_Coffee cup = GetCup(m_indexTarget);
                if (cup.m_depositState == 2 && cup.m_depositIndex == m_index) cup.SerializeVariables();
                else m_indexTarget = -1; 
            }
            SerializeVariables(true);

        }
        public override void OnDrop()
        {
            m_isHeld = false;

            if (m_indexTarget > -1)
            {
                // Check state of deposit of cup for safe if not synced
                Cup_Coffee cup = GetCup(m_indexTarget);
                if (cup.m_depositState == 2 && cup.m_depositIndex == m_index) cup.SerializeVariables();
                else m_indexTarget = -1; 
            }
            SerializeVariables(true);
        }

        void OnTriggerEnter(Collider other)
        {
            if (m_indexTarget > -1) return;

            Cup_Coffee cup = other.GetComponent<Cup_Coffee>();

            // Obj is not cup
            if (!cup) return;

            // The cup is not held or Not mine
            if (!cup.m_pickup.IsHeld) return;
            else if (!Networking.IsOwner(cup.gameObject)) return;

            // The plate is held by other player (Not mine)
            if (m_pickup.IsHeld && !Networking.IsOwner(gameObject)) return;

            // Check type and skin index
            if (cup.m_type != m_type) return;
            if (cup.m_skinIndex != m_skinIndex) return;

            // Check delay time of pickup
            if (cup.time_Delay > Time.time) return;
            // Check states of deposit 
            if (cup.m_depositState != 0) return;
            if (cup.m_depositState == 2 && cup.m_depositIndex == m_index) return;

            // Assign plate info but not serialize temporary
            // Serialize when cup has been dropped
            cup.m_depositState = 2;
            cup.m_depositIndex = m_index;
        }
        void OnTriggerExit(Collider other)
        {
            Cup_Coffee cup = other.GetComponent<Cup_Coffee>();

            // Obj is not cup
            if (!cup) return;

            // The cup is not held or Not mine
            if (!cup.m_pickup.IsHeld) return;
            else if (!Networking.IsOwner(cup.gameObject)) return;

            // Not this plate
            if (cup.m_depositState != 2 || cup.m_depositIndex != m_index) return;

            cup.m_depositState = 0;
        }

        void OnParticleCollision(GameObject other)
        {
            if (!Networking.IsOwner(gameObject)) return;

            string[] nameContact_Splited = other.name.Split(new char[] {'_'});
            if (nameContact_Splited[0] != "Particle") return;

            switch (nameContact_Splited[1])
            {
                case "WaterWash":
                    if (m_stateRing == -1) return;
                    m_stateRing = -1;
                    SerializeVariables(true);
                    break;
                default: return;
            }
        }

        public void SetRing()
        {
            var meshSelf = GetComponent<Renderer>();
            Material[] tempMat = meshSelf.sharedMaterials;
            tempMat[1] = m_stateRing != -1 ? m_controller.mat_Ring[m_stateRing] : m_controller.mat_Empty;
            meshSelf.sharedMaterials = tempMat;
        }

        Cup_Coffee GetCup(int index)
        {
            Cup_Coffee udon = null;
            if (m_type == TypeCup.Espresso) udon = m_controller.dish_espressoCup[index];
            else if (m_type == TypeCup.Coffee) udon = m_controller.dish_coffeeCup[index];
            return udon;
        }

        public void SerializeVariables(bool update = false)
        {
            if (!Networking.IsOwner(gameObject) || !Networking.IsOwner(m_variable.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                Networking.SetOwner(Networking.LocalPlayer, m_variable.gameObject);
            }
            RequestSerialization();
            m_variable.RequestSerialization();
            if (update) UpdateData();
        }

        public void Call_Reset()
        {
            if (m_pickup.IsHeld) return;
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (m_indexTarget > -1) GetCup(m_indexTarget).FuncPickup();
            SerializeVariables(true);
            transform.SetPositionAndRotation(defaultPos, defaultRot);
        }
    }
}