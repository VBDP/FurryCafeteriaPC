
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRCCoffee;

namespace VRCCoffeeSet
{
    public enum TypeCup
    {
        NULL, Coffee, Espresso, Glass
    }

    public enum TypeContent
    {
        Empty=0,
        Syrup_Chocolate=1, Syrup_Caramel=2,
        Water=11,
        Espresso=12, Espresso_Mocha=13, Espresso_Caramel=14,
        Milk=17, Milk_caramel=18,
        Americano=21,
        Latte_1=31, Latte_2=32, Latte_3=33, Latte_4=34,
        Cappuccino_1=35, Cappuccino_2=36,
        Macchiato=37,
        Mocha_1=41, Mocha_2=42, MochaArt_1=43, MochaArt_2=44,
        Ade_Lemon_1=61, Ade_Lemon_2=62,
        Ade_Grape_1=63, Ade_Grape_2=64,
        Ade_GrapeFruit_1=65, Ade_GrapeFruit_2=66,
        Ade_Strawberry_1=67, Ade_Strawberry_2=68,
        Boba_BrownSugarMilk=100, Boba_BrownSugarMilkD=101
    }

    [AddComponentMenu("MiniGreen/Coffee Set/Cup")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Cup_Coffee : UdonSharpBehaviour
    {
        #region Variables

        public MainController m_controller;
        public VariableHolder m_variable;

        [Header("Settings")]
        public TypeCup m_type;
        public byte m_index = 255;
        public int m_skinIndex;
        public Vector2 m_contentHeight;

        [Header("Components")]
        public VRC_Pickup m_pickup;
        public Animator m_animator;
        public ParentConstraint m_constraint;
        public MeshRenderer m_meshCup;
        public Renderer m_meshContent;
        public SkinnedMeshRenderer m_meshCream;

        /// <summary> 0_Out, 1_Mid </summary>
        public Transform[] m_axis;
        /// <summary>
        /// 0_Steam or Bubble, 1~2_Pour
        /// </summary>
        public ParticleSystem[] m_particle;

        /// <summary>
        /// Glass : 0_Straw, 1_Default, 2_Boba
        /// </summary>
        public GameObject[] m_object;

        [Header("Animation")]
        /// <summary> Second </summary>
        float m_contentTime = 0;
        float m_contentFrom = 0;
        bool m_isStirring = false;
        float m_stirTime = 0;

        // Transform Data
        Vector3 defaultPos;
        Quaternion defaultRot;

        /// <summary> Second </summary>
        [HideInInspector] public float time_Delay = 0;

        /// <summary>
        /// <b>Synced variables by VariableHolder</b>                       <para></para>
        ///                                                             
        /// 0_Content Draining state: 0_None, 1_Draining, 2_Filling         <para></para>
        /// 1_Ready to Drink: 0_No, 1_Yes                                   <para></para>
        /// 2_Deposit Type: 0_None, 1_Machine, 2_Plate                      <para></para>
        /// 3_Deposit Index: 0~_index of deposit                            <para></para>
        /// 4_Top (Drizzle or Powder)                                       <para></para>
        /// 5_Cream                                                         <para></para>
        ///
        /// <b>Glass</b>                                                    <para></para>
        /// 6_Ice = 0_None, 1_Ice                                           <para></para>
        /// 7_Straw = 0_None, 1-6_Straw                                     <para></para>
        /// 8_StateStir = 0_None, 1_Stirable, 2_Stirred                     <para></para>
        /// 9_Cup Drizzle = 0_None, 1_BrownSugar                            <para></para>
        /// 10_FruitSlice = 1_Lemon, 2_Grapefruit, 3_Grape, 4_Strawberry    <para></para>
        /// 11_Garnish = 1_Mint, 2-3_Rosemary, 4-5_Thyme                    <para></para>
        /// 12_Pearls = 0_None, 1_Leftover, 2-6_Pearls                      <para></para>
        /// </summary>
        public byte[] m_info { get => m_variable.m_info; set => m_variable.m_info = value; }
        byte[] ml_info;

        /// <summary>
        /// 0_Content Recipe (TypeContent)
        /// </summary>
        public int[] m_infoInt { get => m_variable.m_infoInt; set => m_variable.m_infoInt = value; }
        int[] ml_infoInt;

        /// <summary>
        /// 0_contentLevel 0-100 <para></para>
        /// </summary>
        public float[] m_infoFloat { get => m_variable.m_infoFloat; set => m_variable.m_infoFloat = value; }
        float[] ml_infoFloat;

        #endregion

        #region Variable Indexing

        GameObject m_straw => m_object[0];
        GameObject m_objectStraw => m_object[1];
        GameObject m_objectStrawBoba => m_object[2];

        /// <summary> Recipe state of the content (TypeContent) </summary>
        public TypeContent m_contentType { get => (TypeContent)m_infoInt[0]; set => m_infoInt[0] = (int)value; }
        public int m_contentTypeNum { get => m_infoInt[0]; set => m_infoInt[0] = value; }
        public TypeContent ml_contentType { get => (TypeContent)ml_infoInt[0]; set => ml_infoInt[0] = (int)value; }

        /// <summary> 0-100 </summary>
        public float m_contentLevel { get => m_infoFloat[0]; set => m_infoFloat[0] = value; }
        public float ml_contentLevel { get => ml_infoFloat[0]; set => ml_infoFloat[0] = value; }

        /// <summary> 0_No, 1_Yes </summary>
        public bool m_contentReady { get => m_info[1] == 1; set => m_info[1] = value == true ? (byte)1 : (byte)0; }
        // public bool ml_contentReady { get => ml_info[1] == 1; set => ml_info[1] = value == true ? (byte)1 : (byte)0; }
        /// <summary> 0_None, 1_Draining, 2_Filling </summary>
        public byte m_contentDrain { get => m_info[0]; set => m_info[0] = value; }
        /// <summary> <b>Local:</b> How much drain per sceond? </summary>
        [HideInInspector] public float m_contentDrainSpeed = 20;
        /// <summary> 0_None, 1_Lerping </summary>
        public byte m_contentLerping { get => ml_info[0]; set => ml_info[0] = value; }

        /// <summary> 0_None, 1_Machine, 2_Plate </summary>
        public byte m_depositState { get => m_info[2]; set => m_info[2] = value; }
        public byte ml_depositState { get => ml_info[2]; set => ml_info[2] = value; }
        /// <summary> index of the last deposit </summary>
        public byte m_depositIndex { get => m_info[3]; set => m_info[3] = value; }
        public byte ml_depositIndex { get => ml_info[3]; set => ml_info[3] = value; }

        /// <summary> Powder or drizzle on the top of the content </summary>
        public byte m_contentTop { get => m_info[4]; set => m_info[4] = value; }
        public byte ml_contentTop { get => ml_info[4]; set => ml_info[4] = value; }
        /// <summary> 0_None, 1_Leftover, 2_Half, 3_Full </summary>
        public byte m_contentCream { get => m_info[5]; set => m_info[5] = value; }
        public byte ml_contentCream { get => ml_info[5]; set => ml_info[5] = value; }
        /// <summary> 0_None, 1_Ice </summary>
        public byte m_contentIce { get => m_info[6]; set => m_info[6] = value; }
        public byte ml_contentIce { get => ml_info[6]; set => ml_info[6] = value; }
        /// <summary> 0_None, 1-6_Straw </summary>
        public byte m_contentStraw { get => m_info[7]; set => m_info[7] = value; }
        public byte ml_contentStraw { get => ml_info[7]; set => ml_info[7] = value; }
        /// <summary> 0_None, 1_Stirable, 2_Stirred </summary>
        public byte m_contentStir { get => m_info[8]; set => m_info[8] = value; }
        public byte ml_contentStir { get => ml_info[8]; set => ml_info[8] = value; }
        /// <summary> 0_None, 1_Blacksugar </summary>
        public byte m_cupDrizzle { get => m_info[9]; set => m_info[9] = value; }
        public byte ml_cupDrizzle { get => ml_info[9]; set => ml_info[9] = value; }

        /// <summary> 0_None, 1_Lemon, 2_Grapefruit, 3_Grape, 4_Strawberry </summary>
        public byte m_adeFruitSlice { get => m_info[10]; set => m_info[10] = value; }
        public byte ml_adeFruitSlice { get => ml_info[10]; set => ml_info[10] = value; }
        /// <summary> 0_None, 1_Mint, 2-3_Rosemary, 4-5_Thyme </summary>
        public byte m_adeGarnish { get => m_info[11]; set => m_info[11] = value; }
        public byte ml_adeGarnish { get => ml_info[11]; set => ml_info[11] = value; }

        /// <summary> 0_None, 1_Leftover, 2-11_Full </summary>
        public byte m_bobaPearls { get => m_info[12]; set => m_info[12] = value; }
        public byte ml_bobaPearls { get => ml_info[12]; set => ml_info[12] = value; }

        #endregion

        void Start()
        {
            defaultPos = transform.position;
            defaultRot = transform.rotation;

            // Variables
            ml_info = new byte[m_info.Length];
            // for (int i = 0; i < ml_info.Length; i++) ml_info[i] = m_info[i];
            ml_infoInt = new int[m_infoInt.Length];
            // for (int i = 0; i < ml_infoInt.Length; i++) ml_infoInt[i] = m_info[i];
            ml_infoFloat = new float[m_infoFloat.Length];
            // for (int i = 0; i < ml_infoFloat.Length; i++) ml_infoFloat[i] = m_info[i];

            m_animator.Update(0);

            UpdateData();
            SetContentLevel(0);
        }

        private void Update()
        {
            UpdateContent();
            UpdateStir();
        }
        void UpdateContent()
        {
            if (m_type == TypeCup.Espresso && Time.time < time_Delay) return;

            if (m_contentDrain == 1)
            {
                
                if (!m_contentReady) return;
                if (ml_contentLevel == 0) return;
                ml_contentLevel -= Time.deltaTime * m_contentDrainSpeed;
                if (m_type == TypeCup.Glass && (ml_contentLevel+10) / (m_bobaPearls-1) < 10 && m_bobaPearls > 1)
                {
                    m_bobaPearls--;
                    UpdateData();
                }
                if (ml_contentLevel <= 0)
                {
                    ml_contentLevel = 0;
                    if (m_type == TypeCup.Glass && m_bobaPearls > 1)
                    {
                        m_bobaPearls = 1;
                        UpdateData();
                    }
                }
            }
            else if (m_contentDrain == 2)
            {
                if (ml_contentLevel == 100) return;
                ml_contentLevel += Time.deltaTime * m_contentDrainSpeed;
                if (ml_contentLevel >= 100) ml_contentLevel = 100;
            }
            else if (ml_contentLevel != m_contentLevel)
            {
                if (m_contentLerping == 0)
                {
                    m_contentTime = 0;
                    m_contentLerping = 1;
                    m_contentFrom = ml_contentLevel;
                }
                m_contentTime += Time.deltaTime;
                float easeOutTime = 1 - (1 - m_contentTime) * (1 - m_contentTime);
                ml_contentLevel = Mathf.Lerp(m_contentFrom, m_contentLevel, easeOutTime);
                if (m_contentTime >= 1)
                {
                    m_contentLerping = 0;
                    m_contentTime = 0;
                }
            }

            SetContentLevel(ml_contentLevel);
        }
        
        public override void OnPickup() => FuncPickup();
        public void FuncPickup()
        {
            if (m_depositState == 0) return;

            time_Delay = Time.time + m_controller.m_pickupdelay;

            if (m_depositState == 2)
            {
                var plate = GetPlate(m_depositIndex);
                if (plate.m_pickup.IsHeld && !Networking.IsOwner(plate.gameObject))
                {
                    Drop();
                    return;
                }
                else
                {
                    plate.m_indexTarget = -1;
                    plate.SerializeVariables(true);
                }
            }

            m_depositState = 0;
            SerializeVariables(true);
        }
        public override void OnDrop()
        {
            if (m_depositState == 2)
            {
                var plate = GetPlate(m_depositIndex);

                // Ths plate is not mine anymore or has cup already
                if ( ( plate.m_pickup.IsHeld && !Networking.IsOwner(plate.gameObject) ) || plate.m_indexTarget != -1) m_depositState = 0;
                else 
                {
                    // Set deposit rotation
                    Vector3 targetDir = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
                    plate.m_yDeposit = Quaternion.LookRotation(targetDir, Vector3.up).eulerAngles.y;
                    plate.m_indexTarget = m_index;

                    // Set ring
                    if (plate.m_stateRing == -1 && m_contentTypeNum >= 20 && m_contentLevel > 0 && m_contentLevel < 100)
                        plate.m_stateRing = Random.Range(0, m_controller.mat_Ring.Length);

                    plate.SerializeVariables(true);
                }
            }
            SerializeVariables(true);
        }

        public override void OnPickupUseDown() 
        {
            if (!m_contentReady || m_contentLevel == 0) return;

            // Pouring Espresso
            if (m_type == TypeCup.Espresso)
            if (m_contentLevel == 100 && m_contentCream == 0) // If content full and no creamed
            if (m_axis[0].position.y < m_axis[1].position.y) // If flipped upside down
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "PourContent");
                return;
            }

            // Return if the content is processing
            if (m_contentDrain != 0) return;

            // Eat cream
            if (m_contentCream > 1)
            {
                m_contentCream--;
                if (m_contentCream == 1) m_contentTop = 0;
            }
            // Drink
            else
            {
                m_contentDrain = 1;
            }
            SerializeVariables(true);
        }
        public override void OnPickupUseUp()
        {
            if (m_contentDrain == 1) 
            {
                if (ml_contentLevel == 0)
                {
                    m_contentReady = false;
                    m_contentType = TypeContent.Empty;
                }
                m_contentLevel = ml_contentLevel;
                m_contentDrain = 0;
                SerializeVariables(true);
            }
        }
        
        public void UpdateData()
        {
            // Content type
            if (ml_contentType != m_contentType)
            {
                ml_contentType = m_contentType;
                m_contentLerping = 0; // Reset lerping state

                Material[] tempMat = m_type == TypeCup.Glass ? m_meshContent.materials : m_meshContent.sharedMaterials;
                bool hasToBeInstanced = m_type == TypeCup.Glass;

                // Apply material
                switch (m_contentType)
                {
                    case TypeContent.Syrup_Chocolate:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_Syrup[0], hasToBeInstanced);
                        break;
                    case TypeContent.Syrup_Caramel:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_Syrup[1], hasToBeInstanced);
                        break;
                    case TypeContent.Water:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_WaterIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Water, hasToBeInstanced);
                        break;
                    case TypeContent.Espresso:
                    case TypeContent.Espresso_Mocha:
                    case TypeContent.Espresso_Caramel:
                        int tempInt = m_contentTypeNum - 12;
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_EspressoIce[tempInt], hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Espresso[tempInt], hasToBeInstanced);
                        break;
                    case TypeContent.Milk:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_MilkIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Milk[0], hasToBeInstanced);
                        break;
                    case TypeContent.Milk_caramel:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_Macchiato[0], hasToBeInstanced);
                        break;
                    case TypeContent.Americano:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_AmericanoIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Americano, hasToBeInstanced);
                        break;
                    case TypeContent.Latte_1:
                    case TypeContent.Latte_2:
                    case TypeContent.Latte_3:
                    case TypeContent.Latte_4:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_LatteIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Latte[m_contentTypeNum - 31], hasToBeInstanced);
                        break;
                    case TypeContent.Cappuccino_1:
                    case TypeContent.Cappuccino_2:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_Cappuccino[m_contentTypeNum - 35], hasToBeInstanced);
                        break;
                    case TypeContent.Macchiato:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_MacchiatoIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Macchiato[1], hasToBeInstanced);
                        break;
                    case TypeContent.Mocha_1:
                    case TypeContent.Mocha_2:
                        if (m_type == TypeCup.Glass) AssignNewMaterial(ref tempMat, 0, m_controller.mat_MochaIce, hasToBeInstanced);
                        else AssignNewMaterial(ref tempMat, 0, m_controller.mat_Mocha[m_contentTypeNum - 41], hasToBeInstanced);
                        break;
                    case TypeContent.MochaArt_1:
                    case TypeContent.MochaArt_2:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_MochaArt[m_contentTypeNum - 43], hasToBeInstanced);
                        break;
                    case TypeContent.Ade_Lemon_1: case TypeContent.Ade_Lemon_2:
                    case TypeContent.Ade_Grape_1: case TypeContent.Ade_Grape_2:
                    case TypeContent.Ade_GrapeFruit_1: case TypeContent.Ade_GrapeFruit_2:
                    case TypeContent.Ade_Strawberry_1:case TypeContent.Ade_Strawberry_2:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_Ade[m_contentTypeNum - 61], hasToBeInstanced);
                        break;
                    case TypeContent.Boba_BrownSugarMilk:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_BobaBrownSugar[0], hasToBeInstanced);
                        tempMat[0].SetFloat("_Drizzle", 0);
                        break;
                    case TypeContent.Boba_BrownSugarMilkD:
                        AssignNewMaterial(ref tempMat, 0, m_controller.mat_BobaBrownSugar[0], hasToBeInstanced);
                        tempMat[0].SetFloat("_Drizzle", 1);
                        break;
                }
                if (m_type == TypeCup.Glass) m_meshContent.materials = tempMat;
                else m_meshContent.sharedMaterials = tempMat;
                // m_meshContent.sharedMaterials = tempMat;
                SetContentLevel(m_contentLevel);
            }

            // Top
            if (ml_contentTop != m_contentTop)
            {
                ml_contentTop = m_contentTop;

                Material[] tempMat = m_meshCream.sharedMaterials;
                tempMat[1] = m_contentCream >= 2 && m_contentTop > 0 ? m_controller.mat_Top[m_contentTop - 1] : m_controller.mat_Empty;
                m_meshCream.sharedMaterials = tempMat;
                // Top
                if (m_type == TypeCup.Coffee)
                {
                    tempMat = m_meshContent.sharedMaterials;
                    tempMat[1] = m_contentCream <= 1 && m_contentTop > 0 ? m_controller.mat_Top[m_contentTop - 1] : m_controller.mat_Empty;
                    m_meshContent.sharedMaterials = tempMat;
                }
            }

            // Cream
            if (ml_contentCream != m_contentCream)
            {
                ml_contentCream = m_contentCream;
                m_animator.SetInteger("Cream", m_contentCream);
                m_meshCream.gameObject.SetActive(m_contentCream > 0);
            }

            // Particle - Steam or Bubble
            if (m_type == TypeCup.Glass)
            {
                if (m_particle[0]) switch (m_contentType)
                {
                    case TypeContent.Ade_Grape_2:
                    case TypeContent.Ade_GrapeFruit_2:
                    case TypeContent.Ade_Lemon_2:
                    case TypeContent.Ade_Strawberry_2:
                        if (!m_particle[0].isPlaying) m_particle[0].Play();
                        break;
                    default:
                        if (m_particle[0].isPlaying) m_particle[0].Stop();
                        break;
                }
            }
            else
            {
                if (!m_particle[0].isPlaying && m_contentLevel > 0 && m_contentTypeNum > 10 && m_contentCream <= 1)
                    m_particle[0].Play();
                else if ((m_particle[0].isPlaying && m_contentLevel == 0) || m_contentCream > 1)
                    m_particle[0].Stop();
            }

            // Plate
            if (ml_depositState != m_depositState)
            {
                ml_depositState = m_depositState;

                if (m_depositState == 0) m_constraint.constraintActive = false;
                else if (m_depositState == 2)
                {
                    Plate plate = GetPlate(m_depositIndex);
                    ConstraintSource source = new ConstraintSource();

                    source.sourceTransform = plate.m_posDeposit;
                    source.weight = 1;
                    m_constraint.SetSource(0, source);
                    m_constraint.constraintActive = true;

                    plate.m_audio.clip = m_controller.clip_PlatePut[Random.Range(0, m_controller.clip_PlatePut.Length)];
                    plate.m_audio.Play();
                }
            }

            // Glass cup // 
            if (m_type != TypeCup.Glass) return;

            // Ice
            if (ml_contentIce != m_contentIce)
            {
                ml_contentIce = m_contentIce;
                m_animator.SetBool("Ice", m_contentIce == 1);
            }

            // Straw
            if (ml_contentStraw != m_contentStraw)
            {
                ml_contentStraw = m_contentStraw;

                m_straw.SetActive(m_contentStraw > 0 && m_contentStir == 1);

                m_animator.SetBool("Straw", m_contentStraw != 0);

                if (m_contentStraw > 0)
                {
                    m_objectStraw.GetComponent<Renderer>().sharedMaterial = m_controller.mat_Straw[m_contentStraw - 1];
                    if (m_controller.m_udonDLC[1]) m_objectStrawBoba.GetComponent<Renderer>().sharedMaterial = m_controller.mat_Straw[m_contentStraw - 1];
                }
            }

            // Stir
            if (m_contentStir != ml_contentStir)
            {
                // Debug.Log($"{this.name}: Stir changed to {m_contentStir}");
                ml_contentStir = m_contentStir;
                for (int i = 0; i < m_meshContent.materials.Length; i++)
                {
                    if (m_meshContent.materials[i] == null) continue;
                    m_meshContent.materials[i].SetFloat("_Stir", m_contentStir == 2 ? 1 : 0);
                }
                if (m_cupDrizzle > 0 && m_bobaPearls == 11) m_meshCup.materials[1].SetFloat("_Stir", m_contentStir == 2 ? 1 : 0);

                m_straw.SetActive(m_contentStir == 1 && m_contentStraw > 0);
            }

            // Cup drizzle
            if (ml_cupDrizzle != m_cupDrizzle)
            {
                ml_cupDrizzle = m_cupDrizzle;

                if (m_cupDrizzle > 0)
                {
                    Material[] tempMatCup = m_meshCup.materials;

                    // Adjust length or destroy old material
                    if (tempMatCup.Length != 2) tempMatCup = new Material[] { m_meshCup.material, null };
                    else Destroy(tempMatCup[1]);

                    // Assign Material
                    switch (m_cupDrizzle)
                    {
                        case 1:
                            tempMatCup[1] = m_controller.mat_BobaDrizzle[0];
                            break;
                    }
                    m_meshCup.materials = tempMatCup;
                    
                    m_meshCup.materials[1].SetFloat("_Amount", ml_contentLevel / 100);
                    m_meshCup.materials[1].SetVector("_AmountCap", new Vector4(m_contentHeight.x, m_contentHeight.y, 0, 0));
                }
                else if (m_meshCup.materials.Length != 1)
                {
                    Destroy(m_meshCup.materials[1]);
                    Material[] tempMatCup = { m_meshCup.materials[0] };
                    m_meshCup.materials = tempMatCup;
                }
            }

            // DLC Ade
            if (m_controller.m_udonDLC[0] != null)
            {
                // Fruit
                if (ml_adeFruitSlice != m_adeFruitSlice)
                {
                    ml_adeFruitSlice = m_adeFruitSlice;
                    m_animator.SetFloat("Fruit", m_controller.MathRemap(m_adeFruitSlice, 0, 4, 0, 1));
                }
                // Herb
                if (ml_adeGarnish != m_adeGarnish)
                {
                    ml_adeGarnish = m_adeGarnish;
                    m_animator.SetFloat("Garnish", m_controller.MathRemap(m_adeGarnish, 0, 5, 0, 1));
                }
            }
        
            // DLC Boba
            if (m_controller.m_udonDLC[1] != null)
            {
                // Pearls
                if (ml_bobaPearls != m_bobaPearls)
                {
                    ml_bobaPearls = m_bobaPearls;
                    m_animator.SetInteger("Pearls", m_bobaPearls);

                    if (m_bobaPearls >= 2)
                    {
                        Material[] tempMat = m_meshContent.materials;
                        if (tempMat.Length != 2) tempMat = new Material[] { m_meshContent.material, null };
                        else Destroy(tempMat[1]);
                        tempMat[1] = m_controller.mat_BobaPearls[m_bobaPearls - 2];
                        tempMat[1].SetFloat("_Amount", ml_contentLevel / 100);
                        tempMat[1].SetVector("_AmountCap", new Vector4(m_contentHeight.x, m_contentHeight.y, 0, 0));
                        tempMat[1].SetFloat("_Stir", m_contentStir == 2 ? 1 : 0);
                        m_meshContent.materials = tempMat;
                    }
                    else if (m_meshContent.materials.Length != 1)
                    {
                        Destroy(m_meshContent.materials[1]);
                        Material[] tempMatCup = { m_meshContent.materials[0] };
                        m_meshContent.materials = tempMatCup;
                    }
                }
            }
        }
        public void SetContentLevel(float level)
        {
            m_animator.SetFloat("Content", m_controller.MathRemap(level, 0, 100, 1, 0));
            if (m_type == TypeCup.Glass)
            {
                for (int i = 0; i < m_meshContent.materials.Length; i++)
                {
                    if (m_meshContent.materials[i] == null) continue;
                    m_meshContent.materials[i].SetFloat("_Amount", level / 100);
                    m_meshContent.materials[i].SetVector("_AmountCap", new Vector4(m_contentHeight.x, m_contentHeight.y, 0, 0));
                }
                if (m_cupDrizzle > 0 && m_bobaPearls == 11) m_meshCup.materials[1].SetFloat("_Amount", level / 100);
            }
        }

        void OnParticleCollision(GameObject other)
        {
            // Check owner
            if (!Networking.IsOwner(gameObject)) return;

            // Check particle
            string[] nameContact_Splited = other.name.Split(new char[] { '_' });
            if (nameContact_Splited[0] != "Particle") return;

            // Check cup type
            if (m_type == TypeCup.Espresso) CheckParticle_Espresso(nameContact_Splited[1]);
            else if (m_type == TypeCup.Coffee) CheckParticle_Coffee(nameContact_Splited[1]);
            else if (m_type == TypeCup.Glass) CheckParticle_Glass(nameContact_Splited[1]);

            // Check data change
            bool isChanged = false;
            for (int i = 0; i < m_info.Length; i++) if (ml_info[i] != m_info[i]) { isChanged = true; break; }
            if (!isChanged) for (int i = 0; i < m_infoInt.Length; i++) if (ml_infoInt[i] != m_infoInt[i]) { isChanged = true; break; }
            if (isChanged) SerializeVariables(true);
        }

    #region Espresso

        void CheckParticle_Espresso(string name)
        {
            switch (name)
            {
                case "PowderChoco":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 1;
                    break;
                case "PowderCinamon":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 2;
                    break;
                case "SyrupChoco":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 3;
                    break;
                case "SyrupCaramel":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 4;
                    else if (m_contentType == TypeContent.Espresso && m_contentLevel == 100 && m_contentCream == 0) m_contentType = TypeContent.Espresso_Caramel;
                    break;
                case "Cream":
                    if (m_contentLevel == 100 && m_contentCream < 2) m_contentCream = 3;
                    break;
                case "WaterWash":
                    m_contentReady = false;
                    m_contentCream = 0;
                    m_contentTop = 0;
                    m_contentLevel = 0;
                    break;
                default: break;
            }

        }

        public void PourContent()
        {
            m_particle[m_contentType == TypeContent.Espresso ? 1 : 2].Play();
            m_contentLevel = 0;
            m_contentType = 0;
            UpdateData();
        }

    #endregion

    #region Coffee

        void CheckParticle_Coffee(string name)
        {
            switch (name)
            {
                case "WaterHot":
                    if (m_contentType == TypeContent.Empty || (m_contentType == TypeContent.Water && m_contentLevel != 80))
                        { m_contentType = TypeContent.Water; m_contentLevel = 80; }
                    else if (m_contentType == TypeContent.Espresso && m_contentLevel == 40)
                        { m_contentType = TypeContent.Americano; m_contentLevel = 100; }
                    break;
                case "Espresso":
                    if (m_contentType == TypeContent.Empty)
                        { m_contentType = TypeContent.Espresso; m_contentLevel = 40; }
                    else if (m_contentType == TypeContent.Syrup_Chocolate && m_contentLevel == 20)
                        { m_contentType = TypeContent.Espresso_Mocha; m_contentLevel = 40; }
                    else if (m_contentType == TypeContent.Water && m_contentLevel == 80)
                        { m_contentType = TypeContent.Americano; m_contentLevel = 100; }
                    else if (m_contentType == TypeContent.Milk_caramel && m_contentLevel == 80)
                        { m_contentType = TypeContent.Macchiato; m_contentLevel = 100; }
                    break;
                case "MilkFrothed 1":
                    if (m_contentType == TypeContent.Espresso && m_contentLevel == 40)  // Epsresso > Latte
                        { m_contentType = (TypeContent)(30 + UnityEngine.Random.Range(1, 5)); m_contentLevel = 100; }
                    else if (m_contentType == TypeContent.Espresso_Mocha && m_contentLevel == 40)
                    {
                        if (m_contentTop == 1) // EspressoMocha, PowderChoco > MochaArt
                            { m_contentTypeNum = (byte)(40 + UnityEngine.Random.Range(3, 5)); m_contentLevel = 100; m_contentTop = 0; }
                        else { m_contentTypeNum = (byte)(40 + UnityEngine.Random.Range(1, 3)); m_contentLevel = 100; } // EspressoMocha > Mocha
                    }
                    else if (m_contentType == TypeContent.Syrup_Caramel)
                        { m_contentType = TypeContent.Milk_caramel; m_contentLevel = 80; }
                    break;
                case "MilkFrothed 2":
                    if (m_contentType == TypeContent.Espresso && m_contentLevel == 40) // Espresso > Cappuccino
                        { m_contentTypeNum = (byte)(30 + UnityEngine.Random.Range(5, 7)); m_contentLevel = 100; }
                    break;
                case "PowderChoco":
                    if (m_contentTop == 0 && (m_contentCream == 3 || m_contentType == TypeContent.Espresso_Mocha || m_contentType == TypeContent.Mocha_1 || m_contentType == TypeContent.Mocha_2)) m_contentTop = 1;
                    break;
                case "PowderCinamon":
                    if (m_contentTop == 0 && (m_contentCream == 3 || m_contentType == TypeContent.Cappuccino_1 || m_contentType == TypeContent.Cappuccino_2)) m_contentTop = 2;
                    break;
                case "SyrupChoco":
                    if (m_contentType == TypeContent.Empty && m_contentLevel == 0)
                        { m_contentType = TypeContent.Syrup_Chocolate; m_contentLevel = 20; }
                    else if (m_contentTop == 0 && (m_contentCream == 3 || m_contentType == TypeContent.Mocha_1 || m_contentType == TypeContent.Mocha_2)) m_contentTop = 3;
                    break;
                case "SyrupCaramel":
                    if (m_contentType == TypeContent.Empty && m_contentLevel == 0) { m_contentType = TypeContent.Syrup_Caramel; m_contentLevel = 1; }
                    if (m_contentTop == 0 && (m_contentCream == 3 || m_contentType == TypeContent.Macchiato)) m_contentTop = 4;
                    break;
                case "Cream":
                    if (m_contentLevel == 100 && m_contentCream <= 1 && m_contentTop == 0) m_contentCream = 3;
                    break;
                case "WaterWash":
                    m_contentReady = false;
                    m_contentLevel = 0;
                    m_contentType = 0;
                    m_contentCream = 0;
                    m_contentTop = 0;
                    break;
                default: break;
            }
            // Ready to drink
            if (m_contentLevel == 100) m_contentReady = true;
        }

    #endregion

    #region Glass

        void CheckParticle_Glass(string name)
        {
            switch (name)
            {
                case "Ice":
                    if (m_contentIce == 0) m_contentIce = 1;
                    break;
                case "WaterCold":
                    if (m_contentIce == 0) break;
                    else if (m_contentType == TypeContent.Empty || (m_contentType == TypeContent.Water && m_contentLevel < 80))
                        { m_contentType = TypeContent.Water; m_contentLevel = 80; }
                    else if (m_contentType == TypeContent.Espresso)
                        { m_contentType = TypeContent.Americano; m_contentLevel = 100; }
                    break;
                case "Milk":
                    if (m_contentIce == 0) break;
                    else if (m_contentType == TypeContent.Empty && m_bobaPearls >= 2 && m_cupDrizzle == 1)
                        { m_contentType = TypeContent.Boba_BrownSugarMilk; m_contentLevel = 100; m_contentStir = 1; }
                    else if (m_contentType == TypeContent.Empty || (m_contentType == TypeContent.Milk && m_contentLevel < 80))
                        { m_contentType = TypeContent.Milk; m_contentLevel = 80; }
                    else if (m_contentType == TypeContent.Espresso_Mocha)
                        { m_contentType = TypeContent.Mocha_1; m_contentLevel = 100; }
                    break;
                case "Espresso":
                    if (m_contentIce == 0) {
                        if (m_contentType == TypeContent.Empty)
                            { m_contentType = TypeContent.Espresso; m_contentLevel = 20; }
                    } else {
                        if (m_contentType == TypeContent.Water)
                            { m_contentType = TypeContent.Americano; m_contentLevel += 20; }
                        else if (m_contentType == TypeContent.Milk && m_contentLevel == 80 )
                            { m_contentType = TypeContent.Latte_1; m_contentLevel = 100; }
                    }
                    break;
                case "EspressoCaramel":
                    if (m_contentType == TypeContent.Milk && m_contentLevel == 80)
                        { m_contentType = TypeContent.Macchiato; m_contentLevel = 100; }
                    break;
                case "PowderChoco":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 1;
                    break;
                case "PowderCinamon":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 2;
                    break;
                case "SyrupChoco":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 3;
                    else if (m_contentIce == 0 && m_contentType == TypeContent.Espresso)
                        { m_contentType = TypeContent.Espresso_Mocha; }
                    break;
                case "SyrupCaramel":
                    if (m_contentCream == 3 && m_contentTop == 0) m_contentTop = 4;
                    break;
                case "Cream":
                    if (m_contentTypeNum > 20 && m_contentCream <= 1 && m_contentLevel == 100) m_contentCream = 3;
                    break;
                case "WaterWash":
                    m_contentReady = false;
                    m_contentLevel = 0;
                    m_contentType = 0;
                    m_contentCream = 0;
                    m_contentTop = 0;
                    m_contentIce = 0;
                    m_contentStraw = 0;
                    m_contentStir = 0;
                    m_adeFruitSlice = 0;
                    m_adeGarnish = 0;
                    m_bobaPearls = 0;
                    m_cupDrizzle = 0;
                    break;
                // Ade
                case "JuiceLemon":
                    if (m_contentType == TypeContent.Empty)
                        { m_contentType = TypeContent.Ade_Lemon_1; m_contentLevel = 20; }
                    break;
                case "JuiceGrape":
                    if (m_contentType == TypeContent.Empty)
                        { m_contentType = TypeContent.Ade_Grape_1; m_contentLevel = 20; }
                    break;
                case "JuiceGrapefruit":
                    if (m_contentType == TypeContent.Empty)
                        { m_contentType = TypeContent.Ade_GrapeFruit_1; m_contentLevel = 20; }
                    break;
                case "JuiceStrawberry":
                    if (m_contentType == TypeContent.Empty)
                        { m_contentType = TypeContent.Ade_Strawberry_1; m_contentLevel = 20; }
                    break;
                case "SparklingWater":
                    if (m_contentIce == 0) break;
                    else if (m_contentType == TypeContent.Ade_Lemon_1)
                        { m_contentType = TypeContent.Ade_Lemon_2; m_contentLevel = 100; }
                    else if (m_contentType == TypeContent.Ade_Grape_1)
                        { m_contentType = TypeContent.Ade_Grape_2; m_contentLevel = 100; }
                    else if (m_contentType == TypeContent.Ade_GrapeFruit_1)
                        { m_contentType = TypeContent.Ade_GrapeFruit_2; m_contentLevel = 100; }
                    else if (m_contentType == TypeContent.Ade_Strawberry_1)
                        { m_contentType = TypeContent.Ade_Strawberry_2; m_contentLevel = 100; }
                    break;
                case "LemonSlice":
                    if (m_contentIce == 1 && m_adeFruitSlice == 0) m_adeFruitSlice = 1;
                    Debug.Log("It's lemon slice: " + m_adeFruitSlice);
                    break;
                case "GrapefruitSlice":
                    if (m_contentIce == 1 && m_adeFruitSlice == 0) m_adeFruitSlice = 2;
                    break;
                case "GreenGrape":
                    if (m_contentIce == 1 && m_adeFruitSlice == 0) m_adeFruitSlice = 3;
                    break;
                case "Strawberry":
                    if (m_contentIce == 1 && m_adeFruitSlice == 0) m_adeFruitSlice = 4;
                    break;
                case "Mint":
                    if (m_contentIce == 1 && m_adeGarnish == 0) m_adeGarnish = 1;
                    break;
                case "Rosemary":
                    if (m_contentIce == 1 && m_adeGarnish == 0) m_adeGarnish = (byte)(UnityEngine.Random.Range(2, 4));
                    break;
                case "Thyme":
                    if (m_contentIce == 1 && m_adeGarnish == 0) m_adeGarnish = (byte)(UnityEngine.Random.Range(4, 6));
                    break;
                // Boba
                case "Pearls":
                    if (m_contentType == 0 && m_contentIce == 0 && m_bobaPearls == 0) { m_bobaPearls = 11; }
                    break;
                case "SyrupBrownSugar":
                    if (m_contentLevel == 0 && m_contentIce == 0 && m_cupDrizzle == 0) { m_cupDrizzle = 1; }
                    else if (m_contentType == TypeContent.Boba_BrownSugarMilk && m_contentStir != 2) { m_contentType = TypeContent.Boba_BrownSugarMilkD; }
                    break;
                case "BlackTea":
                    break;
                case "Ube":
                    break;
            }
            // Ready to drink
            if (m_contentLevel == 100) m_contentReady = true;
        }

        void UpdateStir()
        {
            if (!m_isStirring) return;

            m_stirTime += Time.deltaTime;
            if (m_stirTime >= 1)
            {
                m_isStirring = false;
                m_stirTime = 1;
            }

            float easeInOutTime = m_stirTime < 0.5f
                ? 2 * m_stirTime * m_stirTime
                : 1 - Mathf.Pow(-2 * m_stirTime + 2, 2) / 2;

            m_animator.SetFloat("Stir", easeInOutTime);
            for (int i = 0; i < m_meshContent.materials.Length; i++)
            {
                if (m_meshContent.materials[i] == null) continue;
                m_meshContent.materials[i].SetFloat("_Stir", easeInOutTime);
            }
            if (m_cupDrizzle > 0 && m_bobaPearls == 11) m_meshCup.materials[1].SetFloat("_Stir", easeInOutTime);
        }
        public void StartStir()
        {
            m_isStirring = true;
            m_stirTime = 0;
        }

    #endregion

    #region Functions

        public void Drop() { m_pickup.Drop(); }
        public void SetPickup(bool value) { m_pickup.pickupable = value; }

        public void Call_Reset()
        {
            if (m_pickup.IsHeld || !m_pickup.pickupable) return;
            if (m_type != TypeCup.Glass)
            {
                if (m_depositState == 2 && GetPlate(m_depositIndex).m_pickup.IsHeld) return;
                if (m_depositState > 0) FuncPickup();
            }
            SerializeVariables(true);
            transform.SetPositionAndRotation(defaultPos, defaultRot);
        }

        Plate GetPlate(byte index)
        {
            if (m_type == TypeCup.Espresso) return m_controller.dish_espressoPlate[index];
            else if (m_type == TypeCup.Coffee) return m_controller.dish_coffeePlate[index];
            else return null;
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

        void AssignNewMaterial(ref Material[] tempMat, int index, Material mat, bool destroyOld)
        {
            if (destroyOld) Destroy(tempMat[index]);
            tempMat[index] = mat;
        }
        
    #endregion
    }
}