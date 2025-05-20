using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class YayaController : MonoBehaviour
{
    public GameObject RedLight, GreenLight;
    public TrafficLightsController _targetLight;
    public TrafficLightsController targetLight
    {
        get
        {
            return _targetLight;
        }
        set 
        {

            _targetLight = value;

            
        }
    }
    public IEnumerator RemoveDebugText()
    {
        yield return new WaitForSeconds(3);
        TLUI.RemoveDebugText();
    }
    public TrafficLightUI TLUI;
    public YayaState YayaState;
    public TMP_Dropdown YayaStateDropdown, YayaBindingTypeDropdown;
    public YayaBindingType BindingType = YayaBindingType.Sync;
    public Image CurrentSelected;
    public Color OriginalColor;

    private Coroutine flashingCoroutine;

    private void Start()
    {
        if (CurrentSelected!=null)
        {
            OriginalColor = CurrentSelected.color;
        }
        else
        {
            CurrentSelected = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
            OriginalColor = CurrentSelected.color;

        }

        //YayaStateDropdown.onValueChanged.AddListener(SetState);
        TLUI = FindObjectOfType<TrafficLightUI>();
        
        YayaStateDropdown = TLUI.YayaStateDropdown;
        YayaBindingTypeDropdown = TLUI.YayaBindingTypeDropdown;
        if (IsTargetNull())
        {
            
           // Debug.Log("Target Null ve atanýyor.");
            targetLight = gameObject.transform.parent.GetChild(0).GetComponent<TrafficLightsController>();
            targetLight.YayaLights.Add(this);
        }

        // Dropdown'ý ilk baþta güncelleyebiliriz
        UpdateDropdown();
       // CurrentSelected.SetActive(false) ;
    }

    public void SetTarget()
    {
        if (IsTargetNull())
        {
            targetLight = TLUI.TargetLight;
            if (!targetLight.YayaLights.Contains(this))
                targetLight.YayaLights.Add(this);
            CurrentSelected.color = OriginalColor;
            Debug.Log("Target Nulldu artýk deðil");
            return;
        }
        else
        {
            Debug.Log("Target Null deðildi hala deðil");

            if (targetLight.YayaLights.Contains(this))
                targetLight.YayaLights.Remove(this);

            targetLight = TLUI.TargetLight;

            if (!targetLight.YayaLights.Contains(this))
                targetLight.YayaLights.Add(this);
            CurrentSelected.color = OriginalColor;  
          //  Debug.Log("SetTargetEdited");
        }

        // Yeni hedef ýþýk seçildiðinde dropdown'ý güncelle
        ChangeState();
        UpdateDropdown();
        RefreshAllLightStates();
    }
    public void OnTargetDestroyed()
    {
        CurrentSelected.color = new Color(r: 160, g: 32, b: 240);
    }
    private void RefreshAllLightStates()
    {
        var AllTrafficLights = FindObjectsOfType<TrafficLightsController>().ToList();

        foreach (var light in AllTrafficLights)
        {
            if (light.YayaLights.Contains(this))
            {
                light.RedCircleOpen();
                Debug.Log($"{light.name} bu yayaya baðlý. Açýldý.");
            }
            else
            {
                light.RedCircleClose();
            }
        }
    }

    private void UpdateDropdown()
    {
        // Seçilen YayaController'a göre dropdown'larý güncelliyoruz
        if (TLUI.TargetYaya != null)
        {
            // YayaState güncelleme
            if (YayaStateDropdown != null)
            {
                Debug.Log($"Yaya Dropdown guncellendi. Deðer önceki {YayaStateDropdown.value}");
                if (YayaStateDropdown.value != (int)TLUI.TargetYaya.YayaState)
                {
                    YayaStateDropdown.value = (int)TLUI.TargetYaya.YayaState;
                    YayaStateDropdown.RefreshShownValue();
                    TLUI.TargetYaya.SetState((int)TLUI.TargetYaya.YayaState);
                    Debug.Log($"Yaya Dropdown guncellendi. Deðer sonraki {YayaStateDropdown.value}");
                }
                 else 
                    Debug.Log($"Yaya Dropdown guncellenmedi deðer ayný");
               


                   
            }

            // YayaBindingType güncelleme
            if (YayaBindingTypeDropdown != null)
            {
                Debug.Log($"Yaya Binding Type Dropdown guncellendi. Deðer önceki {YayaBindingTypeDropdown.value}");
                YayaBindingTypeDropdown.value = (int)TLUI.TargetYaya.BindingType;
                YayaBindingTypeDropdown.RefreshShownValue();
                Debug.Log($"Yaya Binding Type Dropdown guncellendi. Deðer sonraki {YayaBindingTypeDropdown.value}");
            }
        }
    }


    public bool IsTargetNull()
    {
        if (targetLight==null)
        {
            return true;
        }
        return false;
    }

    public void ChangeState()
    {
        if (IsTargetNull() || YayaState != YayaState.IsAuto)
        {
            return;
        }
        bool IsNight = targetLight.GetNight();
        
            
        //  Debug.Log("Oto ve State Deðiþti");
        switch (BindingType)
        {
            case YayaBindingType.Sync:
                if (flashingCoroutine != null)
                    StopCoroutine(flashingCoroutine);
                Debug.Log("Selected");
                RedLight.SetActive(false);
                GreenLight.SetActive(false);
                if (!IsNight)
                {
                    if (targetLight.CurrentState == TrafficLightState.Green) // IsAuto için ayrýca bakýver
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(true);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Yellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Red)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingRed1s)
                    {
                        Debug.Log("Flashing Rutinine gircem Yaya ben");
                        StartCoroutine(FlashingLight(RedLight, 1f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingRed2s)
                    {
                        StartCoroutine(FlashingLight(RedLight, 2f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingYellow1s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 1f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingYellow2s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 2f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.TempYellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Disable)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(false);
                    }
                }
                else
                {
                    if (targetLight.CurrentNightState == NightModeState.Green) // IsAuto için ayrýca bakýver
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(true);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.Yellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.Red)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingRed1s)
                    {
                        Debug.Log("Flashing Rutinine gircem Yaya ben");
                        StartCoroutine(FlashingLight(RedLight, 1f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingRed2s)
                    {
                        StartCoroutine(FlashingLight(RedLight, 2f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingYellow1s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 1f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingYellow2s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 2f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.Disable)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(false);
                    }
                }
                
                    break;
            case YayaBindingType.ReverseSync:
                if (flashingCoroutine != null)
                    StopCoroutine(flashingCoroutine);
                RedLight.SetActive(false);
                GreenLight.SetActive(false);
                Debug.Log("Selected");

                if (!IsNight)
                {
                    if (targetLight.CurrentState == TrafficLightState.Green) // IsAuto için ayrýca bakýver
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Yellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Red)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(true);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingRed1s)
                    {
                        Debug.Log("Flashing Rutinine gircem Yaya ben");
                        StartCoroutine(FlashingLight(RedLight, 1f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingRed2s)
                    {
                        StartCoroutine(FlashingLight(RedLight, 2f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingYellow1s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 1f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.FlashingYellow2s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 2f));
                    }
                    else if (targetLight.CurrentState == TrafficLightState.TempYellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentState == TrafficLightState.Disable)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(false);
                    }
                }
                else
                {
                    if (targetLight.CurrentNightState == NightModeState.Green) // IsAuto için ayrýca bakýver
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.Yellow)
                    {
                        RedLight.SetActive(true);
                        GreenLight.SetActive(false);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.Red)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(true);
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingRed1s)
                    {
                        Debug.Log("Flashing Rutinine gircem Yaya ben");
                        StartCoroutine(FlashingLight(RedLight, 1f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingRed2s)
                    {
                        StartCoroutine(FlashingLight(RedLight, 2f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingYellow1s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 1f));
                    }
                    else if (targetLight.CurrentNightState == NightModeState.FlashingYellow2s)
                    {
                        StartCoroutine(FlashingLight(GreenLight, 2f));
                    }
                    
                    else if (targetLight.CurrentNightState == NightModeState.Disable)
                    {
                        RedLight.SetActive(false);
                        GreenLight.SetActive(false);
                    }
                }
                
                break;
            case YayaBindingType.CustomSync:
                // ToDo: Custom Sync yap
                break;
            default:
                break;
        }
        
    }

    public void SetState(int a)
    {
        YayaState = (YayaState)a;
        
        switch (YayaState)
        {
            case YayaState.Red:
                SetRed();
                DisableFlashing();

                break;

            case YayaState.Green:
                SetGreen();
                DisableFlashing();

                break;

            case YayaState.IsAuto:
                // Otomatik kontrol targetLight üzerinden yapýlabilir
                DisableFlashing();

                break;

            case YayaState.Disable:
                GreenLight.SetActive(false);
                RedLight.SetActive(false);
                DisableFlashing();
                break;

            case YayaState.FlashingRed1s:
                
                StartCoroutine(FlashingLight(RedLight, 1f));
                break;

            case YayaState.FlashingGreen1s:
                StartCoroutine(FlashingLight(GreenLight, 1f));
                break;

            case YayaState.FlashingRed2s:
                StartCoroutine(FlashingLight(RedLight, 2f));
                break;

            case YayaState.FlashingGreen2s:
                StartCoroutine(FlashingLight(GreenLight, 2f));
                break;

            default:
                break;
        }
    }

    private IEnumerator FlashingLight(GameObject lightObj, float interval)
    {
        if (flashingCoroutine != null)
            StopCoroutine(flashingCoroutine);
        Debug.Log("Flashing Rutinine girdim Yaya ben");
        flashingCoroutine = StartCoroutine(Flashing(lightObj, interval));
        yield return null;
    }

    private IEnumerator Flashing(GameObject lightObj, float interval)
    {
        RedLight.SetActive(false);
        GreenLight.SetActive(false);

        while (true)
        {
            lightObj.SetActive(!lightObj.activeSelf);
            yield return new WaitForSeconds(interval);
        }
    }

    public void SetGreen()
    {
        GreenLight.SetActive(true);
        RedLight.SetActive(false);
    }
    public void DisableFlashing()
    {
        if (flashingCoroutine != null)
            StopCoroutine(flashingCoroutine);
    }
    public void SetRed()
    {
        RedLight.SetActive(true);
        GreenLight.SetActive(false);
    }

    public void SelectTargetYaya()
    {
        TLUI.TargetYaya = this;
        TLUI.OpenYayaUI();
        TLUI.CloseUI();

        if (YayaStateDropdown != null)
        {
            YayaStateDropdown.onValueChanged.RemoveAllListeners();
            YayaStateDropdown.onValueChanged.AddListener((int val) =>
            {
                if (TLUI.TargetYaya != null)
                {
                   
                    TLUI.TargetYaya.SetState(val);
                }
            });
            UpdateDropdown();
        }
        var AllTrafficLights = FindObjectsOfType<TrafficLightsController>().ToList();

        foreach (var trafficLight in AllTrafficLights) // Bütün ýþýklar burada olmalý
        {
            if (trafficLight.YayaLights.Contains(this))
            {
                Debug.Log($"{trafficLight.name} bu yayaya baðlý. RedCircle açýlýyor.");
                trafficLight.RedCircleOpen();
            }
            else
            {
                trafficLight.RedCircleClose();
                trafficLight.CurrentSelected.color = trafficLight.OriginalColor;
            }
        }
        //  CurrentSelected.SetActive(true);
    }



    public void RemoveYayaLight()
    {
       
        TrafficLightsController[] allTrafficLights = FindObjectsOfType<TrafficLightsController>();

       
        foreach (TrafficLightsController trafficLight in allTrafficLights)
        {
            if (trafficLight.YayaLights.Contains(this))
            {
                trafficLight.YayaLights.Remove(this); 
            }
        }

        Destroy(gameObject);
    }
}
