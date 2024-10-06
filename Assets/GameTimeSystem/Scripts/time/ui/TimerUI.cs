using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DenisAlipov.GameTime
{
    /// <summary>
    /// Class for working with time from ui
    /// </summary>
    public class TimerUI : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("If true time will be in 24h format, otherwise in 12h.")]
        public bool use24Hours;

        [Header("Elements")]
        [Tooltip("Text field to set time string to.")]
        public Text timeTextField;
        [Tooltip("Text field to set day string to.")]
        public Text dayTextField;
        [Tooltip("Frame game object that is active on pause")]
        public GameObject pauseFrame;
        [Tooltip("List of buttons in order: pause, normal, fast, super fast")]
        public List<Button> speedButtons;

        private void Start()
        {
            GameTime.AddOnTimeChanged(SetTimeTextField);
            GameTime.AddOnTimeSpeedChanged(TimeSpeedChanged);

            InitButtons();
            SetTimeTextField(GameTime.GetCurrentTime());
        }

        private void InitButtons()
        {
            for (var i = 0; i < speedButtons.Count; i++)
            {
                TimeFlowSpeed timeFlow = (TimeFlowSpeed) i;
                speedButtons[i].onClick.AddListener((
                    () =>
                    {
                        GameTime.Instance.SetTimeFlowSpeed(timeFlow);
                    }
                ));
            }

            TimeSpeedChanged(TimeFlowSpeed.NORMAL);
        }

        private void TimeSpeedChanged(TimeFlowSpeed timeFlow)
        {
            for (var i = 0; i < speedButtons.Count; i++)
            {
                Button btn = speedButtons[i];
                btn.interactable = (int)timeFlow != i; //disable button with current time flow 
            }

            //activate pause frame if needed
            pauseFrame.SetActive(timeFlow == TimeFlowSpeed.PAUSE);
        }

        private void SetTimeTextField(GameDayTime gameDayTime)
        {
            string days = gameDayTime.Days.ToString();
            string mins = gameDayTime.Minutes.ToString("D2");

            string hours;
            if (use24Hours)
            {
                hours = gameDayTime.Hours.ToString("D2");
            }
            else
            {
                int hour = gameDayTime.Hours;
                string postfix = "AM";
                if (hour >= 12)
                {
                    postfix = "PM";
                }

                if (hour == 0)
                {
                    hour = 12;
                }
                else if (hour > 12)
                {
                    hour = hour - 12;
                }

                hours = hour.ToString("D2");
                mins = mins + " " + postfix;
            }

            dayTextField.text = days;
            timeTextField.text = hours + ":" + mins;
        }
    }
}