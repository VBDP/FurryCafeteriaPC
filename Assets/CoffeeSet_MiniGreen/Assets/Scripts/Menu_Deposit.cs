
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Menu Deposit")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Menu_Deposit : UdonSharpBehaviour
    {
        [Header("DO NOT Change variables")]
        [Header("")]
        GameObject menuPlate;

        Vector3 posDeposit;
        Quaternion rotDeposit;

        void Start()
        {
            menuPlate = GetComponentInChildren<Menu_Plate>().gameObject;

            posDeposit = menuPlate.transform.position;
            rotDeposit = menuPlate.transform.rotation;
        }

        public override void Interact()
        {
            VRC_Pickup pickupPlate = (VRC_Pickup)menuPlate.GetComponent(typeof(VRC_Pickup));
            if (pickupPlate.IsHeld) return;
            Networking.SetOwner(Networking.LocalPlayer, menuPlate);
            menuPlate.transform.SetPositionAndRotation(posDeposit, rotDeposit);
        }
    }
}