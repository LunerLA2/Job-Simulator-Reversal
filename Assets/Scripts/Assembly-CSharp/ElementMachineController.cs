using System;
using TMPro;
using UnityEngine;

public class ElementMachineController : MonoBehaviour
{
	[SerializeField]
	private UniqueElement[] uniqueElements;

	[SerializeField]
	private GasElement[] gasElements;

	[SerializeField]
	private LiquidElement[] liquidElements;

	[SerializeField]
	private SolidElement[] solidElements;

	[SerializeField]
	private SliderPoweredDialController leftDial;

	[SerializeField]
	private SliderPoweredDialController rightDial;

	[SerializeField]
	private MeshRenderer solidLight;

	[SerializeField]
	private MeshRenderer liquidLight;

	[SerializeField]
	private Transform leverTransform;

	[SerializeField]
	private Transform solidSpawnPoint;

	[SerializeField]
	private GravityDispensingItem gravityDispenser;

	[SerializeField]
	private ParticleSystem gasParticle;

	[SerializeField]
	private GameObject metalIngot;

	[SerializeField]
	private LightningController lightning;

	[SerializeField]
	private TextMeshPro elementText;

	private int number;

	private Element currElement;

	private bool inUse;

	private float inUseWaitTime = 3f;

	private float endInUse;

	private void Start()
	{
		liquidLight.enabled = false;
		solidLight.enabled = false;
		inUse = false;
		elementText.text = string.Empty;
	}

	private void Update()
	{
		if (inUse && Time.time >= endInUse)
		{
			inUse = false;
			gravityDispenser.enabled = false;
			gasParticle.Stop();
			lightning.Stop();
		}
		if (!inUse && leverTransform.localEulerAngles.x > 50f && leverTransform.localEulerAngles.x < 180f)
		{
			LeverPulled();
		}
	}

	private void OnEnable()
	{
		SliderPoweredDialController sliderPoweredDialController = leftDial;
		sliderPoweredDialController.OnSelectedNumberChanged = (Action)Delegate.Combine(sliderPoweredDialController.OnSelectedNumberChanged, new Action(UpdateNumberFromDials));
		SliderPoweredDialController sliderPoweredDialController2 = rightDial;
		sliderPoweredDialController2.OnSelectedNumberChanged = (Action)Delegate.Combine(sliderPoweredDialController2.OnSelectedNumberChanged, new Action(UpdateNumberFromDials));
	}

	private void OnDisable()
	{
		SliderPoweredDialController sliderPoweredDialController = leftDial;
		sliderPoweredDialController.OnSelectedNumberChanged = (Action)Delegate.Remove(sliderPoweredDialController.OnSelectedNumberChanged, new Action(UpdateNumberFromDials));
		SliderPoweredDialController sliderPoweredDialController2 = rightDial;
		sliderPoweredDialController2.OnSelectedNumberChanged = (Action)Delegate.Remove(sliderPoweredDialController2.OnSelectedNumberChanged, new Action(UpdateNumberFromDials));
	}

	public void UpdateNumberFromDials()
	{
		number = leftDial.CurrentSelectedNumber * 10 + rightDial.CurrentSelectedNumber;
		currElement = FindElementWithNumber(number);
		if (currElement != null)
		{
			elementText.text = currElement.name;
			if (currElement.GetState() == StateOfMatter.Liquid || currElement.GetState() == StateOfMatter.Gas)
			{
				liquidLight.enabled = true;
				solidLight.enabled = false;
			}
			else
			{
				solidLight.enabled = true;
				liquidLight.enabled = false;
			}
		}
		else
		{
			elementText.text = string.Empty;
			liquidLight.enabled = false;
			solidLight.enabled = false;
		}
	}

	public void LeverPulled()
	{
		if (currElement != null)
		{
			inUse = true;
			if (currElement.GetState() == StateOfMatter.Liquid)
			{
				SpawnLiquid();
			}
			if (currElement.GetState() == StateOfMatter.Unique)
			{
				SpawnUniqueObject();
			}
			if (currElement.GetState() == StateOfMatter.Gas)
			{
				SpawnGas();
			}
			if (currElement.GetState() == StateOfMatter.Solid)
			{
				SpawnSolid();
			}
			lightning.Play();
			endInUse = Time.time + inUseWaitTime;
		}
	}

	public void SpawnLiquid()
	{
		LiquidElement liquidElement = (LiquidElement)currElement;
		gravityDispenser.SetFluidToDispense(liquidElement.fluid);
		gravityDispenser.enabled = true;
	}

	public void SpawnSolid()
	{
		UnityEngine.Object.Instantiate(metalIngot, solidSpawnPoint.transform.position, solidSpawnPoint.transform.rotation);
	}

	public void SpawnGas()
	{
		GasElement gasElement = (GasElement)currElement;
		gasParticle.startColor = gasElement.color;
		gasParticle.Play();
	}

	public void SpawnUniqueObject()
	{
		UniqueElement uniqueElement = (UniqueElement)currElement;
		UnityEngine.Object.Instantiate(uniqueElement.objectPrefab, solidSpawnPoint.transform.position, solidSpawnPoint.transform.rotation);
	}

	private Element FindElementWithNumber(int num)
	{
		for (int i = 0; i < gasElements.Length; i++)
		{
			if (num == gasElements[i].number)
			{
				return gasElements[i];
			}
		}
		for (int j = 0; j < liquidElements.Length; j++)
		{
			if (num == liquidElements[j].number)
			{
				return liquidElements[j];
			}
		}
		for (int k = 0; k < uniqueElements.Length; k++)
		{
			if (num == uniqueElements[k].number)
			{
				return uniqueElements[k];
			}
		}
		for (int l = 0; l < solidElements.Length; l++)
		{
			if (num == solidElements[l].number)
			{
				return solidElements[l];
			}
		}
		return null;
	}
}
