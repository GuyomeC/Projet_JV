using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementSettings _mouvementsSetting;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    private void FixedUpdate()
    {
        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack();
        } else
        {
            _UpdateHorizontalSpeed();
            _ChangeOrientFromHorizontalMovement();
        }
        _ApplyHorizontalSpeed();
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
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
        _horizontalSpeed -= _mouvementsSetting.deceleration * Time.deltaTime;
        if (_horizontalSpeed < 0f){
            _horizontalSpeed = 0f;
        }
    }

    private void _TurnBack()
    {
        _horizontalSpeed -= _mouvementsSetting.turnBackFrictions * Time.deltaTime;
        if (_horizontalSpeed < 0f) {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX + _orientX < 0f;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.EndVertical();
    }
}