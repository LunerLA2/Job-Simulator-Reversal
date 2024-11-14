public class BiteResultInfo
{
	private bool wasMainItemFullyConsumed;

	private EdibleItem[] childItemsBitten;

	private EdibleItem[] childItemsFullyConsumed;

	public bool WasMainItemFullyConsumed
	{
		get
		{
			return wasMainItemFullyConsumed;
		}
	}

	public EdibleItem[] ChildItemsBitten
	{
		get
		{
			return childItemsBitten;
		}
	}

	public EdibleItem[] ChildItemsFullyConsumed
	{
		get
		{
			return childItemsFullyConsumed;
		}
	}

	public BiteResultInfo(bool mainItemConsumed, EdibleItem[] childrenBitten, EdibleItem[] childrenConsumed)
	{
		wasMainItemFullyConsumed = mainItemConsumed;
		childItemsBitten = childrenBitten;
		childItemsFullyConsumed = childrenConsumed;
	}
}
