using UnityEngine;

public class VehicleHardware : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint[] trackedAttachPoints;

	[SerializeField]
	private Transform[] optimizableTransforms;

	protected VehicleChassisController parentChassis;

	public AttachablePoint[] TrackedAttachPoints
	{
		get
		{
			return trackedAttachPoints;
		}
	}

	public Transform[] OptimizableTransforms
	{
		get
		{
			return optimizableTransforms;
		}
	}

	public virtual void AttachToChassis(VehicleChassisController chassis)
	{
		parentChassis = chassis;
	}

	public virtual void WillBecomeOptimized()
	{
	}

	public virtual void HasBecomeUnoptimized()
	{
	}
}
