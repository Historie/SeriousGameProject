using System.Collections;
using System.Collections.Generic;
using DenisAlipov.GameTime;
using UnityEngine;

namespace DenisAlipov.GameTime
{
    public class LightDayNight : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Gradient color for day cycle")]
        private Gradient colorNightDay;

        [SerializeField]
        [Tooltip("Sun rotation for day cycle")]
        private AnimationCurve sunRotationCycle;

        [SerializeField]
        [Tooltip("Light component")]
        private Light sunLight;

        [SerializeField]
        [Tooltip("Sun's parent to rotate")]
        private Transform sunParent;

        private void Start()
        {
            GameTime.AddOnTimeChanged(UpdateLightForDay);
        }

        void UpdateLightForDay(GameDayTime newTime)
        {
            float dayProgress = (newTime.Hours * 60 + newTime.Minutes) / 1440f;
            sunLight.color = colorNightDay.Evaluate(dayProgress);

            float sunRotation = sunRotationCycle.Evaluate(dayProgress) * -360;
            sunParent.rotation = Quaternion.Euler(sunRotation, 180, 0);
        }
    }
}