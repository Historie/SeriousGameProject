using System.Collections;
using System.Collections.Generic;
using DenisAlipov.GameTime;
using UnityEngine;

namespace DenisAlipov.GameTime
{
    public class Streetlight : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private int hourEnable = 21;

        [SerializeField]
        private int hourDisable = 5;

        [Header("References")]
        [SerializeField]
        private GameObject lightSource;

        private int lastActionHour;

        private void Start()
        {
            GameTime.AddOnTimeChanged(UpdateLight);
        }

        private void UpdateLight(GameDayTime newTime)
        {
            int newTimeHours = newTime.Hours;
            if (lastActionHour == newTimeHours)
            {
                return;
            }

            if (newTimeHours == hourEnable)
            {
                lightSource.SetActive(true);
                lastActionHour = newTimeHours;
                return;
            }

            if (newTimeHours == hourDisable)
            {
                lightSource.SetActive(false);
                lastActionHour = newTimeHours;
                return;
            }
        }
    }
}
