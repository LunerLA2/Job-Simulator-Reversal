using UnityEngine;

public class TriggerEventInfo
{
	public TriggerListener listener;

	public Collider other;

	public void Set(TriggerListener tl, Collider c)
	{
		listener = tl;
		other = c;
	}
}
