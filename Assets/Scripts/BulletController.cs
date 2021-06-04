using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
	public GameObject impactAnimation;
	public int damage = 1;
	public bool firedByPlayer = true;
	public bool jammed = false;
	private float initializationTime;
	private bool destroyed = false;
	void Awake()
	{
		initializationTime = Time.timeSinceLevelLoad;
	}
	void Update()
	{
		// Prevent memory leaks
		// since a bullet shouldn't be alive for so long
		if (Time.timeSinceLevelLoad > initializationTime + 20)
			Destroy(gameObject);
	}
	void DeleteBullet(Collider2D other)
	{
		if (
			!other.CompareTag("Door") &&
			!other.CompareTag("Wall") &&
			!other.CompareTag("Decorations") &&
			!other.CompareTag("Entrance") &&
			!other.CompareTag("Exit") &&
			!other.CompareTag("PlayerDodgeRollHitbox") &&
			!other.CompareTag("Enemy") &&
			!other.CompareTag("JammedEnemy")
		)
			return;

		// Don't want player bullets to interact with the player
		// Don't want enemy bullets to interact with enemies
		if (
			destroyed ||
			firedByPlayer && other.CompareTag("PlayerDodgeRollHitbox") ||
			!firedByPlayer && other.CompareTag("Enemy") ||
			!firedByPlayer && other.CompareTag("JammedEnemy")
		)
			return;

		destroyed = true;

		// Spawn impact animation
		if (!other.CompareTag("Enemy") && !other.CompareTag("PlayerDodgeRollHitbox"))
			Destroy(Instantiate(impactAnimation, transform.position, Quaternion.identity), 0.25f);

		if (other.CompareTag("Enemy") || other.CompareTag("JammedEnemy"))
			other.transform.parent.GetComponent<EnemyController>().InflictDamage(jammed ? damage * 2 : damage);
		else if (other.CompareTag("PlayerDodgeRollHitbox"))
			other.transform.parent.parent.GetComponent<PlayerController>().InflictDamage(jammed ? damage * 2 : damage);

		Destroy(gameObject);
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		DeleteBullet(other);
	}
	void OnTriggerExit2D(Collider2D other)
	{
		DeleteBullet(other);
	}
}
