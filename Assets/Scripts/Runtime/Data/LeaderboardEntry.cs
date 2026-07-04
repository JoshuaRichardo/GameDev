using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Data
{
    /// <summary>
    /// Represents a single entry in the leaderboard.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public float Time;
    }

    /// <summary>
    /// Wrapper for a list of leaderboard entries for JSON serialization.
    /// </summary>
    [Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
    }
}
