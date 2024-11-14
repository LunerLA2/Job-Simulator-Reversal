using TMPro;
using UnityEngine;

public class MechanicReceiptController : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro mainText;

	public void SetText(string t)
	{
		mainText.text = t;
	}
}
