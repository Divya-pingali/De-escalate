using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }

    [Header("Basic User Info")]
    public string userName;
    public string age;
    public string region;

    [Header("Emergency Contact 1")]
    public string emergencyName;
    public string emergencyContact;

    [Header("Emergency Contact 2")]
    public string emergency2Name;
    public string emergency2Contact;

    [Header("Phobia Profile")]
    public string phobia;
    public string duration;
    public string history;
    public string trigger;
    public string medicalCondition;

    [Header("AI Settings")]
    public string aiMode;
    public string model;
    public string voice;
    public bool sendData;
    public bool subtitles;

    [Header("Permissions")]
    public bool cameraPermission;
    public bool microphonePermission;
    public bool contactEmergencyPermission;
    public bool trackLocationPermission;

    private const string Prefix = "UserData_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPlayerPrefs();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveToPlayerPrefs();
        }
    }

    private void OnApplicationQuit()
    {
        SaveToPlayerPrefs();
    }

    public void LoadFromPlayerPrefs()
    {
        userName = PlayerPrefs.GetString(Prefix + "UserName", "");
        age = PlayerPrefs.GetString(Prefix + "Age", "");
        region = PlayerPrefs.GetString(Prefix + "Region", "");

        emergencyName = PlayerPrefs.GetString(Prefix + "EmergencyName", "");
        emergencyContact = PlayerPrefs.GetString(Prefix + "EmergencyContact", "");

        emergency2Name = PlayerPrefs.GetString(Prefix + "Emergency2Name", "");
        emergency2Contact = PlayerPrefs.GetString(Prefix + "Emergency2Contact", "");

        phobia = PlayerPrefs.GetString(Prefix + "Phobia", "");
        duration = PlayerPrefs.GetString(Prefix + "Duration", "");
        history = PlayerPrefs.GetString(Prefix + "History", "");
        trigger = PlayerPrefs.GetString(Prefix + "Trigger", "");
        medicalCondition = PlayerPrefs.GetString(Prefix + "MedicalCondition", "");

        aiMode = PlayerPrefs.GetString(Prefix + "AIMode", "");
        model = PlayerPrefs.GetString(Prefix + "Model", "");
        voice = PlayerPrefs.GetString(Prefix + "Voice", "");

        sendData = PlayerPrefs.GetInt(Prefix + "SendData", 0) == 1;
        subtitles = PlayerPrefs.GetInt(Prefix + "Subtitles", 0) == 1;

        cameraPermission = PlayerPrefs.GetInt(Prefix + "CameraPermission", 0) == 1;
        microphonePermission = PlayerPrefs.GetInt(Prefix + "MicrophonePermission", 0) == 1;
        contactEmergencyPermission = PlayerPrefs.GetInt(Prefix + "ContactEmergencyPermission", 0) == 1;
        trackLocationPermission = PlayerPrefs.GetInt(Prefix + "TrackLocationPermission", 0) == 1;

        Debug.Log("UserDataManager: Loaded from PlayerPrefs.");
    }

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString(Prefix + "UserName", SafeString(userName));
        PlayerPrefs.SetString(Prefix + "Age", SafeString(age));
        PlayerPrefs.SetString(Prefix + "Region", SafeString(region));

        PlayerPrefs.SetString(Prefix + "EmergencyName", SafeString(emergencyName));
        PlayerPrefs.SetString(Prefix + "EmergencyContact", SafeString(emergencyContact));

        PlayerPrefs.SetString(Prefix + "Emergency2Name", SafeString(emergency2Name));
        PlayerPrefs.SetString(Prefix + "Emergency2Contact", SafeString(emergency2Contact));

        PlayerPrefs.SetString(Prefix + "Phobia", SafeString(phobia));
        PlayerPrefs.SetString(Prefix + "Duration", SafeString(duration));
        PlayerPrefs.SetString(Prefix + "History", SafeString(history));
        PlayerPrefs.SetString(Prefix + "Trigger", SafeString(trigger));
        PlayerPrefs.SetString(Prefix + "MedicalCondition", SafeString(medicalCondition));

        PlayerPrefs.SetString(Prefix + "AIMode", SafeString(aiMode));
        PlayerPrefs.SetString(Prefix + "Model", SafeString(model));
        PlayerPrefs.SetString(Prefix + "Voice", SafeString(voice));

        PlayerPrefs.SetInt(Prefix + "SendData", sendData ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "Subtitles", subtitles ? 1 : 0);

        PlayerPrefs.SetInt(Prefix + "CameraPermission", cameraPermission ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "MicrophonePermission", microphonePermission ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "ContactEmergencyPermission", contactEmergencyPermission ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "TrackLocationPermission", trackLocationPermission ? 1 : 0);

        PlayerPrefs.Save();

        Debug.Log("UserDataManager: Saved to PlayerPrefs.");
    }

    private string SafeString(string value)
    {
        return value == null ? "" : value;
    }

    // Basic User Info

    public void SetUserName(string value)
    {
        userName = SafeString(value);
        PlayerPrefs.SetString(Prefix + "UserName", userName);
        PlayerPrefs.Save();
    }

    public void SetAge(string value)
    {
        age = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Age", age);
        PlayerPrefs.Save();
    }

    public void SetRegion(string value)
    {
        region = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Region", region);
        PlayerPrefs.Save();
    }

    // Emergency Contact 1

    public void SetEmergencyName(string value)
    {
        emergencyName = SafeString(value);
        PlayerPrefs.SetString(Prefix + "EmergencyName", emergencyName);
        PlayerPrefs.Save();
    }

    public void SetEmergencyContact(string value)
    {
        emergencyContact = SafeString(value);
        PlayerPrefs.SetString(Prefix + "EmergencyContact", emergencyContact);
        PlayerPrefs.Save();
    }

    // Emergency Contact 2

    public void SetEmergency2Name(string value)
    {
        emergency2Name = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Emergency2Name", emergency2Name);
        PlayerPrefs.Save();
    }

    public void SetEmergency2Contact(string value)
    {
        emergency2Contact = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Emergency2Contact", emergency2Contact);
        PlayerPrefs.Save();
    }

    // Phobia Profile

    public void SetPhobia(string value)
    {
        phobia = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Phobia", phobia);
        PlayerPrefs.Save();
    }

    public void SetDuration(string value)
    {
        duration = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Duration", duration);
        PlayerPrefs.Save();
    }

    public void SetHistory(string value)
    {
        history = SafeString(value);
        PlayerPrefs.SetString(Prefix + "History", history);
        PlayerPrefs.Save();
    }

    public void SetTrigger(string value)
    {
        trigger = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Trigger", trigger);
        PlayerPrefs.Save();
    }

    public void SetMedicalCondition(string value)
    {
        medicalCondition = SafeString(value);
        PlayerPrefs.SetString(Prefix + "MedicalCondition", medicalCondition);
        PlayerPrefs.Save();
    }

    // AI Settings

    public void SetAIMode(string value)
    {
        aiMode = SafeString(value);
        PlayerPrefs.SetString(Prefix + "AIMode", aiMode);
        PlayerPrefs.Save();
    }

    public void SetModel(string value)
    {
        model = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Model", model);
        PlayerPrefs.Save();
    }

    public void SetVoice(string value)
    {
        voice = SafeString(value);
        PlayerPrefs.SetString(Prefix + "Voice", voice);
        PlayerPrefs.Save();
    }

    public void SetSendData(bool value)
    {
        sendData = value;
        PlayerPrefs.SetInt(Prefix + "SendData", sendData ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSubtitles(bool value)
    {
        subtitles = value;
        PlayerPrefs.SetInt(Prefix + "Subtitles", subtitles ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Permissions

    public void SetCameraPermission(bool value)
    {
        cameraPermission = value;
        PlayerPrefs.SetInt(Prefix + "CameraPermission", cameraPermission ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMicrophonePermission(bool value)
    {
        microphonePermission = value;
        PlayerPrefs.SetInt(Prefix + "MicrophonePermission", microphonePermission ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetContactEmergencyPermission(bool value)
    {
        contactEmergencyPermission = value;
        PlayerPrefs.SetInt(Prefix + "ContactEmergencyPermission", contactEmergencyPermission ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetTrackLocationPermission(bool value)
    {
        trackLocationPermission = value;
        PlayerPrefs.SetInt(Prefix + "TrackLocationPermission", trackLocationPermission ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ClearAllDataAndSave()
    {
        userName = "";
        age = "";
        region = "";

        emergencyName = "";
        emergencyContact = "";

        emergency2Name = "";
        emergency2Contact = "";

        phobia = "";
        duration = "";
        history = "";
        trigger = "";
        medicalCondition = "";

        aiMode = "";
        model = "";
        voice = "";

        sendData = false;
        subtitles = false;

        cameraPermission = false;
        microphonePermission = false;
        contactEmergencyPermission = false;
        trackLocationPermission = false;

        SaveToPlayerPrefs();
    }

    public void DeleteSavedData()
    {
        PlayerPrefs.DeleteKey(Prefix + "UserName");
        PlayerPrefs.DeleteKey(Prefix + "Age");
        PlayerPrefs.DeleteKey(Prefix + "Region");

        PlayerPrefs.DeleteKey(Prefix + "EmergencyName");
        PlayerPrefs.DeleteKey(Prefix + "EmergencyContact");

        PlayerPrefs.DeleteKey(Prefix + "Emergency2Name");
        PlayerPrefs.DeleteKey(Prefix + "Emergency2Contact");

        PlayerPrefs.DeleteKey(Prefix + "Phobia");
        PlayerPrefs.DeleteKey(Prefix + "Duration");
        PlayerPrefs.DeleteKey(Prefix + "History");
        PlayerPrefs.DeleteKey(Prefix + "Trigger");
        PlayerPrefs.DeleteKey(Prefix + "MedicalCondition");

        PlayerPrefs.DeleteKey(Prefix + "AIMode");
        PlayerPrefs.DeleteKey(Prefix + "Model");
        PlayerPrefs.DeleteKey(Prefix + "Voice");

        PlayerPrefs.DeleteKey(Prefix + "SendData");
        PlayerPrefs.DeleteKey(Prefix + "Subtitles");

        PlayerPrefs.DeleteKey(Prefix + "CameraPermission");
        PlayerPrefs.DeleteKey(Prefix + "MicrophonePermission");
        PlayerPrefs.DeleteKey(Prefix + "ContactEmergencyPermission");
        PlayerPrefs.DeleteKey(Prefix + "TrackLocationPermission");

        PlayerPrefs.Save();

        ClearAllDataAndSave();

        Debug.Log("UserDataManager: Deleted saved data.");
    }
}