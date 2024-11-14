using OwlchemyVR;

public class CollectedParticleQuantityInfo
{
	private WorldItemData fluidData;

	private float quantity;

	public WorldItemData FluidData
	{
		get
		{
			return fluidData;
		}
	}

	public float Quantity
	{
		get
		{
			return quantity;
		}
	}

	public CollectedParticleQuantityInfo()
	{
	}

	public CollectedParticleQuantityInfo(WorldItemData data)
	{
		fluidData = data;
	}

	public void SetQuantity(float q)
	{
		quantity = q;
	}

	public void SetFluidData(WorldItemData data)
	{
		fluidData = data;
	}

	public void SetFluidDataAndQuatity(WorldItemData data, float q)
	{
		fluidData = data;
		quantity = q;
	}
}
