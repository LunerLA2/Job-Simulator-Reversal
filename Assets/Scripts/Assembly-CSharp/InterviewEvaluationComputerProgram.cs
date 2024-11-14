using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class InterviewEvaluationComputerProgram : ComputerProgram
{
	[SerializeField]
	private SpreadsheetBoolCell[] cells;

	[SerializeField]
	private ComputerClickable submitClickable;

	[SerializeField]
	private ComputerClickable quitClickable;

	[SerializeField]
	private Text submitText;

	[SerializeField]
	private GameObject sheetWindow;

	[SerializeField]
	private WorldItemData screenWorldItemData;

	[SerializeField]
	private GameObject hiredPrintPrefab;

	[SerializeField]
	private GameObject rejectedPrintPrefab;

	private SpreadsheetBoolCell highlightedCell;

	private SpreadsheetBoolCell selectedCell;

	private bool isDone;

	private GameObject printPrefab;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.InterviewEvaluation;
		}
	}

	private void OnEnable()
	{
		Evaluate();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Evaluate()
	{
		bool flag = true;
		float num = 0f;
		for (int i = 0; i < cells.Length; i++)
		{
			num += (float)cells[i].Value;
			if (cells[i].Value < 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			printPrefab = rejectedPrintPrefab;
			if (UnityEngine.Random.Range(0f, 1f) <= num / (float)cells.Length)
			{
				printPrefab = hiredPrintPrefab;
			}
			submitClickable.SetInteractive(true);
			submitText.text = "Analyze & Print";
		}
		else
		{
			submitClickable.SetInteractive(false);
			submitText.text = "Fill All Fields";
		}
	}

	protected override void OnClickableHighlighted(ComputerClickable clickable)
	{
		SpreadsheetBoolCell component = clickable.GetComponent<SpreadsheetBoolCell>();
		if (component != null)
		{
			highlightedCell = component;
		}
	}

	protected override void OnClickableUnhighlighted(ComputerClickable clickable)
	{
		SpreadsheetBoolCell component = clickable.GetComponent<SpreadsheetBoolCell>();
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
				SelectNextBoolCell();
			}
			else if (highlightedCell != null && highlightedCell != selectedCell)
			{
				SelectBoolCell(highlightedCell);
				highlightedCell.GetComponent<ComputerClickable>().Unhighlight();
				selectedCell.AddDigit(code[0]);
				Evaluate();
				SelectNextBoolCell();
			}
		}
		return true;
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (!isDone && clickable != null)
		{
			if (clickable == submitClickable)
			{
				isDone = true;
				Print();
			}
			else
			{
				SpreadsheetBoolCell component = clickable.GetComponent<SpreadsheetBoolCell>();
				if (component != null)
				{
					SelectBoolCell(component);
				}
			}
		}
		if (clickable != null && clickable == quitClickable)
		{
			ResetProgram();
			Finish();
			isDone = false;
		}
	}

	private void SelectBoolCell(SpreadsheetBoolCell cell)
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

	private void SelectNextBoolCell()
	{
		int num = Array.IndexOf(cells, selectedCell);
		if (num != -1 && num < cells.Length - 1 && cells[num + 1].Value == -1)
		{
			if (selectedCell != null)
			{
				selectedCell.Deselect();
			}
			selectedCell = cells[num + 1];
			cells[num + 1].Select();
		}
	}

	private void ResetProgram()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].ResetDigits();
		}
		if (selectedCell != null)
		{
			selectedCell.Deselect();
		}
	}

	private void Print()
	{
		GameEventsManager.Instance.ItemActionOccurred(screenWorldItemData, "ACTIVATED");
		if (printPrefab != null)
		{
			hostComputer.PrintObject(printPrefab);
		}
		ResetProgram();
		Finish();
		isDone = false;
	}
}
