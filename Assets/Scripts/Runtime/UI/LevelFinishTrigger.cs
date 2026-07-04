using UnityEngine;

namespace TankGame.UI
{
    /// <summary>
    /// Handles the final finish line trigger in Level 4.
    /// Stops the persistent timer and saves to the leaderboard.
    /// </summary>
    public class LevelFinishTrigger : MonoBehaviour
    {
        [SerializeField] private LeaderboardManager leaderboardManager;
        [SerializeField] private string playerTag = "Player";

        private bool triggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggered && other.CompareTag(playerTag))
            {
                triggered = true;
                FinishGame();
            }
        }

        private void FinishGame()
        {
            if (SpeedrunTimer.Instance != null)
            {
                SpeedrunTimer.Instance.StopTimer();
                
                if (leaderboardManager != null)
                {
                    leaderboardManager.AddEntry(SpeedrunTimer.Instance.TotalTime);
                }
            }
        }
    }
}
