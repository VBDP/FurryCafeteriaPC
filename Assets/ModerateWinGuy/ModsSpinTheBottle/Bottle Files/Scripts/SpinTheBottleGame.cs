using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using TMPro;
using System;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;

namespace SpinTheBottle
{
    public class SpinTheBottleGame : UdonSharpBehaviour
    {
        [Header("The URL of a text file to load all the prompts from")]
        public VRCUrl promptsLoadUrl;


        private bool loaded = false;
        private Transform root;
        private VRCPlayerApi playerLocal;
        private Animator anim;
        private int hashSpin;
        
        [Header("All Internal Configuration values below here")]
        [UdonSynced, FieldChangeCallback(nameof(SpinBottleCallback))] public byte SpinBottle;
        public TextMeshPro[] textMeshes;
        public TextMeshPro SelectedText;
        public TextMeshPro lastSpunBy;
        [UdonSynced]
        public int[] currentOptions = new int[8];

        [UdonSynced]
        public double bottleYRotation;


        private string[] textPrompts;

        public byte SpinBottleCallback
        {
            get => SpinBottle;
            set
            {
                SpinBottle = value;
                anim.SetTrigger(hashSpin);
            }
        }

        private void Start()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(2000,1,1);
            UnityEngine.Random.InitState(Convert.ToInt32(t.TotalSeconds));
            bottleYRotation = 0;
            VRCStringDownloader.LoadUrl(promptsLoadUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            textPrompts = result.Result.Split(new string[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            SetUp();
        }

        private void SetUp()
        {
            SelectedText.text = "";
            lastSpunBy.text = "";

            playerLocal = Networking.LocalPlayer;
            root = transform.parent;
            anim = root.GetComponent<Animator>();
            hashSpin = Animator.StringToHash("Spin");

            if (playerLocal.isMaster) // setup initial options
            {
                for (int i = 0; i < textMeshes.Length; i++)
                {
                    SetOptionToRandom(i);
                }
            }
            else
            { // load options if already initilized
                LoadOptions();
            }

            loaded = true;
            RequestSerialization();
        }

        public override void Interact()
        {
            SendCustomEventDelayedSeconds("UpdateText", 2.3f, VRC.Udon.Common.Enums.EventTiming.Update);
            RandomizedSelectedText();

            BottleInteractSpin();
            RequestSerialization();
        }

        public void UpdateText()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetHoverText");
        }


        private void LoadOptions()
        {
            for (int i = 0; i < textMeshes.Length; i++)
            {
                textMeshes[i].text = textPrompts[currentOptions[i]];
            }
        }

        private void SetOptionToRandom(int optionIndex)
        {
            int option = UnityEngine.Random.Range(0, textPrompts.Length);
            while (System.Array.IndexOf(currentOptions, option) != -1)
            {
                option = UnityEngine.Random.Range(0, textPrompts.Length);
            }
            textMeshes[optionIndex].text = textPrompts[option];
            currentOptions[optionIndex] = option;
        }

        private int GetSelectedOption()
        {
            float currentY = root.rotation.eulerAngles.y;
            Debug.Log($"CurrentY {root.rotation.eulerAngles.y}");
            int currentSlot = (int)(currentY / 45);

            return currentSlot;
        }

        private void RandomizedSelectedText()
        {
            int currentSlot = GetSelectedOption();

            Debug.Log($"currentslot {currentSlot}");

            SetOptionToRandom(currentSlot);
        }

        private void BottleInteractSpin()
        {
            //change the text the bottle is pointing at
            //Bottle spin and sync logic
            Networking.SetOwner(playerLocal, gameObject);
            Networking.SetOwner(playerLocal, root.gameObject);
            bottleYRotation = UnityEngine.Random.Range(0, 360);
            root.rotation = Quaternion.Euler(0, (float)bottleYRotation, 0);
            SpinBottleCallback++;
            if (SpinBottleCallback > 250) SpinBottleCallback = 0;

            RequestSerialization();

            SetHoverText();
        }

        public void SetHoverText()
        {
            int currentSlot = GetSelectedOption();
            SelectedText.text = textPrompts[currentOptions[currentSlot]];
            lastSpunBy.text = $"Spun by: {Networking.GetOwner(gameObject).displayName}";
        }

        public override void OnDeserialization()
        {
            root.rotation = Quaternion.Euler(0, (float)bottleYRotation, 0);
            if (!loaded) return;            
            LoadOptions();
            SetHoverText();
        }
    }
}