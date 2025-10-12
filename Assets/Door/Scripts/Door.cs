using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
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

    DoorManager.DoorSettings doorSettings;

    public Door Other { get; set; } = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform.root;
    }

    void Update()
    {
        // Calculate the distance between the door and the player.
        float distance = Vector3.Distance(door.position, player.position);
        if (distance < openDistance)
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Open", false);
        }

        if (Input.GetKeyDown(KeyCode.E) && distance < collectDistance && Other)
        {
            CollectDoor();
        }
    }

    public void CollectDoor()
    {
        // Shoot a ray from the player to the door to see if there is a clear line of sight.
        RaycastHit hit;
        Vector3 direction = (door.position - player.position).normalized;
        if (Physics.Raycast(player.position + Vector3.up, direction, out hit, collectDistance))
        {
            if (hit.transform != door.root)
            {
                return;
            }
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
}
