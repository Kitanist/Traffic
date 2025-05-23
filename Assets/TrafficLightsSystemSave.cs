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
    public GameEvent OnTrafficEnable;// geçici duruma göre silebilirsiniz

    private string saveDirectory => Application.persistentDataPath + "/TrafficSaves";

    public Material  TransparentMaterial;
    void Awake()
    {
        // Easy Save 3 Manager’ın sahnede olduğundan emin ol
        if (GameObject.FindObjectOfType<ES3ReferenceMgr>() == null)
            Debug.LogError("Sahneye bir Easy Save 3 Manager eklemen gerekiyor!");
    
    }


    public void SaveAllState()
    {
        if (!System.IO.Directory.Exists(saveDirectory))
            System.IO.Directory.CreateDirectory(saveDirectory);

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        saveFileName = $"traffic_save_{timestamp}.es3";

        string fullPath = System.IO.Path.Combine(saveDirectory, saveFileName);

        var settings = new ES3Settings(fullPath);

        TrafficSystemManager.Instance.RefreshReferences();
        ıntersectionControllers = TrafficSystemManager.Instance.AllIntersections;
        lightsControllers = TrafficSystemManager.Instance.AllTrafficLights;
        yayaControllers = TrafficSystemManager.Instance.AllYayaLights;

        intersectionCount = ıntersectionControllers.Count;
        LightCount = lightsControllers.Count;
        YayaLightCount = yayaControllers.Count;

        ES3.Save("intersectionCount", intersectionCount, settings);
        ES3.Save("LightCount", LightCount, settings);
        ES3.Save("YayaLightCount", YayaLightCount, settings);
        ıntersectionTransforms.Clear();
        lightTransforms.Clear();
        YayaTransforms.Clear(); 
        int a = 0;
        foreach (var item in ıntersectionControllers)
        {
            ES3.Save($"intersection {a}", item, settings);
            int c = 0;
            foreach (var Group in item.groups)
            {
                int b = 0;
                foreach (var light in Group.lights)
                {
                    ES3.Save($"intersection{a}Group{c}Connection{b}", light.name, settings);
                    b++;
                }
                c++;
            }
            ıntersectionTransforms.Add(item.gameObject.transform);
            a++;
        }

        a = 0;
        foreach (var item in lightsControllers)
        {
            ES3.Save($"Light {a}", item, settings);
            ES3.Save($"LightName{a}", item.gameObject.name, settings);
            lightTransforms.Add(item.gameObject.transform.parent);
            a++;
        }

        a = 0;
        foreach (var item in yayaControllers)
        {
            ES3.Save($"YayaLight{a}", item, settings);
            YayaTransforms.Add(item.gameObject.transform);
            a++;
        }

        ES3.Save("lightTransforms", lightTransforms, settings);
        ES3.Save("ıntersectionTransforms", ıntersectionTransforms, settings);
        ES3.Save("YayaTransforms", YayaTransforms, settings);

        Debug.LogError("SAVE: " + saveFileName);
    }


    public void LoadAllState()
    {
        if (!System.IO.Directory.Exists(saveDirectory))
        {
            Debug.LogError("Save klasörü yok.");
            return;
        }

        string[] files = System.IO.Directory.GetFiles(saveDirectory, "*.es3");
        if (files.Length == 0)
        {
            Debug.LogError("Kayıtlı dosya bulunamadı.");
            return;
        }

        // En son oluşturulan dosyayı bul
        System.Array.Sort(files);
        string latestFile = files[files.Length - 1];
        saveFileName = System.IO.Path.GetFileName(latestFile);
        var settings = new ES3Settings(latestFile);

        Debug.LogError("Load: " + saveFileName);

        intersectionCount = ES3.Load("intersectionCount", 0, settings);
        LightCount = ES3.Load("LightCount", 0, settings);
        YayaLightCount = ES3.Load("YayaLightCount", 0, settings);

        ıntersectionTransforms = ES3.Load("ıntersectionTransforms", new List<Transform>(), settings);
        lightTransforms = ES3.Load("lightTransforms", new List<Transform>(), settings);
        YayaTransforms = ES3.Load("YayaTransforms", new List<Transform>(), settings);

        for (int i = 0; i < LightCount; i++)
        {
            var a = Instantiate(Light, SaveObject2.transform);
            ES3.LoadInto($"Light {i}", a.GetComponentInChildren<TrafficLightsController>(), settings);
            a.transform.position = lightTransforms[i].transform.position;
            a.transform.rotation = lightTransforms[i].transform.rotation;
            a.transform.GetChild(0).name = ES3.Load<string>($"LightName{i}", settings);
            a.GetComponentInChildren<TrafficLightsController>().SetFirstTimeClicked(false);
            OnLightPlaced.Raise();
        }

        for (int i = 0; i < intersectionCount; i++)
        {
            var a = Instantiate(ıntersection, SaveObject.transform);
            ES3.LoadInto($"intersection {i}", a.GetComponent<IntersectionController>(), settings);
            a.transform.position = ıntersectionTransforms[i].transform.position;
            var b = a.GetComponent<IntersectionController>();
            int j = 0;
            foreach (var group in b.groups)
            {
                for (int k = 0; k < group.lights.Count; k++)
                {
                    string key = $"intersection{i}Group{j}Connection{k}";
                    if (ES3.KeyExists(key, settings))
                    {
                        string g = ES3.Load<string>(key, settings);
                        group.lights[k] = GameObject.Find(g).GetComponent<TrafficLightsController>();
                    }
                    else
                    {
                        Debug.LogError($"Eksik: {key}");
                    }
                }
                j++;
            }

            if (!TGUIManager.Intersections.Contains(a))
                TGUIManager.Intersections.Add(a);

            TGUIManager.Controller = a.GetComponent<IntersectionController>();

            foreach (var renderer in a.GetComponentsInChildren<Renderer>())
                renderer.material = TransparentMaterial;
        }

        for (int i = 0; i < YayaLightCount; i++)
        {
            var a = Instantiate(YayaLight, SaveObject2.transform);
            ES3.LoadInto($"YayaLight{i}", a.GetComponent<YayaController>(), settings);
            a.transform.position = YayaTransforms[i].transform.position;
            a.transform.rotation = YayaTransforms[i].transform.rotation;
        }

        OnTrafficEnable.Raise();
    }

}
