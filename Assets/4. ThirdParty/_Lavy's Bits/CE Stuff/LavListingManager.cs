
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LavListingManager : UdonSharpBehaviour
{
    [Header("LISTING ID, not to be confused with products")]
    public string listingId;

    [Header("Group ID")]
    public string groupId;

    [Header("All Access Pass File")]
    public UdonProduct allAccessPass;

    bool ownedProduct = false;

    bool isAlive = false;

    [Header("Objects to turn on if they have the pass")]
    public GameObject[] toOn;
    [Header("Objects to turn off if they have the pass")]
    public GameObject[] toOff;

    [Header("Updatable Zone Description")]
    public TextMeshProUGUI linkloading;

    public VRCUrl url;

    private void Start()
    {
        isAlive = Utilities.IsValid(allAccessPass);
        VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        _ToggleStuff();
    }
    public override void OnPurchasesLoaded(IProduct[] products, VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        ownedProduct = Store.DoesPlayerOwnProduct(Networking.LocalPlayer, allAccessPass);
        _ToggleStuff();
    }

    public void _OpenGroupPage()
    {
        if (!isAlive) return;

        Store.OpenGroupPage(groupId);
    }

    public void _DisplayListing()
    {
        if (!isAlive) return;

        Store.OpenGroupListing(listingId);
    }

    public override void OnPurchaseConfirmed(IProduct product, VRCPlayerApi player, bool purchasedNow)
    {
        if (!player.isLocal) return;

        ownedProduct = true;
        _ToggleStuff();
    }

    public void _ToggleStuff()
    {
        if (ownedProduct)
        {
            foreach (GameObject a in toOn)
            {
                a.SetActive(true);
            }

            foreach (GameObject b in toOff)
            {
                b.SetActive(false);
            }
        } else
        {
            foreach (GameObject a in toOn)
            {
                a.SetActive(false);
            }

            foreach (GameObject b in toOff)
            {
                b.SetActive(true);
            }
        }
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        linkloading.text = result.Result;
    }
}
