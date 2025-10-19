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
        Invoke("DisplaySprint", 4f);
    }

    void DisplayWASD()
    {
        if (!displayedWASD)
        {
            WarningFlash.Instance.FlashWarningImage();
        }
    }

    void DisplaySprint()
    {
        if (!displayedWASD)   
        {
            WarningFlash.Instance.FlashWarning("HOLD SHIFT TO SPRINT", 80);
            displayedWASD = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
