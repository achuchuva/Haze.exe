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
        _door = door.GetComponentInChildren<PortalTeleporter>();

        ExitPlacementMode();

        minimap.ActivateMinimap();
        minimap.Locked = true;
        minimap.OnMinimapClick.AddListener(PlaceSecondDoor);
    }

    void PlaceSecondDoor(Vector3 position)
    {
        position.y = _currentPlacementPosition.y;
        Quaternion rotation = Quaternion.Euler(0f, _door.transform.eulerAngles.y + 180f, 0f);

        GameObject secondDoor = Instantiate(doorPrefab, position, rotation);
        PortalTeleporter _secondDoor = secondDoor.GetComponentInChildren<PortalTeleporter>();

        // Link the two doors.
        _door.reciever = _secondDoor.transform;
        _door.player = player;
        _secondDoor.reciever = _door.transform;
        _secondDoor.player = player;

        minimap.Locked = false;
        minimap.OnMinimapClick.RemoveListener(PlaceSecondDoor);
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
}
