using UnityEngine;

public class ConvenienceStoreLightManager : MonoBehaviour
{
	[SerializeField]
	private Light directionalLight;

	private float cachedDirectionalLightIntensity;

	private void Start()
	{
		cachedDirectionalLightIntensity = directionalLight.intensity;
	}

	private void LightsToggle(bool On)
	{
		if (On)
		{
			directionalLight.intensity = cachedDirectionalLightIntensity;
		}
		else
		{
			directionalLight.intensity = 0f;
		}
	}
}
