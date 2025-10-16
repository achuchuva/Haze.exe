using UnityEngine;

public class DoorPlacer : MonoBehaviour
{
    [Header("Placement Parameters")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject previewDoorPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask placementSurfaceLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private AudioSource placeDoorSound;
    [SerializeField] private GameObject previewUI;

    [Header("Preview Material")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    [Header("Raycast Parameters")]
    [SerializeField] private float doorDistanceFromPlayer;
    [SerializeField] private float raycastStartVerticalOffset;
    [SerializeField] private float raycastDistance;

    [Header("Minimap")]
    [SerializeField] private Minimap minimap;

    private GameObject _previewObject = null;
    private Vector3 _currentPlacementPosition = Vector3.zero;
    private bool _inPlacementMode = false;
    private bool _validPreviewState = false;
    private PortalTeleporter _door = null;

    public bool Disabled { get; set; } = false;
    DoorManager.DoorSettings doorSettings;

    void Start()
    {
        minimap.OnMinimapClick.AddListener(PlaceDoorFromMinimap);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    void FixedUpdate()
    {
        if (_inPlacementMode)
        {
            UpdateCurrentPlacementPosition();

            if (CanPlaceDoor())
            {
                SetValidPreviewState();
            }
            else
            {
                SetInvalidPreviewState();
            }
        }
        previewUI.SetActive(_inPlacementMode);
    }

    void UpdateCurrentPlacementPosition()
    {
        Vector3 cameraForward = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z);
        cameraForward.Normalize();

        Vector3 startPos = playerCamera.transform.position + (cameraForward * doorDistanceFromPlayer);
        startPos.y += raycastStartVerticalOffset;

        RaycastHit hitInfo;
        if (Physics.Raycast(startPos, Vector3.down, out hitInfo, raycastDistance, placementSurfaceLayerMask))
        {
            _currentPlacementPosition = hitInfo.point;
        }

        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
        _previewObject.transform.position = _currentPlacementPosition;
        _previewObject.transform.rotation = rotation;
    }

    void UpdateInput()
    {
        if (Disabled) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_inPlacementMode)
            {
                ExitPlacementMode();
            }
            else
            {
                if (DoorManager.Instance.doorCount < 1)
                {
                    WarningFlash.Instance.FlashWarning("NO DOORS", 80);
                    return;
                }
                EnterPlacementMode();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            PlaceDoor();
        }
    }

    void SetValidPreviewState()
    {
        previewMaterial.color = validColor;
        _validPreviewState = true;
    }

    void SetInvalidPreviewState()
    {
        previewMaterial.color = invalidColor;
        _validPreviewState = false;
    }

    bool CanPlaceDoor()
    {
        if (_previewObject == null) return false;

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 playerPosXZ = new Vector3(playerCamera.transform.position.x, 0f, playerCamera.transform.position.z);
        Vector3 placementPosXZ = new Vector3(_currentPlacementPosition.x, 0f, _currentPlacementPosition.z);
        float horizontalDistance = Vector3.Distance(playerPosXZ, placementPosXZ);
        Vector3 horizontalDirection = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;

        Debug.DrawRay(playerCamera.transform.position, horizontalDirection * horizontalDistance, Color.red);
        if (Physics.Raycast(playerCamera.transform.position, horizontalDirection, horizontalDistance, obstacleLayerMask))
        {
            // Raycast hit an obstacle. The path is blocked.
            return false;
        }

        foreach (var checker in _previewObject.GetComponentsInChildren<DoorValidChecker>())
        {
            if (!checker.IsValid) return false;
        }
        return true;
    }

    void PlaceDoor()
    {
        if (!_inPlacementMode || !_validPreviewState) return;

        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
        GameObject door = Instantiate(doorPrefab, _currentPlacementPosition, rotation);
        doorSettings = DoorManager.Instance.GetDoorSettings();
        door.GetComponent<Door>().SetDoorSettings(doorSettings);
        _door = door.GetComponentInChildren<PortalTeleporter>();

        DoorManager.Instance.doorCount -= 1;

        ExitPlacementMode();

        minimap.ActivateMinimap();
        minimap.OnMinimapClick.AddListener(PlaceSecondDoor);
        minimap.OnMinimapClick.RemoveListener(PlaceDoorFromMinimap);
    }

    public void PlaceDoorFromMinimap(Vector3 position)
    {
        if (DoorManager.Instance.doorCount < 1)
        {
            WarningFlash.Instance.FlashWarning("NO DOORS", 80);
            return;
        }

        if (Physics.CheckSphere(position + new Vector3(0, 1, 0), 1.25f, obstacleLayerMask))
        {
            WarningFlash.Instance.FlashWarning("TOO CLOSE TO WALLS", 50);
            return;
        }

        Vector3 bestDirection = Vector3.forward; // Default starting direction
        float bestDistance = -1f; // Start with -1 to ensure the first valid hit is always chosen
        int rayCount = 16;
        float maxRayDistance = 100f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 rayStartPos = position + Vector3.up; 
            
            if (Physics.Raycast(rayStartPos, direction, out RaycastHit hitInfo, maxRayDistance, obstacleLayerMask))
            {
                if (hitInfo.distance > bestDistance)
                {
                    bestDistance = hitInfo.distance;
                    bestDirection = direction;
                }
            }
            else
            {
                bestDistance = maxRayDistance;
                bestDirection = direction;
                break; 
            }
        }

        Quaternion finalRotation = Quaternion.LookRotation(-bestDirection);

        GameObject door = Instantiate(doorPrefab, position, finalRotation);
        doorSettings = DoorManager.Instance.GetDoorSettings();
        door.GetComponent<Door>().SetDoorSettings(doorSettings);
        _door = door.GetComponentInChildren<PortalTeleporter>();

        DoorManager.Instance.doorCount -= 1;
        placeDoorSound.Play();

        minimap.OnMinimapClick.AddListener(PlaceSecondDoor);
        minimap.OnMinimapClick.RemoveListener(PlaceDoorFromMinimap);
    }

    void PlaceSecondDoor(Vector3 position)
    {
        if (Physics.CheckSphere(position + new Vector3(0, 1, 0), 1.25f, obstacleLayerMask))
        {
            WarningFlash.Instance.FlashWarning("TOO CLOSE TO WALLS", 50);
            return;
        }

        position.y = _currentPlacementPosition.y;

        // Find the rotation that allows for the longest raycast distance without hitting an obstacle.
        Vector3 bestDirection = Vector3.forward; // Default starting direction
        float bestDistance = -1f; // Start with -1 to ensure the first valid hit is always chosen
        int rayCount = 16;
        float maxRayDistance = 100f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 rayStartPos = position + Vector3.up; 
            
            if (Physics.Raycast(rayStartPos, direction, out RaycastHit hitInfo, maxRayDistance, obstacleLayerMask))
            {
                if (hitInfo.distance > bestDistance)
                {
                    bestDistance = hitInfo.distance;
                    bestDirection = direction;
                }
            }
            else
            {
                bestDistance = maxRayDistance;
                bestDirection = direction;
                break; 
            }
        }

        Quaternion finalRotation = Quaternion.LookRotation(-bestDirection);

        placeDoorSound.Play();

        GameObject secondDoor = Instantiate(doorPrefab, position, finalRotation);
        secondDoor.GetComponent<Door>().SetDoorSettings(doorSettings);
        PortalTeleporter _secondDoor = secondDoor.GetComponentInChildren<PortalTeleporter>();

        DoorManager.Instance.doorCount -= 1;

        // Link the two doors.
        _door.reciever = _secondDoor.transform;
        _door.player = player;
        _secondDoor.reciever = _door.transform;
        _secondDoor.player = player;
        _door.GetComponentInParent<Door>().Other = _secondDoor.GetComponentInParent<Door>();
        _door = null;

        minimap.OnMinimapClick.RemoveListener(PlaceSecondDoor);
        minimap.OnMinimapClick.AddListener(PlaceDoorFromMinimap);
    }

    void EnterPlacementMode()
    {
        if (_inPlacementMode) return;

        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);
        _previewObject = Instantiate(previewDoorPrefab, _currentPlacementPosition, rotation, transform);
        _inPlacementMode = true;

    }

    public void ExitPlacementMode()
    {
        Destroy(_previewObject);
        _previewObject = null;
        _inPlacementMode = false;
    }

    public void CleanupDoors()
    {
        if (_door != null)
        {
            GameObject collectEffect = _door.GetComponentInParent<Door>().collectEffect;
            GameObject effect = Instantiate(collectEffect, _door.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Destroy(effect, 3f);
            Destroy(_door.transform.root.gameObject);
            _door = null;
            DoorManager.Instance.doorCount += 1;
        }
        ExitPlacementMode();
        minimap.OnMinimapClick.RemoveListener(PlaceSecondDoor);
        minimap.OnMinimapClick.AddListener(PlaceDoorFromMinimap);
    }
}
