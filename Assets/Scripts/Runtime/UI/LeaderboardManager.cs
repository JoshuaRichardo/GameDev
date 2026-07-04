using System.Collections.Generic;
using UnityEngine;
using TankGame.Data;

namespace TankGame.UI
{
    /// <summary>
    /// Manages the local leaderboard system using PlayerPrefs.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private TMPro.TextMeshProUGUI[] entryTexts;

        private const string LeaderboardKey = "TankGame_Leaderboard";
        private const int MaxEntries = 5;

        private void Start()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Saves a new time to the leaderboard if it qualifies.
        /// </summary>
        /// <param name="newTime">The time to save.</param>
        public void AddEntry(float newTime)
        {
            LeaderboardData data = LoadLeaderboard();
            data.Entries.Add(new LeaderboardEntry { Time = newTime });

            // Sort ascending (fastest first)
            data.Entries.Sort((a, b) => a.Time.CompareTo(b.Time));

            // Keep only the top 5
            if (data.Entries.Count > MaxEntries)
            {
                data.Entries.RemoveRange(MaxEntries, data.Entries.Count - MaxEntries);
            }

            SaveLeaderboard(data);
            UpdateUI(data);
            ShowLeaderboard();
        }

        /// <summary>
        /// Clears all leaderboard data from PlayerPrefs.
        /// </summary>
        public void ClearLeaderboard()
        {
            PlayerPrefs.DeleteKey(LeaderboardKey);
            PlayerPrefs.Save();
            UpdateUI(new LeaderboardData());
        }

        public void ShowLeaderboard()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(true);
                UpdateUI(LoadLeaderboard());
            }
        }

        private LeaderboardData LoadLeaderboard()
        {
            string json = PlayerPrefs.GetString(LeaderboardKey, "");
            if (string.IsNullOrEmpty(json))
            {
                return new LeaderboardData();
            }
            return JsonUtility.FromJson<LeaderboardData>(json);
        }

        private void SaveLeaderboard(LeaderboardData data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(LeaderboardKey, json);
            PlayerPrefs.Save();
        }

        private void UpdateUI(LeaderboardData data)
        {
            for (int i = 0; i < entryTexts.Length; i++)
            {
                if (i < data.Entries.Count)
                {
                    float t = data.Entries[i].Time;
                    int minutes = (int)(t / 60f);
                    int seconds = (int)(t % 60f);
                    int milliseconds = (int)((t * 100f) % 100f);
                    entryTexts[i].text = string.Format("{0}. {1:00}:{2:00}.{3:00}", i + 1, minutes, seconds, milliseconds);
                }
                else
                {
                    entryTexts[i].text = string.Format("{0}. ---", i + 1);
                }
            }
        }
    }
}
