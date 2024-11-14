using UnityEngine;
using UnityEngine.UI;

public class SpreadsheetSumCell : MonoBehaviour
{
	[SerializeField]
	private SpreadsheetNumberCell[] operands;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Text text;

	private long sum;

	public long Sum
	{
		get
		{
			return sum;
		}
	}

	public void Calculate()
	{
		sum = 0L;
		for (int i = 0; i < operands.Length; i++)
		{
			sum += operands[i].Value;
		}
		text.text = string.Format("$ {0:#,0}", sum);
		if (sum > 0)
		{
			background.color = new Color(0.25f, 0.9f, 0.25f);
		}
		else
		{
			background.color = new Color(1f, 0.3f, 0.3f);
		}
	}
}
