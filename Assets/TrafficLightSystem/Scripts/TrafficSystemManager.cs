using System.Collections.Generic;
using UnityEngine;

public class TrafficSystemManager : MonoBehaviour
{
    public static TrafficSystemManager Instance { get; private set; }

    public List<IntersectionController> AllIntersections = new List<IntersectionController>();
    public List<TrafficLightsController> AllTrafficLights = new List<TrafficLightsController>();
    public List<YayaController> AllYayaLights = new List<YayaController>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Sahnedeki tüm nesneleri otomatik olarak bul
        AllIntersections.AddRange(FindObjectsOfType<IntersectionController>());
        AllTrafficLights.AddRange(FindObjectsOfType<TrafficLightsController>());
        AllYayaLights.AddRange(FindObjectsOfType<YayaController>());
    }

    // Eðer runtime'da tekrar güncellemen gerekirse
    public void RefreshReferences()
    {
        AllIntersections.Clear();
        AllTrafficLights.Clear();
        AllYayaLights.Clear();

        AllIntersections.AddRange(FindObjectsOfType<IntersectionController>());
        AllTrafficLights.AddRange(FindObjectsOfType<TrafficLightsController>());
        AllYayaLights.AddRange(FindObjectsOfType<YayaController>());
    }
}
