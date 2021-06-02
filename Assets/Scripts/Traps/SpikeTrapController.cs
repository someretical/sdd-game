using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
	[Header("Sprites")]
	public Sprite passiveState;
	public Sprite activeState;
	private bool timerRunning = false;
	private bool playerTouching = false;
	private bool canDamage = false;
	private SpriteRenderer spriteRenderer;
	private PlayerController player;
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = passiveState;
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
	}
	void Update()
	{
		if (canDamage && playerTouching)
			player.InflictDamage(1);
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		playerTouching = true;

		if (!timerRunning)
			StartCoroutine(DelayedActivation());
	}
	void OnTriggerStay2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (!timerRunning)
			StartCoroutine(DelayedActivation());
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		canDamage = false;
		playerTouching = false;
	}
	IEnumerator DelayedActivation()
	{
		timerRunning = true;

		yield return new WaitForSeconds(0.4f);

		spriteRenderer.sprite = activeState;
		canDamage = true;

		StartCoroutine(DelayedDeactivation());
	}
	IEnumerator DelayedDeactivation()
	{
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
