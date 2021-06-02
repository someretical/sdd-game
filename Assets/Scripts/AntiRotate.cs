using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AntiRotate : MonoBehaviour
{
	void LateUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}