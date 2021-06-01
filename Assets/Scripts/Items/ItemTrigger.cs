using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemTrigger : MonoBehaviour
{
	public string id;
	public int weight;
	private PlayerController player;
	void Start()
	{
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		player.OnItemPickup(id);

		Destroy(gameObject);
	}
}
