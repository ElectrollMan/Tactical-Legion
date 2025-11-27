using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBaseUtil
{
    public class BaseTimer : MonoBehaviour
    {
        private Action timerCallback;
        private float timerDuration;

        public void SetTimer(float duration, Action callback)
        {
            timerDuration = duration;
            timerCallback = callback;
        }

        private void Update()
        {
            if (timerDuration > 0f)
            {
                timerDuration -= Time.deltaTime;
                if (IsTimerComplete())
                {
                    timerCallback?.Invoke();
                }
            }
        }

        private bool IsTimerComplete()
        {
            return timerDuration <= 0;
        }
    }
}
