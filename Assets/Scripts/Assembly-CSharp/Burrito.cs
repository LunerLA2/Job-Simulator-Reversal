using UnityEngine;

public class Burrito : BasicEdibleItem
{
	[SerializeField]
	private ParticleSystem burritoDrips;

	public override BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		burritoDrips.Play();
		return base.TakeBiteAndGetResult(head);
	}
}
