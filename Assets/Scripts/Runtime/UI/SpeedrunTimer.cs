using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace TankGame.UI
{
    /// <summary>
    /// Persistent manager for the campaign speedrun timer.
    /// Tracks total time across multiple levels.
    /// </summary>
    public class SpeedrunTimer : MonoBehaviour
    {
        public static SpeedrunTimer Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private string timerTextTag = "TimerText";

        private TextMeshProUGUI activeTimerText;
        private float totalTime;
        private bool isRunning;

        public float TotalTime => totalTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            totalTime = 0f;
            isRunning = true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void Update()
        {
            if (isRunning)
            {
                totalTime += Time.deltaTime;
                UpdateTimerUI();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        /// <summary>
        /// Resets the timer to zero.
        /// </summary>
        public void ResetTimer()
        {
            totalTime = 0f;
            isRunning = true;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Find the timer text component in the new scene by tag
            GameObject timerObj = GameObject.FindWithTag(timerTextTag);
            if (timerObj != null)
            {
                activeTimerText = timerObj.GetComponent<TextMeshProUGUI>();
            }
        }

        private void UpdateTimerUI()
        {
            if (activeTimerText != null)
            {
                int minutes = (int)(totalTime / 60f);
                int seconds = (int)(totalTime % 60f);
                int milliseconds = (int)((totalTime * 100f) % 100f);
                activeTimerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            }
        }
    }
}
