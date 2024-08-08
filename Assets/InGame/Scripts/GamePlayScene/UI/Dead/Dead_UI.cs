using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Dead_UI : MonoBehaviour
{
    private float _deadTime = 0f;
    [SerializeField] private Text _resurrectionCount;

    private void OnEnable()
    {
        _deadTime = GameSettings.InGameDeadTime;
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        _deadTime -= Time.deltaTime;
        _resurrectionCount.text = ((int)_deadTime).ToString();
    }
}