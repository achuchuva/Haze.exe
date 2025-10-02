using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour {

	public Transform player;
	public Transform reciever;

	private bool playerIsOverlapping = false;
	private float lastTeleportTime = 0f;
	private static readonly float TELEPORT_COOLDOWN = 0.25f;

	// Update is called once per frame
	void Update () {
		if (playerIsOverlapping)
		{
			Vector3 portalToPlayer = player.position - transform.position;
			float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

			// If this is true: The player has moved across the portal
			if (dotProduct < 0f)
			{
				if (Time.time - lastTeleportTime < TELEPORT_COOLDOWN)
				{
					return;
				}
				
				// Teleport him!
				float rotationDiff = reciever.root.eulerAngles.y - transform.root.eulerAngles.y + 180f;
				player.GetComponentInChildren<FirstPersonLook>().AddYaw(rotationDiff);

				Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
				player.position = reciever.position + positionOffset;

				playerIsOverlapping = false;

				reciever.GetComponent<PortalTeleporter>().lastTeleportTime = Time.time;
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = false;
		}
	}
}
