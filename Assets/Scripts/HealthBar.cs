using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image HP;
    public Image Dead;
    public Battler Health;
    private void Awake()
    {
        Health.HealthChanged += OnHealthChanged;
    }
    private void OnDestroy()
    {
        Health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float ValueAsPercent)
    {
        HP.fillAmount = ValueAsPercent;
    }
}
