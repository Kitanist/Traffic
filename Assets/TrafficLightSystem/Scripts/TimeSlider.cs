using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    public Slider hourSlider;
    public TMP_Text hourText;
    public TrafficLightUI TLUI;
    public int currentHour;

    void Start()
    {
        hourSlider.minValue = 0;
        hourSlider.maxValue = 23;
        hourSlider.wholeNumbers = true;

        hourSlider.onValueChanged.AddListener(OnHourChanged);
        OnHourChanged(hourSlider.value); // baþta çaðýr
    }

    public void OnHourChanged(float value)
    {
        currentHour = (int)value;
        hourText.text = "Saat: " + currentHour.ToString("00") + ":00";
        TLUI.TimeValue = currentHour;
        // burada oyun sistemine gönderilebilir
       // Debug.Log("Saat ayarlandý: " + currentHour);
    }
}
