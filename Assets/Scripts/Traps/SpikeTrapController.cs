using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
	[Header("Sprites")]
	public Sprite passiveState;
	public Sprite activeState;
	public SpriteRenderer spriteRenderer;
	private bool timerRunning = false;
	private bool playerTouching = false;
	private bool canDamage = false;
	private PlayerController player;
	void Start()
	{
		player = transform.parent.parent.parent.GetChild(2).GetComponent<PlayerController>();
	}
	void Update()
	{
		// This is why the player needs iframes
		if (canDamage && playerTouching)
			player.InflictDamage(1);
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		//Checks if the object which collides doesn't have the tag "PlayerDodgeRollHitbox."
		if (PrelimCheck(other))
			return;
		//If it does have the tag, it confirms the player is touching and checks if the timer isn't running.
		playerTouching = true;

		if (!timerRunning)
			StartCoroutine(DelayedActivation());
	}
	void OnTriggerStay2D(Collider2D other)
	{
		//For every update frame for which the object that collides doesn't have the tag "PlayerDodgeRollHitbox"
		if (PrelimCheck(other))
			return;

		if (!timerRunning)
			StartCoroutine(DelayedActivation());
	}
	void OnTriggerExit2D(Collider2D other)
	{
		//If the object that is colliding is no longer touching, then it checks the tag.
		//If the tag check fails, it sets the boolean values to false.
		if (PrelimCheck(other))
			return;

		canDamage = false;
		playerTouching = false;
	}
	IEnumerator DelayedActivation()
	{
		// Basically turns trap on
		timerRunning = true;

		yield return new WaitForSeconds(0.4f);

		spriteRenderer.sprite = activeState;
		canDamage = true;

		StartCoroutine(DelayedDeactivation());
	}
	IEnumerator DelayedDeactivation()
	{
		// Turns trap off 
		yield return new WaitForSeconds(1f);

		if (playerTouching)
			StartCoroutine(DelayedDeactivation());
		else
		{
			canDamage = false;
			timerRunning = false;
			spriteRenderer.sprite = passiveState;
		}
	}
}
