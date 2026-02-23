using UdonSharp;
using UnityEngine;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;

// ReSharper disable once CheckNamespace
public class Keypad : UdonSharpBehaviour
{
    private readonly string AUTHOR = "Foorack";
    private readonly string VERSION = "3.6";

    [Header("Keypad")]
    public string solution = "2580";
    public GameObject doorObject = null;

    [Header("Access Lists")]
    public string[] allowList = new string[0];
    public string[] denyList = new string[0];
    public Collider allowListCollider;

    [Header("Remote Admin List")]
    public VRCUrl adminsURL;
    public TextAsset offlineAdminsJSON;
    public bool useOfflineAdmins = false;

    [Header("Audio")]
    public AudioSource soundDenied;
    public AudioSource soundGranted;
    public AudioSource soundButton;

    [Header("Display Text")]
    public string translationPasscode = "PASSCODE";
    public string translationDenied = "DENIED";
    public string translationGranted = "GRANTED";

    [Header("Behaviour")]
    public bool hideDoorOnGranted = true;
    public bool disableDebugging = false;

    public UdonBehaviour programClosed;
    public UdonBehaviour programDenied;
    public UdonBehaviour programGranted;

    public Text internalKeypadDisplay;

    [Header("Additional Doors")]
    public string[] additionalSolutions = new string[0];
    public GameObject[] additionalDoorObjects = new GameObject[0];
    public bool additionalKeySeparation = false;

    // runtime
    private string _buffer;
    private string[] _solutions;
    private GameObject[] _doors;

    private string _prefix;
    private bool adminsLoaded = false;

    private void Log(string msg)
    {
        if (!disableDebugging) Debug.Log(_prefix + msg);
    }

    private void LogWarning(string msg)
    {
        if (!disableDebugging) Debug.LogWarning(_prefix + msg);
    }

    public void Start()
    {
        _prefix = "[UdonKeypad] ";
        Log("Starting Keypad v" + VERSION);

        _buffer = "";

        if (internalKeypadDisplay == null)
        {
            Debug.LogError("Display missing!");
            return;
        }

        // Merge solutions & doors
        _solutions = new string[additionalSolutions.Length + 1];
        _doors = new GameObject[additionalDoorObjects.Length + 1];

        _solutions[0] = solution;
        _doors[0] = doorObject;

        for (int i = 0; i < additionalSolutions.Length; i++)
            _solutions[i + 1] = additionalSolutions[i];

        for (int i = 0; i < additionalDoorObjects.Length; i++)
            _doors[i + 1] = additionalDoorObjects[i];

        internalKeypadDisplay.text = translationPasscode;

        LoadAdminList();
    }

    // =========================
    // ADMIN LIST LOADING
    // =========================

    public void LoadAdminList()
    {
        if (useOfflineAdmins)
        {
            if (offlineAdminsJSON != null)
                ParseAdminJSON(offlineAdminsJSON.text);
            return;
        }

        // use offline first (instant)
        if (offlineAdminsJSON != null)
            ParseAdminJSON(offlineAdminsJSON.text);

        if (adminsURL != null)
            VRCStringDownloader.LoadUrl(adminsURL, (VRC.Udon.Common.Interfaces.IUdonEventReceiver)this);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        ParseAdminJSON(result.Result);
        Log("Remote admin list loaded");
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        LogWarning("Failed loading remote admin list");
    }

    private void ParseAdminJSON(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        json = json.Replace("[", "")
                   .Replace("]", "")
                   .Replace("\"", "")
                   .Replace("\n", "")
                   .Replace("\r", "");

        allowList = json.Split(',');

        adminsLoaded = true;
        UpdateAllowCollider();

        Log("Admins loaded count: " + allowList.Length);

        foreach (var entry in allowList)
        {
            Log("Admin: " + entry);
        }
    }

    private void UpdateAllowCollider()
    {
        if (allowListCollider == null) return;

        var username = Networking.LocalPlayer == null
            ? "UnityEditor"
            : Networking.LocalPlayer.displayName;

        bool allowed = false;

        foreach (var entry in allowList)
        {
            if (entry.Trim() == username)
            {
                allowed = true;
                break;
            }
        }

        allowListCollider.enabled = allowed;
    }

    // =========================
    // KEYPAD LOGIC
    // =========================

    private void CLR()
    {
        internalKeypadDisplay.text = translationPasscode;

        foreach (var door in _doors)
            if (door != gameObject)
                door.SetActive(hideDoorOnGranted);

        _buffer = "";
    }

    private void OK()
    {
        var username = Networking.LocalPlayer == null ? "UnityEditor" : Networking.LocalPlayer.displayName;

        bool isOnAllow = false;
        bool isOnDeny = false;

        foreach (var entry in allowList)
            if (entry.Trim() == username) isOnAllow = true;

        foreach (var entry in denyList)
            if (entry.Trim() == username) isOnDeny = true;

        bool isCorrect = false;
        GameObject correctDoor = null;

        for (int i = 0; i < _solutions.Length; i++)
        {
            if (_solutions[i] == _buffer)
            {
                isCorrect = true;
                if (i < _doors.Length)
                    correctDoor = _doors[i];
            }
        }

        if ((isCorrect && !isOnDeny) || isOnAllow)
        {
            internalKeypadDisplay.text = translationGranted;

            foreach (var door in _doors)
            {
                if (door == gameObject) continue;

                if (additionalKeySeparation && door != correctDoor)
                    door.SetActive(hideDoorOnGranted);
                else
                    door.SetActive(!hideDoorOnGranted);
            }

            if (soundGranted) soundGranted.Play();
        }
        else
        {
            internalKeypadDisplay.text = translationDenied;

            foreach (var door in _doors)
                if (door != gameObject)
                    door.SetActive(hideDoorOnGranted);

            if (soundDenied) soundDenied.Play();
        }

        _buffer = "";
    }

    private void PrintPassword()
    {
        string pass = "*";
        for (int i = 1; i < _buffer.Length; i++)
            pass += " *";

        internalKeypadDisplay.text = pass;
    }

    public void ButtonInput(string inputValue)
    {
        if (inputValue == "CLR")
        {
            CLR();
            return;
        }

        if (inputValue == "OK")
        {
            OK();
            return;
        }

        if (_buffer.Length >= 8) return;

        _buffer += inputValue;
        PrintPassword();

        if (soundButton) soundButton.Play();
    }
}