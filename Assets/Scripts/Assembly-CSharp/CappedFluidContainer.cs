using UnityEngine;

public class CappedFluidContainer : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint capAttachPoint;

	[SerializeField]
	private ContainerFluidSystem fluidContainerToCap;

	[SerializeField]
	private GravityDispensingItem gravityDispensingItemToCap;

	private void Awake()
	{
		if (capAttachPoint.InitialCapacity > 0)
		{
			DisablePouring();
		}
		else
		{
			EnablePouring();
		}
	}

	private void OnEnable()
	{
		capAttachPoint.OnObjectWasAttached += CapAttached;
		capAttachPoint.OnObjectWasDetached += CapDetached;
	}

	private void OnDisable()
	{
		capAttachPoint.OnObjectWasAttached -= CapAttached;
		capAttachPoint.OnObjectWasDetached -= CapDetached;
	}

	private void CapAttached(AttachablePoint point, AttachableObject o)
	{
		o.transform.parent = point.transform;
		DisablePouring();
	}

	private void CapDetached(AttachablePoint point, AttachableObject o)
	{
		o.transform.parent = GlobalStorage.Instance.ContentRoot;
		EnablePouring();
	}

	private void DisablePouring()
	{
		if (fluidContainerToCap != null)
		{
			fluidContainerToCap.SetIsPouringEnabled(false);
		}
		if (gravityDispensingItemToCap != null)
		{
			gravityDispensingItemToCap.SetCanPour(false);
		}
	}

	private void EnablePouring()
	{
		if (fluidContainerToCap != null)
		{
			fluidContainerToCap.SetIsPouringEnabled(true);
		}
		if (gravityDispensingItemToCap != null)
		{
			gravityDispensingItemToCap.SetCanPour(true);
		}
	}
}
