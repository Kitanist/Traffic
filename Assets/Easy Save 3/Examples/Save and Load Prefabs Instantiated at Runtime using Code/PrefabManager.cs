using UnityEngine;
using System.Collections.Generic;

namespace SaveAndLoadPrefabsInstantiatedAtRuntime
{
    /*
     * This class instantiates a prefab at a random position whenever the CreateRandomPrefab() method is called.
     * When the SavePrefabInstances() method is called, these prefab instances will be saved.
     * When the LoadPrefabInstances() method is called, the prefab instances will be restored (if they don't already exist with the same reference IDs).
     */
    public class PrefabManager : MonoBehaviour
    {
        // The prefabs we want to instantiate.
        // Before adding prefabs to this array, you should right-click them and select Easy Save 3 > Enable Easy Save for Prefab.
        // You should also add an Easy Save 3 Manager to your scene by going to Tools > Easy Save 3 > Add Manager to Scene.
        public GameObject[] prefabs;
        // When we instantiate a prefab we'll put it in this list.
        private List<GameObject> prefabInstances = new List<GameObject>();

        // Instantiates a random prefab at a random position.
        public void CreateRandomPrefab()
        {
            // Pick a prefab from the prefabs array at random.
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            // Instantiate the prefab at a random location within 5 units of (0,0).
            var prefabInstance = Instantiate(prefab, Random.insideUnitSphere * 5, Random.rotation);
            // Add the prefab instance to the prefabInstances List.
            prefabInstances.Add(prefabInstance);
        }

        public void SavePrefabInstances()
        {
            // Save our prefabInstances list to file using "prefabInstances" as the unique key to idetify the data.
            ES3.Save("prefabInstances", prefabInstances);
        }

        public void LoadPrefabInstances()
        {
            // Load our prefabInstances List using the same unique key as we used to save it.
            // If no save data exists it will return an empty List.
            // If prefab instances with these reference IDs still exist.
            prefabInstances = ES3.Load("prefabInstances", new List<GameObject>());
        }
    }
}