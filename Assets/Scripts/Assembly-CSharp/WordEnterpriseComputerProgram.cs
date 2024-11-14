public class WordEnterpriseComputerProgram : WordComputerProgram
{
	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.WordEnterprise;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.OnFinish += base.ResetProgram;
	}

	protected override void OnDisable()
	{
		base.OnFinish -= base.ResetProgram;
		base.OnDisable();
	}
}
