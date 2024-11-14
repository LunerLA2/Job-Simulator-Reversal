using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class AccountingComputerProgram : ComputerProgram
{
	public const float GRAPH_DURATION = 3f;

	[SerializeField]
	private SpreadsheetSumCell profitCell;

	[SerializeField]
	private ComputerClickable submitClickable;

	[SerializeField]
	private ComputerClickable quitClickable;

	[SerializeField]
	private Text submitText;

	[SerializeField]
	private Text printingText;

	[SerializeField]
	private GameObject sheetWindow;

	[SerializeField]
	private GameObject resultsWindow;

	[SerializeField]
	private Transform graphCover;

	[SerializeField]
	private AudioClip drumrollSound;

	[SerializeField]
	private AudioClip successSound;

	[SerializeField]
	private Text finalProfitText;

	[SerializeField]
	private GameObject confetti;

	[SerializeField]
	private WorldItemData screenWorldItemData;

	private SpreadsheetNumberCell highlightedCell;

	private SpreadsheetNumberCell selectedCell;

	private bool isDone;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Accounting;
		}
	}

	private void OnEnable()
	{
		Evaluate();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		FinalizeGraph();
		if (isDone)
		{
			GameEventsManager.Instance.ItemActionOccurred(screenWorldItemData, "ACTIVATED");
			hostComputer.StopSound(drumrollSound);
		}
	}

	private void Evaluate()
	{
		profitCell.Calculate();
		if (profitCell.Sum > 0)
		{
			submitClickable.SetInteractive(true);
			submitText.text = "Submit";
		}
		else
		{
			submitClickable.SetInteractive(false);
			submitText.text = "Profit Too Low!";
		}
	}

	protected override void OnClickableHighlighted(ComputerClickable clickable)
	{
		SpreadsheetNumberCell component = clickable.GetComponent<SpreadsheetNumberCell>();
		if (component != null)
		{
			highlightedCell = component;
		}
	}

	protected override void OnClickableUnhighlighted(ComputerClickable clickable)
	{
		SpreadsheetNumberCell component = clickable.GetComponent<SpreadsheetNumberCell>();
		if (component != null && component == highlightedCell)
		{
			highlightedCell = null;
		}
	}

	protected override bool OnKeyPress(string code)
	{
		if (!isDone && (code == "0" || code == "1"))
		{
			if (selectedCell != null)
			{
				selectedCell.AddDigit(code[0]);
				Evaluate();
			}
			else if (highlightedCell != null && highlightedCell != selectedCell)
			{
				SelectNumberCell(highlightedCell);
				highlightedCell.GetComponent<ComputerClickable>().Unhighlight();
				selectedCell.AddDigit(code[0]);
				Evaluate();
			}
		}
		return true;
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (!isDone)
		{
			if (!(clickable != null))
			{
				return;
			}
			if (clickable.name == "Submit")
			{
				sheetWindow.SetActive(false);
				resultsWindow.SetActive(true);
				StartCoroutine(ShowGraphAsync());
				isDone = true;
			}
			else
			{
				SpreadsheetNumberCell component = clickable.GetComponent<SpreadsheetNumberCell>();
				if (component != null)
				{
					SelectNumberCell(component);
				}
			}
		}
		else if (clickable != null && clickable.name == "Quit")
		{
			Finish();
		}
	}

	private void SelectNumberCell(SpreadsheetNumberCell cell)
	{
		if (selectedCell != null)
		{
			selectedCell.Deselect();
		}
		selectedCell = cell;
		if (selectedCell != null)
		{
			selectedCell.Select();
		}
	}

	private IEnumerator ShowGraphAsync()
	{
		if (drumrollSound != null)
		{
			hostComputer.PlaySound(drumrollSound, 1f, 1f, true);
		}
		float t = 0f;
		long profit = profitCell.Sum;
		while (t <= 1f)
		{
			hostComputer.SetSoundPitch(Mathf.Lerp(0.75f, 2f, t));
			t += Time.deltaTime / 3f;
			graphCover.localScale = new Vector3(1f - t, 1f, 1f);
			finalProfitText.text = string.Format("$ {0:#,0}", (long)((float)profit * t));
			yield return null;
		}
		FinalizeGraph();
		hostComputer.PlaySound(successSound);
		GameEventsManager.Instance.ItemActionOccurred(screenWorldItemData, "ACTIVATED");
		float printMessageTime = 0f;
		while (hostComputer.IsPrinterBusy || printMessageTime < 2f)
		{
			yield return new WaitForSeconds(0.15f);
			printMessageTime += 0.15f;
			Color color = printingText.color;
			color.a = 1f - color.a;
			printingText.color = color;
		}
		printingText.color = Color.clear;
		quitClickable.gameObject.SetActive(true);
		yield return null;
	}

	private void FinalizeGraph()
	{
		confetti.SetActive(true);
		graphCover.localScale = new Vector3(0f, 1f, 1f);
		finalProfitText.text = string.Format("$ {0:#,0}", profitCell.Sum);
	}
}
