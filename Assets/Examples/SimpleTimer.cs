using Time = Moqunity.Abstract.UnityEngine.Time;//comment this line to get rid of Moqunity
using UnityEngine;
using System;

public class SimpleTimer : MonoBehaviour
{
    private float fireTime = 0.0f;
    private bool isActive = false;
    private Action fireCallback = null;

    private Time Time { get; } = Moqunity.Context.Factory.Time;//comment this line to get rid of Moqunity

    public void Set(float time, Action callback)
    {
        if (isActive) return;
        fireTime = Time.realtimeSinceStartup + time;
        fireCallback = callback;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;
        if (Time.realtimeSinceStartup >= fireTime)
        {
            fireCallback?.Invoke();
            isActive = false;
        }
    }
}