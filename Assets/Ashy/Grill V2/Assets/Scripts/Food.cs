
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Food : UdonSharpBehaviour
{
    [Header("References")]
    public GameObject smoke;
    public GameObject CookedModel;
    public GameObject RawModel;

    [Header("Parameters")]
    public float cookingTime;
    public string itemType;

    //Private
    GameObject grill;
    [HideInInspector] [UdonSynced] public bool isCooked;
    float timeCooking;

    //Hidden
    [HideInInspector] public bool pickedUp;

    private void OnTriggerStay(Collider other)
    {
        if (!isCooked)
        {
            //If on the grill
            if (other.gameObject.name == "GrillArea")
            {
                smoke.SetActive(true);
                if (timeCooking >= cookingTime)
                {
                    //When cooked, stop smoking and show cooked model
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                    SendCustomNetworkEvent(NetworkEventTarget.All, nameof(FinishedCooking));
                }
                else
                {
                    //Increase timer for how long it has been cooking
                    timeCooking += Time.deltaTime;

                    //Make smoke always go upwards
                    smoke.transform.rotation = Quaternion.Euler(-90f, 0, 0);
                }
            }
        }
    }
    public void FinishedCooking()
    {
        smoke.SetActive(false);
        CookedModel.SetActive(true);
        RawModel.SetActive(false);
        isCooked = true;
    }

    private void OnTriggerExit(Collider other)
    {
        smoke.SetActive(false);
    }
    private void Start()
    {
        pickedUp = false;
        grill = GameObject.Find("Grill");
        if (grill.name == null)
            Debug.LogError("Grill must be called 'Grill' not 'Grill(0)' etc...");
    }
    public override void OnPickup()
    {
        //First time being picked up, tell the grill to make another in its place
        if (!pickedUp)
        {
            grill.GetComponent<Grill>().TakeItem(itemType);
            pickedUp = true;
            GetComponent<Rigidbody>().isKinematic = false;

        }
    }


    public void Disable()
    {
        GetComponent<BoxCollider>().enabled = false;
        RawModel.SetActive(false);
        CookedModel.SetActive(false);
        ((VRC_Pickup)GetComponent(typeof(VRC_Pickup))).Drop();
    }
    public void Enable()
    {
        Debug.Log("Enabling");
        GetComponent<BoxCollider>().enabled = true;
        RawModel.SetActive(true);
    }
    public override void OnDrop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public void ResetFood()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        smoke.SetActive(false);
        CookedModel.SetActive(false);
        RawModel.SetActive(true);
        isCooked = false;
        timeCooking = 0;
        pickedUp = false;
    }
    private void Update()
    {
        //If fall off map, teleport back to grill
        if (transform.position.y < -205f)
            transform.position = grill.transform.position;
    }


}
