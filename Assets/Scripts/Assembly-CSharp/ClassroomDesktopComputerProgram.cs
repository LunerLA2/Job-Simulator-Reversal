public class ClassroomDesktopComputerProgram : ComputerProgram
{
	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Alert;
		}
	}

	private void OnEnable()
	{
		hostComputer.StartProgram(ComputerProgramID.SiliconTrail);
	}
}
