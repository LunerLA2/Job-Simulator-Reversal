using UnityEngine;

public class CoffeeMakerController : MonoBehaviour
{
	[SerializeField]
	private Renderer lightRenderer;

	[SerializeField]
	private Material lightOnMaterial;

	[SerializeField]
	private Material lightOffMaterial;

	[SerializeField]
	private float mlOfLiquidToDispenseBeforeShuttingOff = 300f;

	[SerializeField]
	private GravityDispensingItem gravityDispensingItem;

	[SerializeField]
	private AudioClip[] ambientSounds;

	private ElementSequence<AudioClip> ambientSoundSequence;

	private bool isDispensing;

	private float mlOfLiquidDispensedSinceTurningOn;

	private float nextAmbientSound;

	private void Awake()
	{
		ambientSoundSequence = new ElementSequence<AudioClip>(ambientSounds);
		ResetNextAmbientSoundTimer();
	}

	public void ToggleDispensing()
	{
		isDispensing = !isDispensing;
		mlOfLiquidDispensedSinceTurningOn = 0f;
		if (lightRenderer != null && lightOnMaterial != null && lightOffMaterial != null)
		{
			lightRenderer.material = ((!isDispensing) ? lightOffMaterial : lightOnMaterial);
		}
		gravityDispensingItem.enabled = isDispensing;
	}

	private void ResetNextAmbientSoundTimer()
	{
		nextAmbientSound = Random.Range(5f, 12f);
	}

	private void Update()
	{
		if (nextAmbientSound <= 0f && ambientSounds.Length > 0)
		{
			AudioManager.Instance.Play(base.transform, ambientSoundSequence.GetNext(), 1f, 1f);
			ResetNextAmbientSoundTimer();
		}
		else
		{
			nextAmbientSound -= Time.deltaTime;
		}
		if (isDispensing)
		{
			mlOfLiquidDispensedSinceTurningOn += Time.deltaTime * gravityDispensingItem.DispenseQuantityMLPerSecond;
			if (mlOfLiquidDispensedSinceTurningOn >= mlOfLiquidToDispenseBeforeShuttingOff)
			{
				ToggleDispensing();
			}
		}
	}
}
