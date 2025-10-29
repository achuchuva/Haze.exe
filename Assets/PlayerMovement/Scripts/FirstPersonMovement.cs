using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public GroundCheck groundCheck;
    public float speed = 5;
    public float gravity = -9.81f;
    public AudioSource walkSound;
    public AudioSource runSound;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    public bool Disabled { get; set; } = false;
    Vector3 velocity;

    void Update()
    {
        if (Disabled || Pause.Instance.Paused)
        {
            walkSound.Stop();
            runSound.Stop();
            return;
        }

        if (groundCheck.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (groundCheck.isGrounded && (x != 0 || z != 0))
        {
            if (IsRunning)
            {
                if (!runSound.isPlaying)
                {
                    runSound.Play();
                }
                walkSound.Stop();
            }
            else
            {
                if (!walkSound.isPlaying)
                {
                    walkSound.Play();
                }
                runSound.Stop();
            }
        }
        else
        {
            walkSound.Stop();
            runSound.Stop();
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * (IsRunning ? runSpeed : speed) * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Disable()
    {
        Disabled = true;
    }

    public void Enable()
    {
        Disabled = false;
    }
}