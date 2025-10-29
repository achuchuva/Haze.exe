using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using URPGlitch;

public class GlitchManager : MonoBehaviour
{
    public static GlitchManager Instance;
    public bool GlitchEffectEnabled;
    [SerializeField] private Volume volume;
    private bool isEnabled = true;

    private AnalogGlitchVolume analogGlitchVolume;
    private DigitalGlitchVolume digitalGlitchVolume;
    private AudioSource glitchAudioSource;

    private float scanLineJitter;
    private float verticalJump;
    private float horizontalShake;
    private float colorDrift;
    private float intensity;
    private float glitchPitch;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGlitchEffect(bool isEnabled)
    {
        GlitchEffectEnabled = isEnabled;
        if (!GlitchEffectEnabled)
        {
            DisableEffects();
        }
        else if (SceneManager.GetActiveScene().name == "Menu")
        {
            EnableEffects();
        }
    }

    private void Start()
    {
        volume.profile.TryGet<AnalogGlitchVolume>(out analogGlitchVolume);
        scanLineJitter = analogGlitchVolume.scanLineJitter.value;
        verticalJump = analogGlitchVolume.verticalJump.value;
        horizontalShake = analogGlitchVolume.horizontalShake.value;
        colorDrift = analogGlitchVolume.colorDrift.value;
        volume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);
        intensity = digitalGlitchVolume.intensity.value;
        glitchAudioSource = GetComponent<AudioSource>();
        glitchPitch = glitchAudioSource.pitch;
        if (GlitchEffectEnabled && SceneManager.GetActiveScene().name == "Menu")
            EnableEffects();
        else
            DisableEffects();
    }

    public void EnableEffects()
    {
        if (!GlitchEffectEnabled) return;
        isEnabled = true;
        if (glitchAudioSource != null && !glitchAudioSource.isPlaying)
            glitchAudioSource.Play();
        if (analogGlitchVolume != null)
            analogGlitchVolume.active = true;
        if (digitalGlitchVolume != null)
            digitalGlitchVolume.active = true;
    }

    public void DisableEffects()
    {
        isEnabled = false;
        if (glitchAudioSource != null && glitchAudioSource.isPlaying)
            glitchAudioSource.Stop();
        if (analogGlitchVolume != null)
            analogGlitchVolume.active = false;
        if (digitalGlitchVolume != null)
            digitalGlitchVolume.active = false;
    }

    public void Reset()
    {
        if (glitchAudioSource != null && glitchAudioSource.isPlaying)
            glitchAudioSource.Stop();
        glitchAudioSource.pitch = glitchPitch;
        analogGlitchVolume.active = false;
        digitalGlitchVolume.active = false;
        // Reset to default values
        analogGlitchVolume.scanLineJitter.value = scanLineJitter;
        analogGlitchVolume.verticalJump.value = verticalJump;
        analogGlitchVolume.horizontalShake.value = horizontalShake;
        analogGlitchVolume.colorDrift.value = colorDrift;
        digitalGlitchVolume.intensity.value = intensity;
    }

    public void SetEffects(float intensity)
    {
        if (!GlitchEffectEnabled) return;
        intensity = Mathf.Clamp(intensity, 0f, 0.4f);
        if (glitchAudioSource != null && !glitchAudioSource.isPlaying)
            glitchAudioSource.Play();
            glitchAudioSource.pitch = 0.5f + intensity * 3f;

        if (analogGlitchVolume != null)
            analogGlitchVolume.active = true;
            analogGlitchVolume.scanLineJitter.value = intensity;
            analogGlitchVolume.verticalJump.value = intensity;
            analogGlitchVolume.horizontalShake.value = intensity;
            analogGlitchVolume.colorDrift.value = intensity;
        if (digitalGlitchVolume != null)
            digitalGlitchVolume.active = true;
            digitalGlitchVolume.intensity.value = intensity;
    }
}
