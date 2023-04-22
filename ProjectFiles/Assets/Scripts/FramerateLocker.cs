using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLocker : MonoBehaviour
{

    [SerializeField] private int _frameRateMax = 60;
    private void Awake()
    {
        Application.targetFrameRate = _frameRateMax;
    }
}
