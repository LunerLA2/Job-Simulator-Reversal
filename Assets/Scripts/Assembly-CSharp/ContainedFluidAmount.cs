using UnityEngine;

public class ContainedFluidAmount : MonoBehaviour
{
	[SerializeField]
	private ContainedFluidAmountInfo[] containedFluids;

	public ContainedFluidAmountInfo[] ContainedFluids
	{
		get
		{
			return containedFluids;
		}
	}
}
