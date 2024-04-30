using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementSettings _mouvementsSetting;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSetting _fallSetting;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; }

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSetting;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    public void _ActivateDash()
    {
        _dashSetting.dashTimer = 0f;
        if (_dashSetting.dashTimer < _dashSetting.duration && _dashSetting.isDashing == false) {
            _dashSetting.isDashing = true;
            _horizontalSpeed += _dashSetting.speed;
        } else {
            _dashSetting.isDashing = false;
            return;
        }

    }

    private void FixedUpdate()
    {

        _ApplyGroundDetection();
        if (_AreOrientAndMovementOpposite()) {
            _TurnBack();
        } else {
            _UpdateHorizontalSpeed();
            _ChangeOrientFromHorizontalMovement();
        }

        if (!IsTouchingGround) {
            _ApplyFallGravity();
        } else {
            _ResetVerticalSpeed();
        }


        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyFallGravity()
    {
        _verticalSpeed -= _fallSetting.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -_fallSetting.fallSpeedMax)
        {
            _verticalSpeed = -_fallSetting.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }

    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    
    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void _UpdateHorizontalSpeed()
    {
        if (_moveDirX != 0f)
        {
            _Accelerate();
        } else {
            _Decelerate();
        }
    }

    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _Accelerate()
    {
        _horizontalSpeed += _mouvementsSetting.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > _mouvementsSetting.speedMax) {
            _horizontalSpeed = _mouvementsSetting.speedMax;
        }
    }

    private void _Decelerate()
    {
        _horizontalSpeed -= _mouvementsSetting.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f){
            _horizontalSpeed = 0f;
        }
    }

    private void _TurnBack()
    {
        _horizontalSpeed -= _mouvementsSetting.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f) {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround) {
            GUILayout.Label("OnGround");
        } else {
            GUILayout.Label("InAir");
        }
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}