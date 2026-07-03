using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages global master volume via AudioListener.volume.
/// Persists the value across sessions using PlayerPrefs.
/// Attach this to a persistent GameObject (DontDestroyOnLoad) in your first scene,
/// or directly on the Main Menu canvas root in "Demo 2".
/// </summary>
public class VolumeManager : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private const float DefaultVolume = 0.5f;

    [Header("Assign the master volume Slider from the Demo 2 scene here")]
    [SerializeField] private Slider masterVolumeSlider;

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        ApplyVolume(savedVolume);
    }

    private void Start()
    {
        if (masterVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);

            // Update the slider without triggering OnValueChanged
            masterVolumeSlider.SetValueWithoutNotify(savedVolume);

            // Subscribe after setting the initial value to avoid a double-save on Start
            masterVolumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogWarning("[VolumeManager] masterVolumeSlider is not assigned. " +
                             "Volume will still be applied globally, but the UI will not update.");
        }
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    /// <summary>
    /// Called automatically by the Slider's onValueChanged event.
    /// Applies the new volume and saves it immediately to PlayerPrefs.
    /// </summary>
    /// <param name="value">Volume level in the range [0, 1].</param>
    public void OnSliderValueChanged(float value)
    {
        ApplyVolume(value);
        SaveVolume(value);
    }

    private static void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }

    private static void SaveVolume(float value)
    {
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}
