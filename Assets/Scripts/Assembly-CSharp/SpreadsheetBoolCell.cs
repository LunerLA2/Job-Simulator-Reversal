using UnityEngine;
using UnityEngine.UI;

public class SpreadsheetBoolCell : MonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Text text;

	[SerializeField]
	private SpriteBlinker blinker;

	[SerializeField]
	private string trueConditionDisplayText = "Yes";

	[SerializeField]
	private string falseConditionDisplayText = "No";

	private bool isSelected;

	private string oldDigitText;

	public int Value
	{
		get
		{
			if (text.text == trueConditionDisplayText)
			{
				return 1;
			}
			if (text.text == falseConditionDisplayText)
			{
				return 0;
			}
			return -1;
		}
	}

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
	}

	private void OnEnable()
	{
		blinker.gameObject.SetActive(false);
		GetComponent<ComputerClickable>().SetShowHighlight(true);
	}

	public void Select()
	{
		background.color = new Color(1f, 1f, 0.52f);
		blinker.gameObject.SetActive(true);
		oldDigitText = text.text;
		GetComponent<ComputerClickable>().SetShowHighlight(false);
		ResetDigits();
		UpdateText();
		isSelected = true;
	}

	public void Deselect()
	{
		background.color = Color.white;
		blinker.gameObject.SetActive(false);
		GetComponent<ComputerClickable>().SetShowHighlight(true);
		if (text.text.Length == 0)
		{
			text.text = oldDigitText;
			UpdateText();
		}
		isSelected = false;
	}

	public void AddDigit(char digit)
	{
		switch (digit)
		{
		case '1':
			text.text = trueConditionDisplayText;
			break;
		case '0':
			text.text = falseConditionDisplayText;
			break;
		}
	}

	public void ResetDigits()
	{
		text.text = string.Empty;
	}

	private void UpdateText()
	{
		switch (Value)
		{
		case 1:
			text.text = trueConditionDisplayText;
			break;
		case 0:
			text.text = falseConditionDisplayText;
			break;
		case -1:
			break;
		}
	}
}
