using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class PrintReceiptComputerProgram : ComputerProgram
{
	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private Text laborCostsText;

	private int laborCosts;

	[SerializeField]
	private Text partCostsText;

	private int partCosts;

	[SerializeField]
	private Text totalText;

	[SerializeField]
	public ReceiptPrinter printer;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.PrintReceipt;
		}
	}

	private void Awake()
	{
		partCosts = 0;
		laborCosts = 0;
	}

	private void OnEnable()
	{
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		}
	}

	private void OnDisable()
	{
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable.name == "X")
			{
				Finish();
			}
			else if (clickable.name == "AddLabor")
			{
				AddLaborCosts();
			}
			else if (clickable.name == "AddParts")
			{
				AddPartCosts();
			}
			else if (clickable.name == "Print")
			{
				PrintReceipt();
			}
		}
	}

	private void UpdateDisplay()
	{
		laborCostsText.text = "$" + laborCosts;
		partCostsText.text = "$" + partCosts;
		totalText.text = "$" + (laborCosts + partCosts);
	}

	public void AddLaborCosts()
	{
		laborCosts += 375;
		UpdateDisplay();
	}

	public void AddPartCosts()
	{
		partCosts += 85;
		UpdateDisplay();
	}

	public void PrintReceipt()
	{
		if (printer != null && !printer.PrinterFull)
		{
			printer.PrintReceipt("..GRIFTY LUBE..\nLabor: $" + laborCosts + "\nParts: $" + partCosts + "\nTotal: $" + (partCosts + laborCosts));
		}
		partCosts = 0;
		laborCosts = 0;
		UpdateDisplay();
	}
}
