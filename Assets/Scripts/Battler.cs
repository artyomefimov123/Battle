using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;

public class Battler : MonoBehaviour
{
    public string battlerName;
    public int _maxHealth = 100;
    public int _currentHealth;
    public event System.Action<float> HealthChanged;
    public UnityEvent EventOnDie;
    public bool isEnemy;
    public int turnOrder;
    public float markerOffset; 
    public bool usesSpine;
    public GameObject Smoke;

    public Spine.AnimationState animationState;
    [SpineAnimation]
    public string idleAnim;
    [SpineAnimation]
    public string walkAnim;
    [SpineAnimation]
    public string attackAnim;
    [SpineAnimation]
    public string hitAnim;

    private void Start()
    {
        _currentHealth = _maxHealth;
        if (usesSpine) animationState = GetComponent<SkeletonAnimation>().AnimationState;
    }

    public Battler AI_MakeDecision(List<Battler> enemies)
    {
        if (Random.value < 0.1)
        {
            return null; 
        }
        else
        {
            return enemies[Random.Range(0, enemies.Count)];
        }
    }
    public void ChangeHealth(int value)
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
        Destroy(battler);
        Vector3 EffectPosition = battler.transform.position;
        EffectPosition.y += 5f;
        Instantiate(Smoke, EffectPosition, battler.transform.rotation);
    }
}
