using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HeroDashSettings
{
    public float speed = 40f;
    public float duration = 0.2f;
    public float dashTimer;
    public bool isDashing;
}
