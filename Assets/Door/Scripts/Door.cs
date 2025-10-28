using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Door : MonoBehaviour
{
    // Static reference to the door currently controlling the collect prompt (closest valid one)
    private static Door focusedDoor;

    [Header("Door Interaction")]
    public float openDistance = 10f;
    public float collectDistance = 3f;
    public GameObject collectEffect;
    public Transform door;
    Animator animator;
    Transform player;

    [Header("Door Settings")]
    public MeshRenderer doorMesh;
    public ParticleSystem particleSystem;
    public Image icon;
    public bool endingDoor = false;
    public bool endGameDoor = false;
    public AudioSource completionMusic;
    public AudioSource mazeMusic;
    public string sceneToLoad = "Level 2";

    DoorManager.DoorSettings doorSettings;
    GameObject collectPrompt;

    public Door Other { get; set; } = null;
    public bool Close { get; set; } = false;
    public bool Far { get; set; } = false;
    bool endingTriggered = false;
    bool completionMusicPlayed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform.root;
        collectPrompt = GameObject.FindObjectOfType<DoorManager>().collectPrompt;
    }

    void Update()
    {
        float distance = Vector3.Distance(door.position, player.position);

        // Door open animation purely distance-based (can stay regardless of focus)
        animator.SetBool("Open", distance < openDistance);

        if (endingDoor && distance < 8f && !endingTriggered)
        {
            endingTriggered = true;
            if (endGameDoor)
            {
                LevelLoader.Instance.EndGame();
            }
            else
            {
                LevelLoader.Instance.LoadNextMazeLevel(sceneToLoad);
            }
        }

        if (endGameDoor && distance < 20f && !completionMusicPlayed)
        {
            completionMusicPlayed = true;
            if (mazeMusic.isPlaying)
            {
                // Fade out maze music
                StartCoroutine(FadeOut(mazeMusic, 1f));
            }
        }

        // Attempt to acquire focus if this door is a valid candidate
        TryAcquireFocus(distance);

        // Handle prompt visibility only if this door is the focused door
        if (focusedDoor == this)
        {
            bool canShow = CanShowPrompt(distance);
            if (canShow && !collectPrompt.activeSelf)
            {
                collectPrompt.SetActive(true);
            }
            else if (!canShow && collectPrompt.activeSelf)
            {
                collectPrompt.SetActive(false);
            }

            // Collect attempt
            if (canShow && Input.GetKeyDown(KeyCode.E) && distance < collectDistance)
            {
                CollectDoor();
            }
        }
        else
        {
            // Non-focused doors never toggle the global prompt
        }
    }

    public void CollectDoor()
    {
        // Re-validate LOS before collecting
        if (!HasLineOfSight()) return;

        if (focusedDoor == this)
        {
            collectPrompt.SetActive(false);
            focusedDoor = null; // release focus so next closest can claim
        }

        // Collect the doors
        DoorManager.Instance.doorCount += 2;
        doorSettings.inUse = false;

        GameObject effect = Instantiate(collectEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(effect, 3f);

        GameObject otherEffect = Instantiate(collectEffect, Other.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(otherEffect, 3f);

        if (Other)
        {
            Destroy(Other.gameObject);
        }
        Destroy(gameObject);
    }

    public void SetDoorSettings(DoorManager.DoorSettings settings)
    {
        doorSettings = settings;
        doorMesh.material = settings.material;
        var main = particleSystem.main;
        main.startColor = new ParticleSystem.MinMaxGradient(settings.particleColor1, settings.particleColor2);
        icon.color = settings.iconColor;
    }

    /// <summary>
    /// Determine if this door should show a prompt based on distance, having a paired door (Other), and line of sight.
    /// </summary>
    private bool CanShowPrompt(float distance)
    {
        if (!Other) return false; // needs pair to collect
        if (distance > openDistance) return false; // outside interaction radius
        if (!HasLineOfSight()) return false; // blocked
        return true;
    }

    /// <summary>
    /// Determines if there is unobstructed line of sight from the player to the door root.
    /// </summary>
    private bool HasLineOfSight()
    {
        // Use eye-height ray (approx 1.6m). Could parameterize.
        Vector3 origin = player.position + Vector3.up * 1.6f;
        Vector3 target = door.position + Vector3.up * 0.5f; // mid-height inside door area
        Vector3 dir = (target - origin).normalized;
        float maxDistance = Vector3.Distance(origin, target);
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, maxDistance))
        {
            // Accept hits that are part of this door's hierarchy
            if (hit.transform != door && hit.transform.root != door.root)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Attempt to set this door as the focused door if no focused door exists or this one is closer.
    /// </summary>
    private void TryAcquireFocus(float distance)
    {
        // If current focus is invalid (destroyed) clear it
        if (focusedDoor == null)
        {
            if (CanShowPrompt(distance))
            {
                focusedDoor = this;
            }
            return;
        }

        if (focusedDoor == this)
        {
            // If this door no longer qualifies, release focus
            if (!CanShowPrompt(distance))
            {
                collectPrompt.SetActive(false);
                focusedDoor = null;
            }
            return;
        }

        // Compete for focus only if we qualify
        if (!CanShowPrompt(distance)) return;

        // Compare distances; if this door is closer than current focused and current focused cannot collect (LOS lost) or this is closer, switch
        float currentFocusedDistance = Vector3.Distance(focusedDoor.door.position, player.position);
        if (distance < currentFocusedDistance)
        {
            // Turn off prompt controlled by previous focused door
            if (collectPrompt.activeSelf)
            {
                collectPrompt.SetActive(false);
            }
            focusedDoor = this;
        }
    }

    private void OnDestroy()
    {
        if (focusedDoor == this && collectPrompt)
        {
            collectPrompt.SetActive(false);
            focusedDoor = null;
        }
    }

    public IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop ();
        audioSource.volume = startVolume;
        StartCoroutine(FadeIn(completionMusic, FadeTime));
    }

    public IEnumerator FadeIn (AudioSource audioSource, float FadeTime) {
        float targetVolume = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume) {
            audioSource.volume += targetVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && endingDoor)
        {
            float normalizedDistance = 1f - (Vector3.Distance(door.position, other.transform.position) / 15f);
            GlitchManager.Instance.SetEffects(normalizedDistance);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && endingDoor)
        {
            GlitchManager.Instance.Reset();
        }
    }
}
