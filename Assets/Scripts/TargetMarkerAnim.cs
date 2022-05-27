using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingMarkerAnimation : MonoBehaviour
{
    [SerializeField] float Amount;
    [SerializeField] float Speed;
    Vector3 localPos;
    void Start()
    {
        localPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = localPos + Vector3.up * Mathf.Sin(Time.time * Speed) * Amount;
    }
}
