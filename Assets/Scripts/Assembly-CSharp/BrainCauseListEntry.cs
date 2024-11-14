public class BrainCauseListEntry
{
	public BrainCauseStatusController cause;

	public BrainStatusController parentBrain;

	public BrainCauseListEntry(BrainCauseStatusController _cause, BrainStatusController _parentBrain)
	{
		cause = _cause;
		parentBrain = _parentBrain;
	}
}
