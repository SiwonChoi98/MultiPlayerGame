using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AICanvas : CharacterCanvas
{
    [SerializeField] private Image _findTargetImage;
    protected override void Start()
    {
        base.Start();
        
        _statusComponent.UpdateCombat += UpdateStat;
    }
    protected override void UpdateStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.HEALTH:
                UpdateHealth();
                break; 
            case StatType.COMBAT:
                if (_statusComponent.IsCombat)
                {
                    Show_FindTargetImage();
                }
                else
                {
                    Hide_FindTargetImage();
                }
                break;
        }
    }
    
    public void Show_FindTargetImage()
    {
        _findTargetImage.gameObject.SetActive(true);
    }

    public void Hide_FindTargetImage()
    {
        _findTargetImage.gameObject.SetActive(false);
    }
}
