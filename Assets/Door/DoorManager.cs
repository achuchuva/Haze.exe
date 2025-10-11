using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;
    public int doorCount = 0;
    public bool receivedDoors = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
