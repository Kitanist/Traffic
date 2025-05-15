using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeCustomOrderUI : MonoBehaviour
{
    public IntersectionController controller;
    public GameObject buttonPrefab;
    public Transform listParent;

    private List<GameObject> buttonObjects = new List<GameObject>();

    void Start()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        // Temizle
        foreach (var obj in buttonObjects)
        {
            Destroy(obj);
        }
        buttonObjects.Clear();

        for (int i = 0; i < controller.customOrder.Count; i++)
        {
            int index = i;
            TrafficLightGroup group = controller.customOrder[i];

            GameObject btnObj = Instantiate(buttonPrefab, listParent);
            btnObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = group.groupName;

            btnObj.transform.Find("Up").GetComponent<Button>().onClick.AddListener(() => MoveGroup(index, -1));
            btnObj.transform.Find("Down").GetComponent<Button>().onClick.AddListener(() => MoveGroup(index, 1));

            buttonObjects.Add(btnObj);
        }
    }
    public void ApplyOrder()
    {
        controller.ApplyCustomOrder(new List<TrafficLightGroup>(controller.customOrder));
    }

    private void MoveGroup(int index, int direction)
    {
        int newIndex = index + direction;
        if (newIndex < 0 || newIndex >= controller.customOrder.Count)
            return;

        var temp = controller.customOrder[index];
        controller.customOrder[index] = controller.customOrder[newIndex];
        controller.customOrder[newIndex] = temp;

        RefreshList();
    }
}
