using OwlchemyVR;
using UnityEngine;

public class EndOfWorkLeverController : SimpleMovableBrainControlledObject
{
	[SerializeField]
	private GrabbableHinge hinge;

	[SerializeField]
	private WorldItem myWorldItem;

	public override void Appear(BrainData brain)
	{
		base.gameObject.SetActive(true);
		base.Appear(brain);
	}

	public override void Disappear()
	{
		base.Disappear();
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", hinge.NormalizedAngle);
	}
}
