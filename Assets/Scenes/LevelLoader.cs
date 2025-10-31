using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    public Animator transition;
    public float transitionTime = 1f;
    public float mazeTransitionTime = 2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            // Unlock the cursor in the menu scene
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        string lastScene = PlayerPrefs.GetString("LastScene");
        if (lastScene == string.Empty)
        {
            lastScene = "Level 1";
        }
        LoadLevel(lastScene);
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(Load(levelName));
    }

    public void LoadNextMazeLevel(string levelName)
    {
        StartCoroutine(LevelLoad(levelName));
    }

    public void EndGame()
    {
        // Play animation
        PlayerPrefs.DeleteKey("LastScene");
        transition.SetTrigger("WhiteEnd");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    IEnumerator LevelLoad(string name)
    {
        // Play animation
        transition.SetTrigger("WhiteStart");

        // Wait
        yield return new WaitForSeconds(mazeTransitionTime);

        // Load scene
        SceneManager.LoadScene(name);
    }

    IEnumerator Load(string name)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(name);
    }
}
