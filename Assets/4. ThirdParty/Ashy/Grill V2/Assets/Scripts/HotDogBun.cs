using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class HotDogBun : UdonSharpBehaviour
{
    public string saussageName;
    [HideInInspector] public bool isEatable;
    public Transform cookedSaussage;
    Grill grill;
    public bool pickedUp;
    [HideInInspector] public string lastTouched;
    ManualEventDispatcher MED;

    private void Start()
    {
        pickedUp = false;
        grill = GameObject.Find("Grill").GetComponent<Grill>();
        MED = grill.transform.GetComponent<ManualEventDispatcher>();
        cookedSaussage.gameObject.SetActive(false);
    }
    private void  OnCollisionEnter(Collision other)
    {
        if (Networking.GetOwner(other.gameObject) == Networking.LocalPlayer)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (other.gameObject.name.Length >= 8)
            {
                if (other.gameObject.name.Substring(0, 8) == saussageName)
                {
                    lastTouched = other.gameObject.name;
                    MED.hotDogBun = this.name;
                    MED.isHotDog = true;
                    MED.Sync();

                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //If the hotdog can be eaten
        if (isEatable)
        {
            //If touching mouth area
            if (other.gameObject.name == "MouthCollider")
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                SendCustomNetworkEvent(NetworkEventTarget.All,nameof(ResetHotDogBun));
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Disable));
            }
        }
    }
    public void ResetHotDogBun()
    {
        isEatable = false;
        cookedSaussage.gameObject.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
        pickedUp = false;

    }
    public void Disable()
    {
        GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        ((VRC_Pickup)GetComponent(typeof(VRC_Pickup))).Drop();

    }
    public void Enable()
    {
        GetComponent<BoxCollider>().enabled = true; 
        transform.GetChild(1).gameObject.SetActive(true);

    }
    public override void OnDrop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public override void OnPickup()
    {
        //First time being picked up, tell the grill to make another in its place
        if (!pickedUp)
        {

            grill.TakeItem("hotdogbun");
            GetComponent<Rigidbody>().isKinematic = false;
            pickedUp = true;
        }
    }
}


