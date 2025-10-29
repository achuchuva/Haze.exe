using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Toggle glitchEffectToggle;
    public AudioSource toggleSound;
    bool canPlayToggleSound = false;

    void Start()
    {
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        if (PlayerPrefs.HasKey("GlitchEffect"))
        {
            bool isEnabled = PlayerPrefs.GetInt("GlitchEffect") == 1;
            GlitchManager.Instance.SetGlitchEffect(isEnabled);
            glitchEffectToggle.isOn = isEnabled;
        }
        else
        {
            GlitchManager.Instance.SetGlitchEffect(true);
            glitchEffectToggle.isOn = true;
        }
        Invoke("EnableToggleSound", 0.1f);
    }

    void EnableToggleSound()
    {
        canPlayToggleSound = true;
    }

    public void SetMasterVolume(float volume)
    {
        if (volume == -40f)
        {
            volume = -80f; // Mute
        }
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == -40f)
        {
            volume = -80f; // Mute
        }
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetGlitchEffect(bool isEnabled)
    {
        int value = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt("GlitchEffect", value);
        GlitchManager.Instance.SetGlitchEffect(isEnabled);
        if (canPlayToggleSound)
            toggleSound.Play();
    }
}
