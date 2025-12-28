
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.Udon.Serialization.OdinSerializer;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Tool")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Tool : UdonSharpBehaviour
    {
        #region Variables

        public MainController m_controller;
        public VariableHolder m_variable;
        public VRC_Pickup m_pickup;
        public Animator m_animator;

        public TypeTool m_type;
        public byte m_index;
        /// <summary>
        /// Fruit: 100_Lemon, 101_Grapefruit, 102_Grape, 103_Strawberry
        /// </summary>
        public byte m_skinIndex;

        public ParticleSystem[] m_particle;
        public AudioSource m_audio;
        /// <summary>
        /// 0_Out, 1_Mid
        /// </summary>
        public Transform[] m_axis;
        /// <summary>
        /// <b>Common</b>                                   <para></para>
        /// 0_Content                                       <para></para>
        /// <b>Filter</b>                                   <para></para>
        /// Flow: 1_Bottomless, 2_Single, 3_Double
        /// </summary>
        public Renderer[] m_contentMesh;
        public Renderer[] m_objMesh;

        public bool isCap;
        bool stateCap = true;
        public bool isAnim;
        public float lengthAnim;
        public string eventAnim;
        public bool isTilt;
        public bool isTrig;
        public bool isHold;

        /// <summary> Prevent deposit immediately (time + controller.pickUpDelay) </summary>
        [HideInInspector] public float timeDelay = 0;

        // Transform Data
        [HideInInspector] public Vector3 defaultPos;
        [HideInInspector] public Quaternion defaultRot;
        
        /// <summary>
        /// <b>Common</b>                                               <para></para>
        /// 0_State : 1_Grinder, 2_Machine                              <para></para>
        /// 1_Last Index of Machine                                     <para></para>
        /// 
        /// <b>Filter</b>                                               <para></para>
        /// 2_Content - 0_Empty, 1_Grinded, 2_Tampered, 3_Extracted     <para></para>
        /// 3_Spout - 0-2_Count                                         <para></para>
        /// 
        /// <b>Pitcher</b>                                              <para></para>
        /// 2_Content - 0_Empty, 1_Milk, 2_Latte, 3_Cappuccino          <para></para>
        /// </summary>
        public byte[] m_info { get => m_variable.m_info; set => m_variable.m_info = value; }
        byte[] ml_info;

        /// <summary>
        /// <b>Filter</b> 0_ContentLevel - 0-100
        /// </summary>
        public float[] m_infoFloat { get => m_variable.m_infoFloat; set => m_variable.m_infoFloat = value; }
        float[] ml_infoFloat;

        #endregion

        #region Variable Indexing

        /// <summary> 0_None, 1_Grinder, 2_Machine  </summary>
        public byte m_depositState { get => m_info[0]; set => m_info[0] = value; }
        /// <summary> Last index of the deposit </summary>
        public byte m_depositIndex { get => m_info[1]; set => m_info[1] = value; }
        /// <summary>
        /// Filter                                                      <para></para>
        /// 0_Empty, 1_Grinded, 2_Tampered, 3_Extracted                 <para></para>
        /// Pitcher                                                     <para></para>
        /// 0_Empty, 1_Milk, 2_Latte, 3_Cappuccino                      <para></para>
        /// </summary>
        public byte m_content { get => m_info[2]; set => m_info[2] = value; }
        byte ml_content { get => ml_info[2]; set => ml_info[2] = value; }
        /// <summary> 0_BottomLess, 1-2_SpoutCount </summary>
        public byte m_spout { get => m_info[3]; set => m_info[3] = value; }
        public int m_spoutCount { get => m_spout == 2 ? 2 : 1; }

        /// <summary> Filter: 0-100 </summary>
        public float m_contentLevel { get => m_infoFloat[0]; set => m_infoFloat[0] = value; }
        float ml_contentLevel { get => ml_infoFloat[0]; set => ml_infoFloat[0] = value; }
        bool m_animStarted { get => isCap; set => isCap = value; }
        /// <summary> 0_None, 1_Flowing, 2_Stopping </summary>
        public byte m_animFlow { get => ml_info[0]; set => ml_info[0] = value; }

        #endregion

        void Start()
        {
            if (m_variable != null)
            {
                ml_info = new byte[m_info.Length];
                for (int i = 0; i < ml_info.Length; i++) ml_info[i] = m_info[i];
                ml_infoFloat = new float[m_infoFloat.Length];
                for (int i = 0; i < ml_infoFloat.Length; i++) ml_infoFloat[i] = m_infoFloat[i];
            }

            defaultPos = transform.position;
            defaultRot = transform.rotation;

            UpdateData();
            if (m_type == TypeTool.Filter) SetContentLevel(m_contentLevel);
        }

        void LateUpdate()
        {
            CheckTilt();
            FilterContent();
            FilterFlow();
        }
        void CheckTilt()
        {
            if (m_type != TypeTool.Ingredient) return;
            if (!isTilt || isTrig) return;

            if (isCap && stateCap)
            {
                if (m_particle[0].isPlaying) ParticleStop();
                return;
            }
            if (m_axis[0].position.y < m_axis[1].position.y && m_pickup.IsHeld && !m_particle[0].isPlaying && Time.time > timeDelay) ParticlePlay();
            else if ( (m_axis[0].position.y > m_axis[1].position.y || !m_pickup.IsHeld) && m_particle[0].isPlaying) ParticleStop();
        }
        void FilterContent()
        {
            if (m_type != TypeTool.Filter) return;
            
            if (ml_contentLevel == m_contentLevel) return;
            else if (!isAnim) ml_contentLevel = m_contentLevel;
            else 
            {
                ml_contentLevel += Time.deltaTime * (m_contentLevel / timeDelay);
                if (ml_contentLevel >= m_contentLevel) ml_contentLevel = m_contentLevel;
            }

            SetContentLevel(ml_contentLevel);
        }
        void FilterFlow()
        {
            if (m_type != TypeTool.Filter) return;
            if (m_animFlow == 0) return;

            if (!m_animStarted)
            {
                m_animStarted = true;
                lengthAnim = m_animFlow == 1 ? 0 : 1;
                m_contentMesh[m_spout+1].gameObject.SetActive(true);
                m_contentMesh[m_spout+1].sharedMaterial.SetInt("_FlowFlip", m_animFlow == 1 ? 0 : 1);
            }

            // Flowing
            if (m_animFlow == 1)
            {
                lengthAnim += Time.deltaTime / timeDelay;
                if (lengthAnim >= 1)
                {
                    m_animStarted = false;
                    lengthAnim = 1;
                    m_animFlow = 0;
                }
            }
            // Stopping
            else if (m_animFlow == 2)
            {
                lengthAnim -= Time.deltaTime / timeDelay;
                if (lengthAnim <= 0)
                {
                    m_contentMesh[m_spout+1].gameObject.SetActive(false);
                    m_animStarted = false;
                    lengthAnim = 0;
                    m_animFlow = 0;
                }
            }

            m_contentMesh[m_spout+1].sharedMaterial.SetFloat("_Flow", lengthAnim);
        }
        
        public void UpdateData()
        {
            switch (m_type)
            {
                case TypeTool.Filter:
                    m_animator.SetInteger("Spout", m_spout);
                    m_animator.SetBool("Tamped", m_content >= 2);
                    
                    if (ml_content != m_content)
                    {
                        ml_content = m_content;
                        m_contentMesh[0].sharedMaterial = m_controller.mat_FilterContent[m_content == 3 ? 1 : 0];
                    }
                    break;
                case TypeTool.Pitcher:
                    m_contentMesh[0].gameObject.SetActive(m_content > 0);
                    if (m_content > 0)
                    {
                        var tempMat = m_contentMesh[0].sharedMaterials;
                        tempMat[1] = m_controller.mat_Milk[m_content];
                        m_contentMesh[0].sharedMaterials = tempMat;
                    }
                    break;
            }
        }
        public void SetContentLevel(float value)
        {
            switch (m_type)
            {
                case TypeTool.Filter:
                    m_animator.SetFloat("Content", m_controller.MathRemap(value, 0, 100, 0, 1));
                    break;
            }
        }

        void OnTriggerEnter(Collider other) 
        {
            if (!Networking.IsOwner(other.gameObject)) return;

            switch (m_type)
            {
                case TypeTool.Straw:
                    // Check obj has valid script
                    Cup_Coffee udonCup = other.GetComponent<Cup_Coffee>();
                    if (!udonCup) return;
                    if (udonCup.m_type != TypeCup.Glass) return;
                    // Check there is a straw
                    if (udonCup.m_contentStraw > 0) return;
                    // Setup
                    udonCup.m_contentStraw = (byte)(m_skinIndex + 1);
                    udonCup.SerializeVariables(true);
                    // Reset Position
                    if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
                    Drop();
                    transform.SetPositionAndRotation(defaultPos, defaultRot);
                    break;
                case TypeTool.Tamper:
                    // Not tampering if tamper is not held
                    if (!m_pickup.IsHeld) return;
                    // Check obj has valid script
                    Tool udonFiter = other.GetComponent<Tool>();
                    if (udonFiter == null) return;
                    if (udonFiter.m_type != TypeTool.Filter) return;
                    // Check State
                    if (udonFiter.m_skinIndex != m_skinIndex) return;
                    if (udonFiter.m_content != 1) return;
                    // If filter is held, Variable will be set on owner's side
                    if (udonFiter.m_pickup.IsHeld && !Networking.IsOwner(other.gameObject)) return;
                    // Setup
                    udonFiter.m_content = 2;
                    udonFiter.SerializeVariables(true);
                    break;
                case TypeTool.KnockBox:
                    // Check obj has valid script
                    Tool udonFilter = other.GetComponent<Tool>();
                    if (!udonFilter) return;
                    if (udonFilter.m_type != TypeTool.Filter) return;
                    // Check state
                    if (udonFilter.m_content == 0) return;
                    // Setup
                    udonFilter.m_content = 0;
                    udonFilter.m_contentLevel = 0;
                    udonFilter.SerializeVariables(true);
                    SendCustomNetworkEvent(NetworkEventTarget.All, "AudioPlay");
                    break;
                case TypeTool.Squeezer:
                    Tool udonFruit = other.GetComponent<Tool>();
                    if (!udonFruit) return;
                    if (udonFruit.m_type != TypeTool.Ingredient) return;
                    switch (udonFruit.m_skinIndex)
                    {
                        case 100: // Lemon
                            SendCustomNetworkEvent(NetworkEventTarget.All, "ParticlePlay");
                            break;
                        case 101: // Grapefruit
                            SendCustomNetworkEvent(NetworkEventTarget.All, "ParticlePlay_1");
                            break;
                    }
                    break;
            }
        }

        public override void OnPickup() => FuncPickup();
        public void FuncPickup()
        {
            if (!m_variable) return;
            if (m_depositState == 0) return;
            timeDelay = Time.time + m_controller.m_pickupdelay;

            byte tempState = m_depositState;
            m_depositState = 0;
            SerializeVariables();

            if (tempState == 2)
            {
                switch (m_type)
                {
                    case TypeTool.Filter:
                        m_controller.m_machineF[m_depositIndex].SendCustomNetworkEvent(NetworkEventTarget.Owner, "DepositOff");
                        break;
                    case TypeTool.Pitcher:
                        m_controller.m_machineP[m_depositIndex].SendCustomNetworkEvent(NetworkEventTarget.Owner, "DepositOff");
                        break;
                }
            }
        }
        
        public override void OnPickupUseDown()
        {
            switch (m_type)
            {
                case TypeTool.Ingredient:
                    if (isCap && stateCap)
                    {
                        SendCustomNetworkEvent(NetworkEventTarget.All, "CapOpen");
                        timeDelay = Time.time + lengthAnim;
                    }
                    
                    if (lengthAnim > 0 && Time.time < timeDelay) return;

                    if (!isTrig) return;

                    if (isAnim && !isCap) SendCustomNetworkEvent(NetworkEventTarget.All, isHold ? "AnimStart" : "AnimTrig");

                    if (isTilt) if (m_axis[0].position.y > m_axis[1].position.y) return;

                    SendCustomNetworkEvent(NetworkEventTarget.All, "ParticlePlay");

                    SendCustomNetworkEvent(NetworkEventTarget.All, "AudioPlay");
                    break;
                case TypeTool.Filter:
                    m_spout++;
                    if (m_spout > 2) m_spout = 0;
                    UpdateData();
                    SerializeVariables();
                    break;
                case TypeTool.Pitcher:
                    if (m_axis[0].position.y < m_axis[1].position.y && m_content > 1)
                    {
                        m_particle[m_content - 2].Play();
                        m_content = 0;
                        UpdateData();
                        SerializeVariables();
                    }
                break;
            }
        }
        public override void OnPickupUseUp()
        {
            if (isTrig && isHold)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "ParticleStop");
                if (isHold) SendCustomNetworkEvent(NetworkEventTarget.All, "AnimStop");
            }
            SendCustomNetworkEvent(NetworkEventTarget.All, "AudioStop");
        }

        public override void OnDrop()
        {
            if (isCap) SendCustomNetworkEvent(NetworkEventTarget.All, "CapClose");
        }

        void OnParticleCollision(GameObject other)
        {
            if (m_type != TypeTool.Pitcher) return;
            if (!Networking.IsOwner(gameObject)) return;

            string[] nameContact_Splited = other.name.Split(new char[] {'_'});
            if (nameContact_Splited[0] != "Particle") return;

            switch (nameContact_Splited[1])
            {
                case "Milk":
                    if (m_content == 0) m_content = 1;
                break;
                case "WaterWash":
                    if (m_content != 0) m_content = 0;
                break;
                default: return;
            }
            SerializeVariables();
            UpdateData();
        }

        public void SetActive(bool value) { foreach (var item in m_objMesh) item.enabled = value; }
        public void SetPickup(bool value) { m_pickup.pickupable = value; }
        public void Drop() { m_pickup.Drop(); }
        public void ParticlePlay() { m_particle[0].Play(); }
        public void ParticlePlay_1() { m_particle[1].Play(); }
        public void ParticleStop() { m_particle[0].Stop(); }
        public void AnimTrig() { m_animator.SetTrigger(eventAnim); }
        public void AnimStart() { m_animator.SetBool(eventAnim, true); }
        public void AnimStop() { m_animator.SetBool(eventAnim, false); }
        public void AudioPlay() { if (m_audio) m_audio.Play(); }
        public void AudioStop() { if (m_audio) m_audio.Stop(); }
        public void CapOpen()
        {
            timeDelay = Time.time + lengthAnim;
            stateCap = false;
            if (isAnim) m_animator.SetBool(eventAnim, false);
        }
        public void CapClose()
        {
            stateCap = true;
            if (isAnim) m_animator.SetBool(eventAnim, true);
        }

        /// <summary> Local, by MainController </summary>
        public void Call_Reset()
        {
            if (m_pickup.IsHeld || !m_pickup.pickupable) return;
            if (m_variable && m_depositState > 0) FuncPickup();

            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

            transform.SetPositionAndRotation(defaultPos,defaultRot);
        }
        /// <summary> Automatically setting the owner </summary>
        public void SerializeVariables(bool updateData = false)
        {
            if (!Networking.IsOwner(gameObject) || !Networking.IsOwner(m_variable.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                Networking.SetOwner(Networking.LocalPlayer, m_variable.gameObject);
            }
            RequestSerialization();
            m_variable.RequestSerialization();
            if (updateData) UpdateData();
        }
    }
}