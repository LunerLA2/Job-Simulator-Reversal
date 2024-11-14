using UnityEngine;

public class SpatializerDebugParameters : MonoBehaviour
{
	[SerializeField]
	private AudioSource source;

	public bool invSqrUsed;

	public bool reflectionIsDisabled;

	public float gain;

	private float invSq;

	public float near;

	public float far;

	public float reflectionfloat;

	private void Update()
	{
		source.GetSpatializerFloat(0, out gain);
		source.GetSpatializerFloat(1, out invSq);
		if (invSq == 1f)
		{
			invSqrUsed = true;
		}
		else
		{
			invSqrUsed = false;
		}
		source.GetSpatializerFloat(2, out near);
		source.GetSpatializerFloat(3, out far);
		source.GetSpatializerFloat(4, out reflectionfloat);
		if (reflectionfloat == 1f)
		{
			reflectionIsDisabled = true;
		}
		else
		{
			reflectionIsDisabled = false;
		}
	}
}
