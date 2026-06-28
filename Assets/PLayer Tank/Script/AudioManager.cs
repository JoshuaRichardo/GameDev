using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent singleton that owns master-volume save/load across all scenes.
///
/// Place a single GameObject named "AudioManager" in the Demo 2 scene and
/// attach this component. DontDestroyOnLoad keeps it alive for the whole
/// session so no other scene needs a copy.
///
/// PlayerPrefs key "Volume" matches the key already used by MainMenuManager,
/// so both scripts stay in sync automatically.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ── Constants ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Shared PlayerPrefs key. Must match the key used in MainMenuManager
    /// ("Volume") so that saves written by either script are read by both.
    /// </summary>
    private const string VolumeKey = "Volume";

    private const float DefaultVolume = 0.7f;

    // ── Singleton ─────────────────────────────────────────────────────────────

    private static AudioManager instance;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        // Only one AudioManager may exist. Destroy any duplicate that appears
        // if the Demo 2 scene is ever reloaded during a session.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Apply immediately so audio is correct from the very first frame.
        ApplySavedVolume();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ── Scene Events ──────────────────────────────────────────────────────────

    /// <summary>
    /// Fired every time a scene finishes loading.
    /// Re-applying here guarantees the correct volume even if any scene-level
    /// setup or AudioListener reset happened during the load.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySavedVolume();
    }

    // ── Public Volume API ─────────────────────────────────────────────────────

    /// <summary>
    /// Sets the master volume, applies it to AudioListener, and persists it.
    /// Wire this to the volume Slider's OnValueChanged event in the Inspector
    /// (use the non-static / instance version shown in the dropdown).
    /// </summary>
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);

        // PlayerPrefs.Save() forces an immediate disk write.
        // Without it the OS may buffer the write; on abnormal exit the save is lost.
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns the persisted volume, or DefaultVolume if nothing is saved yet.
    /// Use this in other scripts to initialise a Slider's starting value.
    /// Example:  volumeSlider.value = AudioManager.GetSavedVolume();
    /// </summary>
    public static float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Reads PlayerPrefs and directly sets AudioListener.volume.
    /// Does not touch any UI slider.
    /// </summary>
    private void ApplySavedVolume()
    {
        AudioListener.volume = GetSavedVolume();
    }
}
