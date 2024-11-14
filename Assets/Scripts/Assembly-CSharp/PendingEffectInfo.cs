public class PendingEffectInfo
{
	private BrainEffect effect;

	private int index;

	public BrainEffect Effect
	{
		get
		{
			return effect;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
	}

	public PendingEffectInfo(BrainEffect _effect, int _index)
	{
		effect = _effect;
		index = _index;
	}
}
