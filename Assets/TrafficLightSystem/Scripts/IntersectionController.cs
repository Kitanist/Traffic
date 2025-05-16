using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class IntersectionController : MonoBehaviour
{
    public List<TrafficLightGroup> groups = new List<TrafficLightGroup>();
    public float greenDuration = 10f;
    public float yellowDuration = 3f;
    public float redBuffer = 2f;
    public string IntersectionName;
    [Header("Intersection Mode")]
    public IntersectionMode mode;
    public GameObject CurrentSelected;
    public TMP_Dropdown GroupLights; 

    [Header("Custom Order (Used only if mode = Custom)")]
    public List<TrafficLightGroup> customOrder = new List<TrafficLightGroup>();

    private void Start()
    {
        StartCoroutine(StartCycleSequence());
        CurrentSelected.SetActive(false);
    }
    private void Awake()
    {
      //  SelectIntersection();
    }
    private IEnumerator StartCycleSequence()
    {
        while (true)
        {
            float cumulativeDelay = 0f;

            List<TrafficLightGroup> cycleGroups = GetGroupsInOrder();
          //  Debug.Log($"Running mode: {mode}");

            for (int i = 0; i < cycleGroups.Count; i++)
            {
                StartCoroutine(GroupCycle(cycleGroups[i], cumulativeDelay));
                cumulativeDelay +=  yellowDuration + redBuffer;
            }

            // Tüm cycle bittikten sonra tekrar başlatmak için bekle
            yield return new WaitForSeconds(cumulativeDelay);
        }
    }

    private IEnumerator GroupCycle(TrafficLightGroup group, float delay)
    {
        // Başlama sırasını bekle
        yield return new WaitForSeconds(delay);

        // 🔴 Başlamadan önce tüm ışıkları kırmızı yap
        SetGroupState(group, TrafficLightState.Red);
        yield return new WaitForSeconds(redBuffer);

        // 🟡 Sarı
        SetGroupState(group, TrafficLightState.Yellow);
        yield return new WaitForSeconds(yellowDuration);

        // 🟢 Yeşil
        SetGroupState(group, TrafficLightState.Green);
        yield return new WaitForSeconds(greenDuration);

        // 🟡 Sarı
        SetGroupState(group, TrafficLightState.Yellow);
        yield return new WaitForSeconds(yellowDuration);

        
    }

    private void SetGroupState(TrafficLightGroup group, TrafficLightState state)
    {
        foreach (var light in group.lights)
        {
            light.OverrideState(state);
        }
    }
    private List<TrafficLightGroup> GetGroupsInOrder()
    {
        switch (mode)
        {
            case IntersectionMode.Sequential:
                return groups;
            case IntersectionMode.Custom:
                return customOrder;
            default:
                return groups;
        }
    }
    public void ApplyCustomOrder(List<TrafficLightGroup> newOrder)
    {
        StopAllCoroutines(); 
        customOrder = newOrder;
        mode = IntersectionMode.Custom;
        StartCoroutine(StartCycleSequence()); 
    }
    public void RemoveLightFromAllGroups(TrafficLightsController targetLight)
    {
        foreach (var group in groups)
        {
            if (group.lights.Contains(targetLight))
            {
                group.lights.Remove(targetLight);
                //Debug.Log($"'{targetLight.name}' ışığı '{group.groupName}' grubundan çıkarıldı.");
            }
        }

        foreach (var group in customOrder)
        {
            if (group.lights.Contains(targetLight))
            {
                group.lights.Remove(targetLight);
                //Debug.Log($"(Custom Order) '{targetLight.name}' ışığı '{group.groupName}' grubundan çıkarıldı.");
            }
        }
    }
    public bool IsLightInAnyGroup(TrafficLightsController targetLight)
    {
        foreach (var group in groups)
        {
            if (group.lights.Contains(targetLight))
            {
                return true;
            }
        }
        foreach (var group in customOrder)
        {
            if (group.lights.Contains(targetLight))
            {
                return true;
            }
        }

        return false;
    }

    public void SelectIntersection() // bağımlılık sökucu
    {
        var TrafficUIManager = GameObject.Find("TrafficUIManager");
        if (!TrafficUIManager.GetComponent<TrafficGroupUIManager>().Intersections.Contains(gameObject))
        {
            TrafficUIManager.GetComponent<TrafficGroupUIManager>().Intersections.Add(gameObject);
        }
        TrafficUIManager.GetComponent<TrafficGroupUIManager>().Controller = this;
        TrafficUIManager.GetComponent<TrafficGroupUIManager>().OpenUI();

        TrafficUIManager.GetComponent<TrafficLightUI>().intersectionController = this;
        TrafficUIManager.GetComponent<TrafficLightUI>().IntersectionName.text = IntersectionName;
        TrafficUIManager.GetComponent<TrafficLightUI>().SetupGroupDropdown();
        CurrentSelected.SetActive(true);
       // Debug.Log("Selected");
    }
    public void SetupDropdown(int index)
    {
        GroupLights.ClearOptions();
        var selectedGroup = groups[index];
        List<string> options = new List<string>();

        
            foreach (var light in selectedGroup.lights)
            {
                options.Add(light.name);

           }
        

        GroupLights.AddOptions(options);
    }

   public void OnDropdownValueChanged(int index)
    {
        if (index < 0 || index >= groups.Count) return;

        TrafficLightGroup selectedGroup = groups[index];

        // Buraya seçilen gruba göre ne yapmak istiyorsan yaz
        Debug.Log("Seçilen grup: " + selectedGroup.groupName);
        Debug.Log("Bu gruptaki ışık sayısı: " + selectedGroup.lights.Count);
    }
}
[System.Serializable]
public class TrafficLightGroup
{
    public string groupName;
    public List<TrafficLightsController> lights;
}

