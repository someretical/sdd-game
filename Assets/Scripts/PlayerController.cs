using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed;
	public float maxSquaredSpeed;
	public Rigidbody2D rb2d;
	public bool canBlank = true;
	void Awake()
	{
		transform.position = new Vector3Int(79, 71, 0);
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q) && canBlank)
		{
			canBlank = false;

			FireBlank();

			Debug.Log("fired blank");
			StartCoroutine(EnableBlanks());
		}
	}
	void FixedUpdate()
	{
		if (rb2d.velocity.sqrMagnitude < maxSquaredSpeed)
		{
			var movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
			rb2d.AddForce(speed * Time.fixedDeltaTime * movement);
		}
	}
	IEnumerator EnableBlanks()
	{
		yield return new WaitForSeconds(0.5f);

		canBlank = true;
		Debug.Log("can fire blank again");
	}
	void FireBlank()
	{
		// Dungeon is always instantiated after player
		transform.parent.GetChild(1).gameObject.GetComponent<DungeonManager>().ProcessBlank(transform.position);
	}
}
