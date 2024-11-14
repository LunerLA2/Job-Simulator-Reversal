public class JobCartridgeWithGenieFlags
{
	private JobCartridge baseJobCartridge;

	private JobGenieCartridge.GenieModeTypes genieFlags;

	public JobCartridge BaseJobCartridge
	{
		get
		{
			return baseJobCartridge;
		}
	}

	public JobGenieCartridge.GenieModeTypes GenieFlags
	{
		get
		{
			return genieFlags;
		}
	}

	public JobCartridgeWithGenieFlags(JobCartridge baseJobCart, JobGenieCartridge.GenieModeTypes flags)
	{
		baseJobCartridge = baseJobCart;
		genieFlags = flags;
	}
}
