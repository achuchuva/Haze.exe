using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [System.Serializable]
    public class DoorSettings
    {
        public Material material;
        public Color particleColor1;
        public Color particleColor2;
        public Color iconColor;
        public bool inUse = false;
    }

    public static DoorManager Instance;
    public int doorCount = 0;
    public bool receivedDoors = false;
    public DoorSettings[] doorSettings;
    public GameObject collectPrompt;

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

    public DoorSettings GetDoorSettings()
    {
        // Shuffle the door settings array
        for (int i = 0; i < doorSettings.Length; i++)
        {
            DoorSettings temp = doorSettings[i];
            int randomIndex = Random.Range(i, doorSettings.Length);
            doorSettings[i] = doorSettings[randomIndex];
            doorSettings[randomIndex] = temp;
        }
        foreach (var setting in doorSettings)
        {
            if (!setting.inUse)
            {
                setting.inUse = true;
                return setting;
            }
        }

        return doorSettings[0];
    }
}
