using UnityEngine;
using UnityEngine.UI;

public class SpawnObject : MonoBehaviour
{
    public GameObject objectToSpawn; // Instantiate edilecek obje (Prefab)
    public Vector3 spawnPosition = new Vector3(0, 0, 0); // Konum
    public Vector3 spawnRotation = new Vector3(0, 0, 0); // Euler rotasyonu
    public Button spawnButton; // UI buton
    public float moveSpeed = 2f; // Hareket hýzý
    private GameObject currentObj;

    void Start()
    {
      //  spawnButton.onClick.AddListener(Spawn);
    }

    void Spawn()
    {
        Quaternion rotation = Quaternion.Euler(spawnRotation); // Rotasyonu çevir
        currentObj = Instantiate(objectToSpawn, spawnPosition, rotation);
    }

    void Update()
    {
        if (currentObj != null)
        {
            // Rotasyon (ok tuþlarý)
            if (Input.GetKey(KeyCode.LeftArrow))
                currentObj.transform.Rotate(Vector3.up, -90f * Time.deltaTime);
            if (Input.GetKey(KeyCode.RightArrow))
                currentObj.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
            if (Input.GetKey(KeyCode.UpArrow))
                currentObj.transform.Rotate(Vector3.right, -90f * Time.deltaTime);
            if (Input.GetKey(KeyCode.DownArrow))
                currentObj.transform.Rotate(Vector3.right, 90f * Time.deltaTime);

            // Pozisyon (WASD + Q/E)
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                moveDir += Vector3.forward;
            if (Input.GetKey(KeyCode.S))
                moveDir += Vector3.back;
            if (Input.GetKey(KeyCode.A))
                moveDir += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                moveDir += Vector3.right;
            if (Input.GetKey(KeyCode.Q))
                moveDir += Vector3.down;
            if (Input.GetKey(KeyCode.E))
                moveDir += Vector3.up;

            currentObj.transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
