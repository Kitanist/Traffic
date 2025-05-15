using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gley.TrafficSystem.Internal;
using Gley.TrafficSystem;
using UnityEngine.UI;
using Gley.UrbanSystem.Internal;
using System.Linq;
using System;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

public class TrafficUIManager : MonoBehaviour
{
    #region Variables
    public DebugSettings _save;
    public float DisableAreaRadius;
    public TextMeshProUGUI WaypointName, RoadName, MenuName, Penalty, AllowedCars, LaneWidth, MaxSpeed, waypointDistance, LaneWidthRoad, MaxSpeedRoad, Fps, MaxFps, MinFps, laneSpeed, AllowedCarsLane;
    public bool LaneDirection;
    public TrafficWaypointsData trafficWaypointsDatas;
    public GameObject EditMenu, EditWaypointMenu, EditRoadMenu, LaneEditMenu;
    public List<int> DisabledWaypoints;
    public Material WaypointMat, SelectedWaypointMat;
    public TMP_Dropdown Lanes;
    public Mesh Mesh;
    public Sprite LaneSprite;
    public TMP_InputField LaneSpeed, WaypointWidth, Waypointname, LaneSpeedEdit;
    public float MaxFpsf, MinFpsf = 100;
    public int SelectedLane;
    public Road SelectedRoad;
    public Toggle carToggle;
    public Toggle busToggle;
    private Vector3 PointerPos;
    private bool isSelectingArea = false, isSelectingWaypoint = false, isSelectingVehicle = false, isSpawnCar = false, isSelectingRoad = false;
    private int SelectedVehicleIndex, SelectedWaypointIndex;
    
    public Transform AvrupaBusSpawn, AsyaBusSpawn;
    
    public TextMeshProUGUI debugText;
    public bool isCarsActive = true;

    #endregion

    #region ButtonFunc
    GameObject FindNearbyObject(Vector3 position, float radius, string tag)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius); // Çember içinde colliderlarý bul
        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag(tag)) // Tag kontrolü
            {
                return col.gameObject; // Ýlk bulunaný döndür
            }
        }
        return null; // Bulunamazsa null döndür
    }
    public void EnableAllWaypoints()
    {
        DisabledWaypoints.Clear();
        trafficWaypointsDatas.DisabledWaypoints = DisabledWaypoints;

        API.EnableAllWaypoints();
    }

    public void DisableAreaWaypoints()
    {
        isSelectingArea = true;
    }
    public void AAACarMeshChange(bool value)
    {
        isCarsActive = value;
        var allcar = API.GetAllVehicles();
        foreach (var vehicle in allcar)
        {
            var rendererList = new List<MeshRenderer>();
            if (vehicle.ListIndex < 848)
            {
                if (vehicle.gameObject.activeSelf)
                {
                    Transform carHolder = vehicle.gameObject.transform.GetChild(0);
                    rendererList = carHolder.GetComponentsInChildren<MeshRenderer>(true).ToList(); // bunun çocuðunun çocuðunu kapa
                    Transform tekerler = carHolder.Find("Tekerler");
                //   Debug.LogError(vehicle.ListIndex);
                //   Debug.LogError(tekerler.name);
                    for (int i = 0; i < tekerler.childCount; i++)
                    {
                        rendererList.Add(tekerler.GetChild(i).GetComponentInChildren<MeshRenderer>());
                    }
                }
                
            }
          //  Debug.LogError("SAyýýý:"+rendererList.Count);
            foreach (var renderer in rendererList)
            {
                renderer.enabled = value;
            }

        }
    }

    public void SelectWaypoint()
    {
        isSelectingWaypoint = true;
    }

    public void SelectVehicle()
    {
        isSelectingVehicle = true;
    }

    public void SpawnCar()
    {
        isSpawnCar = true;
    }
    public void SelectRoad()
    {
        isSelectingRoad = true;
    }
    public void SetWaypointDistance(string value)
    {
        SelectedRoad.waypointDistance = int.Parse(value);
        UpdateRoadUI();
    }
    public void Lines()
    {
        if (SelectedRoad == null)
        {
            Debug.LogError("SelectedRoad nesnesi null!");
            return;
        }

        if (SelectedRoad.lanes == null || SelectedRoad.lanes.Count == 0)
        {
            Debug.LogError("SelectedRoad.lanes listesi boþ!");
            return;
        }

        Lanes.ClearOptions(); // Dropdown'ý temizle

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int a = 0;
        foreach (var lane in SelectedRoad.lanes)
        {
            a++;
            Debug.Log($"Lane: {lane} {a}"); // Ýçeriði logla
            options.Add(new TMP_Dropdown.OptionData("lane " + a.ToString()));
        }
        a = 0;
        Lanes.AddOptions(options);
        Lanes.RefreshShownValue(); // Dropdown'u güncelle
    }

    public void SelectLane(int lane)
    {
        SelectedLane = lane;
        LaneSettingsMenuOpen();

    }
    public void LaneSettingsMenuOpen()
    {
        string a;
        if (SelectedRoad.lanes[SelectedLane].allowedCars[0]) { a = "car"; }
        else { a = ""; }
        LaneEditMenu.SetActive(true);
        laneSpeed.text = "Þerit Hýzý : " + SelectedRoad.lanes[SelectedLane].laneSpeed.ToString();
        AllowedCarsLane.text = "Ýzin verilen araçlar : " + a;
        LaneDirection = SelectedRoad.lanes[SelectedLane].laneDirection;
    }
    public void SetRoadWidth(string value)
    {
        if (int.TryParse(value, out int width))
        {
            SelectedRoad.laneWidth = width;
            UpdateRoadUI();
        }
        else
        {
            Debug.LogError($"Geçersiz giriþ: {value}. Lütfen geçerli bir tam sayý girin.");
        }
    }

    public void SetAllowedCars()
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);

        AllowedCars.text = "Ýzin verilen araçlar :" + GetAllowedVehiclesText(waypoint.AllowedVehicles);
        UpdateWaypointUI();
    }
    public void SetLaneWidth(string value)
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);
        waypoint.LaneWidth = int.Parse(value);
        UpdateWaypointUI();

    }
    public void SetName(string value)
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);
        waypoint.Name = value;
        UpdateWaypointUI();
    }
    public void SetMaxSpeed(string value)
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);
        waypoint.MaxSpeed = int.Parse(value);
        UpdateWaypointUI();
    }
    public void UpdateRoadUI()
    {
        string a;
        if (SelectedRoad.lanes[SelectedLane].allowedCars[0]) { a = "car"; }
        else { a = ""; }
        MenuName.text = "Road Editör (Demo)";
        RoadName.text = " Þerit Sayýsý: " + SelectedRoad.lanes.Count.ToString();
        MaxSpeedRoad.text = "Hýz Limiti:" + SelectedRoad.lanes[0].laneSpeed.ToString();
        LaneWidthRoad.text = "Þerit kalýnlýðý:" + SelectedRoad.laneWidth.ToString();
        waypointDistance.text = "Yol noktasý kalýnlýðý:" + SelectedRoad.waypointDistance.ToString();
        laneSpeed.text = "Þerit Hýzý : " + SelectedRoad.lanes[SelectedLane].laneSpeed.ToString();
        AllowedCarsLane.text = "Ýzin verilen araçlar : " + a;
    }
    public void UpdateWaypointUI()
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);


        MenuName.text = "Waypoint Editör (Demo)";
        WaypointName.text = waypoint.Name;
        AllowedCars.text = "Ýzin verilen araçlar :" + GetAllowedVehiclesText(waypoint.AllowedVehicles);
        LaneWidth.text = "Waypoint mesafesi:" + waypoint.LaneWidth.ToString();
        MaxSpeed.text = "Hýz Limiti:" + waypoint.MaxSpeed.ToString();

    }

    public void EditRoad()
    {
        EditMenu.SetActive(true);
        EditWaypointMenu.SetActive(false);
        EditRoadMenu.SetActive(true);
        LaneEditMenu.SetActive(false);

        MenuName.text = "Road Editör (Demo)";
        RoadName.text = " Þerit Sayýsý: " + SelectedRoad.lanes.Count.ToString();
        MaxSpeedRoad.text = "Hýz Limiti:" + SelectedRoad.lanes[0].laneSpeed.ToString();
        LaneWidthRoad.text = "Þerit kalýnlýðý:" + SelectedRoad.laneWidth.ToString();
        waypointDistance.text = "Yol noktasý kalýnlýðý:" + SelectedRoad.waypointDistance.ToString();
        Lines();
    }

    public void DisableSingleWaypoint()
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);

        List<GameObject> waypointSettings = FindObjectsOfType<WaypointSettings>()
                                   .Select(w => w.gameObject)
                                   .ToList();
        foreach (var obj in waypointSettings)
        {
            if (obj.GetComponent<WaypointSettings>().position == waypoint.Position)
            {
                if (waypoint.TemporaryDisabled)
                {
                    obj.GetComponent<WaypointSettings>().TempDis = true;
                }

            }
        }

        DisabledWaypoints.Add(SelectedWaypointIndex);
        trafficWaypointsDatas.DisabledWaypoints = DisabledWaypoints;
        API.DisableSingleWaypoint(SelectedWaypointIndex, true);
    }
    public void EnableSingleWaypoint()
    {
        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);

        List<GameObject> waypointSettings = FindObjectsOfType<WaypointSettings>()
                                   .Select(w => w.gameObject)
                                   .ToList();
        foreach (var obj in waypointSettings)
        {
            if (obj.GetComponent<WaypointSettings>().position == waypoint.Position)
            {
                if (waypoint.TemporaryDisabled)
                {
                    obj.GetComponent<WaypointSettings>().TempDis = false;
                }

            }
        }

        DisabledWaypoints.Remove(SelectedWaypointIndex);
        trafficWaypointsDatas.DisabledWaypoints = DisabledWaypoints;
        API.DisableSingleWaypoint(SelectedWaypointIndex, false);
    }
    public void DisableLaneWaypoints()
    {
        Transform parent = SelectedRoad.lanes[SelectedLane].laneEdges.inConnector.gameObject.transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            var a = API.GetClosestWaypoint(child.position);
            API.DisableSingleWaypoint(a, true);
        }

    }
    public void AssignToggles()
    {
        if (carToggle.isOn && busToggle.isOn)
        {
            ChangeVehicleType(new VehicleTypes[] { VehicleTypes.Car, VehicleTypes.Bus });
        }
        else if (carToggle.isOn)
        {
            ChangeVehicleType(new VehicleTypes[] { VehicleTypes.Car });
        }
        else if (busToggle.isOn)
        {
            ChangeVehicleType(new VehicleTypes[] { VehicleTypes.Bus });
        }
        else
        {
            ChangeVehicleType(new VehicleTypes[] { }); // Boþ bir dizi (hiçbir araç giremez)
        }
    }

    public void ChangeVehicleType(VehicleTypes[] vehicleTypes)
    {
        Transform parent = SelectedRoad.lanes[SelectedLane].laneEdges.inConnector.gameObject.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            var a = API.GetWaypointFromIndex(API.GetClosestWaypoint(child.position));

            a.AllowedVehicles = vehicleTypes;
        }
    }
    public void ChangeIsOvertake()
    {
        Transform parent = SelectedRoad.lanes[SelectedLane].laneEdges.inConnector.gameObject.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            var a = API.GetWaypointFromIndex(API.GetClosestWaypoint(child.position));
            if (a.tempOtherLanes == null)
            {
                a.tempOtherLanes = a.OtherLanes;
                int[] b = new int[0];
                a.OtherLanes = b;
            }
            else
            {
                a.OtherLanes = a.tempOtherLanes;
                int[] b = new int[0];
                a.tempOtherLanes = b;
            }

        }
        Debug.LogError("Sollama izni iptal edildi");
    }


    public void SetDebug(bool debug)
    {
        if (debug)
        {
            _save.debug = true;
        }
        else
        {
            _save.debug = false;

        }
    }

    public void ChangeRoadSpeed(string value)
    {
        foreach (var lane in SelectedRoad.lanes)
        {
            lane.laneSpeed = int.Parse(value);
            Debug.Log("Gelen Deðer: " + lane.laneSpeed);
            UpdateRoadUI();
        }
    }
    public void ChangeLaneSpeed(string value)
    {

        SelectedRoad.lanes[SelectedLane].laneSpeed = int.Parse(value);
        //  Debug.Log("Gelen Deðer: " + lane.laneSpeed);
        UpdateRoadUI();


    }

    private string GetAllowedVehiclesText(VehicleTypes[] allowedCars)
    {
        List<string> vehicleList = new List<string>();

        foreach (VehicleTypes vehicleType in allowedCars)
        {
            vehicleList.Add(vehicleType.ToString());
            Debug.Log(vehicleType.ToString());
        }

        return string.Join(", ", vehicleList);
    }
    public void EditWaypoint()
    {
        EditMenu.SetActive(true);
        EditWaypointMenu.SetActive(true);
        EditRoadMenu.SetActive(false);
        LaneEditMenu.SetActive(false);

        TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);
        Debug.Log(waypoint.Neighbors.Length);
        MenuName.text = "Waypoint Editör (Demo)";
        WaypointName.text = waypoint.Name;
        AllowedCars.text = "Ýzin verilen araçlar :" + GetAllowedVehiclesText(waypoint.AllowedVehicles);
        LaneWidth.text = "Waypoint mesafesi::" + waypoint.LaneWidth.ToString();
        MaxSpeed.text = "Hýz Limiti:" + waypoint.MaxSpeed.ToString();
        List<GameObject> waypointSettings = FindObjectsOfType<WaypointSettings>()
                                     .Select(w => w.gameObject)
                                     .ToList();
        foreach (var obj in waypointSettings)
        {
            if (obj.GetComponent<WaypointSettings>().position == waypoint.Position)
            {
                if (waypoint.TemporaryDisabled)
                {
                    obj.GetComponent<WaypointSettings>().TempDis = true;
                }

                obj.GetComponent<MeshRenderer>().material = SelectedWaypointMat;
            }
        }



    }
    #endregion
    public void ShowFps()
    {
        float fps = 1.0f / Time.deltaTime;
        Fps.text = "FPS: " + Mathf.RoundToInt(fps).ToString();
        if (fps > MaxFpsf)
        {
            MaxFpsf = fps;
            MaxFps.text = "Max FPS: " + Mathf.RoundToInt(MaxFpsf);
        }

        // Minimum FPS güncelleme
        if (fps < MinFpsf)
        {
            MinFpsf = fps;
            MinFps.text = "Min FPS: " + Mathf.RoundToInt(MinFpsf);
            Debug.Log($"Min Fps : {MinFpsf}");
        }
    }
    public void StartTraffic()
    {
        EditMenu.SetActive(true);
        trafficWaypointsDatas = MonoBehaviourUtilities.GetOrCreateObjectScript<TrafficWaypointsData>(TrafficSystemConstants.PlayHolder, false);
        DisabledWaypoints = trafficWaypointsDatas.DisabledWaypoints;
        foreach (var item in DisabledWaypoints)
        {
            TrafficManager.Instance.TrafficWaypointsDataHandler.SetTemperaryDisabledValue(item, true);
        }
        ShowAllWP();
        carToggle.onValueChanged.AddListener(delegate { AssignToggles(); });
        busToggle.onValueChanged.AddListener(delegate { AssignToggles(); });

    }
    #region monoBehaviorFuncs

    private void Start()
    {
        //   EditMenu.SetActive(false);



    }
    


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //ShowFps();

        if (Physics.Raycast(ray, out hit))
        {
            PointerPos = hit.point;

        }
        if (isSelectingArea && Input.GetMouseButtonDown(0))
        {


            if (Physics.Raycast(ray, out hit))
            {
                PointerPos = hit.point;
                API.DisableAreaWaypoints(PointerPos, DisableAreaRadius);
                isSelectingArea = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {

        }



        if (isSelectingVehicle && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                PointerPos = hit.point;
                Transform selectedTransform = hit.collider.transform;

                if (hit.collider.gameObject.name == "FrontTrigger")
                {
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                }
                else
                {
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                }

                GameObject selectedVehicle = selectedTransform.gameObject;
                int vehicleIndex = API.GetVehicleIndex(selectedVehicle);

                Debug.Log("Seçilen Araç: " + selectedVehicle.name);
                Debug.Log("Araç Indexi: " + vehicleIndex);
                SelectedVehicleIndex = vehicleIndex;

                isSelectingVehicle = false;
            }
        }
        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
        {
            if (Physics.Raycast(ray, out hit))
            {
                PointerPos = hit.point;
                Transform selectedTransform = hit.collider.transform;

                if (hit.collider.gameObject.name == "FrontTrigger")
                {
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                }
                else
                {
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                }

                GameObject selectedVehicle = selectedTransform.gameObject;
                if (selectedVehicle != null && selectedVehicle.layer == 9)
                {
                    int vehicleIndex = API.GetVehicleIndex(selectedVehicle);
                    Debug.Log("Seçilen Araç: " + selectedVehicle.name);
                    Debug.Log("Araç Indexi: " + vehicleIndex);
                    API.RemoveVehicle(selectedVehicle);

                }
            }
        }

        if (isSelectingWaypoint && Input.GetMouseButtonDown(0))
        {


            if (Physics.Raycast(ray, out hit))
            {
                PointerPos = hit.point;
                int waypointIndex = API.GetClosestWaypoint(hit.point);
                Debug.Log(waypointIndex);
                SelectedWaypointIndex = waypointIndex;
                EditWaypoint();
                isSelectingWaypoint = false;
            }
        }
        if (isSelectingRoad && Input.GetMouseButtonDown(0))
        {


            if (Physics.Raycast(ray, out hit))
            {
                Vector3 WaypointPos = Vector3.zero;
                PointerPos = hit.point;
                int waypointIndex = API.GetClosestWaypoint(hit.point);
                Debug.Log(waypointIndex);
                SelectedWaypointIndex = waypointIndex;
                Waypoint waypointobject = API.GetWaypointFromIndex(waypointIndex);
                // SelectedWaypointIndex = waypointIndex;
                WaypointPos = waypointobject.Position;
                Debug.Log($"WaypointPos : {WaypointPos}   WaypointIndex  : {waypointIndex}   waypointObject  :  {waypointobject.Name}");

                TrafficWaypoint waypoint = API.GetWaypointFromIndex(SelectedWaypointIndex);

                List<GameObject> waypointSettings = FindObjectsOfType<WaypointSettings>()
                                            .Select(w => w.gameObject)
                                            .ToList();
                foreach (var obj in waypointSettings)
                {
                    if (obj.GetComponent<MeshFilter>() == null)
                    {
                        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                        meshFilter.mesh = Mesh;
                    }

                    if (obj.GetComponent<MeshRenderer>() == null)
                    {
                        MeshRenderer a = obj.AddComponent<MeshRenderer>();
                        a.material = WaypointMat;
                    }
                    if (obj.GetComponent<BoxCollider>() == null)
                    {
                        BoxCollider a = obj.AddComponent<BoxCollider>();
                        a.isTrigger = true;
                    }
                    obj.tag = "WaypointObj";


                    if (obj.GetComponent<WaypointSettings>().position == waypoint.Position)
                    {

                        if (waypoint.TemporaryDisabled)
                        {
                            obj.GetComponent<WaypointSettings>().TempDis = true;
                            obj.GetComponent<MeshRenderer>().material = SelectedWaypointMat;
                        }


                    }


                }
                foreach (var obj in waypointSettings)
                {
                    if (obj.GetComponent<WaypointSettings>().position == waypoint.Position)
                    {
                        var Road = obj.transform.parent.parent;
                        foreach (Transform Lane in Road.transform)
                        {
                            foreach (Transform waypoints in Lane.transform)
                            {
                                waypoints.GetComponent<MeshRenderer>().material = SelectedWaypointMat;

                            }
                        }


                    }
                }



                GameObject waypointGameObject = FindNearbyObject(WaypointPos, 20f, "WaypointObj");
                if (waypointGameObject != null)
                {
                    Debug.Log(waypointGameObject.name);
                    var selectedTransform = waypointGameObject.transform;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    if (selectedTransform.parent != null) selectedTransform = selectedTransform.parent;
                    Road selectedroad = selectedTransform.gameObject.GetComponent<Road>();
                    SelectedRoad = selectedroad;
                    Debug.Log(selectedroad.lanes + "            " + selectedroad.nrOfLanes);
                }



                isSelectingRoad = false;


                EditRoad();
            }

        }

    }
    public void ShowAllWP()
    {
        List<GameObject> waypointSettings = FindObjectsOfType<WaypointSettings>()
                                            .Select(w => w.gameObject)
                                            .ToList();
        foreach (var obj in waypointSettings)
        {
            if (obj.GetComponent<MeshFilter>() == null)
            {
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                meshFilter.mesh = Mesh;
            }

            if (obj.GetComponent<MeshRenderer>() == null)
            {
                MeshRenderer a = obj.AddComponent<MeshRenderer>();
                a.material = WaypointMat;
            }
            if (obj.GetComponent<BoxCollider>() == null)
            {
                BoxCollider a = obj.AddComponent<BoxCollider>();
                a.isTrigger = true;
            }
            obj.tag = "WaypointObj";
        }
    }

    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        if (PointerPos != Vector3.zero)
        {
            // DisableAreaWaypoints - Kýrmýzý Çember
            if (isSelectingArea)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(PointerPos, DisableAreaRadius);
            }

            // Araç Spawn - Mavi Küre
            if (isSpawnCar)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(PointerPos, 1f);
            }

            // Araç Seçme - Yeþil Küre
            if (isSelectingVehicle)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(PointerPos, 1f);
            }

            // Waypoint Seçme - Sarý Küre
            if (isSelectingWaypoint)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(PointerPos, 0.5f);
            }
        }
    }

    #endregion
}
