using UnityEngine;

public class Door : MonoBehaviour
{
    public float openDistance = 10f;
    Animator animator;
    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Calculate the distance between the door and the player.
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < openDistance)
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Open", false);
        }
    }
}
