using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class BuildManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject buildPanel;

    [Header("Prefabs")]
    public GameObject[] buildPrefabs;
    public Material ghostMaterial,TransparentMaterial;
    [Header("Dependicies")]
    public TrafficGroupUIManager TGUIManager;

    [Header("GameEvents")]
    public GameEvent OnLightPlaced;
    public GameEvent OnIntersectionPlaced;

    private GameObject currentPrefabToPlace;
    private GameObject ghostObject;
    private bool isPlacing = false;
    private Quaternion ghostRotation = Quaternion.identity;



    public void ToggleBuildPanel()
    {
        buildPanel.SetActive(!buildPanel.activeSelf);
    }

    public void SelectPrefabToPlace(int index)
    {
        currentPrefabToPlace = buildPrefabs[index];
        isPlacing = true;
        buildPanel.SetActive(false);
        ghostRotation = Quaternion.identity;

        CreateGhost();
    }

    void Update()
    {
        if (!isPlacing || ghostObject == null) return;

        // Raycast ile mouse pozisyonu takip
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ghostObject.transform.SetPositionAndRotation(hit.point, ghostRotation);

            // Sol týk: Yerleþtir

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                GameObject a = Instantiate(currentPrefabToPlace, hit.point, ghostRotation);
                if (currentPrefabToPlace == buildPrefabs[1])
                {
                  //  Debug.Log("Intersection eklendi: " + a.name);
                    if (!TGUIManager.Intersections.Contains(a))
                    {
                        TGUIManager.Intersections.Add(a);
                    }
                    TGUIManager.Controller = a.GetComponent<IntersectionController>();
                    OnIntersectionPlaced.Raise();
                    // Debug.LogError("OnIntersectionPlaced");
                    foreach (var renderer in a.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = TransparentMaterial;
                    }
                }
                else
                {
                    OnLightPlaced.Raise();
                    //Debug.LogError("OnLightPlaced");

                }
                CancelPlacement();
            }
        }

        // Sað týk: Ýptal
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }

        // Q / E: Rotate
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ghostRotation *= Quaternion.Euler(0f, -45f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ghostRotation *= Quaternion.Euler(0f, 45f, 0f);
        }
    }

    private void CreateGhost()
    {
        ghostObject = Instantiate(currentPrefabToPlace);
        SetLayerRecursive(ghostObject.transform, "Ignore Raycast");

        foreach (var renderer in ghostObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = ghostMaterial;
        }
    }

    private void CancelPlacement()
    {
        TGUIManager.Intersections.Remove(ghostObject);
        if (ghostObject != null) Destroy(ghostObject);
        ghostObject = null;
        isPlacing = false;
        currentPrefabToPlace = null;
    }

    private void SetLayerRecursive(Transform obj, string layerName)
    {
        obj.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in obj)
        {
            SetLayerRecursive(child, layerName);
        }
    }
}
