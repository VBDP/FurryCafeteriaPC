
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Variable Holder")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VariableHolder : UdonSharpBehaviour
    {
        public Cup_Coffee scriptCup;
        public Plate scriptPlate;
        public Tool scriptTool;

        /// <summary>
        /// Cup                                     <para></para>
        /// 0_State : 0_None, 1_Machine, 2_Plate    <para></para>
        /// 1_Index : -1_None 0~_index of deposit   <para></para>
        /// 
        /// Filter                                  <para></para>
        /// 0_State : 1_Grinder, 2_Machine          <para></para>
        /// 1_Last Index of Machine                 <para></para>
        /// 
        /// Pitcher                                 <para></para>
        /// 0_State : 1_Machine                     <para></para>
        /// 1_Last Index of Machine                 <para></para>
        /// </summary>
        // [UdonSynced] public sbyte[] m_deposit;

        
        /// <summary>
        /// Plate                                                       <para></para>
        /// 0_Index of cup                                              <para></para>
        /// 1_Index of ring                                             <para></para>
        /// 2_isHeld                                                    <para></para>
        ///                                                             <para></para>
        /// Filter                                                      <para></para>
        /// 0_Content - 0_Empty, 1_Grinded, 2_Tampered, 3_Extracted     <para></para>
        /// 1_NozzleCount - 1 or 2                                      <para></para>
        /// 2_Index of content material                                 <para></para>
        ///                                                             <para></para>
        /// Pitcher                                                     <para></para>
        /// 0_Content - 0_Empty, 1_Milk, 2_Latte, 3_Cappuccino          <para></para>
        /// </summary>
        [UdonSynced] public byte[] m_info;
        [UdonSynced] public int[] m_infoInt;

        /// <summary>
        /// Plate                                                       <para></para>
        /// 0_deposit Y rotation                                        <para></para>
        /// </summary>
        [UdonSynced] public float[] m_infoFloat;

        // Cup Top List
        // 0_None
        // Powders : 1_Cocoa, 2_Cinamon
        // Syrups : 3_Chocolate, 4_Caramel

        // Cream
        // 0_None, 1_Stain, 2~3_Cream

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject)) RequestSerialization();
        }
        public override void OnDeserialization()
        {
            if (scriptCup) scriptCup.UpdateData();
            else if (scriptPlate) scriptPlate.UpdateData();
            else if (scriptTool) scriptTool.UpdateData();
        }
    }
}