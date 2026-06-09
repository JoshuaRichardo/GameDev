using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Unity.AI.Assistant.PlayModeTest
{
    [InitializeOnLoad]
    internal static class PlayModeTestRunner
    {
        private const string StateKey = "PlayModeTest.State";
        private const string ResultKey = "PlayModeTest.Result";
        private const string ScriptPathKey = "PlayModeTest.ScriptPath";
        private const string SentinelLog = "PLAY_MODE_TEST_COMPLETE";

        private static readonly int WaitFrames = SessionState.GetInt("PlayModeTest.WaitFrames", 25);
        private static readonly float TestTimeout = SessionState.GetFloat("PlayModeTest.TestTimeout", 6.0f);

        private static List<string> _capturedLogs = new List<string>();
        private const int MaxCapturedLogs = 80;

        static PlayModeTestRunner()
        {
            string state = SessionState.GetString(StateKey, "Idle");
            switch (state)
            {
                case "WaitingForCompile":
                    EditorApplication.delayCall += () =>
                    {
                        SessionState.SetString(StateKey, "EnteringPlayMode");
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        EditorApplication.isPlaying = true;
                    };
                    break;
                case "EnteringPlayMode":
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                        SessionState.SetString(StateKey, "InPlayMode");
                        EditorApplication.update += Tick;
                    }
                    break;
                case "InPlayMode":
                    if (EditorApplication.isPlaying) EditorApplication.update += Tick;
                    break;
                case "Done":
                    Debug.Log(SentinelLog);
                    EditorApplication.delayCall += SelfDestruct;
                    break;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                SessionState.SetString(StateKey, "InPlayMode");
                EditorApplication.update += Tick;
            }
        }

        private static int _frameCount = 0;
        private static bool _setupDone = false;
        private static bool _spawned = false;
        private static bool _testDone = false;
        private static double _testStartTime = 0;

        private static int _frontDamage = -1;
        private static int _sideDamage = -1;
        private static int _rearDamage = -1;

        private static void Tick()
        {
            EditorApplication.QueuePlayerLoopUpdate();

            _frameCount++;
            if (_frameCount < WaitFrames) return;
            if (_testDone) return;

            if (!_setupDone)
            {
                _setupDone = true;
                Application.logMessageReceived += OnLogMessage;
                _testStartTime = EditorApplication.timeSinceStartup;
                Time.timeScale = 1f;
                Application.runInBackground = true;

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                foreach (var en in new[] { "Sherman_Aggro", "Sherman_Flanker" })
                {
                    GameObject e = GameObject.Find(en);
                    if (e != null) e.transform.position = new Vector3(60f, 60f, 0f);
                }
                return;
            }

            float elapsed = (float)(EditorApplication.timeSinceStartup - _testStartTime);

            // Verify percentage damage directly via the armor (deterministic), then
            // verify the engine-hit stun + UI.
            if (!_spawned && elapsed >= 0.5f)
            {
                _spawned = true;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                var armor = player != null ? player.GetComponent<TankArmor>() : null;
                var th = player != null ? player.GetComponent<TankHealth>() : null;
                if (armor != null && th != null)
                {
                    int hp0 = th.GetCurrentHealth();
                    armor.RegisterHit(TankArmor.ArmorSide.Front, 25);
                    int hp1 = th.GetCurrentHealth(); _frontDamage = hp0 - hp1;
                    armor.RegisterHit(TankArmor.ArmorSide.Left, 25);
                    int hp2 = th.GetCurrentHealth(); _sideDamage = hp1 - hp2;
                    armor.RegisterHit(TankArmor.ArmorSide.Rear, 25);
                    int hp3 = th.GetCurrentHealth(); _rearDamage = hp2 - hp3;
                }
                return;
            }

            if (elapsed >= TestTimeout) FinishTest();
        }

        private static void FinishTest()
        {
            _testDone = true;
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLogMessage;

            var sb = new System.Text.StringBuilder();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            TankArmor armor = player != null ? player.GetComponent<TankArmor>() : null;

            sb.Append("FrontDmg=" + _frontDamage + " (expect 3-5)");
            sb.Append(" SideDmg=" + _sideDamage + " (expect 30)");
            sb.Append(" RearDmg=" + _rearDamage + " (expect 15)");
            if (armor != null)
            {
                sb.Append(" | Front=" + armor.FrontHits + " Left=" + armor.LeftHits + " Right=" + armor.RightHits + " Rear=" + armor.RearHits);
                sb.Append(" IsStunned=" + armor.IsStunned + " StunRemaining=" + armor.StunTimeRemaining.ToString("F1"));
                var pt = player.GetComponent<PlayerTank>();
                sb.Append(" MoveEnabledDuringStun=" + (pt != null ? pt.enabled.ToString() : "?"));
            }

            var ui = Object.FindAnyObjectByType<CombatNotificationUI>();
            if (ui != null)
            {
                sb.Append(" | shotText='" + (ui.shotCountText != null ? ui.shotCountText.text.Replace("\n", " / ") : "null") + "'");
                sb.Append(" engineActive=" + (ui.engineNotificationText != null ? ui.engineNotificationText.gameObject.activeSelf.ToString() : "null"));
                sb.Append(" engineText='" + (ui.engineNotificationText != null && ui.engineNotificationText.gameObject.activeSelf ? ui.engineNotificationText.text : "(hidden)") + "'");
            }

            var result = new TestResult { success = true, summary = sb.ToString(), logs = _capturedLogs.ToArray() };
            SessionState.SetString(ResultKey, JsonUtility.ToJson(result));
            SessionState.SetString(StateKey, "Done");
            EditorApplication.isPlaying = false;
        }

        private static void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (_capturedLogs.Count >= MaxCapturedLogs) return;
            if (type == LogType.Error || type == LogType.Exception || message.Contains("[Test]"))
                _capturedLogs.Add("[" + type + "] " + message);
        }

        private static void SelfDestruct()
        {
            string scriptPath = SessionState.GetString(ScriptPathKey, "");
            if (!string.IsNullOrEmpty(scriptPath) && AssetDatabase.AssetPathExists(scriptPath))
                AssetDatabase.DeleteAsset(scriptPath);
            SessionState.EraseString(StateKey);
            SessionState.EraseString(ScriptPathKey);
        }

        [System.Serializable]
        private class TestResult
        {
            public bool success;
            public string summary;
            public string[] logs;
        }
    }
}
