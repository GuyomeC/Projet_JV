using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform[] _detectionPoints;
    [SerializeField] private Transform[] _detectionPointsWalls;
    [SerializeField] private float _detectionLength = 0.1f;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private LayerMask _wallsLayerMask;

    public bool DetectGroundNearBy()
    {
        foreach (Transform detectionPoint in _detectionPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionPoint.position,
                Vector2.down,
                _detectionLength,
                _groundLayerMask
            );
            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }
    public bool DetectRightWallsNearBy()
    {
        foreach (Transform detectionPoint in _detectionPointsWalls)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionPoint.position,
                Vector2.right,
                _detectionLength,
                _wallsLayerMask
            );
            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }

    public bool DetectLeftWallsNearBy()
    {
        foreach (Transform detectionPoint in _detectionPointsWalls)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionPoint.position,
                Vector2.left,
                _detectionLength,
                _wallsLayerMask
            );
            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }
}
