using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int _maxHealth = 100;
    private int _currentHealth;
    public event System.Action<float> HealthChanged;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void ChangeHealth(int value)
    {
        _currentHealth -= value;

        if (_currentHealth <= 0)
        {
            Death(this.gameObject);
        }
        else
        {
            float currentHPasPercent = (float)_currentHealth / _maxHealth;
            HealthChanged?.Invoke(currentHPasPercent);
        }

    }
    public void Death(GameObject battler)
    {
        battler = null;
    }
}
