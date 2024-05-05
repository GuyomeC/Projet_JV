using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEffectBounce : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Object to ounce")]
    [SerializeField] private Transform _objectToBounce;

    [Header("Bounce Loop")]
    [SerializeField] private float bouncePeriod;
    //[SerializeField] private float bounceHeight = 0.5f;
    //private float _bounceTimer = 0f;
    private float _currentHeight = 0f;

    private void _SetObjectToBounceHeight(float height)
    {
        Vector3 newPosition = _objectToBounce.localPosition;
        newPosition.y -= _currentHeight;
        _currentHeight = height;
        newPosition.y += _currentHeight;
        _objectToBounce.localPosition = newPosition;
    }
}
