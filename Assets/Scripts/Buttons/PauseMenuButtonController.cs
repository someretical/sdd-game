using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private TextMeshProUGUI textMeshPro;
	void Awake()
	{
		textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		textMeshPro.color = new Color32(144, 144, 144, 255);
	}
	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		textMeshPro.color = new Color32(255, 255, 255, 255);
	}
	public void OnPointerExit(PointerEventData pointerEventData)
	{
		textMeshPro.color = new Color32(144, 144, 144, 255);
	}
}
