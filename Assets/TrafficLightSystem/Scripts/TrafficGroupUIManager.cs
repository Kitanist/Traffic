using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class TrafficGroupUIManager : MonoBehaviour // Mevcut kavþaklarý tutmak ve Gerektiðinde UI guncellemek için var.
{
    public GameObject slotPrefab, slotEmpty,UI;
    public List<GameObject> Intersections = new();
    public Transform slotContainer,SlotParent;
    public TMP_InputField IntersectionNameInputfield;
    public TMP_Dropdown IntersectionsDropdown;
    public TMP_InputField IntersectionUIInputfield;
    public TrafficLightUI TLUI;
    private IntersectionController _controller;
    private List<Transform> dropZones = new();

    public IntersectionController Controller
    {
        get => _controller;
        set
        {
            if (_controller != value)
            {
                _controller = value;
                GenerateUI();
            }
        }
    }

    private List<GameObject> slotInstances = new();

    void Start()
    {
        if (Intersections.Count >0)
        {
            Controller = Intersections[0].GetComponent<IntersectionController>();
            PopulateDropdown();
        }
        IntersectionUIInputfield.onEndEdit.AddListener(ChangeName);
       
        //GenerateUI();  // Yakýnda yukardaki ve buna gerek olmucak 
       

    }
    public void PopulateDropdown()
    {
        IntersectionsDropdown.ClearOptions();
        List<string> names = new List<string>();
        if (Intersections.Count > 0)
        { 
            foreach (var intersection in Intersections)
            {
                var controller = intersection.GetComponent<IntersectionController>();
                if (controller != null)
                    names.Add(string.IsNullOrEmpty(controller.IntersectionName) ? "Ýsimsiz Kavþak" : controller.IntersectionName);
                else
                    names.Add("Tanýmsýz");
            }
        }
           

        IntersectionsDropdown.AddOptions(names);
        IntersectionsDropdown.onValueChanged.AddListener(OnIntersectionsDropdownChanged);
    }
    void OnIntersectionsDropdownChanged(int index)
    {
        if (index >= 0 && index < Intersections.Count)
        {
            var selectedIntersection = Intersections[index];
            var controller = selectedIntersection.GetComponent<IntersectionController>();
            if (controller != null)
            {
                Controller = controller;
                var TLUI = FindObjectOfType<TrafficLightUI>();
                TLUI.intersectionController = controller;
                TLUI.IntersectionName.text = controller.IntersectionName;
                TLUI.SetupGroupDropdown();
               // Debug.Log($"Seçilen kavþak: {controller.IntersectionName}");
            }
        }
    }
    public void GenerateUI()
    {
        if (_controller == null) return;

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);
        foreach (Transform child in SlotParent)
            Destroy(child.gameObject);

        slotInstances.Clear();
        dropZones.Clear();

        foreach (var group in _controller.groups)
        {
            GameObject slotspace = Instantiate(slotEmpty, slotContainer);
            dropZones.Add(slotspace.transform);
        }

        for (int i = 0; i < _controller.groups.Count; i++)
        {
            var group = _controller.groups[i];
            GameObject slot = Instantiate(slotPrefab, dropZones[i]);
            var slotScript = slot.GetComponent<TrafficGroupSlot>();
            slotScript.Setup(group);
            slotScript.originalParent = dropZones[i]; // önemli 
            slotInstances.Add(slot);
        }
    }


    public void ApplyOrder()
    {
        List<TrafficLightGroup> newOrder = new();

        foreach (Transform slot in slotContainer)
        {
            var slotScript = slot.GetComponentInChildren<TrafficGroupSlot>().groupRef;

            if (slotScript != null)
            {
                newOrder.Add(slotScript);
            }
        }

        _controller.ApplyCustomOrder(newOrder);
    }
    public void ChangeIntersection() // TODO: Liste yapýsýna geçir
    {
        if (Controller == Intersections[0].GetComponent<IntersectionController>())
        {
            Controller = Intersections[0].GetComponent<IntersectionController>();

        }
        else 
        {
            Controller = Intersections[0].GetComponent<IntersectionController>();
        }

    }
    public void NewNameToCreatedIntersection()
    {
        var TrafficUIManager = GameObject.Find("TrafficUIManager");
        var IntersectionName = IntersectionNameInputfield.text;
        Debug.Log(IntersectionName);
        bool a = false;
        if (Intersections.Count>0)
        {
            foreach (var kavþak in Intersections)
            {
                if (kavþak.GetComponent<IntersectionController>().IntersectionName == IntersectionName)
                {
                    Controller.IntersectionName = IntersectionName + $"Clone {Random.Range(2, 500)}";
                    //Debug.Log($"NewNameCreated {Controller.IntersectionName}");
                    a = true;
                }
            }
        }
       
       if (!a) 
        Controller.IntersectionName = IntersectionName;
       // Debug.Log($"NewNameCreated {Controller.IntersectionName}");
        TrafficUIManager.GetComponent<TrafficLightUI>().CreateIntersection(IntersectionName, Controller);
        PopulateDropdown();

    }
    public void ChangeName(string b)
    {
        bool a = false;
        var IntersectionName = IntersectionUIInputfield.text;
        foreach (var kavþak in Intersections)
        {
            if (kavþak.GetComponent<IntersectionController>().IntersectionName == IntersectionName)
            {
                Controller.IntersectionName = b + $"Clone {Random.Range(2, 500)}";
                //Debug.Log($"NewNameCreated {Controller.IntersectionName}");
                a = true;
            }
        }
        if (!a)
            Controller.IntersectionName = b;

         TLUI.CreateIntersection(b, Controller);
        PopulateDropdown();
    }
    public void IsLightInAnyIntersectionRemoveLightFromIntersections(TrafficLightsController targetLight)
    {
        foreach (var kavsak in Intersections)
        {
            if (kavsak.GetComponent<IntersectionController>().IsLightInAnyGroup(targetLight))
            {
                kavsak.GetComponent<IntersectionController>().RemoveLightFromAllGroups(targetLight);
            }

        }
    }
    public void OpenUI()
    {
        UI.SetActive(true);
    }
    public void CloseUI() 
    {
        UI.SetActive(false);
    }
}
