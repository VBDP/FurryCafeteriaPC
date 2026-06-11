
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Grill : UdonSharpBehaviour
{
    [Header("SpawnPoints")]
    public Transform burgerPoint;
    public Transform saussagePoint;
    public Transform hotdogBunPoint;
    public Transform burgerBunPoint;

    public GameObject[] allBurgers;
    public GameObject[] allSaussages;
    public GameObject[] allHotdogBuns;
    public GameObject[] allBurgerBuns;

    //Synced
    [HideInInspector] [UdonSynced] public int burgerIndex;
    [HideInInspector] [UdonSynced] public int saussageIndex;
    [HideInInspector] [UdonSynced] public int hotdogBunIndex;
    [HideInInspector] [UdonSynced] public int burgerBunIndex;

    private void Start()
    {
        transform.name = "Grill";
    }
    public void TakeItem(string name)
    {
        GameObject objectToMove = null;
        switch (name)
        {
            case ("burger"):
                //Selects earliest taken burger
                burgerIndex++;
                if (burgerIndex >= allBurgers.Length)
                    burgerIndex = 0;

                //Finds burger from array
                objectToMove = allBurgers[burgerIndex];
                Networking.SetOwner(Networking.LocalPlayer, objectToMove);
                //Moves burger back to start point
                objectToMove.transform.position = burgerPoint.position;
                objectToMove.transform.rotation = burgerPoint.rotation;

                RequestSerialization();
                break;

            case ("saussage"):
                saussageIndex++;
                if (saussageIndex >= allSaussages.Length)
                    saussageIndex = 0;
                objectToMove = allSaussages[saussageIndex];
                Networking.SetOwner(Networking.LocalPlayer, objectToMove);
                objectToMove.SetActive(true);
                objectToMove.transform.position = saussagePoint.position;
                objectToMove.transform.rotation = saussagePoint.rotation;
                RequestSerialization();
                break;

            case ("hotdogbun"):
                hotdogBunIndex++;
                if (hotdogBunIndex >= allHotdogBuns.Length)
                    hotdogBunIndex = 0;
                objectToMove = allHotdogBuns[hotdogBunIndex];
                Networking.SetOwner(Networking.LocalPlayer, objectToMove);
                objectToMove.transform.position = hotdogBunPoint.position;
                objectToMove.transform.rotation = hotdogBunPoint.rotation;
                RequestSerialization();
                break;

            case ("burgerbun"):
                burgerBunIndex++;
                if (burgerBunIndex >= allBurgerBuns.Length)
                    burgerBunIndex = 0;
                objectToMove = allBurgerBuns[burgerBunIndex];
                Networking.SetOwner(Networking.LocalPlayer, objectToMove);
                objectToMove.transform.position = burgerBunPoint.position;
                objectToMove.transform.rotation = burgerBunPoint.rotation;
                RequestSerialization();
                break;

        }
        if (name == "hotdogbun")
        {
            HotDogBun bun = objectToMove.GetComponent<HotDogBun>();
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, bun.gameObject);
            bun.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(bun.ResetHotDogBun));
            bun.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(bun.Enable));
        }
        else if (name == "burgerbun")
        {
            BurgerBun bun = objectToMove.GetComponent<BurgerBun>();
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, bun.gameObject);
            bun.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(bun.ResetBurgerBun));
            bun.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(bun.Enable));
        }
        else
        {
            Food food = objectToMove.GetComponent<Food>();
            food.pickedUp = false;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, food.gameObject);
            food.SendCustomNetworkEvent(NetworkEventTarget.All,nameof(food.ResetFood));
            food.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(food.Enable));
        }


    }
}
