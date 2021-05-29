using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private PlayerController player;
	void Start()
	{
		player = transform.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
	}
	void LateUpdate()
	{
		transform.position = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, transform.position.z);
	}
}
