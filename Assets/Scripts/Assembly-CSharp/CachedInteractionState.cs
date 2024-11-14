public class CachedInteractionState
{
	public bool IsInteractable;

	public AttachableObject AttachableObject;

	public CachedInteractionState(AttachableObject obj, bool isEnabled)
	{
		IsInteractable = isEnabled;
		AttachableObject = obj;
	}
}
