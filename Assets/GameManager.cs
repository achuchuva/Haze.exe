using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    bool displayedWASD = false;
    public GameObject[] hazeEffects;
    bool hazeEnabled = true;
    bool hazeTogglePressed = false;
    bool levelSkipPressed = false;

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
        if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.M))
        {
            if (!hazeTogglePressed)
            {
                hazeEnabled = !hazeEnabled;
                if (!hazeEnabled)
                {
                    WarningFlash.Instance.FlashWarning("HAZE EFFECTS DISABLED", 40);
                    GlitchManager.Instance.DisableEffects();
                }
                else
                {
                    WarningFlash.Instance.FlashWarning("HAZE EFFECTS ENABLED", 40);
                }
                foreach (GameObject haze in hazeEffects)
                {
                    haze.SetActive(hazeEnabled);
                }
                hazeTogglePressed = true;
            }
        }
        else
        {
            hazeTogglePressed = false;
        }

        if (Input.GetKey(KeyCode.B) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.E))
        {
            if (!levelSkipPressed)
            {
                if (SceneManager.GetActiveScene().name == "Level 1")
                {
                    LevelLoader.Instance.LoadLevel("Level 2");
                }
                else if (SceneManager.GetActiveScene().name == "Level 2")
                {
                    LevelLoader.Instance.LoadLevel("Level 3");
                }
                levelSkipPressed = true;
            }
        }
        else
        {
            levelSkipPressed = false;
        }
    }
}
