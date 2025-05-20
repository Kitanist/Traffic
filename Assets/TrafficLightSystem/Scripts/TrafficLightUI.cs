using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TrafficLightUI : MonoBehaviour
{
    #region Variables 
    public TrafficLightsController _targetLight;
    public TrafficLightsController TargetLight
    {
        get => _targetLight;
        set
        {
            if (_targetLight != value)
            {
                _targetLight = value;
                OnTargetLightChanged();
            }
        }
    }
    public YayaController _targetYaya;
    public YayaController TargetYaya
    {
        get => _targetYaya;
        set
        {
            if (_targetYaya != value)
            {
                _targetYaya = value;
                OnTargetYayaChanged();
            }
        }
    }
    private void OnTargetYayaChanged()
    {
        Debug.Log("Yeni yaya seçildi. Tüm ışıklar taranıyor.");
       
    }
    public void ConnectYayaToSelectedLight()
    {
        if (TargetYaya != null)
        {
            
            if (_targetLight != null)
            {
                
                TargetYaya.SetTarget();
            }
        }
    } 
    private void OnTargetLightChanged()
    {
        if (_targetLight == null) return;
        
        SingleLightID.text = "Işık ID:  " + TargetLight.gameObject.name;
        if (TargetLight.IsFirstTimeClicked())
        {
            _targetLight.SetFirstTimeClicked(false);
            nightDropDown.value = 4;
            nightDropDown.RefreshShownValue();
            SetIsAutoDropdownNight(4);
            stateDropdown.value = 8;
            stateDropdown.RefreshShownValue();
            SetIsAutoDropdown(8);
           // TargetLight.RedCircleClose();
            if (!TargetLight)
                return;
            if (!intersectionController) 
                return;
            if (intersectionController.IsLightInAnyGroup(TargetLight))
            {
                nightDropDown.interactable = false;
                stateDropdown.interactable = false;
            }
            nightGreenTime.interactable = false;
            nightRedPreHoldTime.interactable = false;
            nightRedTime.interactable = false;
            nightYellowTime.interactable = false;
            GreenTime.interactable = false;
            YellowTime.interactable = false;
            RedPreHoldTime.interactable = false;
            RedTime.interactable = false;
        }
        else
        {
            //   Debug.LogError("sa");
          //  if (TargetLight.YayaLights.Count>0)
          //  {
          //      Debug.Log("Yayam var bakalım yayam bana mı bağlı");
          //      foreach (YayaController Yayalight in TargetLight.YayaLights)
          //      {
          //          if (Yayalight == TargetYaya)
          //          {
          //              TargetLight.RedCircleOpen();
          //              Debug.Log("Evet Bana Bağlıymış");
          //              break;
          //          }
          //          else
          //          {
          //              TargetLight.RedCircleClose();
          //          }
          //      }
          //
          //  }
          //  else
          //  {
          //      Debug.Log("Yayam Yok");
          //      TargetLight.RedCircleClose();   
          //  }
            var a = TargetLight.CurrentState;
            var b = TargetLight.CurrentNightState;
            if (intersectionController.IsLightInAnyGroup(TargetLight))
            {
                SetIsAutoDropdown(8);
                stateDropdown.value = 8;
                SetIsAutoDropdownNight(4);
                nightDropDown.value = 4;
                return;
            }
            switch (a)
            {
                case TrafficLightState.Red:
                    SetIsAutoDropdown(0);
                    stateDropdown.value = 0;
                    break;
                case TrafficLightState.Yellow:
                    SetIsAutoDropdown(1);
                    stateDropdown.value = 1;

                    break;
                case TrafficLightState.TempYellow:
                    break;
                case TrafficLightState.Green:
                    SetIsAutoDropdown(2);
                    stateDropdown.value = 2;

                    break;
                case TrafficLightState.FlashingRed1s:
                    SetIsAutoDropdown(4);
                    stateDropdown.value = 4;

                    break;
                case TrafficLightState.FlashingYellow1s:
                    SetIsAutoDropdown(5);
                    stateDropdown.value = 5;

                    break;
                case TrafficLightState.FlashingRed2s:
                    SetIsAutoDropdown(6);
                    stateDropdown.value = 6;

                    break;
                case TrafficLightState.FlashingYellow2s:
                    SetIsAutoDropdown(7);
                    stateDropdown.value = 7;

                    break;
                case TrafficLightState.ManualHold:
                    break;
                case TrafficLightState.EmergencyStop:
                    break;
                case TrafficLightState.IsAuto:
                    SetIsAutoDropdown(3);
                    stateDropdown.value = 3;

                    break;
                case TrafficLightState.Disable:
                    SetIsAutoDropdown(8);
                    stateDropdown.value = 8;

                    break;
                default:
                    break;
            }
            switch (b)
            {
                case NightModeState.Red:
                    SetIsAutoDropdownNight(0);
                    nightDropDown.value = 0;


                    break;
                case NightModeState.Yellow:
                    SetIsAutoDropdownNight(1);
                    nightDropDown.value = 1;

                    break;
                case NightModeState.Green:
                    SetIsAutoDropdownNight(2);
                    nightDropDown.value = 2;

                    break;
                case NightModeState.IsAuto:
                    SetIsAutoDropdownNight(3);
                    nightDropDown.value = 3;

                    break;
                case NightModeState.Disable:
                    SetIsAutoDropdownNight(4);
                    nightDropDown.value = 4;

                    break;
                case NightModeState.FlashingRed1s:
                    SetIsAutoDropdown(4);
                    nightDropDown.value = 4;

                    break;
                case NightModeState.FlashingYellow1s:
                    SetIsAutoDropdown(5);
                    nightDropDown.value = 5;

                    break;
                case NightModeState.FlashingRed2s:
                    SetIsAutoDropdown(6);
                    nightDropDown.value = 6;

                    break;
                case NightModeState.FlashingYellow2s:
                    SetIsAutoDropdown(7);
                    nightDropDown.value = 7;

                    break;
                default:
                    break;
            }
            nightDropDown.RefreshShownValue();
            stateDropdown.RefreshShownValue();
        }
      //  Debug.Log("Target light değişti!");
        
    }

    public int TimeValue
    {
        get
        {
            return timeValue;
        }
        set
        {
            timeValue = value;
            ApplySelectedState();
        }
    }
    public int TimeValueSetter;
    public GameObject YayaUI;
    public PedestrianLightSpawner PLSpawner;
    public Button PLSpawnerButton;
    public TMP_Dropdown YayaStateDropdown,YayaBindingTypeDropdown;

    [Header("Single UI Elements")]
    public GameObject CanvasTrafficLightSingle;
    public TMP_Dropdown stateDropdown;
    public TMP_InputField RedTime;
    public TMP_InputField GreenTime;
    public TMP_InputField YellowTime;
    public TMP_InputField RedPreHoldTime;
    public Button applyButton;
    public Button emergencyButton;
    public Button RemoveFromGroup;
    public TextMeshProUGUI SingleLightID;

    [Header("Night UI Elements")]
    public TMP_Dropdown nightDropDown;
    public TMP_InputField nightRedTime;
    public TMP_InputField nightGreenTime;
    public TMP_InputField nightYellowTime;
    public TMP_InputField nightRedPreHoldTime;

    [Header("Intersection UI Elements")]
    public IntersectionController intersectionController;
    public TMP_Dropdown groupDropdown; // 👈 Grup seçimi için dropdown
    public TextMeshProUGUI IntersectionName;
    public TMP_InputField groupNameInput;
    public TrafficGroupUIManager trafficGroupUIManager;
    public Button deleteGroupButton;
    public TextMeshProUGUI DeleteConfirmDialog;
    public TMP_Dropdown groupLights;
    private int timeValue = 0;
    private bool isNightTime = false, a = false;
    private bool isDisable = false;
    public TMP_Dropdown GroupLightsDropdown;
    #endregion

    #region Monobehavior Funcs
    private void Start()
    {
        if (TargetLight)
            CanvasTrafficLightSingle.SetActive(true);
        else CanvasTrafficLightSingle.SetActive(false);
        TimeValue = TimeValueSetter;
        SetupDropdowns();
        SetupGroupDropdown();
        applyButton.onClick.AddListener(ApplySelectedState);
        YayaBindingTypeDropdown.onValueChanged.AddListener(OnYayaBindingDropdownValueChanged);
        stateDropdown.onValueChanged.AddListener(SetIsAutoDropdown);
        nightDropDown.onValueChanged.AddListener(SetIsAutoDropdownNight);
        PLSpawnerButton.onClick.AddListener(StartPLSpawn);
        groupDropdown.onValueChanged.AddListener(OnGroupSelected);
        groupNameInput.onEndEdit.AddListener(OnGroupNameChanged);
        
    }
    private void Update()
    {
        if (TargetLight&&!a)
        {
            a = true;
            Debug.LogWarning(a);
            CanvasTrafficLightSingle.SetActive(true);
        }
    }
    #endregion

    #region Single Light Funcs
    public void SetIsAutoDropdown(int a)
    {
        
        if (a == 3) 
        {
            GreenTime.interactable = true;
            YellowTime.interactable = true;
            RedPreHoldTime.interactable = true;
            RedTime.interactable = true;
            FixInvalidInput(GreenTime);
            FixInvalidInput(YellowTime);
            FixInvalidInput(RedPreHoldTime);
            FixInvalidInput(RedTime);

        }
        else
        {
            GreenTime.interactable = false;
            YellowTime.interactable = false;
            RedPreHoldTime.interactable = false;
            RedTime.interactable = false;
        }
        
    }

    public void OpenUI()
    {
        CanvasTrafficLightSingle.SetActive(true);
        trafficGroupUIManager.UI.SetActive(false);
    }
    public void CloseUI()
    {
        CanvasTrafficLightSingle.SetActive(false);
        trafficGroupUIManager.UI.SetActive(false);
    }
    public string ShowID()
    {
        return TargetLight.gameObject.name;
    }
    public void SetIsAutoDropdownNight(int a)
    {
        if (a == 3)
        {
            nightGreenTime.interactable = true;
            nightRedPreHoldTime.interactable = true;
            nightRedTime.interactable = true;
            nightYellowTime.interactable = true;
            FixInvalidInput(nightRedTime);
            FixInvalidInput(nightGreenTime);
            FixInvalidInput(nightYellowTime);
            FixInvalidInput(nightRedPreHoldTime);
        }
        else 
        {
            nightGreenTime.interactable = false;
            nightRedPreHoldTime.interactable = false;
            nightRedTime.interactable = false;
            nightYellowTime.interactable = false;
        }
        if (a == 4)
        {
            isDisable = true;
        }
        else isDisable = false;

    }
    void SetupDropdowns()
    { 
        stateDropdown.ClearOptions();
        stateDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Red", "Yellow", "Green", "IsAuto","FlashingRed1s", "FlashingYellow1s","FlashingRed2s","FlashingYellow2s","Disable",
        });
        nightDropDown.ClearOptions();
        nightDropDown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Red", "Yellow", "Green", "IsAuto","Disable","FlashingRed1s", "FlashingYellow1s","FlashingRed2s","FlashingYellow2s",
        });
        nightDropDown.value = 4;
        nightDropDown.RefreshShownValue();
        SetIsAutoDropdownNight(4);
        stateDropdown.value = 8;
        stateDropdown.RefreshShownValue();
        SetIsAutoDropdown(8);
        
        if (!TargetLight)
            return;
        if (intersectionController.IsLightInAnyGroup(TargetLight))
        {
            nightDropDown.interactable = false;
            stateDropdown.interactable = false;
        }
    }

    void FixInvalidInput(TMP_InputField input)
    {
        if (!float.TryParse(input.text, out float value) || value <= 0f)
        {
            input.text = "1";
        }
    }
    void ApplySelectedState()
    {
       // Debug.Log("Asama1");
        if (TargetLight == null) return;
       // Debug.Log("Asama2");

        if (!intersectionController) return;
       // Debug.Log("Asama3");

        if (intersectionController.IsLightInAnyGroup(TargetLight))
        {
            nightDropDown.interactable = false;
            stateDropdown.interactable = false;
            return;
        }
        else
        {
            nightDropDown.interactable = true;
            stateDropdown.interactable = true;
        }
        string selectedNight = nightDropDown.options[nightDropDown.value].text;
        if (System.Enum.TryParse(selectedNight, out NightModeState parsedNight))
        {
            TargetLight.OverrideStateNight(parsedNight);//disable için buraya girip içeriyi dağıt

            if (TargetLight.CurrentNightState != NightModeState.Disable)
            {
                TargetLight.SetNight(timeValue <= 6);
            }
        }
        string selected = stateDropdown.options[stateDropdown.value].text;
        if (System.Enum.TryParse(selected, out TrafficLightState parsed))
        {
            if (TargetLight.CurrentNightState != NightModeState.Disable)
            {
                TargetLight.SetNight(timeValue <= 6);
            }
            else TargetLight.SetNight(false);
            TargetLight.OverrideState(parsed);// disable için buraya girip içeriyi dağıt
        }

        if (TargetLight.CurrentState == TrafficLightState.IsAuto)
        {

            GreenTime.interactable = true;
            YellowTime.interactable = true;
            RedPreHoldTime.interactable = true;
            RedTime.interactable = true;

                // Gece ve Gece modu aktifse
            if (timeValue <= 6 && !isDisable)
            {
                float.TryParse(nightRedTime.text, out float nightRed);
                float.TryParse(nightGreenTime.text, out float nightGreen);
                float.TryParse(nightYellowTime.text, out float nightYellow);
                float.TryParse(nightRedPreHoldTime.text, out float nightRedPreHold);

                TargetLight.redDuration = nightRed;
                TargetLight.greenDuration = nightGreen;
                TargetLight.yellowDuration = nightYellow;
                TargetLight.preHoldDuration = nightRedPreHold;
                Debug.Log($"oTO AMA DİSABLE DEİL VE GECE  SAAT :{timeValue}  MOD : {parsedNight}");
                TargetLight.SetNight(true);
                FixInvalidInput(nightRedTime);
                FixInvalidInput(nightGreenTime);
                FixInvalidInput(nightYellowTime);
                FixInvalidInput(nightRedPreHoldTime);

            }
            //gunduz yada gece modu disable
            else
            {
                float.TryParse(RedTime.text, out float Red);
                float.TryParse(GreenTime.text, out float Green);
                float.TryParse(YellowTime.text, out float Yellow);
                float.TryParse(RedPreHoldTime.text, out float RedPreHold);

                TargetLight.redDuration = Red;
                TargetLight.greenDuration = Green;
                TargetLight.yellowDuration = Yellow;
                TargetLight.preHoldDuration = RedPreHold;
                TargetLight.SetNight(false);

                TargetLight.SetAutoMode(true);// interaktifleri aç
               // Debug.Log("Yarabbişukur");
               // Debug.LogError($"TimeValue : {timeValue}");
                FixInvalidInput(nightRedTime);
                FixInvalidInput(nightGreenTime);
                FixInvalidInput(nightYellowTime);
                FixInvalidInput(nightRedPreHoldTime);

            }

        }
        else
        {

            GreenTime.interactable = false;
            YellowTime.interactable = false;
            RedPreHoldTime.interactable = false;
            RedTime.interactable = false;

            if (timeValue <= 6 && !isDisable)
            {
                TargetLight.SetNight(true);

            }
            else
            {
                TargetLight.SetAutoMode(false);
             //   Debug.LogError($"TimeValue : {timeValue}");
                TargetLight.SetNight(false);

            }
        }
        if (TargetLight.CurrentNightState == NightModeState.IsAuto)
        {
            nightDropDown.interactable = true;
            nightGreenTime.interactable = true;
            nightRedPreHoldTime.interactable = true;
            nightRedTime.interactable = true;
            nightYellowTime.interactable = true;
            if (timeValue <= 6 && !isDisable)
            {
                float.TryParse(nightRedTime.text, out float nightRed);
                float.TryParse(nightGreenTime.text, out float nightGreen);
                float.TryParse(nightYellowTime.text, out float nightYellow);
                float.TryParse(nightRedPreHoldTime.text, out float nightRedPreHold);

                TargetLight.redDuration = nightRed;
                TargetLight.greenDuration = nightGreen;
                TargetLight.yellowDuration = nightYellow;
                TargetLight.preHoldDuration = nightRedPreHold;
                TargetLight.SetNight(true);

                TargetLight.SetAutoMode(true);// interaktifleri aç
            //    Debug.Log("BEN DELİYİM OGLİM");
                FixInvalidInput(nightRedTime);
                FixInvalidInput(nightGreenTime);
                FixInvalidInput(nightYellowTime);
                FixInvalidInput(nightRedPreHoldTime);

            }
            else
            {
                float.TryParse(RedTime.text, out float Red);
                float.TryParse(GreenTime.text, out float Green);
                float.TryParse(YellowTime.text, out float Yellow);
                float.TryParse(RedPreHoldTime.text, out float RedPreHold);

                TargetLight.redDuration = Red;
                TargetLight.greenDuration = Green;
                TargetLight.yellowDuration = Yellow;
                TargetLight.preHoldDuration = RedPreHold;
                TargetLight.SetNight(false);
                FixInvalidInput(nightRedTime);
                FixInvalidInput(nightGreenTime);
                FixInvalidInput(nightYellowTime);
                FixInvalidInput(nightRedPreHoldTime);
            }

        }
        else
        {

            nightGreenTime.interactable = false;
            nightRedPreHoldTime.interactable = false;
            nightRedTime.interactable = false;
            nightYellowTime.interactable = false;


            if (timeValue <= 6 && !isDisable)
            {
              //  Debug.LogError($"TimeValue : {timeValue}");
                TargetLight.SetAutoMode(false);
                TargetLight.SetNight(true);

            }

        }
    }
    public void AddToIntersection()
    {
        if (TargetLight == null) return;
        TrafficLightGroup newGroup = new();
        if (!newGroup.lights.Contains(TargetLight))
        {
            newGroup.lights.Add(TargetLight);
        }
        intersectionController.customOrder.Add(newGroup);
    }
    public void AddTargetToSelectedGroup()
    {

        if (intersectionController == null || TargetLight == null) return;
        int selectedIndex = groupDropdown.value;
        if (selectedIndex < 0 || selectedIndex >= intersectionController.groups.Count) return;
        TargetLight.gameObject.transform.parent.SetParent(intersectionController.gameObject.transform);

        TrafficLightGroup selectedGroup = intersectionController.groups[selectedIndex];
        if (!selectedGroup.lights.Contains(TargetLight))
        {
            selectedGroup.lights.Add(TargetLight);
           // Debug.Log($"{TargetLight.name} added to group {selectedGroup.groupName}");
        }
    }

    public TrafficLightGroup FindGroupOfTargetLight()
    {
        foreach (var group in intersectionController.groups)
        {
            if (group.lights != null && group.lights.Contains(TargetLight))
            {
                return group;
            }
        }

        foreach (var group in intersectionController.customOrder)
        {
            if (group.lights != null && group.lights.Contains(TargetLight))
            {
                return group;
            }
        }

        return null;
    }
    public TextMeshProUGUI DebugText;
    public void RemoveDebugText()
    {
        DebugText.text =  " ";
    }
    public void RemoveLightFromGroups()
    {
        if (!TargetLight) 
        { 
            DebugText.text = "Hedef Trafik Işığı Bulunamadı!";
            Invoke(nameof(RemoveDebugText),3f);
            return;
        }
        if (intersectionController == null) 
        {
            DebugText.text = "Hedef Kavşak Bulunamadı!";
            Invoke(nameof(RemoveDebugText), 3f);

            return;
        }


        if (intersectionController.IsLightInAnyGroup(TargetLight))
        {
            nightDropDown.interactable = true;
            stateDropdown.interactable = true;
            intersectionController.RemoveLightFromAllGroups(TargetLight);
            TargetLight.DisabledVisuals();
            RemoveFromGroup.interactable = false;
            Debug.Log("RemovedFromGroup");
        }
        else
        {
            RemoveFromGroup.interactable = true;
            Debug.Log("NotRemovedFromGroup");

        }
    }
    public void DeleteSelectedLight()
    {
        if (TargetLight)
        {
            trafficGroupUIManager.IsLightInAnyIntersectionRemoveLightFromIntersections(TargetLight);
            foreach (var YayaLight in TargetLight.YayaLights)
            {
                YayaLight.OnTargetDestroyed();
                YayaLight.targetLight = null;
            } 
            Destroy(TargetLight.transform.parent.gameObject);
            TargetLight = null;
           // DebugText.text = "Seçili Işık Silindi";
            CloseUI();
           // Invoke(nameof(RemoveDebugText), 3);

        }
        else
        {
            DebugText.text = "Seçili Işık yok";
            Invoke(nameof(RemoveDebugText), 3);
        }
        
    }
    public void OnLightEditEnd()
    {
        TrafficSystemManager.Instance.RefreshReferences();
        foreach (var light in TrafficSystemManager.Instance.AllTrafficLights)
        {
           light.CurrentSelected.color = light.OriginalColor;
        }
    }
    #endregion

    #region Intersection Funcs
    public void SetupGroupDropdown()
    {
        groupDropdown.ClearOptions();

        if (intersectionController == null || intersectionController.groups == null) return;
        IntersectionName.text = string.IsNullOrEmpty(intersectionController.IntersectionName) ? "İsimsiz kavşak" : intersectionController.IntersectionName;


        List<string> groupNames = new();
        foreach (var group in intersectionController.groups)
        {
            groupNames.Add(group.groupName);
        }

        groupDropdown.AddOptions(groupNames);
    }
   
    public void CreateIntersection(string intersectionName, IntersectionController intersectionController)
    {
        this.intersectionController = intersectionController;
        intersectionController.IntersectionName = intersectionName;
        IntersectionName.text = string.IsNullOrEmpty(intersectionName) ? "İsimsiz kavşak" : intersectionName;

        
    }
    void OnGroupSelected(int index)
    {
        if (index >= 0 && index < intersectionController.groups.Count)
        {
            groupNameInput.text = intersectionController.groups[index].groupName;
        }
        intersectionController.GroupLights = GroupLightsDropdown;
        intersectionController.SetupDropdown(index);
    }
    void OnGroupNameChanged(string newName)
    {
        int index = groupDropdown.value;
        if (index >= 0 && index < intersectionController.groups.Count)
        {
            intersectionController.groups[index].groupName = newName;
            SetupGroupDropdown();
            groupDropdown.value = index; // tekrar aynı seçili olsun
        }
        trafficGroupUIManager.GenerateUI();
    }
    public void CreateNewGroup()
    {
        if (intersectionController == null) return;

        // Yeni grup oluştur
        TrafficLightGroup newGroup = new()
        {
            groupName = $"Group {intersectionController.groups.Count + 1}",
            lights = new List<TrafficLightsController>()
        };

        // targetLight varsa ekle OPSİYONEL
        //if (TargetLight != null && !newGroup.lights.Contains(TargetLight))
        //{
        //    newGroup.lights.Add(TargetLight);
        //}

        // Listeye ekle
        intersectionController.groups.Add(newGroup);

        // Dropdown'u güncelle
        SetupGroupDropdown();

        // En son eklenen seçili olsun
        groupDropdown.value = groupDropdown.options.Count - 1;
        trafficGroupUIManager.GenerateUI();
      //  Debug.Log($"Yeni grup oluşturuldu: {newGroup.groupName}");
        
    }
    public void CreateNewGroupWithTargetLight()
    {
        if (TargetLight == null || intersectionController == null) return;

        TrafficLightGroup newGroup = new()
        {
            groupName = "Group_" + Random.Range(1000, 9999),
            lights = new List<TrafficLightsController> { TargetLight }
        };

        intersectionController.groups.Add(newGroup);
        Debug.Log($"Yeni grup oluşturuldu: {newGroup.groupName}");
    }
   
    public void DeleteSelectedGroup()
    {
        int index = groupDropdown.value;

        if (index >= 0 && index < intersectionController.groups.Count)
        {
            // Güvenlik: Kullanıcıdan onay al
            bool confirmed = true; // TODO: Buraya aşağıdaki fonksiyonlarla beraber UI bağlanabilir.

            if (confirmed)
            {
                intersectionController.groups.RemoveAt(index);

                // Eğer customOrder'da varsa oradan da çıkar
                if (intersectionController.customOrder.Count > index)
                {
                    intersectionController.customOrder.RemoveAt(index);
                }

                SetupGroupDropdown();

                // Input temizle
                groupNameInput.text = "";

              //  Debug.Log($"Group at index {index} deleted.");
            }
        }
        trafficGroupUIManager.GenerateUI();
        IntersectionName.text =  trafficGroupUIManager.Controller.IntersectionName;

    }

    public GameObject confirmDialog;
    public void AskGroupDelete()
    {
        if (groupDropdown.options.Count<=0) return; 
       

        int index = groupDropdown.value;
        string groupName = groupDropdown.options[index].text;

        confirmDialog.SetActive(true);
        DeleteConfirmDialog.text = $"\"{groupName}\" grubunu silmek istediğinden emin misin?";
    }
    public void AskIntersectionDelete()
    {
        if (intersectionController)
            ConfirmDelete(false);
    }
    public void DeleteSelectedIntersection()
    {
        trafficGroupUIManager.Intersections.Remove(intersectionController.gameObject);
        Destroy(intersectionController.gameObject);
        intersectionController = null;
        intersectionController =    trafficGroupUIManager.Intersections[0].GetComponent<IntersectionController>();
        trafficGroupUIManager.CloseUI();    
      //  intersectionController.SelectIntersection();
      //  SetupGroupDropdown();
       // groupDropdown.RefreshShownValue();
       // IntersectionName.text =  trafficGroupUIManager.Controller.IntersectionName;

        trafficGroupUIManager.PopulateDropdown();
     //   trafficGroupUIManager.GenerateUI();

        

    }
    public void ConfirmDelete(bool yes)
    {
        if (yes)
            DeleteSelectedGroup();
        else
            DeleteSelectedIntersection();
        confirmDialog.SetActive(false);
    }
    public void OnIntersectionEditEnd()
    {
        TrafficSystemManager.Instance.RefreshReferences();
        foreach (var kavsak in TrafficSystemManager.Instance.AllIntersections)
        {
            kavsak.CurrentSelected.SetActive(false);
        }
    }
    #endregion

    #region Yaya Light Funcs
    public void OpenYayaUI()
    {
        YayaUI.SetActive(true);
    }
    public void CloseYayaUI()
    {
        YayaUI.SetActive(false);
    }
    public void DeleteYayaLight()
    {
       TargetYaya.RemoveYayaLight();
        CloseYayaUI();
    }
    public void StartPLSpawn()
    {
        
        PLSpawner = TargetLight.transform.parent.GetComponent<PedestrianLightSpawner>();
        PLSpawner.StartSpawnYayaLight(TargetLight.transform);
    }
    public void UpdateYayaBindingDropdownFromSelectedYaya()
    {
        if (TargetYaya != null)
        {
            // Enum değerini, dropdown'ın seçebileceği indekse çevir
            YayaBindingTypeDropdown.value = (int)TargetYaya.BindingType;
        }
    }
    public void OnYayaBindingDropdownValueChanged(int index)
    {
        if (TargetYaya == null) return;

        // Dropdown'dan gelen değeri enum'a çevir
        TargetYaya.BindingType = (YayaBindingType)index;
        TargetYaya.ChangeState();
        // Güncellenmiş BindingType için gerekli işlemleri yap
        Debug.Log($"Binding Type changed to: {TargetYaya.BindingType}");
    }
    public void OnYayalightSpawned()
    {
        Invoke(nameof(Yayaspawncooldowner),0.01f);
    }
    void Yayaspawncooldowner()
    {
        ApplySelectedState();

        Debug.Log("YayaSpawnlandı");

       

    }

    public void OnYayaLightEditEnd()
    {
        TrafficSystemManager.Instance.RefreshReferences();
        foreach (var yaya in TrafficSystemManager.Instance.AllYayaLights)
        {
          //  yaya.CurrentSelected.SetActive(false);
        }
    }
    #endregion
}
