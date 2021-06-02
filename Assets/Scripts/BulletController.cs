using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
	public GameObject impactAnimation;
	public int damage = 1;
	public bool firedByPlayer = true;
	void OnTriggerEnter2D(Collider2D other)
	{
		if (
			!other.CompareTag("Door") &&
			!other.CompareTag("Wall") &&
			!other.CompareTag("Decorations") &&
			!other.CompareTag("Entrance") &&
			!other.CompareTag("Exit") &&
			!other.CompareTag("PlayerDodgeRollHitbox") &&
			!other.CompareTag("Enemy")
		)
			return;

		if (
			firedByPlayer && other.CompareTag("PlayerDodgeRollHitbox") ||
			!firedByPlayer && other.CompareTag("Enemy")
		)
			return;

		var impact = Instantiate(impactAnimation, transform.position, Quaternion.identity);
		Destroy(impact, 0.25f);
		Destroy(gameObject);

		if (other.CompareTag("Enemy"))
			other.transform.parent.gameObject.GetComponent<EnemyController>().InflictDamage(damage);
		else if (other.CompareTag("PlayerDodgeRollHitbox"))
			other.transform.parent.parent.gameObject.GetComponent<PlayerController>().InflictDamage(damage);
	}
}
