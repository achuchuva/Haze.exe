using UnityEngine;

public class DoorPlacer : MonoBehaviour
{
    [Header("Placement Parameters")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject previewDoorPrefab;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask placementSurfaceLayerMask;

    [Header("Preview Material")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    [Header("Raycast Parameters")]
    [SerializeField] private float doorDistanceFromPlayer;
    [SerializeField] private float raycastStartVerticalOffset;
    [SerializeField] private float raycastDistance;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothSpeed;
    [SerializeField] private float rotationSmoothSpeed;

    private GameObject _previewObject = null;
    private Vector3 _currentPlacementPosition = Vector3.zero;
    private Vector3 _targetPlacementPosition = Vector3.zero;
    private float _targetRotationY = 0f;
    private bool _inPlacementMode = false;
    private bool _validPreviewState = false;
    private Transform _characterTransform = null;

    void Start()
    {
        // Get the character transform (parent of camera) for stable rotation
        if (playerCamera != null)
        {
            FirstPersonMovement fpsMovement = playerCamera.GetComponentInParent<FirstPersonMovement>();
            if (fpsMovement != null)
            {
                _characterTransform = fpsMovement.transform;
            }
        }
        
        // Fallback to camera's parent if FirstPersonMovement not found
        if (_characterTransform == null && playerCamera != null)
        {
            _characterTransform = playerCamera.transform.parent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

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
        // Use character transform for stable rotation, not camera
        Transform rotationSource = _characterTransform != null ? _characterTransform : playerCamera.transform;
        
        // Get forward direction on horizontal plane only (ignore vertical tilt)
        Vector3 horizontalForward = new Vector3(rotationSource.forward.x, 0f, rotationSource.forward.z);
        
        // Ensure we have a valid direction
        if (horizontalForward.sqrMagnitude < 0.01f)
        {
            horizontalForward = rotationSource.forward;
            horizontalForward.y = 0f;
        }
        
        horizontalForward.Normalize();

        // Calculate raycast start position
        Vector3 startPos = playerCamera.transform.position + (horizontalForward * doorDistanceFromPlayer);
        startPos.y += raycastStartVerticalOffset;

        // Perform raycast to find ground position
        RaycastHit hitInfo;
        if (Physics.Raycast(startPos, Vector3.down, out hitInfo, raycastDistance, placementSurfaceLayerMask))
        {
            _targetPlacementPosition = hitInfo.point;
        }

        // Smooth position to reduce jitter
        _currentPlacementPosition = Vector3.Lerp(_currentPlacementPosition, _targetPlacementPosition, 
            Time.deltaTime * positionSmoothSpeed);

        // Smooth rotation using the stable character rotation
        float targetYaw = rotationSource.eulerAngles.y;
        _targetRotationY = Mathf.LerpAngle(_targetRotationY, targetYaw, Time.deltaTime * rotationSmoothSpeed);
        
        Quaternion rotation = Quaternion.Euler(0f, _targetRotationY, 0f);
        
        // Update preview object
        _previewObject.transform.position = _currentPlacementPosition;
        _previewObject.transform.rotation = rotation;
    }

    void UpdateInput()
    {
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

        foreach (var checker in _previewObject.GetComponentsInChildren<DoorValidChecker>())
        {
            if (!checker.IsValid) return false;
        }
        return true;
    }

    void PlaceDoor()
    {
        if (!_inPlacementMode || !_validPreviewState) return;

        // Use the actual preview object's rotation for consistency
        Quaternion rotation = _previewObject.transform.rotation;
        Instantiate(doorPrefab, _currentPlacementPosition, rotation, transform);

        ExitPlacementMode();
    }

    void EnterPlacementMode()
    {
        if (_inPlacementMode) return;

        // Initialize target rotation from character transform
        Transform rotationSource = _characterTransform != null ? _characterTransform : playerCamera.transform;
        _targetRotationY = rotationSource.eulerAngles.y;
        
        Quaternion rotation = Quaternion.Euler(0f, _targetRotationY, 0f);
        
        // Initialize placement position to avoid initial jump
        Vector3 horizontalForward = new Vector3(rotationSource.forward.x, 0f, rotationSource.forward.z);
        if (horizontalForward.sqrMagnitude < 0.01f)
        {
            horizontalForward = rotationSource.forward;
            horizontalForward.y = 0f;
        }
        horizontalForward.Normalize();
        
        Vector3 startPos = playerCamera.transform.position + (horizontalForward * doorDistanceFromPlayer);
        startPos.y += raycastStartVerticalOffset;
        
        RaycastHit hitInfo;
        if (Physics.Raycast(startPos, Vector3.down, out hitInfo, raycastDistance, placementSurfaceLayerMask))
        {
            _currentPlacementPosition = hitInfo.point;
            _targetPlacementPosition = hitInfo.point;
        }
        
        _previewObject = Instantiate(previewDoorPrefab, _currentPlacementPosition, rotation, transform);
        _inPlacementMode = true;

    }

    void ExitPlacementMode()
    {
        Destroy(_previewObject);
        _previewObject = null;
        _inPlacementMode = false;
    }
}
