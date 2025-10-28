using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Minimap : MonoBehaviour
{
    public TMP_Text doorCountText;
    public GameObject minimapPrompt;
    public GameObject camera;
    public GameObject image;
    public GameObject mazeGEO;
    public FirstPersonMovement firstPersonMovement;
    public FirstPersonLook firstPersonLook;
    public GameObject player;
    private bool _isMinimapActive = false;

    public UnityEvent<Vector3> OnMinimapClick;
    public bool Locked { get; set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        doorCountText.text = "Doors Remaining: " + Mathf.Ceil(DoorManager.Instance.doorCount / 2f).ToString();

        if (Locked) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isMinimapActive)
            {
                ActivateMinimap();
            }
            else
            {
                DisableMinimap();
            }
        }
    }

    public void ActivateMinimap()
    {
        _isMinimapActive = true;

        minimapPrompt.SetActive(false);
        camera.SetActive(true);
        image.SetActive(true);
        mazeGEO.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        firstPersonMovement.Disable();
        firstPersonLook.Disabled = true;

        DoorPlacer doorPlacer = GameObject.FindObjectOfType<DoorPlacer>();
        doorPlacer.ExitPlacementMode();
        doorPlacer.Disabled = true;

        MoveMinimapToPlayer();
    }

    public void DisableMinimap()
    {
        _isMinimapActive = false;

        minimapPrompt.SetActive(true);
        camera.SetActive(false);
        image.SetActive(false);
        mazeGEO.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        firstPersonMovement.Enable();
        firstPersonLook.Disabled = false;

        DoorPlacer doorPlacer = GameObject.FindObjectOfType<DoorPlacer>();
        doorPlacer.Disabled = false;
        doorPlacer.CleanupDoors();
    }

    void MoveMinimapToPlayer()
    {
        Vector3 newPosition = player.transform.position;

        newPosition.y = camera.transform.position.y;
        camera.transform.position = newPosition;
    }

    public void MinimapClick(Vector3 position)
    {
        OnMinimapClick?.Invoke(position);
    }
}
