
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class BurgerBun : UdonSharpBehaviour
{
    public string burgerName;
    [HideInInspector] [UdonSynced] public bool isEatable;
    public Transform cookedBurger;
    public bool pickedUp;
    Grill grill;
    [HideInInspector] public string lastTouched;
    ManualEventDispatcher MED;

    private void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.name.Length >= 6||other.gameObject.name.Length==10))
        {
            if (other.gameObject.name.Substring(0, 6) == burgerName)
            {
                if (other.gameObject.name.Length >= 9)  if (other.gameObject.name.Substring(0, 9) == "BurgerBun") return;

                
                Debug.Log("Accepted as burger");
                lastTouched = other.gameObject.name;
                MED.burgerBun = this.name;
                MED.isHotDog = false;
                MED.Sync();
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
                SendCustomNetworkEvent(NetworkEventTarget.All,nameof(ResetBurgerBun));
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Disable));
            }
        }
    }

    public void ResetBurgerBun()
    {
        isEatable = false;
        cookedBurger.gameObject.SetActive(false);
        pickedUp = false;
        //Open burger bun
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        GetComponent<Rigidbody>().isKinematic = true;

    }
    public void Disable()
    {
        GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        ((VRC_Pickup) GetComponent(typeof(VRC_Pickup))).Drop();
    }
    public void Enable()
    {
        GetComponent<BoxCollider>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);

    }

    public override void OnPickup()
    {
        //First time being picked up, tell the grill to make another in its place
        if (!pickedUp)
        {
            grill.TakeItem("burgerbun");
            GetComponent<Rigidbody>().isKinematic = false;
            pickedUp = true;
        }
    }
    public override void OnDrop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    private void Start()
    {
        pickedUp = false;
        grill = GameObject.Find("Grill").GetComponent<Grill>();
        MED = grill.transform.GetComponent<ManualEventDispatcher>();
    }

}
