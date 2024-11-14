using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SpreadsheetNumberCell : MonoBehaviour
{
	private string digitText;

	[SerializeField]
	private bool isNegative;

	[SerializeField]
	private int maxDigits = 6;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Text text;

	[SerializeField]
	private SpriteBlinker blinker;

	private bool isSelected;

	private string oldDigitText;

	public long Value
	{
		get
		{
			if (string.IsNullOrEmpty(digitText))
			{
				digitText = long.Parse(text.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture).ToString();
			}
			long num = 0L;
			for (int i = 0; i < digitText.Length; i++)
			{
				num += long.Parse(digitText[i].ToString(), CultureInfo.InvariantCulture);
				if (i < digitText.Length - 1)
				{
					num *= 10;
				}
			}
			return (!isNegative) ? num : (-num);
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
		oldDigitText = digitText;
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
		if (digitText.Length == 0)
		{
			digitText = oldDigitText;
			UpdateText();
		}
		isSelected = false;
	}

	public void AddDigit(char digit)
	{
		if (digitText.Length < maxDigits)
		{
			if (digitText.StartsWith("0"))
			{
				digitText = digit.ToString();
			}
			else
			{
				digitText += digit;
			}
			UpdateText();
		}
	}

	public void ResetDigits()
	{
		digitText = string.Empty;
	}

	private void UpdateText()
	{
		if (digitText.Length > 0)
		{
			text.text = string.Format(CultureInfo.InvariantCulture, "$ {0:#,0}", Math.Abs(Value));
		}
		else
		{
			text.text = "$" + '\u00a0';
		}
	}
}
