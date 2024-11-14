using UnityEngine;

public class BehaviourControls : MonoBehaviour
{
	[SerializeField]
	private Behaviour b;

	public void ToggleEnabled()
	{
		b.enabled = !b.enabled;
	}
}
