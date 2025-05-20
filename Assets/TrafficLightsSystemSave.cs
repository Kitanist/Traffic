using System.Collections.Generic;
using UnityEngine;
using ES3Internal; // veya projenizdeki ES3 namespace’i
using UnityEngine.SceneManagement;

public class TrafficLightsSystemSave : MonoBehaviour
{
    [Header("Save File")]
    public string saveFileName;
    public GameObject SaveObject,SaveObject2;

    [Header("Prefabs")]
    public GameObject ıntersection;
    public GameObject Light;
    public GameObject YayaLight;

    public List<IntersectionController> ıntersectionControllers;
    public List<Transform> ıntersectionTransforms;
    public List<TrafficLightsController> lightsControllers;
    public List<Transform> lightTransforms;
    public List<YayaController> yayaControllers;
    public List<Transform> YayaTransforms;
    public int intersectionCount, LightCount,YayaLightCount;

    [Header("Dependicies")]
    public TrafficGroupUIManager TGUIManager;


    [Header("GameEvents")]
    public GameEvent OnLightPlaced;


    public Material  TransparentMaterial;
    void Awake()
    {
        // Easy Save 3 Manager’ın sahnede olduğundan emin ol
        if (GameObject.FindObjectOfType<ES3ReferenceMgr>() == null)
            Debug.LogError("Sahneye bir Easy Save 3 Manager eklemen gerekiyor!");
    
    }


    public void SaveAllState()
    {
        TrafficSystemManager.Instance.RefreshReferences();
        ıntersectionControllers =  TrafficSystemManager.Instance.AllIntersections;
        lightsControllers = TrafficSystemManager.Instance.AllTrafficLights;
        yayaControllers = TrafficSystemManager.Instance.AllYayaLights;
        intersectionCount = ıntersectionControllers.Count;
        LightCount = lightsControllers.Count;
        YayaLightCount = yayaControllers.Count;
        ES3.Save("intersectionCount", intersectionCount);
        ES3.Save("LightCount", LightCount);
        ES3.Save("YayaLightCount", YayaLightCount);
        int a = 0;
        foreach (var item in ıntersectionControllers)
        {
            ES3.Save($"intersection {a}",item);
            int c = 0;//her Grup İçin
            foreach (var Group in item.groups)
            {
                
                int b = 0;// her ışık için

                foreach (var light in Group.lights)
                {
                    ES3.Save($"intersection{a}Group{c}Connection{b}", light.name);
                    b++;
                }
                c++;

            }
            ıntersectionTransforms.Add(item.gameObject.transform);
            a++;
        }
        a = 0;
        
        foreach(var item in lightsControllers)
        {
            ES3.Save($"Light {a}",item);
            ES3.Save($"LightName{a}", item.gameObject.name);
            lightTransforms.Add(item.gameObject.transform.parent);
            a++;
        }
        a = 0;
        foreach (var item in yayaControllers)
        {
            ES3.Save($"YayaLight{a}",item);
            YayaTransforms.Add(item.gameObject.transform);
            a++;
        }
        ES3.Save("lightTransforms", lightTransforms);
        ES3.Save("ıntersectionTransforms", ıntersectionTransforms);// Pozisyon bilgileri 
        ES3.Save("YayaTransforms", YayaTransforms);
        Debug.LogError("SAVE");
    }

    public void LoadAllState()
    {
        intersectionCount=ES3.Load("intersectionCount", intersectionCount);
        LightCount=ES3.Load("LightCount", LightCount);
        YayaLightCount = ES3.Load("YayaLightCount", YayaLightCount);
        ıntersectionTransforms = ES3.Load("ıntersectionTransforms", ıntersectionTransforms);
        lightTransforms = ES3.Load("lightTransforms", lightTransforms);
        YayaTransforms = ES3.Load("YayaTransforms", YayaTransforms);

        Debug.LogError("Load");
        for (int i = 0; i < LightCount; i++)// Işık Build
        {
            var a = Instantiate(Light, SaveObject2.transform);
            ES3.LoadInto($"Light {i}", a.GetComponentInChildren<TrafficLightsController>());
            a.transform.position = lightTransforms[i].transform.position;
            a.transform.GetChild(0).name = ES3.Load<string>($"LightName{i}");
            OnLightPlaced.Raise();
        }
        for (int i = 0; i < intersectionCount; i++)// Kavşak Build
        {
            var a = Instantiate(ıntersection, SaveObject.transform);
            ES3.LoadInto($"intersection {i}", a.GetComponent<IntersectionController>());
            a.transform.position = ıntersectionTransforms[i].transform.position;
            var b = a.GetComponent<IntersectionController>();
            int j = 0;
            foreach (var group in b.groups)
            {
                for (int k = 0; k < group.lights.Count; k++)
                {
                    string key = $"intersection{i}Group{j}Connection{k}";
                    if (ES3.KeyExists(key))
                    {
                        string g = (string)ES3.Load(key); // açık cast
                        group.lights[k] = GameObject.Find(g).GetComponent<TrafficLightsController>();
                    }
                    else
                    {
                        Debug.Log(key);
                        Debug.LogError("Çalışmadı");
                    }
                }
                j++;
            }
            if (!TGUIManager.Intersections.Contains(a))
            {
                TGUIManager.Intersections.Add(a);
            }
            TGUIManager.Controller = a.GetComponent<IntersectionController>();
          
            // Debug.LogError("OnIntersectionPlaced");
            foreach (var renderer in a.GetComponentsInChildren<Renderer>())
            {
                renderer.material = TransparentMaterial;
            }
        }
        for (int i = 0; i < YayaLightCount; i++)
        {
            var a = Instantiate(YayaLight,SaveObject2.transform);
            ES3.LoadInto($"YayaLight{i}",a.GetComponent<YayaController>());
            a.transform.position = YayaTransforms[i].transform.position;
        }
    }
}
