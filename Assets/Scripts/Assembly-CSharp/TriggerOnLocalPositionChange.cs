using UnityEngine;
using UnityEngine.Events;

public class TriggerOnLocalPositionChange : MonoBehaviour
{
	[Tooltip("If no transform is added, will default to this transform")]
	public Transform transformToWatch;

	public bool watchX;

	public float minXTrigger;

	public float maxXTrigger = 1f;

	public UnityEvent minXEvent;

	public UnityEvent maxXEvent;

	private float? previousX;

	public bool watchY;

	public float minYTrigger;

	public float maxYTrigger = 1f;

	public UnityEvent minYEvent;

	public UnityEvent maxYEvent;

	private float? previousY;

	public bool watchZ;

	public float minZTrigger;

	public float maxZTrigger = 1f;

	public UnityEvent minZEvent;

	public UnityEvent maxZEvent;

	private float? previousZ;

	private void Awake()
	{
		if (transformToWatch == null)
		{
			transformToWatch = base.transform;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!watchX && !watchY && !watchZ)
		{
			Debug.LogWarning("TriggerOnLocalPositionChange wasn't watching anything! Disabling.");
			base.enabled = false;
			return;
		}
		if (watchX)
		{
			if (!previousX.HasValue)
			{
				previousX = transformToWatch.localPosition.x;
			}
			if (minXEvent.GetPersistentEventCount() > 0 && transformToWatch.localPosition.x < minXTrigger)
			{
				float? num = previousX;
				if (num.HasValue && num.Value > minXTrigger)
				{
					minXEvent.Invoke();
					goto IL_013a;
				}
			}
			if (maxXEvent.GetPersistentEventCount() > 0 && transformToWatch.localPosition.x > maxXTrigger)
			{
				float? num2 = previousX;
				if (num2.HasValue && num2.Value < maxXTrigger)
				{
					maxXEvent.Invoke();
				}
			}
			goto IL_013a;
		}
		goto IL_0159;
		IL_034a:
		previousZ = transformToWatch.localPosition.z;
		return;
		IL_0159:
		if (watchY)
		{
			if (!previousY.HasValue)
			{
				previousY = transformToWatch.localPosition.y;
			}
			if (transformToWatch.localPosition.y < minYTrigger)
			{
				float? num3 = previousY;
				if (num3.HasValue && num3.Value > minYTrigger)
				{
					minYEvent.Invoke();
					goto IL_0242;
				}
			}
			if (transformToWatch.localPosition.y > maxYTrigger)
			{
				float? num4 = previousY;
				if (num4.HasValue && num4.Value < maxYTrigger)
				{
					maxYEvent.Invoke();
				}
			}
			goto IL_0242;
		}
		goto IL_0261;
		IL_0242:
		previousY = transformToWatch.localPosition.y;
		goto IL_0261;
		IL_013a:
		previousX = transformToWatch.localPosition.x;
		goto IL_0159;
		IL_0261:
		if (!watchZ)
		{
			return;
		}
		if (!previousZ.HasValue)
		{
			previousZ = transformToWatch.localPosition.z;
		}
		if (transformToWatch.localPosition.z < minZTrigger)
		{
			float? num5 = previousZ;
			if (num5.HasValue && num5.Value > minZTrigger)
			{
				minZEvent.Invoke();
				goto IL_034a;
			}
		}
		if (transformToWatch.localPosition.z > maxZTrigger)
		{
			float? num6 = previousZ;
			if (num6.HasValue && num6.Value < maxZTrigger)
			{
				maxZEvent.Invoke();
			}
		}
		goto IL_034a;
	}
}
