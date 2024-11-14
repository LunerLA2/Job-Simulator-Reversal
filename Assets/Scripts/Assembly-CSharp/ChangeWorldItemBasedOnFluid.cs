using System;
using OwlchemyVR;
using UnityEngine;

public class ChangeWorldItemBasedOnFluid : MonoBehaviour
{
	[SerializeField]
	private ContainerFluidSystem containerFluid;

	[SerializeField]
	private FluidWorldItemPair[] pairs;

	private void OnEnable()
	{
		ContainerFluidSystem containerFluidSystem = containerFluid;
		containerFluidSystem.OnFluidEmptied = (Action<ContainerFluidSystem>)Delegate.Combine(containerFluidSystem.OnFluidEmptied, new Action<ContainerFluidSystem>(FluidEmptied));
		ContainerFluidSystem containerFluidSystem2 = containerFluid;
		containerFluidSystem2.OnFluidPartiallyFilled = (Action<ContainerFluidSystem>)Delegate.Combine(containerFluidSystem2.OnFluidPartiallyFilled, new Action<ContainerFluidSystem>(FluidFilled));
	}

	private void OnDisable()
	{
		ContainerFluidSystem containerFluidSystem = containerFluid;
		containerFluidSystem.OnFluidEmptied = (Action<ContainerFluidSystem>)Delegate.Remove(containerFluidSystem.OnFluidEmptied, new Action<ContainerFluidSystem>(FluidEmptied));
		ContainerFluidSystem containerFluidSystem2 = containerFluid;
		containerFluidSystem2.OnFluidPartiallyFilled = (Action<ContainerFluidSystem>)Delegate.Remove(containerFluidSystem2.OnFluidPartiallyFilled, new Action<ContainerFluidSystem>(FluidFilled));
	}

	private void FluidEmptied(ContainerFluidSystem fluid)
	{
		WorldItem component = fluid.GetComponent<WorldItem>();
		if (component != null)
		{
			WorldItemBecameEmpty(component);
		}
	}

	private void FluidFilled(ContainerFluidSystem fluid)
	{
		WorldItem component = fluid.GetComponent<WorldItem>();
		if (component != null)
		{
			WorldItemBecameFilled(component);
		}
	}

	private void WorldItemBecameEmpty(WorldItem item)
	{
		for (int i = 0; i < pairs.Length; i++)
		{
			if (item.Data == pairs[i].whenFilled)
			{
				item.ManualSetData(pairs[i].whenEmpty);
			}
		}
	}

	private void WorldItemBecameFilled(WorldItem item)
	{
		for (int i = 0; i < pairs.Length; i++)
		{
			if (item.Data == pairs[i].whenEmpty)
			{
				item.ManualSetData(pairs[i].whenFilled);
			}
		}
	}
}
