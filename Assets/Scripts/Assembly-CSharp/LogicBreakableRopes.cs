using UnityEngine;

public class LogicBreakableRopes : MonoBehaviour
{
	public UltimateRope Rope1;

	public UltimateRope Rope2;

	private bool bBroken1;

	private bool bBroken2;

	private void Start()
	{
		bBroken1 = false;
		bBroken2 = false;
	}

	private void OnRopeBreak(UltimateRope.RopeBreakEventInfo breakInfo)
	{
		if (breakInfo.rope == Rope1)
		{
			bBroken1 = true;
		}
		if (breakInfo.rope == Rope2)
		{
			bBroken2 = true;
		}
	}
}
