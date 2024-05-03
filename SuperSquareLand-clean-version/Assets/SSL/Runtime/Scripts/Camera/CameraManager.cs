using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance { get; private set; }
    [Header("Camera")]
    [SerializeField] private Camera _camera;

    [Header("Profile System")]
    [SerializeField] private CameraProfile _defaultCameraProfile;
    private CameraProfile _currentCameraProfile;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _InitToDefaultProfile();
    }

    private void Update()
    {
        _SetCameraPosition(_currentCameraProfile.Position);
        _SetCameraSize(_currentCameraProfile.CameraSize);
    }

    public void EnterProfile(CameraProfile cameraProfile)
    {
        _currentCameraProfile = cameraProfile;
    }

    public void ExitProfile(CameraProfile cameraProfile)
    {
        if (_currentCameraProfile != cameraProfile) return;
        _currentCameraProfile = _defaultCameraProfile;
    }

    private void _SetCameraPosition(Vector3 position)
    {
        Vector3 newCameraPosition = _camera.transform.position;
        newCameraPosition.x = position.x;
        newCameraPosition.y = position.y;
        _camera.transform.position = newCameraPosition;
    }

    private void _SetCameraSize(float size)
    {
        _camera.orthographicSize = size;
    }

    private void _InitToDefaultProfile()
    {
        _currentCameraProfile = _defaultCameraProfile;
        _SetCameraPosition(_currentCameraProfile.Position);
        _SetCameraSize(_currentCameraProfile.CameraSize);
    }
}