using TMPro;
using UnityEngine;

public class CalculatorController : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro label;

	private string display = string.Empty;

	public void ButtonZeroPressed()
	{
		AddLetterToDisplay("0");
	}

	public void ButtonOnePressed()
	{
		AddLetterToDisplay("1");
	}

	private void AddLetterToDisplay(string l)
	{
		display += l;
		if (display.Length > 8)
		{
			display = display.Substring(display.Length - 8, 8);
		}
		label.text = display;
	}
}
