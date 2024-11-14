public class EndOfWorkComputerProgram : ComputerProgram
{
	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.EndOfWork;
		}
	}

	private void OnEnable()
	{
		hostComputer.HideCursor();
	}
}
