using System.Collections.Generic;
using UnityEngine;

public class FluidImpactParticleManager : MonoBehaviour
{
	public class FluidImpactDetails
	{
		public Vector3 pos;

		public float time;

		public ParticleImpactZone zone;

		public CollectedParticleQuantityInfo[] fluidQuantities = new CollectedParticleQuantityInfo[20];

		public int numOfFluids;

		public float totalAmountOfFluid;

		public Color avgColor;

		public float temperatureCelsius;

		public FluidImpactDetails()
		{
			for (int i = 0; i < fluidQuantities.Length; i++)
			{
				fluidQuantities[i] = new CollectedParticleQuantityInfo();
			}
		}

		public void Clear()
		{
			time = -1f;
			zone = null;
			numOfFluids = 0;
			totalAmountOfFluid = 0f;
		}
	}

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private Transform particleContainer;

	private float timeSinceEmit = -0.1f;

	private int numOfImpactsInList;

	private int showParticleEffectFrameIndex = 2;

	private int nextPoolIndex = -1;

	private List<FluidImpactDetails> pool = new List<FluidImpactDetails>(400);

	private List<FluidImpactDetails> fluidImpactDetailsList = new List<FluidImpactDetails>();

	private FluidImpactDetails tempFluidImpactDetails;

	private static FluidImpactParticleManager _instance;

	public static FluidImpactParticleManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(FluidImpactParticleManager)) as FluidImpactParticleManager;
				if (_instance == null)
				{
					FluidImpactParticleManager original = Resources.Load<FluidImpactParticleManager>("FluidImpactParticleManager");
					_instance = ((FluidImpactParticleManager)Object.Instantiate(original, Vector3.zero, Quaternion.identity)).GetComponent<FluidImpactParticleManager>();
					_instance.gameObject.name = "_FluidImpactParticleManager";
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			Setup();
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Setup()
	{
		for (int i = 0; i < pool.Capacity; i++)
		{
			FluidImpactDetails fluidImpactDetails = new FluidImpactDetails();
			fluidImpactDetails.Clear();
			pool.Add(fluidImpactDetails);
		}
	}

	private void Update()
	{
		ImpactParticleUpdate();
	}

	private void ImpactParticleUpdate()
	{
		if (numOfImpactsInList > 0)
		{
			float time = Time.time;
			bool flag = Time.frameCount % showParticleEffectFrameIndex == 0;
			int num = 0;
			while (num < fluidImpactDetailsList.Count)
			{
				tempFluidImpactDetails = fluidImpactDetailsList[num];
				if (tempFluidImpactDetails.time > 0f && tempFluidImpactDetails.time <= time)
				{
					if (flag)
					{
						particleContainer.position = tempFluidImpactDetails.pos;
						particles.startColor = tempFluidImpactDetails.avgColor;
						particles.Emit(1);
					}
					if (tempFluidImpactDetails.zone != null)
					{
						tempFluidImpactDetails.zone.ApplyFuildImpact(tempFluidImpactDetails);
					}
					fluidImpactDetailsList[num].Clear();
					pool[nextPoolIndex--] = fluidImpactDetailsList[num];
					fluidImpactDetailsList.RemoveAt(num);
					num--;
					numOfImpactsInList--;
					num++;
					continue;
				}
				break;
			}
		}
		timeSinceEmit += Time.deltaTime;
	}

	public FluidImpactDetails FetchPooledFluidImpactDetails()
	{
		if (nextPoolIndex + 1 >= pool.Count)
		{
			Debug.LogWarning("Insufficent Pooled Fluid Impact (ignore impact)");
			return null;
		}
		return pool[++nextPoolIndex];
	}

	public void ReleasePooledFluidImpactDetails(FluidImpactDetails item)
	{
		item.Clear();
		pool[nextPoolIndex--] = item;
	}

	public void AddFluidImpact(FluidImpactDetails fluidImpactDetails)
	{
		fluidImpactDetails.time -= 0.02f;
		bool flag = false;
		for (int i = 0; i < fluidImpactDetailsList.Count; i++)
		{
			if (fluidImpactDetails.time <= fluidImpactDetailsList[i].time)
			{
				fluidImpactDetailsList.Insert(i, fluidImpactDetails);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			fluidImpactDetailsList.Add(fluidImpactDetails);
		}
		numOfImpactsInList++;
	}
}
