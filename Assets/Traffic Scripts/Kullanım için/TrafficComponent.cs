using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;
using Gley.TrafficSystem.Internal;
using System;
using System.Linq;

namespace Gley.TrafficSystem
{
    
    public class TrafficComponent : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI AracSayi,AktifkareSayisi,RemoveVehicleDistance,AddVehicleDistance,GreenTime,YellowTime,RedTime,CarDensity;
        public Image SiyahPerde;
        public GameObject Cam,Cam1,TrafficSettingsMenu;
        public Transform pos1, pos2;
        [Header("Required Settings")]
        [Tooltip("Player is used to instantiate vehicles out of view")]
        public Transform player;
        [Tooltip("Max number of possible vehicles. Cannot be increased during game-play")]
        public int nrOfVehicles = 1;
        [Tooltip("List of different vehicles (Right Click->Create->Traffic System->Vehicle Pool)")]
        public VehiclePool vehiclePool;

        [Header("Optional Settings")]

        [Header("Spawning")]
        [Tooltip("Square located at this distance from the player are actively update. Ex: if set is to 2 -> intersections will update on a 2 square distance from the player")]
        public int activeSquareLevels = 1;
        [Tooltip("Minimum distance from the player where a vehicle can be instantiated. (If -1 the system will automatically determine this value)")]
        public float minDistanceToAdd = -1;
        [Tooltip("Distance from the player where a vehicle can be removed. (If -1 the system will automatically determine this value)")]
        public float distanceToRemove = -1;

        [Header("Intersection")]
        [Tooltip("How long yellow light is on. (If -1 the value from the intersection component will be used)")]
        public float yellowLightTime = -1;
        [Tooltip("How long green light is on. (If -1 the value from the intersection component will be used)")]
        public float greenLightTime = -1;

        [Header("Density")]
        [Tooltip("Nr of vehicles instantiated around the player from the start. Set it to something < nrOfVehicles for low density right at the start. (If -1 all vehicles will be instantiated from the beginning)")]
        public int initialActiveVehicles = -1;
        [Tooltip("Set high priority on roads for higher traffic density(ex highways). See priority setup")]
        public bool useWaypointPriority = false;


        [Header("Lights")]
        [Tooltip("Set the initial state of the car lights")]
        public bool lightsOn = false;

        [Header("Waypoints")]
        [Tooltip("Area to disable from the start if cars are not allowed to spawn there")]
        public Area disableWaypointsArea = default;
        int a = 0,o = 0;
      //  public TMP_InputField activeGridInput;
      //  public TMP_InputField minDistanceInput;
      //  public TMP_InputField removeDistanceInput;
      //  public TMP_InputField greenLightInput;
      //  public TMP_InputField yellowLightInput;
      //  //public TMP_InputField redLightInput;
      //  public TMP_InputField vehicleDensityInput;

        #endregion

        #region MonoBehaviorFuncs
        void Start()
        {
            Application.targetFrameRate = 120;
            //AracSayi.text = " ";
            //  SiyahPerde.DOFade(0, 10);
            //  Cam.transform.DOMove(new Vector3(Cam.transform.position.x, Cam.transform.position.y-50, Cam.transform.position.z) ,15f,false);
           
            // 
            a = 0;
            TrafficOptions options = new()
            {
                activeSquaresLevel = activeSquareLevels,
                disableWaypointsArea = disableWaypointsArea,
                distanceToRemove = distanceToRemove,
                greenLightTime = greenLightTime,
                initialDensity = initialActiveVehicles,
                lightsOn = lightsOn,
                minDistanceToAdd = minDistanceToAdd,
                useWaypointPriority = useWaypointPriority,
                yellowLightTime = yellowLightTime,
            };
            API.Initialize(player, nrOfVehicles, vehiclePool, options);  
            InvokeRepeating(nameof(TempPosUpdate), 0, 4);

        }
        public void StartTraffic()
        {
        //    TrafficSettingsMenu.SetActive(false);
           
           

        }
        Vector3 TempPos;
        public void TempPosUpdate()
        {
           
                TempPos = player.position;
                a = 0;
        }
        public void Update()
        {
            float distance = Vector3.Distance(TempPos,player.position);
            if (distance > 450 && a == 0)
            {
                a = 1;
                RemoveAllDistancedVehicles();
            }
          //  if (Input.GetKeyDown(KeyCode.B))
          //  {
          //      GetActiveVehicles(); 
          //  }
          //  if (Input.GetKeyDown(KeyCode.M))
          //  {
          //     
          //  
          //  }
          //if (Input.GetKeyDown(KeyCode.N))
          //{
          //    StartCoroutine(Blinking(2f, pos1.position));
          //    Invoke(nameof(NotBlinking), 2.5f);
          //
          //}
          //if (Input.GetKeyDown(KeyCode.H))
          //{
          //   StartCoroutine( Blinking(2f, pos2.position));
          //    Invoke(nameof(NotBlinking),2.5f);
          //        
          //} 
             
        } 

        #endregion

        #region TrafficFuncs

    //   public void UpdateValues()
    //    {
    //        // Kullanıcı girişlerini değişkenlere ata
    //        if (int.TryParse(activeGridInput.text, out int activeGrid))
    //            activeSquareLevels = activeGrid;
    // 
    //        if (float.TryParse(minDistanceInput.text, out float minDist))
    //            minDistanceToAdd = minDist;
    // 
    //        if (float.TryParse(removeDistanceInput.text, out float removeDist))
    //            distanceToRemove = removeDist;
    // 
    //        if (float.TryParse(greenLightInput.text, out float greenTime))
    //            greenLightTime = greenTime;
    // 
    //        if (float.TryParse(yellowLightInput.text, out float yellowTime))
    //            yellowLightTime = yellowTime;
    // 
    //     //   if (float.TryParse(redLightInput.text, out float redTime))
    //     //       redLightTime = redTime;
    // 
    //        if (int.TryParse(vehicleDensityInput.text, out int density))
    //            initialActiveVehicles = density;
    // 
    //        Debug.Log("Traffic system values updated.");
    //    }
       
        public VehicleComponent[] GetAllVehicleCount()
        {
            var a = API.GetAllVehicles();
            return a;
            // Debug.Log(a.Length);
        }
        public void GetActiveVehicles()
        {
            var a = GetAllVehicleCount();
            int c = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].gameObject.activeSelf)
                {
                    c++;
                }

            }
            AracSayi.text = "Guncel Araç Sayısı : " + c.ToString();
            Debug.Log("Aktif Araç Sayısı Şu Anda Toplam " + c.ToString());
            Invoke(nameof(GetActiveVehicles), 1);
        }
        public void TrafikDolsun(int doluluk)
        {
             API.SetTrafficDensity(doluluk);
            
        }
        public IEnumerator Blinking(float BlinkTime,Vector3 TargetPos)
        {
      //      SiyahPerde.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            Cam1.transform.position = TargetPos;
          //  API.SetTrafficDensity(400);
            API.RemoveAndAddVehicleOnCamChanged();
            yield return new WaitForSeconds(0.2f);
            API.RemoveAndAddVehicleOnCamChanged();
            yield return new WaitForSeconds(0.2f);
            API.RemoveAndAddVehicleOnCamChanged();
            yield return new WaitForSeconds(0.2f);
            API.RemoveAndAddVehicleOnCamChanged();
            yield return new WaitForSeconds(0.3f);

            //  AracSayi.text = "Trafik Yoğunluğu ayarlanıyor";
            //SiyahPerde.DOFade(0f, BlinkTime).SetEase(Ease.InQuart);
           

        }
        public void NotBlinking()
        {
            Debug.Log("Yey");
        //    API.ClearTrafficOnArea(pos1.position,700);
              RemoveAllDistancedVehicles();

        }
        public void RemoveAllDistancedVehicles()
        {
            List<GameObject> Cars = new();
            var a = TrafficManager.Instance.AllVehiclesDataHandler.GetAllVehicles();
            Profiler.BeginSample("RemoveAllDVForeach");
            foreach (var b in a)
            {
                if (b.gameObject.activeSelf) { 
                float distance = Vector3.Distance(b.gameObject.transform.position, player.position);
                if (distance > 600f)
                {
                    Cars.Add(b.gameObject);
                }
                }
            }

           StartCoroutine( RemoveParts(Cars));

           
        }
        public IEnumerator RemoveParts(List<GameObject> Cars)
        {
            int partCount = 8;
            int chunkSize = (int)Math.Ceiling(Cars.Count / (double)partCount);
            List<List<GameObject>> chunks = new();

            for (int i = 0; i < Cars.Count; i += chunkSize)
            {
                chunks.Add(Cars.Skip(i).Take(chunkSize).ToList());
            }

            // Örnek olarak her parçadaki araçları sil
            for (int i = 0; i < chunks.Count; i++)
            {
                switch (o)
                {
                    case 0:

                        API.RemoveAllVehicles(chunks[o]);
                        o++;
                        break;
                    case 1:

                        API.RemoveAllVehicles(chunks[o]);
                        o++;

                        break;
                    case 2:

                        API.RemoveAllVehicles(chunks[o]);
                        o++;

                        break;
                    case 3:

                        API.RemoveAllVehicles(chunks[o]);
                        o++;

                        break;
                    case 4:
                        API.RemoveAllVehicles(chunks[o]);
                        o++;
                        break;
                    case 5:
                        API.RemoveAllVehicles(chunks[o]);
                        o++;
                        break;
                    case 6:
                        API.RemoveAllVehicles(chunks[o]);
                        o++;
                        break;
                    case 7:
                        API.RemoveAllVehicles(chunks[o]);
                        o = 0;
                        break;
                    default:

                        break;
                }
                yield return new WaitForSeconds(0.3f);
            }
        }
        #endregion
    }
}
