using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightsController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private TrafficLightState currentState = TrafficLightState.Red;
    [SerializeField] private NightModeState currentNightState = NightModeState.Red;

    [Header("Durations (seconds)")]
    public float greenDuration = 15f;
    public float yellowDuration = 3f;
    public float redDuration = 10f;
    public float preHoldDuration = 5f; // Kırmızıdan önce ekstra bekleme

    public Image CurrentSelected;
    public Color MyBlue,OriginalColor;
    public bool isAutoMode = true;
    public GameObject redLight, greenLight, yellowLight, greenYaya, RedYaya;
    public List<YayaController> YayaLights = new(); 
    public TextMeshProUGUI RemainingTime;
    public GameObject RedCircleFull;
    private Coroutine stateRoutine;
    private bool isManualOverride = false, isNight, isFirstTimeClicked = true;
    private Coroutine flashingRoutine;
    public TrafficLightState CurrentState => currentState;
    public NightModeState CurrentNightState => currentNightState;
    public GameEvent OnLightConnectEnd;

    private void Start()
    {
        ApplyVisuals();
        //   StartCycle();
        RedCircleClose();
       // OriginalColor = CurrentSelected.color;

    }
    public void SelectLight()
    {
        var TrafficUIManager = GameObject.Find("TrafficUIManager");
        TrafficUIManager.GetComponent<TrafficLightUI>().TargetLight = this;
        TrafficUIManager.GetComponent<TrafficLightUI>().OpenUI();// Burda foreach olmalı ve o foreach tum ıntersectionları gezmeli
        TrafficUIManager.GetComponent<TrafficLightUI>().CloseYayaUI();

        if (!TrafficUIManager.GetComponent<TrafficLightUI>().intersectionController)
        {
            TrafficUIManager.GetComponent<TrafficLightUI>().RemoveFromGroup.interactable = false;
            return;
        }
        if (TrafficUIManager.GetComponent<TrafficLightUI>().intersectionController.IsLightInAnyGroup(this))
        {
            TrafficUIManager.GetComponent<TrafficLightUI>().RemoveFromGroup.interactable = true;
            TrafficUIManager.GetComponent<TrafficLightUI>().stateDropdown.interactable = false;
            TrafficUIManager.GetComponent<TrafficLightUI>().nightDropDown.interactable = false;



        }
        else
        {
            TrafficUIManager.GetComponent<TrafficLightUI>().RemoveFromGroup.interactable = false;

            TrafficUIManager.GetComponent<TrafficLightUI>().stateDropdown.interactable = true;
            TrafficUIManager.GetComponent<TrafficLightUI>().nightDropDown.interactable = true;
        }

        CurrentSelected.color = MyBlue;
    }
    public void StartCycle()
    {
        isManualOverride = false;
        if (stateRoutine != null) StopCoroutine(stateRoutine);
        stateRoutine = StartCoroutine(LightCycleRoutine());
    }
    public IEnumerator RemainingTimeUpdater(float Time)
    {
        var time = (int)Time;
        if (time <= 0)
        {
            RemainingTime.text = "Kalan Işık Suresi : ---";
            yield break;

        }
        RemainingTime.text = "Kalan Işık Suresi : " + time.ToString();
        yield return new WaitForSeconds(1);
        Time -= 1;
        if (Time>0)
        {
            StartCoroutine(RemainingTimeUpdater(Time));

        }

    }
    public bool IsFirstTimeClicked()
    {
        return isFirstTimeClicked;
    }
    public void SetFirstTimeClicked(bool value)
    {
        isFirstTimeClicked = value;
    }
    public void StopCycle()
    {
        if (stateRoutine != null) StopCoroutine(stateRoutine);
    }
    public void SetYayas()
    {
        
        foreach (var Yaya in YayaLights)
        {
            Yaya.ChangeState();
        }
    //   Debug.Log("SetYayas");
       // greenYaya = YayaLights[0].GreenLight;
       // RedYaya = YayaLights[0].RedLight;

    }
    public void OverrideState(TrafficLightState newState)
    {
        StopCycle();
        isManualOverride = true;
        SetState(newState);
    }
    public void OverrideStateNight(NightModeState newState)
    {
        StopCycle();
        isManualOverride = true;
        SetNightState(newState);
    }
    public void SetAutoMode(bool auto)
    {
        isAutoMode = auto;
        if (isAutoMode)
        {
            StopCycle();
            stateRoutine = StartCoroutine(LightCycleRoutine()); // Varsa başlat
        }
        else
        {
            StopCycle(); // Manuel'e geçince durdur
        }
    }
    public void SetState(TrafficLightState newState)
    {
        currentState = newState;
        ApplyVisuals();
        StartCoroutine(RemainingTimeUpdater(0));
        // Debug.Log($"{gameObject.name} → {newState}");
    }
    public void SetNightState(NightModeState newState)
    {
        currentNightState = newState;
        StartCoroutine(RemainingTimeUpdater(0));
        ApplyVisuals();
        

        // Debug.Log($"{gameObject.name} → {newState}");

    }

    private IEnumerator LightCycleRoutine()
    {

        if (!isNight&& currentState != TrafficLightState.Disable)
        {
            while (isManualOverride)
            {
                SetState(TrafficLightState.Green);
                ApplyVisuals();
                if (greenDuration <= 0)
                    greenDuration = 1;
                StartCoroutine(RemainingTimeUpdater(greenDuration));

                yield return new WaitForSeconds(greenDuration);
                SetState(TrafficLightState.Yellow);
                ApplyVisuals();
                if (yellowDuration <= 0)
                    yellowDuration = 1;
                StartCoroutine(RemainingTimeUpdater(yellowDuration));

                yield return new WaitForSeconds(yellowDuration);
                SetState(TrafficLightState.Red);
                ApplyVisuals();
                if (redDuration + preHoldDuration <= 0)
                    redDuration = 1;
                StartCoroutine(RemainingTimeUpdater(redDuration + preHoldDuration));

                yield return new WaitForSeconds(redDuration + preHoldDuration);
                SetState(TrafficLightState.TempYellow);
                ApplyVisuals();
                if (yellowDuration <= 0)
                    yellowDuration = 1;
                StartCoroutine(RemainingTimeUpdater(yellowDuration));

                yield return new WaitForSeconds(yellowDuration);
            }
        }

        else if (currentNightState != NightModeState.Disable)
        {
            while (isManualOverride)
            {
                SetState(TrafficLightState.Green);
                SetNightState(NightModeState.Green);
                ApplyVisuals();
                if (greenDuration <= 0)
                    greenDuration = 1;
                yield return new WaitForSeconds(greenDuration);
                SetState(TrafficLightState.Yellow);
                SetNightState(NightModeState.Yellow);
                ApplyVisuals();
                if (yellowDuration <= 0)
                    yellowDuration = 1;
                yield return new WaitForSeconds(yellowDuration);
                SetState(TrafficLightState.Red);
                SetNightState(NightModeState.Red);
                ApplyVisuals();
                if (redDuration + preHoldDuration <= 0)
                    redDuration = 1;
                preHoldDuration = 1;
                yield return new WaitForSeconds(redDuration + preHoldDuration);
                SetState(TrafficLightState.TempYellow);
                SetNightState(NightModeState.Yellow);

                ApplyVisuals();
                if (yellowDuration <= 0)
                    yellowDuration = 1;
                yield return new WaitForSeconds(yellowDuration);
            }
        }
    }

    private IEnumerator FlashRoutine(GameObject flashColor, float interval = 1f)
    {
        StopCycle();
        //Debug.Log("EnterFlashingMethot");
        //StopAllCoroutines(); 
        if (!isNight) {
        while (currentState == TrafficLightState.FlashingRed1s || currentState == TrafficLightState.FlashingYellow1s)
        {
           // Debug.Log("EnterFlashing1s");

            flashColor.SetActive(true);
            if (currentState == TrafficLightState.FlashingRed1s)
            {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(true);

                    }
                }
            else if (currentState == TrafficLightState.FlashingYellow1s)
            {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(true);
                    }
                }
            yield return new WaitForSeconds(1f);
            if (currentState == TrafficLightState.FlashingRed1s)
            {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(false);

                    }
                }
            else if (currentState == TrafficLightState.FlashingYellow1s)
            {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(false);
                    }
                }
            flashColor.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
        while (currentState == TrafficLightState.FlashingRed2s || currentState == TrafficLightState.FlashingYellow2s)
        {
           // Debug.Log("EnterFlashing2s");

            flashColor.SetActive(true);
            if (currentState == TrafficLightState.FlashingRed2s)
            {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(true);

                    }
                }
            else if (currentState == TrafficLightState.FlashingYellow2s)
            {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(true);
                    }
                }
            yield return new WaitForSeconds(2f);
            flashColor.SetActive(false);
            if (currentState == TrafficLightState.FlashingRed2s)
            {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(false);

                    }
                }
            else if (currentState == TrafficLightState.FlashingYellow2s)
            {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(false);
                    }
                }
          //  Debug.Log("MiddleFlashing2s");

            yield return new WaitForSeconds(2f);
        }

        }
        else 
        {
            while (currentNightState == NightModeState.FlashingRed1s || currentNightState == NightModeState.FlashingYellow1s)
            {
              //  Debug.Log("EnterFlashing1s");

                flashColor.SetActive(true);
                if (currentNightState == NightModeState.FlashingRed1s)
                {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(true);

                    }
                }
                else if (currentNightState == NightModeState.FlashingYellow1s)
                {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(1f);
                if (currentNightState == NightModeState.FlashingRed1s)
                {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(false);

                    }
                }
                else if (currentNightState == NightModeState.FlashingYellow1s)
                {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(false);
                    }
                }
                flashColor.SetActive(false);
                yield return new WaitForSeconds(1f);
            }
            while (currentNightState == NightModeState.FlashingRed2s || currentNightState == NightModeState.FlashingYellow2s)
            {
                //Debug.Log("EnterFlashing2s");

                flashColor.SetActive(true);
                if (currentNightState == NightModeState.FlashingRed2s)
                {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(true);

                    }
                }
                else if (currentNightState == NightModeState.FlashingYellow2s)
                {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(2f);
                flashColor.SetActive(false);
                if (currentNightState == NightModeState.FlashingRed2s)
                {
                    if (greenYaya)
                    {
                        greenYaya.SetActive(false);

                    }
                }
                else if (currentNightState == NightModeState.FlashingYellow2s)
                {
                    if (greenYaya)
                    {
                        RedYaya.SetActive(false);
                    }
                }
               // Debug.Log("MiddleFlashing2s");

                yield return new WaitForSeconds(2f);
            }
        }
    }
    public void DisabledVisuals()
    {
        redLight.SetActive(false);
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        if (greenYaya)
        {
            greenYaya.SetActive(false);
            RedYaya.SetActive(false);
        }
       
    }
    public void SetNight(bool IsNight)
    {
        isNight = IsNight;
    }
    public bool GetNight()
    {
        return isNight;
    }
    private void ApplyVisuals()
    {

        redLight.SetActive(false);
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        SetYayas();
        if (greenYaya)
        {
            greenYaya.SetActive(false);
            RedYaya.SetActive(false);
          //  if (YayaLights.Count > 0)
          //      SetYayas();

        }
        else
        {
           // SetYayas();
           // Debug.Log("Yayalarr atanmadı");
        }
        
        if (currentState != TrafficLightState.Disable || currentNightState != NightModeState.Disable)
        {
            if (!isNight)
            {
                switch (currentState)
                {
                    case TrafficLightState.IsAuto:
                        Debug.Log("Ben gunduzum otoyum");
                        break;
                    case TrafficLightState.Red:
                        SetYayas();
                        if (greenYaya) 
                        {
                            greenYaya.SetActive(true);

                        }
                            redLight.SetActive(true);
                       
                        break;
                    case TrafficLightState.Yellow:
                        yellowLight.SetActive(true);
                        if (greenYaya) 
                        {
                        
                        RedYaya.SetActive(true);
                        }
                        SetYayas();
                        break;
                    case TrafficLightState.TempYellow:
                        yellowLight.SetActive(true);
                        if (greenYaya) 
                        {
                        RedYaya.SetActive(true);
                        }
                        SetYayas();
                        break;
                    case TrafficLightState.Green:
                        SetYayas();

                        greenLight.SetActive(true);
                        if (greenYaya) 
                        {
                        RedYaya.SetActive(true);
                        }
                        break; 
                    case TrafficLightState.FlashingRed1s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(redLight));
                        SetYayas();
                        return;
                    case TrafficLightState.FlashingYellow1s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(yellowLight));
                        SetYayas();
                        return;
                    case TrafficLightState.FlashingRed2s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(redLight));
                        SetYayas();
                        return;
                    case TrafficLightState.FlashingYellow2s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(yellowLight));
                        SetYayas();
                        return;
                    default:
                        Debug.Log("DefaultBruhhh");
                        break;
                }
            }
            else
            {
                switch (currentNightState)
                {
                    case NightModeState.Red:
                        SetYayas();
                        if (greenYaya)
                        {
                            greenYaya.SetActive(true);

                        }
                        redLight.SetActive(true);
                        break;
                    case NightModeState.Yellow:
                        yellowLight.SetActive(true);
                        SetYayas();
                        if (greenYaya)
                        {
                            RedYaya.SetActive(true);

                        }
                        Debug.Log("YellowStateBruhhh");
                        break;
                    case NightModeState.Green:
                        greenLight.SetActive(true);
                        SetYayas();
                        if (greenYaya)
                        {
                            RedYaya.SetActive(true);

                        }
                        break;
                    case NightModeState.FlashingRed1s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(redLight));
                        SetYayas();
                        return;
                    case NightModeState.FlashingYellow1s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(yellowLight));
                        SetYayas();
                        return;
                    case NightModeState.FlashingRed2s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(redLight));
                        SetYayas();
                        return;
                    case NightModeState.FlashingYellow2s:
                        if (flashingRoutine != null) StopCoroutine(flashingRoutine);
                        flashingRoutine = StartCoroutine(FlashRoutine(yellowLight));
                        SetYayas();
                        return;
                    default:
                        break;
                }
            }
        }

        else StartCoroutine(RemainingTimeUpdater(0));

        if (flashingRoutine != null) // ?
        {
            StopCoroutine(flashingRoutine);
            flashingRoutine = null;
        }
    }
    public void RedCircleOpen()
    {
        CurrentSelected.color = MyBlue;
    }
    public void RedCircleClose()
    {
        CurrentSelected.color = OriginalColor;
    }
    public void SelectConnectYayaTarget()
    {
        var TrafficUIManager = GameObject.Find("TrafficUIManager");
        TrafficUIManager.GetComponent<TrafficLightUI>().TargetLight = this;
        TrafficUIManager.GetComponent<TrafficLightUI>().ConnectYayaToSelectedLight();
        OnLightConnectEnd.Raise();
        RedCircleOpen();
    }

}
