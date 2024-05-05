using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_mouvementsSetting")]
    [SerializeField] private HeroHorizontalMovementSettings _groundHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementsSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSetting _fallSetting;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Wall Slide")]
    [SerializeField] private HeroWallSlideSettings _wallSlideSettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; }
    public bool IsTouchingRightWall { get; private set; }
    public bool IsTouchingLeftWall { get; private set; }
    private float _timeBetweenWallTouch = 2f;
    private bool _wallTouch = false;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSetting;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    public float _orientX = 1f;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings[] allJumpSettings;
    [SerializeField] private HeroFallSetting _jumpFallSettings;
    [SerializeField] private HeroHorizontalMovementSettings _jumpHorizontalMovementsSettings;
    private HeroJumpSettings _jumpSettings;
    private float recupGravity;
    public int jumpLeft = 2;


    private CameraFollowable _cameraFollowable;

    public bool IsHorizontalMoving => _moveDirX != 0f;

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling
    }

    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void Awake()
    {
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    public void _ActivateDash()
    {
        if (_dashSetting.dashTimer < _dashSetting.duration) {
            if (!IsTouchingGround)
            {
                recupGravity = _jumpFallSettings.fallGravity;
                _jumpFallSettings.fallGravity = 0f;
                _horizontalSpeed = _dashSetting.speed + 15f;
                _jumpFallSettings.fallGravity = recupGravity;
                _dashSetting.isDashing = true;
            }
            else
            {
                _horizontalSpeed = _dashSetting.speed;
                _dashSetting.isDashing = true;
            }
        }
    }

    public void _Dash()
    {
        if (_dashSetting.isDashing)
        {
            if (_dashSetting.dashTimer <= _dashSetting.duration) {
                _dashSetting.dashTimer += Time.deltaTime;
            } else {
                _dashSetting.isDashing = false;
                _horizontalSpeed = 0f;
                _dashSetting.dashTimer = 0f;
            }
        }
    }

    public void JumpStart()
    {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    public bool IsJumping => _jumpState != JumpState.NotJumping;

    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }

    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;

    public bool IsJumpMinDurationReached => _jumpTimer >= allJumpSettings[0].jumpMinDuration;

    private void FixedUpdate()
    {

        _ApplyGroundDetection();
        _ApplyWallLeftDetection();
        _ApplyWallRightDetection();
        _UpdateCameraFollowPosition();

        HeroHorizontalMovementSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();
        if (_AreOrientAndMovementOpposite()) {
            _TurnBack(horizontalMovementSettings);
        } else {
            _UpdateHorizontalSpeed(horizontalMovementSettings);
            _ChangeOrientFromHorizontalMovement();
        }
        if (IsJumping) {
            _UpdateJump();
        } else {
            if (!IsTouchingGround && !_dashSetting.isDashing) {
                _ApplyFallGravity(_fallSetting);
            } else {
                _ResetVerticalSpeed();
            }
        }

        if ((IsTouchingLeftWall || IsTouchingRightWall) && !IsTouchingGround)
        {
            _ApplyWallSlideGravity(_wallSlideSettings);
            jumpLeft = 1;
            if (IsJumping)
            {
                _UpdateJump();
            }
        }

        if (!_wallTouch && _timeBetweenWallTouch > 0 && !IsJumping)
        {
            IsTouchingWalls();
        }
        else
        {
            _wallTouch = false;
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

    private void _ApplyFallGravity(HeroFallSetting setting)
    {
        _verticalSpeed -= setting.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -setting.fallSpeedMax)
        {
            _verticalSpeed = -setting.fallSpeedMax;
        }
    }

    private void _ApplyWallSlideGravity(HeroWallSlideSettings setting)
    {
        _verticalSpeed -= setting.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -setting.fallSpeedMax)
        {
            _verticalSpeed = -setting.fallSpeedMax;
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

    private void _ApplyWallRightDetection()
    {
        IsTouchingRightWall = _groundDetector.DetectRightWallsNearBy();
    }

    private void _ApplyWallLeftDetection()
    {
        IsTouchingLeftWall = _groundDetector.DetectLeftWallsNearBy();
    }

    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    
    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < allJumpSettings[0].jumpMaxDuration && jumpLeft == 2)
        {
            _verticalSpeed = allJumpSettings[0].jumpSpeed;
        }
        else if (_jumpTimer < allJumpSettings[1].jumpMaxDuration && jumpLeft == 1)
        {
            _verticalSpeed = allJumpSettings[1].jumpSpeed;
        }
        else if (_jumpTimer < allJumpSettings[2].jumpMaxDuration && jumpLeft == 0)
        {
            _verticalSpeed = allJumpSettings[2].jumpSpeed;
        } else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJumpStateFalling()
    {
        if (!IsTouchingGround) {
            _ApplyFallGravity(_jumpFallSettings);
        } else {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion();
                break;
            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else {
            _Decelerate(settings);
        }
    }

    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _Accelerate( HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax) {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f){
            _horizontalSpeed = 0f;
        }
    }

    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f) {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    //private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    //{
    //    return IsTouchingGround ? _groundHorizontalMovementsSettings : _airHorizontalMovementsSettings;
    //}

    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        if (IsTouchingGround) {
            return _groundHorizontalMovementsSettings;
        } else {
            return _airHorizontalMovementsSettings;
        }
    }

    private void _UpdateCameraFollowPosition()
    {
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        if(IsTouchingGround && !IsJumping)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    }

    private void IsTouchingWalls()
    {
        if (IsTouchingLeftWall || IsTouchingRightWall)
        {
            _horizontalSpeed = 0f;
            _timeBetweenWallTouch -= Time.fixedDeltaTime;
            _wallTouch = true;
        }
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
        if (IsTouchingLeftWall)
        {
            GUILayout.Label("onLeftWall");
        }
        else if (IsTouchingRightWall)
        {
            GUILayout.Label("onRightWall");
        }
        else
        {
            GUILayout.Label("OnGround");
        }
        GUILayout.Label($"JumpState = {_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}