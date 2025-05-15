using UnityEngine;

public class PedestrianLightSpawner : MonoBehaviour
{
    public GameObject pedestrianLightPrefab;
    public GameObject ghostPrefab;
    public Transform trafficLightTransform;
    public float offsetDistance = 1.5f;
    public bool IsOldPrefab=false;
    public TrafficLightUI TLUI;
    public TrafficLightsController TLCthis;
    public GameEvent OnYayaLightSpawned;

    private int directionIndex = 0;
    private const int maxDirections = 8;
    private GameObject ghostInstance;
    public bool IsPlacing;


    private void Start()
    {
        TLUI = FindObjectOfType<TrafficLightUI>();
    }
    void Update()
    {
        if (!IsPlacing) return; 
        // Yön deðiþtirme
        if (Input.GetKeyDown(KeyCode.Q))
            directionIndex = (directionIndex + maxDirections - 1) % maxDirections;
        else if (Input.GetKeyDown(KeyCode.E))
            directionIndex = (directionIndex + 1) % maxDirections;

        // Gerçek spawn
        if (Input.GetKeyDown(KeyCode.Space)&&TLCthis==TLUI.TargetLight)
            SpawnRealLight();

        // Ghost'u güncelle
        if (IsPlacing)
            UpdateGhostPreview();
    }
    public void StartSpawnYayaLight(Transform parent)
    {
       // Debug.Log("SAA");
       // if (IsOldPrefab)
            parent = parent.parent;
        if (ghostInstance)
        {
            IsPlacing = true;
        }
        else
        {
            ghostInstance = Instantiate(ghostPrefab, parent);
            IsPlacing = true;
        }
       
    }
    void UpdateGhostPreview()
    {
        float angle = directionIndex * 45f;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 offset = rotation * Vector3.forward * offsetDistance;

        ghostInstance.transform.SetLocalPositionAndRotation(offset, rotation);
        if (!IsOldPrefab)
           ghostInstance.transform.localScale = new Vector3(2,2,2);
    }

    void SpawnRealLight()
    {
        float angle = directionIndex * 45f;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 offset = rotation * Vector3.forward * offsetDistance;

        GameObject pedestrianLight = Instantiate(pedestrianLightPrefab, trafficLightTransform);
        pedestrianLight.transform.SetLocalPositionAndRotation(offset, rotation);
        if (IsOldPrefab)
        pedestrianLight.transform.localScale = Vector3.one;
       // Debug.Log(ghostInstance, ghostInstance);
      if(ghostInstance!= null)  Destroy(ghostInstance);

        IsPlacing = false;
        OnYayaLightSpawned.Raise();
    }
}
