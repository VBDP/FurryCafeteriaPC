
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
public class ManualEventDispatcher : UdonSharpBehaviour
{
    [HideInInspector] [UdonSynced] public bool isHotDog;
    [HideInInspector] [UdonSynced] public string _lastTouched;
    [HideInInspector] [UdonSynced] public string hotDogBun;
    [HideInInspector] [UdonSynced] public string burgerBun;
    [HideInInspector] [UdonSynced] public bool cooked;
    public void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (isHotDog) _lastTouched = GameObject.Find(hotDogBun).GetComponent<HotDogBun>().lastTouched;
        else _lastTouched = GameObject.Find(burgerBun).GetComponent<BurgerBun>().lastTouched;
        cooked = GameObject.Find(_lastTouched).GetComponent<Food>().isCooked;
        RequestSerialization();
        OnDeserialization();
    }

    public override void OnDeserialization() => AddMeat();

    private void Start() => HealthCheck();
    public void HealthCheck()
    {
        Debug.Log("Alive");
        SendCustomEventDelayedSeconds(nameof(HealthCheck), 1);
    }
    public void AddMeat()
    {
        if ((hotDogBun != null || burgerBun != null) && cooked)
        {
            GameObject lastTouchedObject=null;
            Debug.Log("Adding Meat");
            if (_lastTouched != null)
            {
                lastTouchedObject = GameObject.Find(_lastTouched);
            }
            if (isHotDog)
            {
                HotDogBun hdb = GameObject.Find(hotDogBun).GetComponent<HotDogBun>();

                lastTouchedObject.GetComponent<Food>().ResetFood();
                //Hide initial hot dog
                lastTouchedObject.GetComponent<Food>().Disable();

                //Show burger model in bun
                hdb.cookedSaussage.gameObject.SetActive(true);

                //Allow eating of bun
                hdb.isEatable = true;                
            }
            else
            {
                Debug.Log("3");
                Debug.Log("burgerBun:" + burgerBun);
                if (GameObject.Find(burgerBun).activeInHierarchy)
                {
                    BurgerBun bb = GameObject.Find(burgerBun).GetComponent<BurgerBun>();

                    Networking.SetOwner(Networking.LocalPlayer, bb.gameObject);
                    //Show burger model in bun
                    bb.cookedBurger.gameObject.SetActive(true);

                    //Close burger bun
                    bb.transform.GetChild(0).gameObject.SetActive(true);
                    bb.transform.GetChild(1).gameObject.SetActive(false);

                    //Allow eating of bun
                    bb.isEatable = true;
                }
                Debug.Log("4");
                //Hide initial burger

                lastTouchedObject.GetComponent<Food>().Disable();
                Debug.Log("5");


               
            }
        }
    }
}
