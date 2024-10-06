using UnityEngine;
using System;
using System.Collections.Generic;

namespace DenisAlipov.GameTime
{
    /// <summary>
    ///   <para>Class that controls time flow.</para>
    /// </summary>
    public class GameTime : MonoBehaviour
    {
        #region Singleton pattern
        private static GameTime instance;
        public static GameTime Instance
        {
            get
            {
                if (instance == null)
                {
                    //try to find object if its null on request
                    instance = FindObjectOfType<GameTime>();
                }
                if (instance == null)
                {
                    //if object not found - create one
                    GameObject obj = new GameObject();
                    obj.name = nameof(GameTime);
                    instance = obj.AddComponent<GameTime>();
                }
                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance == null)
            {
                //if instance is null - set this object as instance
                instance = this;
                Init();
            }
            else
            {
                //otherwise destroy curren object, as there is other one that is marked as instance
                Destroy(gameObject);
            }
        }
        #endregion

        public string startTime = "8:00";

        [Tooltip("If Unity systems (such as physics) should be affected by time speed.")]
        public bool affectUnitySystems = true;

        [Header("Time Config")]
        [Tooltip("Day duration in realtime seconds.")] public float DayDuration = 300.0f;
        [Tooltip("Time flow multiplier for fast speed.")] public float fastFlowSpeed = 2;
        [Tooltip("Time flow multiplier for super fast speed.")] public float superFastFlowSpeed = 6;

        [Header("Hotkeys")]
        public KeyCode hotkeyTogglePause = KeyCode.Space;
        public KeyCode hotkeyPause = KeyCode.BackQuote;
        public KeyCode hotkeySpeedNormal = KeyCode.Alpha1;
        public KeyCode hotkeySpeedFast = KeyCode.Alpha2;
        public KeyCode hotkeySpeedSuperFast = KeyCode.Alpha3;

        public int CurrentDay { get; private set; } = 0;
        public int CurrentHour { get; private set; } = 0;
        public int CurrentMinute { get; private set; } = 0;

        /// <summary>
        /// Total minutes that has passed since start
        /// </summary>
        public static int TotalMinutes { get { return Instance.totalMinutes; } }
        /// <summary>
        /// If game is currently on pause
        /// </summary>
        public static bool IsPaused { get { return Instance.paused; } }

        private bool paused = false;
        private TimeFlowSpeed lastSpeed;

        private float hourDuration;
        private float minuteDuration;
        private float CurrentTime = 0.0f;

        private int totalMinutes = 0;

        private Action<GameDayTime> OnTimeChanged = delegate (GameDayTime newDayTime) { };
        private Action<TimeFlowSpeed> OnTimeSpeedChanged = delegate (TimeFlowSpeed newTimeFlowSpeed) { };

        private TimeFlowSpeed currentFlowSpeed = TimeFlowSpeed.PAUSE;
        private float currentTimeScale;

        /// <summary>
        /// Adds on time changed callback event
        /// </summary>
        /// <param name="callback"></param>
        public static void AddOnTimeChanged(Action<GameDayTime> callback)
        {
            Instance.OnTimeChanged += callback;
        }

        /// <summary>
        /// Removes on time changed callback event
        /// </summary>
        /// <param name="callback"></param>
        public static void RemoveOnTimeChanged(Action<GameDayTime> callback)
        {
            Instance.OnTimeChanged -= callback;
        }

        /// <summary>
        /// Adds on time speed changed callback event
        /// </summary>
        /// <param name="callback"></param>
        public static void AddOnTimeSpeedChanged(Action<TimeFlowSpeed> callback)
        {
            Instance.OnTimeSpeedChanged += callback;
        }

        /// <summary>
        /// Removes on time speed changed callback event
        /// </summary>
        /// <param name="callback"></param>
        public static void RemoveOnTimeSpeedChanged(Action<TimeFlowSpeed> callback)
        {
            Instance.OnTimeSpeedChanged -= callback;
        }

        /// <summary>
        /// Get current time as GameDayTime
        /// </summary>
        /// <returns></returns>
        public static GameDayTime GetCurrentTime()
        {
            return new GameDayTime(Instance.CurrentDay, Instance.CurrentHour, Instance.CurrentMinute);
        }

        /// <summary>
        /// Enable pause
        /// </summary>
        public static void Pause()
        {
            if (Instance.currentFlowSpeed == TimeFlowSpeed.PAUSE)
            {
                return;
            }
            Instance.SetTimeFlowSpeed(0);
        }


        /// <summary>
        /// Return to normal time flow.
        /// </summary>
        public static void NormalSpeed()
        {
            if (Instance.paused)
            {
                Instance.SetTimeFlowSpeed(TimeFlowSpeed.NORMAL);
            }
        }

        /// <summary>
        /// Sets time flow speed to @newTimeSpeed
        /// </summary>
        /// <param name="newTimeSpeed"></param>
        public void SetTimeFlowSpeed(TimeFlowSpeed newTimeSpeed)
        {
            if (currentFlowSpeed == newTimeSpeed)
            {
                return;
            }

            if (newTimeSpeed == TimeFlowSpeed.PAUSE)
            {
                lastSpeed = currentFlowSpeed;
            }
            currentFlowSpeed = newTimeSpeed;

            paused = false;
            switch (currentFlowSpeed)
            {
                case TimeFlowSpeed.PAUSE:
                    paused = true;
                    currentTimeScale = 0f;
                    break;
                case TimeFlowSpeed.NORMAL:
                    currentTimeScale = 1f;
                    break;
                case TimeFlowSpeed.FAST:
                    currentTimeScale = fastFlowSpeed;
                    break;
                case TimeFlowSpeed.SUPER_FAST:
                    currentTimeScale = superFastFlowSpeed;
                    break;

                default:
                    currentTimeScale = 1.0f;
                    break;
            }
            if (affectUnitySystems)
            {
                Time.timeScale = currentTimeScale;
            }
            OnTimeSpeedChanged(currentFlowSpeed);
        }

        // Use this for initialization
        void Init()
        {
            //Set speed to normal on start
            SetTimeFlowSpeed(TimeFlowSpeed.NORMAL);

            if (Mathf.Equals(DayDuration, 0.0f))
            {
                Debug.LogWarning("Day duration shouldn't be 0. Setting default to 100.");
                DayDuration = 100.0f;
            }
            hourDuration = DayDuration / 24;
            minuteDuration = hourDuration / 60;

            var split_values = startTime.Split(':');
            CurrentHour = 0;
            CurrentMinute = 0;
            if (split_values.Length == 2)
            {
                CurrentHour = Int32.Parse(split_values[0]);
                CurrentMinute = Int32.Parse(split_values[1]);
            }
            else
            {
                Debug.LogError("Timer: Incorrect time set");
            }
            CurrentTime = DayDuration + (float)CurrentHour * hourDuration + (hourDuration / 60.0f * (float)CurrentMinute);
            CurrentDay = Mathf.FloorToInt(CurrentTime / DayDuration);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTime();
            ProcessInput();
        }

        void UpdateTime()
        {
            float deltaTime = Time.deltaTime;

            if (!affectUnitySystems)
            {
                deltaTime *= currentTimeScale;
            }

            CurrentTime += deltaTime;

            int newHour = Mathf.FloorToInt((CurrentTime * 24) / DayDuration) % 24;
            int newMinutes = Mathf.FloorToInt(((CurrentTime - (newHour * hourDuration)) * 60) / hourDuration) % 60;
            int newDay = Mathf.FloorToInt(CurrentTime / DayDuration);

            CurrentDay = newDay;

            bool timeChanged = false;

            if (newHour != CurrentHour)
            {
                CurrentHour = newHour;
                timeChanged = true;

                if (CurrentHour == 0)
                {
                    //TODO - new day!
                }
            }

            if (newMinutes != CurrentMinute)
            {
                CurrentMinute = newMinutes;
                totalMinutes = Mathf.FloorToInt((CurrentTime * 60) / hourDuration);
                timeChanged = true;
            }

            if (timeChanged)
            {
                OnTimeChanged?.Invoke(new GameDayTime(CurrentDay, CurrentHour, CurrentMinute));
            }
        }

        void ProcessInput()
        {
            if (Input.GetKeyDown(hotkeyTogglePause))
            {
                if (paused)
                {
                    SetTimeFlowSpeed(lastSpeed);
                }
                else
                {
                    SetTimeFlowSpeed(TimeFlowSpeed.PAUSE);
                }
            }
            else if (Input.GetKeyDown(hotkeyPause))
            {
                SetTimeFlowSpeed(TimeFlowSpeed.PAUSE);
            }
            else if (Input.GetKeyDown(hotkeySpeedNormal))
            {
                SetTimeFlowSpeed(TimeFlowSpeed.NORMAL);
            }
            else if (Input.GetKeyDown(hotkeySpeedFast))
            {
                SetTimeFlowSpeed(TimeFlowSpeed.FAST);
            }
            else if (Input.GetKeyDown(hotkeySpeedSuperFast))
            {
                SetTimeFlowSpeed(TimeFlowSpeed.SUPER_FAST);
            }
        }
    }
}