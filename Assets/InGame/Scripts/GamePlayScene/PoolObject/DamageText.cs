using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : RunTimePoolObject
{
    [SerializeField] private Text _damageText;

    public void SetDamage(int damage)
    {
        _damageText.text = damage.ToString();
    }
}
