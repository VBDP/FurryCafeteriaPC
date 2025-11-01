
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;



#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System;
using UnityEditor;
// using UdonSharpEditor;
#endif

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Machine Component")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Machine_Component : UdonSharpBehaviour
    {
        #region Variables

        MainController m_controller { get { return GetComponentInParent<MainController>(); } }
        Machine m_machine { get { return GetComponentInParent<Machine>(); } }
        public Machine_Display m_display { get { return m_machine.m_display; } }
        
        public TypeMachineE m_type;

        /// <summary>
        /// E_DP : 0_Mode, 1_Steam
        /// </summary>
        public Machine_Button[] m_button;

        /// <summary>
        /// E_DP : 0_Steam, 1_Froth
        /// </summary>
        public AudioSource[] m_audio;

        /// <summary>
        /// E_DP : 0_Steam, 1_Froth
        /// </summary>
        public ParticleSystem[] m_particle;

        /// <summary>
        /// <b>Only for E_DF : </b>
        /// 0_L, 1_R
        /// </summary>
        /// <returns>E_DC</returns>
        public Machine_Component[] m_deposit;
        public Transform[] m_depositTrans;
        
        /// <summary>
        /// -1_None, 0-_Index of the obj
        /// </summary>
        [UdonSynced, HideInInspector] public int m_indexTarget = -1;
        int ml_indexTarget = -1;

        /// <summary> 
        /// G_D : 0_Manual, 1_Auto <para></para>
        /// E_DC, E_DF : 0-_Index among all of machines <para></para>
        /// E_DP : 1_Latte, 2_Cappuccino <para></para>
        /// E_W  : 0_Off, 1_On
        /// </summary>
        [UdonSynced, HideInInspector] public byte m_mode = 0;
        [UdonSynced, HideInInspector] public bool m_isWorking = false;

        #endregion

        void Start()
        {
            if (m_type == TypeMachineE.E_DC)
            {
                m_indexTarget = 0;
                return;
            }

            for (int i = 0; i < m_button.Length; i++)
            {
                m_button[i].m_target = this;
                m_button[i].m_nameEvent = $"InputButton{i}";
            }

            if  (m_type == TypeMachineE.E_DP) m_mode = 1;

            LocalUpdate();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(gameObject)) RequestSerialization();
        }
        public override void OnDeserialization() => LocalUpdate();
        public void LocalUpdate()
        {
            switch (m_type)
            {
                case TypeMachineE.E_DF: LocalUpdate_MachineFilter();
                    break;
                case TypeMachineE.E_DP:
                    m_button[0].SetColor(m_mode);
                    m_button[0].SetInteract(m_isWorking ? false : true);
                    m_button[1].SetInteract(m_isWorking ? false : true);
                    break;
                case TypeMachineE.E_W:
                    if (m_mode == 1)
                    {
                        m_button[0].SetColor(2);
                        m_particle[0].Play();
                    }
                    else
                    {
                        m_button[0].SetColor(1);
                        m_particle[0].Stop();
                    }
                    break;
            }
        }

        public void DepositOff()
        {
            if (m_indexTarget == -1) return;
            if (!Networking.IsOwner(gameObject)) return;

            Tool tempTool;
            if (m_type == TypeMachineE.E_DF || m_type == TypeMachineE.G_D) tempTool = GetFilter(m_indexTarget);
            else if (m_type == TypeMachineE.E_DP) tempTool = GetPitcher(m_indexTarget);
            else return;

            if (m_isWorking)
            {
                if (m_type == TypeMachineE.G_D) tempTool.m_depositState = 1;
                else tempTool.m_depositState = 2;
                if (m_type == TypeMachineE.E_DF) tempTool.m_depositIndex = m_mode;
                else tempTool.m_depositIndex =  m_machine.m_index;
                tempTool.SerializeVariables();
                tempTool.Drop();
                tempTool.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
            else
            {
                m_indexTarget = -1;
                SerializeVariables();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check obj owner
            if (!Networking.IsOwner(other.gameObject)) return;

            switch (m_type)
            {
                case TypeMachineE.G_D: TrigEnter_Grinder(other);
                    break;
                case TypeMachineE.E_DF: TrigEnter_MachineE_DF(other);
                    break;
                case TypeMachineE.E_DC: TrigEnter_MachineE_DC(other);
                    break;
                case TypeMachineE.E_DP: TrigEnter_MachineE_DP(other);
                    break;
                case TypeMachineE.ICE: TrigEnter_MachineIce(other);
                    break;
            }
        }
        
        public void InputButton0()
        {
            switch (m_type)
            {
                case TypeMachineE.G_D: Input_Grinder();
                    break;
                case TypeMachineE.E_DF: Input_MachineE_DF();
                    break;
                case TypeMachineE.E_DP: Input_MachineE_DPMode();
                    break;
                case TypeMachineE.E_W: Input_MachineW();
                    break;
            }
        }
        public void InputButton1()
        {
            switch (m_type)
            {
                case TypeMachineE.E_DP: Input_MachineE_DPSteam();
                    break;
            }
        }

    #region Grinder

        void TrigEnter_Grinder(Collider other)
        {
            if (m_isWorking) return;

            Tool udonFilter = null;

            // Check last obj is on deposit
            if (m_indexTarget > -1)
            {
                udonFilter = GetFilter(m_indexTarget);
                if (udonFilter.m_depositState == 1 && udonFilter.m_depositIndex == m_machine.m_index) return;
            }

            udonFilter = other.GetComponent<Tool>();

            // Check type
            if (!udonFilter) return;
            if (udonFilter.m_type != TypeTool.Filter) return;

            // Check skin
            if (m_machine.m_skinIndex != udonFilter.m_skinIndex) return;

            // Check pickup delay time and states of obj
            if (udonFilter.timeDelay > Time.time || udonFilter.m_depositState != 0 || udonFilter.m_content != 0) return;

            // Filter
            udonFilter.m_depositState = 1;
            udonFilter.m_depositIndex = m_machine.m_index;
            udonFilter.SerializeVariables();
            udonFilter.Drop();
            udonFilter.transform.SetPositionAndRotation(transform.position, transform.rotation);

            // Setup
            m_indexTarget = udonFilter.m_index;
            SerializeVariables();
        }

        void Input_Grinder()
        {
            if (m_indexTarget == -1) return;

            Tool udonFilter = GetFilter(m_indexTarget);

            if (udonFilter.m_depositState != 1 || udonFilter.m_content != 0)
            {
                Debug.LogError($"{this.name} : Deposit state of filter not matching.");
                m_indexTarget = -1;
                SerializeVariables();
                return;
            }

            // Filter
            udonFilter.m_depositState = 1;
            udonFilter.m_depositIndex = m_machine.m_index;
            udonFilter.SerializeVariables();

            // Machine
            m_indexTarget = udonFilter.m_index;
            m_isWorking = true;
            SerializeVariables();
            SendCustomNetworkEvent(NetworkEventTarget.All, "StartGrind");
        }

        // Networked
        public void StartGrind()
        {
            // Play Particle, Sound
            m_audio[0].Play();
            foreach (var item in m_particle) item.Play();

            // Filter
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.Drop();
            udonFilter.SetPickup(false);
            udonFilter.transform.SetPositionAndRotation(transform.position, transform.rotation);
            // udonFilter.GetComponent<Animator>().SetTrigger("Grind");
            udonFilter.m_content = 1;
            udonFilter.m_contentLevel = 100;
            udonFilter.isAnim = true;
            udonFilter.timeDelay = m_audio[0].clip.length;
            // udonFilter.SendCustomEventDelayedSeconds("UpdateData", m_audio[0].clip.length/2);

            SendCustomEventDelayedSeconds("EndGrind", m_audio[0].clip.length);
        }
        public void EndGrind()
        {
            foreach (var item in m_particle) item.Stop();

            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.SetPickup(true);
            udonFilter.isAnim = false;

            if (!Networking.IsOwner(gameObject)) return;
            m_isWorking = false;
            SerializeVariables();
        }

    #endregion
    
    #region Machine - Filter

        void LocalUpdate_MachineFilter()
        {
            // Filter deposit animation
            if (ml_indexTarget != m_indexTarget)
            {
                ml_indexTarget = m_indexTarget;
                if (m_indexTarget > -1) MachineE_FilterDeposit();
            }

            Tool udonFilter = null;
            if (m_indexTarget != -1) udonFilter = GetFilter(m_indexTarget);

            // Cup deposit activation
            m_deposit[0].gameObject.SetActive(udonFilter);
            m_deposit[1].gameObject.SetActive(udonFilter ? udonFilter.m_spout == 2 : false);
            // Set first cup deposit position
            if (!udonFilter || udonFilter.m_spout > 0) m_deposit[0].transform.SetPositionAndRotation(m_depositTrans[0].position, m_depositTrans[0].rotation);
            else m_deposit[0].transform.SetPositionAndRotation(m_depositTrans[2].position, m_depositTrans[2].rotation);
            // Set cup position
            if (udonFilter)
            {
                Cup_Coffee cup = GetEspressoCup(m_deposit[0].m_indexTarget);
                if (cup.m_depositState == 1 && cup.m_depositIndex == m_mode && Networking.IsOwner(cup.gameObject)) // Only owner
                    cup.transform.SetPositionAndRotation(m_deposit[0].transform.position, m_deposit[0].transform.rotation);
            }

            // Set button
            m_button[0].SetColor(m_isWorking ? 2 : 1);
            m_button[0].SetInteract(m_isWorking ? false : true);
        }

        void TrigEnter_MachineE_DF(Collider other) // Local
        {
            if (m_isWorking) return;

            Tool udonFilter = null;

            // Check last obj is on deposit
            if (m_indexTarget > -1)
            {
                udonFilter = GetFilter(m_indexTarget);
                if (udonFilter.m_depositState == 2 && udonFilter.m_depositIndex == m_mode) return;
            }

            udonFilter = other.GetComponent<Tool>();
            // Check type
            if (!udonFilter) return;
            if (udonFilter.m_type != TypeTool.Filter) return;
            // Check skin
            if (m_machine.m_skinIndex != udonFilter.m_skinIndex) return;
            // Check pickup delay time of obj
            if (udonFilter.timeDelay > Time.time) return;
            // Check states
            if (udonFilter.m_depositState != 0 || udonFilter.m_content != 2)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_filterNeedTamped"), 5);
                return;
            }

            // Filter
            udonFilter.m_depositState = 2;
            udonFilter.m_depositIndex = m_mode;
            udonFilter.SerializeVariables();

            // Setup
            m_indexTarget = udonFilter.m_index;
            SerializeVariables();
            m_display.SetText(m_controller.GetLangValueFromCurrent("machine_filterPlaced"), 5);
        }
        public void MachineE_FilterDeposit() // Called by LocalUpdate
        {
            //Filter
            if (m_indexTarget == -1) return;
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.Drop();
            udonFilter.SetActive(false);
            udonFilter.SetPickup(false);
            udonFilter.transform.SetPositionAndRotation(transform.position, transform.rotation);

            // Machine
            Animator animator = m_machine.GetComponent<Animator>();
            animator.SetInteger("FilterSpout", udonFilter.m_spout);
            animator.SetTrigger("FilterDeposit");

            SendCustomEventDelayedSeconds("MachineE_FilterDeposit_End", 1);
        }
        public void MachineE_FilterDeposit_End()
        {
            if (m_indexTarget == -1) return;
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.SetActive(true);
            udonFilter.SetPickup(true);
        }

        void Input_MachineE_DF()
        {
            if (m_indexTarget == -1)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_filterNeed"), 5);
                return;
            }

            Tool udonFilter = GetFilter(m_indexTarget);

            // Filter validation
            if (udonFilter.m_depositState != 2 || udonFilter.m_depositIndex != m_mode)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_filterNeed"), 5);
                m_indexTarget = -1;
                SerializeVariables();
                return;
            }
            // Not tamped filter
            else if (udonFilter.m_content != 2)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_filterChange"), 5);
                return;
            }

            // Cups validation
            int nozzleCount = udonFilter.m_spout == 2 ? 2 : 1;
            for (int i=0; i < nozzleCount; i++)
            {
                Cup_Coffee udonCup = GetEspressoCup(m_deposit[i].m_indexTarget);
                // Not empty
                if (udonCup.m_contentLevel != 0)
                {
                    m_display.SetText(m_controller.GetLangValueFromCurrent("machine_cupNeedEmpty"), 5);
                    return;
                }
                // Not deposited
                else if (udonCup.m_depositState != 1)
                { 
                    m_display.SetText(m_controller.GetLangValueFromCurrent("machine_cupNeed"), 5);
                    return;
                }
                // Info mismatch between cup and machine
                else if (udonCup.m_depositIndex != m_deposit[i].m_mode)
                {
                    if (!udonCup.m_pickup.IsHeld) // Preventing from conflict when another player holding the cup
                    {
                        udonCup.m_depositState = 0;
                        udonCup.SerializeVariables();
                    }
                    m_display.SetText(m_controller.GetLangValueFromCurrent("machine_cupNeed"), 5);
                    return;
                }
            }
            
            // Machine
            m_indexTarget = udonFilter.m_index;
            m_isWorking = true;
            SerializeVariables();

            // Filter
            udonFilter.m_depositState = 2;
            udonFilter.m_depositIndex = m_machine.m_index;
            udonFilter.m_content = 3;
            udonFilter.SerializeVariables(true);
            
            for (int i=0; i < nozzleCount; i++)
            {
                // Deposit
                m_deposit[i].m_indexTarget = m_deposit[i].m_indexTarget;
                m_deposit[i].SerializeVariables();
                // Cup
                Cup_Coffee udonCup = GetEspressoCup(m_deposit[i].m_indexTarget);
                udonCup.m_depositState = 1;
                udonCup.m_depositIndex = m_deposit[i].m_mode;
                udonCup.SerializeVariables();
            }

            // Initiate
            m_display.SetText(m_controller.GetLangValueFromCurrent("machine_extracting"), m_audio[0].clip.length);
            SendCustomNetworkEvent(NetworkEventTarget.All,"StartExtract");
        }

        public void StartExtract() // Networked
        {
            // Machine
            m_audio[0].Play();
            
            // Filter
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.Drop();
            udonFilter.SetPickup(false);
            udonFilter.transform.SetPositionAndRotation(transform.position, transform.rotation);

            // Cup(s)
            for (int i=0; i<udonFilter.m_spoutCount; i++)
            {
                Cup_Coffee udonCup = GetEspressoCup(m_deposit[i].m_indexTarget);
                udonCup.Drop();
                udonCup.SetPickup(false);
                udonCup.transform.SetPositionAndRotation(m_deposit[i].transform.position, m_deposit[i].transform.rotation);
                // Fill animation
                // udonCup.ml_contentLevel = 0;
                // udonCup.SetContentLevel(0);
                // udonCup.m_contentLevel = 100;
                // udonCup.m_contentDrain = 2;
                // udonCup.m_contentDrainSpeed = 100 / (m_audio[0].clip.length - m_controller.m_extractDelay);
                // udonCup.time_Delay = Time.time + m_controller.m_extractDelay;
            }

            SendCustomEventDelayedSeconds("StartExtract_Delay", m_controller.m_extractDelay);
            SendCustomEventDelayedSeconds("StartExtract_End", m_audio[0].clip.length);
        }
        public void StartExtract_Delay()
        {
            // Filter
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.timeDelay = 0.5f;
            udonFilter.m_animFlow = 1;

            // Cups
            for (int i=0; i<udonFilter.m_spoutCount; i++)
            {
                Cup_Coffee udonCup = GetEspressoCup(m_deposit[i].m_indexTarget);
                udonCup.m_contentType = TypeContent.Espresso;
                udonCup.ml_contentLevel = 0;
                udonCup.SetContentLevel(0);
                udonCup.m_contentLevel = 100;
                udonCup.m_contentDrain = 2;
                udonCup.m_contentDrainSpeed = 100 / (m_audio[0].clip.length - m_controller.m_extractDelay);
                udonCup.time_Delay = 0;
                udonCup.UpdateData();
            }
        }
        public void StartExtract_End()
        {
            // Machine
            m_button[0].SetInteract(true);

            // Filter
            Tool udonFilter = GetFilter(m_indexTarget);
            udonFilter.SetPickup(true);
            udonFilter.m_animFlow = 2;

            // Cups
            for (int i=0; i<udonFilter.m_spoutCount; i++)
            {
                Cup_Coffee udonCup = GetEspressoCup(m_deposit[i].m_indexTarget);
                udonCup.SetPickup(true);
                udonCup.m_contentDrain = 0;
                udonCup.m_contentDrainSpeed = 20;
                udonCup.m_contentReady = true;
                udonCup.time_Delay = m_controller.m_pickupdelay;
            }

            // Player who started extracting
            if (!Networking.IsOwner(gameObject)) return;

            // Machine
            m_isWorking = false;
            SerializeVariables();
        }

    #endregion

    #region Machine - Cup

        void TrigEnter_MachineE_DC(Collider other)
        {
            // Check last obj is on deposit
            Cup_Coffee cup = GetEspressoCup(m_indexTarget);
            // Cup_Coffee cup = m_controller.dish_espressoCup[m_indexTarget];
            if (cup.m_depositState == 1 && cup.m_depositIndex == m_mode) return;

            cup = other.GetComponent<Cup_Coffee>();

            // Check contact obj is matching with target
            if (!cup) return;
            if (cup.m_type != TypeCup.Espresso) return;

            // Check pickup delay time of obj
            if (cup.time_Delay > Time.time) return;

            // Check states of obj
            if (cup.m_depositState != 0 || cup.m_contentLevel != 0) return;
            
            // Setup
            m_indexTarget = cup.m_index;
            SerializeVariables();

            // Cup
            cup.Drop();
            other.transform.SetPositionAndRotation(transform.position, transform.rotation);
            cup.m_depositState = 1;
            cup.m_depositIndex = m_mode;
            cup.SerializeVariables();
        }

    #endregion

    #region Machine - Pitcher

        void TrigEnter_MachineE_DP(Collider other)
        {
            Tool udonPitcher = null;

            // Check last obj is on deposit
            if (m_indexTarget > -1)
            {
                udonPitcher = GetPitcher(m_indexTarget);
                if (udonPitcher.m_depositState == 2 && udonPitcher.m_depositIndex == m_machine.m_index) return;
            }

            udonPitcher = other.GetComponent<Tool>();

            // Check contact obj is matching with target
            if (!udonPitcher) return;
            if (udonPitcher.m_type != TypeTool.Pitcher) return;

            // Check pickup delay time of obj
            if (udonPitcher.timeDelay > Time.time) return;
            
            // Check states of obj
            if (udonPitcher.m_depositState != 0)
            {
                Debug.LogError("Espresso machine : Error occured while depositing pitcher. Deposit state of steam pitcher not matching.");
                return;
            }
            else if (udonPitcher.m_content != 1)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_PitcherNeed"), 5);
                return;
            }

            // Setup
            m_indexTarget = udonPitcher.m_index;
            SerializeVariables();
            m_display.SetText(m_controller.GetLangValueFromCurrent("machine_PitcherPlaced"), 5);

            // Steam Pitcher
            udonPitcher.m_depositState = 2;
            udonPitcher.m_depositIndex = m_machine.m_index;
            udonPitcher.Drop();
            udonPitcher.transform.SetPositionAndRotation(transform.position, transform.rotation);
            udonPitcher.SerializeVariables();
        }

        // Froth mode change
        void Input_MachineE_DPMode()
        {
            m_mode = m_mode == 1 ? (byte)2 : (byte)1;
            SerializeVariables();
            if (m_mode == 1) m_display.SetText(m_controller.GetLangValueFromCurrent("machine_frothLatte"), 5);
            else m_display.SetText(m_controller.GetLangValueFromCurrent("machine_frothCappu"), 5);
        }
        // Steam
        void Input_MachineE_DPSteam()
        {
            if (m_indexTarget == -1)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All,"StartSteam");
                return;
            }

            Tool udonPitcher = GetPitcher(m_indexTarget);

            if (udonPitcher.m_depositState != 2 || udonPitcher.m_depositIndex != m_machine.m_index)
            {
                m_indexTarget = -1;
                SerializeVariables();
                SendCustomNetworkEvent(NetworkEventTarget.All,"StartSteam");
                return;
            }
            else if (udonPitcher.m_content != 1)
            {
                m_display.SetText(m_controller.GetLangValueFromCurrent("machine_PitcherChange"), 5);
                return;
            }

            m_indexTarget = udonPitcher.m_index;
            m_isWorking = true;
            SerializeVariables();

            m_display.SetText(m_controller.GetLangValueFromCurrent("machine_frothing"), m_audio[0].clip.length);
            SendCustomNetworkEvent(NetworkEventTarget.All,"StartFroth");
        }

        public void StartSteam() // Networked
        {
            m_audio[0].Play();
            m_particle[0].Play();
            m_button[1].SetInteract(false);
            m_machine.GetComponent<Animator>().SetBool("SteamLever", true);
            
            SendCustomEventDelayedSeconds("StartSteam_Delayed", 1);
        }
        public void StartSteam_Delayed()
        {
            m_machine.GetComponent<Animator>().SetBool("SteamLever", false);
            m_button[1].SetInteract(true);
        }
        public void StartFroth() // Networked
        {
            // Machine
            m_audio[1].Play();
            m_particle[1].Play();
            m_machine.GetComponent<Animator>().SetBool("SteamLever", true);

            //Pitcher
            Tool udonPitcher = GetPitcher(m_indexTarget);
            udonPitcher.Drop();
            udonPitcher.SetPickup(false);
            udonPitcher.transform.SetPositionAndRotation(transform.position, transform.rotation);
            udonPitcher.m_content = (byte)(m_mode + 1);
            udonPitcher.m_depositState = 1;
            udonPitcher.UpdateData();

            SendCustomEventDelayedSeconds("StartFroth_Delayed", m_audio[1].clip.length);
        }
        public void StartFroth_Delayed()
        {
            m_button[0].SetInteract(true);
            m_button[1].SetInteract(true);
            m_machine.GetComponent<Animator>().SetBool("SteamLever", false);

            Tool udonPitcher = GetPitcher(m_indexTarget);
            udonPitcher.SetPickup(true);

            if (!Networking.IsOwner(gameObject)) return;
            m_isWorking = false;
            RequestSerialization();
        }

    #endregion

    #region Machine - Water

        void Input_MachineW()
        {
            m_mode = m_mode == 0 ? (byte)1 : (byte)0;
            SerializeVariables();
            if (m_mode == 0) m_display.SetText(m_controller.GetLangValueFromCurrent("machine_waterOff"), 5);
            else m_display.SetText(m_controller.GetLangValueFromCurrent("machine_waterOn"), 5);
        }

    #endregion

    #region Machine - Ice

        void TrigEnter_MachineIce(Collider other)
        {
            if (m_isWorking) return;

            // Check is obj has valid script
            Cup_Coffee udonCup = other.GetComponent<Cup_Coffee>();
            if (!udonCup) return;
            if (udonCup.m_type != TypeCup.Glass || udonCup.m_contentIce != 0) return;

            SendCustomNetworkEvent(NetworkEventTarget.All, "StartIce");
        }

        public void StartIce()
        {
            m_isWorking = true;
            m_particle[0].Play();
            m_audio[0].Play();
            SendCustomEventDelayedSeconds("StartIce_Delayed", m_audio[0].clip.length);
        }
        public void StartIce_Delayed() => m_isWorking = false;

    #endregion

    #region Function

        /// <summary> SetOwner, LocalUpdate, Serialization </summary>
        void SerializeVariables()
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            LocalUpdate();
            RequestSerialization();
        }
        Tool GetFilter(int index) { return m_controller.tool_filter[index]; }
        Tool GetPitcher(int index) { return m_controller.tool_pitcher[index]; }
        Cup_Coffee GetEspressoCup(int index) { return m_controller.dish_espressoCup[index]; }

    #endregion
    }

    
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(Machine_Component))]
    public class Machine_ContacterEditor : Editor
    {
        Machine_Component script;
        TypeMachineE _type;

        bool toggle_Base;

        GUIStyle paddingLeft = new GUIStyle{};

        private void Awake()
        {
            script = target as Machine_Component;
            paddingLeft.padding.left = 15;
        }

        void Initialize()
        {
            _type = script.m_type;

            switch (script.m_type)
            {
                case TypeMachineE.G_D:
                    Array.Resize<Machine_Button>(ref script.m_button, 1);
                    Array.Resize<AudioSource>(ref script.m_audio, 1);
                    script.m_deposit = null;
                    break;
                case TypeMachineE.E_DF:
                    Array.Resize<Machine_Button>(ref script.m_button, 1);
                    Array.Resize<AudioSource>(ref script.m_audio, 1);
                    Array.Resize<ParticleSystem>(ref script.m_particle, 2);
                    Array.Resize<Machine_Component>(ref script.m_deposit, 2);
                    break;
                case TypeMachineE.E_DC:
                    script.m_button = null;
                    script.m_audio = null;
                    script.m_particle = null;
                    script.m_deposit = null;
                    break;
                case TypeMachineE.E_DP:
                    Array.Resize<Machine_Button>(ref script.m_button, 2);
                    Array.Resize<AudioSource>(ref script.m_audio, 2);
                    Array.Resize<ParticleSystem>(ref script.m_particle, 2);
                    script.m_deposit = null;
                    break;
                case TypeMachineE.E_W:
                    Array.Resize<Machine_Button>(ref script.m_button, 1);
                    script.m_audio = null;
                    Array.Resize<ParticleSystem>(ref script.m_particle, 1);
                    script.m_deposit = null;
                    break;
                case TypeMachineE.ICE:
                    script.m_button = null;
                    Array.Resize<AudioSource>(ref script.m_audio, 1);
                    Array.Resize<ParticleSystem>(ref script.m_particle, 1);
                    script.m_deposit = null;
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_type"));
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            if (_type != script.m_type) Initialize();

            switch (script.m_type)
            {
                case TypeMachineE.G_D:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_button").GetArrayElementAtIndex(0), new GUIContent("Button"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio").GetArrayElementAtIndex(0), new GUIContent("Audio"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle"), new GUIContent("Particle"));
                    break;
                case TypeMachineE.E_DF:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_button").GetArrayElementAtIndex(0), new GUIContent("Button"));
                    EditorGUILayout.Space();
                    GUILayout.Label("Cup Deposit");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_deposit").GetArrayElementAtIndex(0), new GUIContent("L"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_deposit").GetArrayElementAtIndex(1), new GUIContent("R"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_depositTrans"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio").GetArrayElementAtIndex(0), new GUIContent("Audio"));
                    break;
                case TypeMachineE.E_DP:
                    GUILayout.Label("Button");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_button").GetArrayElementAtIndex(0), new GUIContent("Mode"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_button").GetArrayElementAtIndex(1), new GUIContent("Steam"));
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(10);
                    GUILayout.Label("Audio");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio").GetArrayElementAtIndex(0), new GUIContent("Steam"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio").GetArrayElementAtIndex(1), new GUIContent("Froth"));
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(10);
                    GUILayout.Label("Particle");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Steam"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(1), new GUIContent("Froth"));
                    EditorGUILayout.EndVertical();
                    break;
                case TypeMachineE.E_W:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_button").GetArrayElementAtIndex(0), new GUIContent("Button"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Particle"));
                    break;
                case TypeMachineE.ICE:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Particle"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio").GetArrayElementAtIndex(0), new GUIContent("Audio"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(20);

            toggle_Base = EditorGUILayout.Foldout(toggle_Base, "Udon");
            if (toggle_Base)
            {
                // if ( UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target) ) return;
                base.OnInspectorGUI();
            }
        }
    }
    #endif
}