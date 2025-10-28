using UnityEngine;

public class Glitch : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlitchManager.Instance.EnableEffects();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlitchManager.Instance.DisableEffects();
        }
    }
}
