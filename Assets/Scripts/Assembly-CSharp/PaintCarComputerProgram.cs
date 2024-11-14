using OwlchemyVR;
using UnityEngine;

public class PaintCarComputerProgram : ComputerProgram
{
	[SerializeField]
	private WorldItemData worldItemData;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.PaintCar;
		}
	}

	private void Awake()
	{
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
	}
}
