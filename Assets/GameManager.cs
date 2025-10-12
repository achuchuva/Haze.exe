using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    bool displayedWASD = false;

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

        DontDestroyOnLoad(gameObject);
        Invoke("DisplayWASD", 2f);
    }

    void DisplayWASD()
    {
        if (!displayedWASD)
        {
            WarningFlash.Instance.FlashWarning("WASD", 100);
            displayedWASD = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
