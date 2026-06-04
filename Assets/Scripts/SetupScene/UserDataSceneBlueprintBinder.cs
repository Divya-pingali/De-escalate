using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDataSceneBlueprintBinder : MonoBehaviour
{
    [Header("Manager Settings")]
    [SerializeField] private bool createManagerIfMissing = true;
    [SerializeField] private bool loadOnEnable = true;
    [SerializeField] private bool refreshFromPlayerPrefsBeforeLoading = true;
    [SerializeField] private bool doNotOverwriteWithEmptyData = true;

    [Header("Basic User Info")]
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField ageInput;
    [SerializeField] private TMP_Dropdown regionDropdown;
    [SerializeField] private TMP_InputField regionInput;

    [Header("Emergency Contact 1")]
    [SerializeField] private TMP_InputField emergencyNameInput;
    [SerializeField] private TMP_InputField emergencyContactInput;

    [Header("Emergency Contact 2")]
    [SerializeField] private TMP_InputField emergency2NameInput;
    [SerializeField] private TMP_InputField emergency2ContactInput;

    [Header("Phobia Profile")]
    [SerializeField] private TMP_InputField phobiaInput;
    [SerializeField] private TMP_InputField durationInput;
    [SerializeField] private TMP_InputField historyInput;
    [SerializeField] private TMP_InputField triggerInput;
    [SerializeField] private TMP_InputField medicalConditionInput;

    [Header("AI Settings")]
    [SerializeField] private TMP_Dropdown aiModeDropdown;
    [SerializeField] private TMP_Dropdown modelDropdown;
    [SerializeField] private TMP_Dropdown voiceDropdown;
    [SerializeField] private Toggle sendDataToggle;
    [SerializeField] private Toggle subtitlesToggle;

    [Header("Permissions")]
    [SerializeField] private Toggle cameraPermissionToggle;
    [SerializeField] private Toggle microphonePermissionToggle;
    [SerializeField] private Toggle contactEmergencyPermissionToggle;
    [SerializeField] private Toggle trackLocationPermissionToggle;

    private bool isLoading;

    private void OnEnable()
    {
        if (loadOnEnable)
        {
            LoadAllUIFromUserData();
        }
    }

    public void LoadAllUIFromUserData()
    {
        UserDataManager data = GetManager();

        if (data == null)
        {
            Debug.LogWarning("UserDataSceneBlueprintBinder: UserDataManager not found.");
            return;
        }

        if (refreshFromPlayerPrefsBeforeLoading)
        {
            data.LoadFromPlayerPrefs();
        }

        isLoading = true;

        SetInput(userNameInput, data.userName);
        SetInput(ageInput, data.age);
        SetInput(regionInput, data.region);
        SetDropdown(regionDropdown, data.region);

        SetInput(emergencyNameInput, data.emergencyName);
        SetInput(emergencyContactInput, data.emergencyContact);

        SetInput(emergency2NameInput, data.emergency2Name);
        SetInput(emergency2ContactInput, data.emergency2Contact);

        SetInput(phobiaInput, data.phobia);
        SetInput(durationInput, data.duration);
        SetInput(historyInput, data.history);
        SetInput(triggerInput, data.trigger);
        SetInput(medicalConditionInput, data.medicalCondition);

        SetDropdown(aiModeDropdown, data.aiMode);
        SetDropdown(modelDropdown, data.model);
        SetDropdown(voiceDropdown, data.voice);

        SetToggle(sendDataToggle, data.sendData);
        SetToggle(subtitlesToggle, data.subtitles);

        SetToggle(cameraPermissionToggle, data.cameraPermission);
        SetToggle(microphonePermissionToggle, data.microphonePermission);
        SetToggle(contactEmergencyPermissionToggle, data.contactEmergencyPermission);
        SetToggle(trackLocationPermissionToggle, data.trackLocationPermission);

        isLoading = false;
    }

    public void SaveAllCurrentUIValues()
    {
        UserDataManager data = GetManager();

        if (data == null)
        {
            Debug.LogWarning("UserDataSceneBlueprintBinder: UserDataManager not found.");
            return;
        }

        if (userNameInput != null) data.SetUserName(userNameInput.text);
        if (ageInput != null) data.SetAge(ageInput.text);

        if (regionInput != null) data.SetRegion(regionInput.text);
        if (regionDropdown != null) data.SetRegion(GetDropdownText(regionDropdown, regionDropdown.value));

        if (emergencyNameInput != null) data.SetEmergencyName(emergencyNameInput.text);
        if (emergencyContactInput != null) data.SetEmergencyContact(emergencyContactInput.text);

        if (emergency2NameInput != null) data.SetEmergency2Name(emergency2NameInput.text);
        if (emergency2ContactInput != null) data.SetEmergency2Contact(emergency2ContactInput.text);

        if (phobiaInput != null) data.SetPhobia(phobiaInput.text);
        if (durationInput != null) data.SetDuration(durationInput.text);
        if (historyInput != null) data.SetHistory(historyInput.text);
        if (triggerInput != null) data.SetTrigger(triggerInput.text);
        if (medicalConditionInput != null) data.SetMedicalCondition(medicalConditionInput.text);

        if (aiModeDropdown != null) data.SetAIMode(GetDropdownText(aiModeDropdown, aiModeDropdown.value));
        if (modelDropdown != null) data.SetModel(GetDropdownText(modelDropdown, modelDropdown.value));
        if (voiceDropdown != null) data.SetVoice(GetDropdownText(voiceDropdown, voiceDropdown.value));

        if (sendDataToggle != null) data.SetSendData(sendDataToggle.isOn);
        if (subtitlesToggle != null) data.SetSubtitles(subtitlesToggle.isOn);

        if (cameraPermissionToggle != null) data.SetCameraPermission(cameraPermissionToggle.isOn);
        if (microphonePermissionToggle != null) data.SetMicrophonePermission(microphonePermissionToggle.isOn);
        if (contactEmergencyPermissionToggle != null) data.SetContactEmergencyPermission(contactEmergencyPermissionToggle.isOn);
        if (trackLocationPermissionToggle != null) data.SetTrackLocationPermission(trackLocationPermissionToggle.isOn);

        data.SaveToPlayerPrefs();
    }

    // Input field event methods

    public void SaveUserName(string value)
    {
        if (isLoading) return;
        GetManager()?.SetUserName(value);
    }

    public void SaveAge(string value)
    {
        if (isLoading) return;
        GetManager()?.SetAge(value);
    }

    public void SaveRegion(string value)
    {
        if (isLoading) return;
        GetManager()?.SetRegion(value);
    }

    public void SaveEmergencyName(string value)
    {
        if (isLoading) return;
        GetManager()?.SetEmergencyName(value);
    }

    public void SaveEmergencyContact(string value)
    {
        if (isLoading) return;
        GetManager()?.SetEmergencyContact(value);
    }

    public void SaveEmergency2Name(string value)
    {
        if (isLoading) return;
        GetManager()?.SetEmergency2Name(value);
    }

    public void SaveEmergency2Contact(string value)
    {
        if (isLoading) return;
        GetManager()?.SetEmergency2Contact(value);
    }

    public void SavePhobia(string value)
    {
        if (isLoading) return;
        GetManager()?.SetPhobia(value);
    }

    public void SaveDuration(string value)
    {
        if (isLoading) return;
        GetManager()?.SetDuration(value);
    }

    public void SaveHistory(string value)
    {
        if (isLoading) return;
        GetManager()?.SetHistory(value);
    }

    public void SaveTrigger(string value)
    {
        if (isLoading) return;
        GetManager()?.SetTrigger(value);
    }

    public void SaveMedicalCondition(string value)
    {
        if (isLoading) return;
        GetManager()?.SetMedicalCondition(value);
    }

    // Dropdown event methods

    public void SaveRegionDropdown(int index)
    {
        if (isLoading) return;
        if (regionDropdown == null) return;

        GetManager()?.SetRegion(GetDropdownText(regionDropdown, index));
    }

    public void SaveAIModeDropdown(int index)
    {
        if (isLoading) return;
        if (aiModeDropdown == null) return;

        GetManager()?.SetAIMode(GetDropdownText(aiModeDropdown, index));
    }

    public void SaveModelDropdown(int index)
    {
        if (isLoading) return;
        if (modelDropdown == null) return;

        GetManager()?.SetModel(GetDropdownText(modelDropdown, index));
    }

    public void SaveVoiceDropdown(int index)
    {
        if (isLoading) return;
        if (voiceDropdown == null) return;

        GetManager()?.SetVoice(GetDropdownText(voiceDropdown, index));
    }

    // Toggle event methods

    public void SaveSendData(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetSendData(value);
    }

    public void SaveSubtitles(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetSubtitles(value);
    }

    public void SaveCameraPermission(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetCameraPermission(value);
    }

    public void SaveMicrophonePermission(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetMicrophonePermission(value);
    }

    public void SaveContactEmergencyPermission(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetContactEmergencyPermission(value);
    }

    public void SaveTrackLocationPermission(bool value)
    {
        if (isLoading) return;
        GetManager()?.SetTrackLocationPermission(value);
    }

    private UserDataManager GetManager()
    {
        if (UserDataManager.Instance != null)
        {
            return UserDataManager.Instance;
        }

        if (!createManagerIfMissing)
        {
            return null;
        }

        GameObject managerObject = new GameObject("UserDataManager");
        return managerObject.AddComponent<UserDataManager>();
    }

    private void SetInput(TMP_InputField inputField, string value)
    {
        if (inputField == null)
        {
            return;
        }

        if (doNotOverwriteWithEmptyData && string.IsNullOrEmpty(value))
        {
            return;
        }

        inputField.text = value;
    }

    private void SetToggle(Toggle toggle, bool value)
    {
        if (toggle == null)
        {
            return;
        }

        toggle.isOn = value;
    }

    private void SetDropdown(TMP_Dropdown dropdown, string savedValue)
    {
        if (dropdown == null)
        {
            return;
        }

        if (doNotOverwriteWithEmptyData && string.IsNullOrEmpty(savedValue))
        {
            return;
        }

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == savedValue)
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
                return;
            }
        }

        int savedIndex;

        if (int.TryParse(savedValue, out savedIndex))
        {
            if (savedIndex >= 0 && savedIndex < dropdown.options.Count)
            {
                dropdown.value = savedIndex;
                dropdown.RefreshShownValue();
            }
        }
    }

    private string GetDropdownText(TMP_Dropdown dropdown, int index)
    {
        if (dropdown == null)
        {
            return "";
        }

        if (index < 0 || index >= dropdown.options.Count)
        {
            return "";
        }

        return dropdown.options[index].text;
    }
}