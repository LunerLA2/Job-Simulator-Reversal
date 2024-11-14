using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class CarPainterController : MonoBehaviour
{
	public enum CarPainterStatus
	{
		ready = 0,
		painting = 1,
		noCar = 2
	}

	private CarPainterStatus carPainterStatus = CarPainterStatus.noCar;

	private Color targetColor;

	[SerializeField]
	private GrabbableSlider colorSlider;

	[SerializeField]
	private Gradient colorSelectionGradient;

	[SerializeField]
	private Gradient colorSelectionGradientDrifty;

	[SerializeField]
	private Animation paintLoadUpAnimation;

	[SerializeField]
	private ParticleSystem paintLoadUpParticle;

	[SerializeField]
	private MeshRenderer colorPreview;

	[SerializeField]
	private TextMeshPro statusLabel;

	[SerializeField]
	private string statusTextReady = string.Empty;

	[SerializeField]
	private string statusTextBusy = string.Empty;

	[SerializeField]
	private string statusTextBayEmpty = string.Empty;

	[SerializeField]
	private Color statusTextColorReady;

	[SerializeField]
	private Color statusTextColorNotReady;

	[SerializeField]
	private MeshRenderer readyLight;

	[SerializeField]
	private Material readyMaterial;

	[SerializeField]
	private Material notReadyMaterial;

	[SerializeField]
	private MeshRenderer paintInTubeRenderer;

	[SerializeField]
	private ParticleSystem[] sprayParticles;

	[SerializeField]
	private ParticleSystem[] sprayParticlesPSVR;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private AudioSourceHelper paintSprayStart;

	[SerializeField]
	private AudioSourceHelper paintSprayLoop;

	[SerializeField]
	private AudioSourceHelper paintLiquidLoop;

	private Color desiredColor = Color.white;

	private Material carMaterialReference;

	private bool endless;

	public Color TargetColor
	{
		get
		{
			return targetColor;
		}
	}

	private void Awake()
	{
		CarExit();
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			endless = true;
		}
	}

	private void Update()
	{
		if (endless)
		{
			desiredColor = colorSelectionGradientDrifty.Evaluate(colorSlider.NormalizedOffset);
		}
		else
		{
			desiredColor = colorSelectionGradient.Evaluate(colorSlider.NormalizedOffset);
		}
		colorPreview.material.color = desiredColor;
	}

	public void CarReady()
	{
		carPainterStatus = CarPainterStatus.ready;
		statusLabel.text = statusTextReady;
		readyLight.material = readyMaterial;
		statusLabel.color = statusTextColorReady;
	}

	public void CarExit()
	{
		carPainterStatus = CarPainterStatus.noCar;
		statusLabel.text = statusTextBayEmpty;
		readyLight.material = notReadyMaterial;
		statusLabel.color = statusTextColorNotReady;
	}

	public void StartPainter(Color col)
	{
		desiredColor = col;
		PaintBasedOnSliderPosition();
	}

	public void PaintBasedOnSliderPosition()
	{
		if (carPainterStatus == CarPainterStatus.ready)
		{
			targetColor = desiredColor;
			targetColor.a = 1f;
			for (int i = 0; i < sprayParticles.Length; i++)
			{
				sprayParticles[i].startColor = desiredColor;
			}
			paintLoadUpParticle.startColor = desiredColor;
			paintInTubeRenderer.material.color = desiredColor;
			StartCoroutine(PainterRoutine());
		}
	}

	private IEnumerator PaintLiquidLoopAudio(float duration)
	{
		paintLiquidLoop.SetLooping(true);
		paintLiquidLoop.Play();
		yield return new WaitForSeconds(duration);
		paintLiquidLoop.Stop();
	}

	private IEnumerator PainterRoutine()
	{
		carPainterStatus = CarPainterStatus.painting;
		statusLabel.text = statusTextBusy;
		readyLight.material = notReadyMaterial;
		paintSprayStart.Play();
		paintLoadUpAnimation.Play();
		StartCoroutine(PaintLiquidLoopAudio(paintLoadUpAnimation.clip.length));
		yield return new WaitForSeconds(2.85f);
		paintSprayLoop.SetLooping(true);
		paintSprayLoop.Play();
		for (int i = 0; i < sprayParticles.Length; i++)
		{
			sprayParticles[i].Play();
		}
		yield return new WaitForSeconds(1f);
		bool hideDetailTextureWhenDone = false;
		bool swapDetailTextureWhenDone = false;
		Color currentColor = Color.white;
		if (AutoMechanicManager.CurrentVehicle != null)
		{
			currentColor = TexturePainterController.Instance.CurrentTintColor;
			hideDetailTextureWhenDone = AutoMechanicManager.CurrentVehicle.HideDetailTextureOncePainted;
			swapDetailTextureWhenDone = AutoMechanicManager.CurrentVehicle.SwapDetailTextureOncePainted;
		}
		float t = 0f;
		float actualTime = 1f;
		float pfxDuration2 = 0f;
		pfxDuration2 = sprayParticles[0].duration;
		while (t < 1f)
		{
			actualTime += Time.deltaTime;
			if (actualTime >= pfxDuration2 && paintSprayLoop.IsPlaying)
			{
				paintSprayLoop.Stop();
			}
			t = Mathf.Min(t + Time.deltaTime / 1.5f, 1f);
			if (AutoMechanicManager.CurrentVehicle != null)
			{
				Color carColorLerp = Color.Lerp(currentColor, targetColor, t);
				TexturePainterController.Instance.SetTintColor(carColorLerp);
				TexturePainterController.Instance.Refresh();
			}
			yield return null;
		}
		if (hideDetailTextureWhenDone)
		{
			TexturePainterController.Instance.HideDetailTexture();
		}
		else if (swapDetailTextureWhenDone && AutoMechanicManager.CurrentVehicle.detailTextureOnPaint != null)
		{
			TexturePainterController.Instance.SwapDetailTexture(AutoMechanicManager.CurrentVehicle.detailTextureOnPaint);
		}
		WorldItem currentVehicle = AutoMechanicManager.CurrentVehicle.GetChassis.GetVehicleWorldItem;
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		GameEventsManager.Instance.ItemActionOccurred(currentVehicle.Data, "CLEANED");
		TexturePainterController.Instance.Refresh();
		if (carPainterStatus == CarPainterStatus.painting)
		{
			statusLabel.text = statusTextReady;
			readyLight.material = readyMaterial;
			carPainterStatus = CarPainterStatus.ready;
		}
	}
}
