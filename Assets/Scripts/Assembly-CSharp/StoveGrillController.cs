using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class StoveGrillController : KitchenTool
{
	[SerializeField]
	private WorldItem stoveWorldItem;

	[SerializeField]
	private TemperatureModifierArea temperatureModifierArea;

	[SerializeField]
	private TemperatureStateItem counterTemperatureItem;

	[SerializeField]
	private Transform effectsSurface;

	[SerializeField]
	private Vector3 effectsPositionClampSize = Vector3.one;

	[SerializeField]
	private AudioClip sizzleSound;

	[SerializeField]
	private AudioSourceHelper sizzleAudioHelper;

	private float maxVolume;

	private float currentVolume;

	private float volumeChangeRage = 1f;

	[SerializeField]
	private AudioClip foodBurnedSound;

	[SerializeField]
	private ParticleSystem stoveOnPFX;

	[SerializeField]
	private GameObject cookingEffectPrefab;

	[SerializeField]
	private GameObject enterCookedStatePrefab;

	[SerializeField]
	private GameObject enterBurnStatePrefab;

	[SerializeField]
	private GameObject hissSurface;

	[SerializeField]
	private bool tryToConnectToStove = true;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerArea;

	[SerializeField]
	private MeshRenderer surfaceRenderer;

	[SerializeField]
	private Color hotColor;

	[SerializeField]
	private string stoveUniqueObjName = "Stove";

	private Transform effectHolder;

	private ObjectPool<GameObject> cookingEffects;

	private List<CookableItem> cookablesOnSurface = new List<CookableItem>();

	private List<Rigidbody> normalObjectsOnSurface = new List<Rigidbody>();

	private List<GameObject> cookableCookingEffects = new List<GameObject>();

	private List<GameObject> normalObjectCookingEffects = new List<GameObject>();

	private float maxCookTemp;

	private bool isOn;

	private List<Rigidbody> ignoreRBs = new List<Rigidbody>();

	private bool hasStarted;

	private bool hasTriedStoveConnect;

	private bool hasTriedRBIgnore;

	public float MaxCookTemp
	{
		get
		{
			return maxCookTemp;
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerArea;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerArea;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExited));
		if (effectHolder != null)
		{
			effectHolder.gameObject.SetActive(true);
		}
		if (hasStarted && tryToConnectToStove)
		{
			if (!hasTriedStoveConnect)
			{
				TryStoveConnect();
				hasTriedStoveConnect = true;
			}
			if (!hasTriedRBIgnore)
			{
				TryRBIgnore();
				hasTriedRBIgnore = true;
			}
		}
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerArea;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerArea;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExited));
		if (effectHolder != null)
		{
			effectHolder.gameObject.SetActive(false);
		}
	}

	private void Awake()
	{
		effectHolder = new GameObject("FXHolder").transform;
		effectHolder.SetParent(GlobalStorage.Instance.ContentRoot);
		effectHolder.SetToDefaultPosRotScale();
		cookingEffects = new ObjectPool<GameObject>(cookingEffectPrefab, 15, true, true, effectHolder, Vector3.zero);
		maxCookTemp = temperatureModifierArea.TargetTemperature;
	}

	private IEnumerator Start()
	{
		sizzleAudioHelper.SetClip(sizzleSound);
		sizzleAudioHelper.SetLooping(true);
		maxVolume = sizzleAudioHelper.DefaultStartingVolume;
		hasStarted = true;
		yield return new WaitForEndOfFrame();
		if (tryToConnectToStove)
		{
			TryStoveConnect();
			hasTriedStoveConnect = true;
		}
		yield return new WaitForEndOfFrame();
		if (tryToConnectToStove)
		{
			TryRBIgnore();
			hasTriedRBIgnore = true;
		}
	}

	private void TryRBIgnore()
	{
		Rigidbody component = base.transform.parent.parent.GetComponent<Rigidbody>();
		if (component != null)
		{
			ignoreRBs.Add(component);
		}
		component = base.transform.parent.parent.parent.parent.GetComponent<Rigidbody>();
		if (component != null)
		{
			ignoreRBs.Add(component);
		}
	}

	private void TryStoveConnect()
	{
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(stoveUniqueObjName);
		if (objectByName != null)
		{
			StoveController component = objectByName.GetComponent<StoveController>();
			component.AttachToGrillController(this);
			SetPowerState(component.IsOn);
		}
		else
		{
			SetPowerState(false);
		}
	}

	private void Update()
	{
		if (Time.deltaTime <= 0f)
		{
			return;
		}
		if (cookablesOnSurface.Count > 0 && isOn)
		{
			currentVolume += volumeChangeRage * Time.deltaTime;
		}
		else
		{
			currentVolume -= volumeChangeRage * Time.deltaTime;
		}
		if (currentVolume <= 0f)
		{
			currentVolume = 0f;
			sizzleAudioHelper.SetVolume(currentVolume * maxVolume);
			if (sizzleAudioHelper.IsPlaying)
			{
				sizzleAudioHelper.Stop();
			}
		}
		else
		{
			if (currentVolume >= 1f)
			{
				currentVolume = 1f;
			}
			sizzleAudioHelper.SetVolume(currentVolume * maxVolume);
			if (!sizzleAudioHelper.IsPlaying)
			{
				sizzleAudioHelper.SetClip(sizzleSound);
				sizzleAudioHelper.Play();
			}
		}
		if (!isOn)
		{
			return;
		}
		for (int i = 0; i < cookablesOnSurface.Count; i++)
		{
			if (cookablesOnSurface[i] != null)
			{
				cookableCookingEffects[i].transform.position = GetEffectPosition(cookablesOnSurface[i].transform);
				continue;
			}
			cookablesOnSurface.RemoveAt(i);
			cookingEffects.Release(cookableCookingEffects[i]);
			cookableCookingEffects.RemoveAt(i);
		}
		for (int j = 0; j < normalObjectCookingEffects.Count; j++)
		{
			if (normalObjectsOnSurface[j] != null)
			{
				normalObjectCookingEffects[j].transform.position = GetEffectPosition(normalObjectsOnSurface[j].transform);
				continue;
			}
			normalObjectsOnSurface.RemoveAt(j);
			cookingEffects.Release(normalObjectCookingEffects[j]);
			normalObjectCookingEffects.RemoveAt(j);
		}
	}

	private Vector3 GetEffectPosition(Transform effect)
	{
		return new Vector3(Mathf.Clamp(effect.position.x, effectsSurface.position.x - effectsPositionClampSize.x, effectsSurface.position.x + effectsPositionClampSize.x), Mathf.Clamp(effect.position.y, effectsSurface.position.y - effectsPositionClampSize.y, effectsSurface.position.y + effectsPositionClampSize.y), Mathf.Clamp(effect.position.z, effectsSurface.position.z - effectsPositionClampSize.z, effectsSurface.position.z + effectsPositionClampSize.z));
	}

	public void SetPowerState(bool on)
	{
		if (stoveOnPFX != null)
		{
			ParticleSystem.EmissionModule emission = stoveOnPFX.emission;
			emission.enabled = on;
		}
		if (hissSurface != null)
		{
			hissSurface.SetActive(on);
		}
		for (int i = 0; i < cookableCookingEffects.Count; i++)
		{
			cookableCookingEffects[i].SetActive(on);
		}
		for (int j = 0; j < normalObjectCookingEffects.Count; j++)
		{
			normalObjectCookingEffects[j].SetActive(on);
		}
		if (on)
		{
			SetCounterTemperature(maxCookTemp);
			StartCookingEverything();
		}
		else
		{
			SetCounterTemperature(21f);
			StopCookingEverything();
		}
		isOn = on;
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(PowerToggleColorChangeAsync(on));
		}
	}

	private IEnumerator PowerToggleColorChangeAsync(bool on)
	{
		Color oldColor = surfaceRenderer.material.GetColor("_DiffColor");
		Color newColor = Color.white;
		if (on)
		{
			newColor = hotColor;
		}
		float currentLerp = 0f;
		while (currentLerp < 1f && isOn == on)
		{
			surfaceRenderer.material.SetColor("_DiffColor", Color.Lerp(oldColor, newColor, currentLerp));
			currentLerp += Time.deltaTime * 0.9f;
			yield return null;
		}
	}

	public void SetCounterTemperature(float temperature)
	{
		temperatureModifierArea.SetTargetTemperature(temperature);
		counterTemperatureItem.SetManualTemperature(temperature);
	}

	private void StopCookingEverything()
	{
		effectsSurface.gameObject.SetActive(false);
	}

	private void StartCookingEverything()
	{
		effectsSurface.gameObject.SetActive(true);
	}

	private void SomethingWasCooked(CookableItem item)
	{
		GameEventsManager.Instance.ItemActionOccurred(stoveWorldItem.Data, "USED");
	}

	private void SomethingWasBurned(CookableItem item)
	{
		if (foodBurnedSound != null)
		{
			AudioManager.Instance.Play(item.transform.position, foodBurnedSound, 1f, 1f);
		}
	}

	private void RigidbodyEntered(Rigidbody r)
	{
		CookableItem component = r.GetComponent<CookableItem>();
		if (component != null)
		{
			if (!cookablesOnSurface.Contains(component))
			{
				cookablesOnSurface.Add(component);
				component.OnCooked = (Action<CookableItem>)Delegate.Combine(component.OnCooked, new Action<CookableItem>(SomethingWasCooked));
				component.OnBurned = (Action<CookableItem>)Delegate.Combine(component.OnBurned, new Action<CookableItem>(SomethingWasBurned));
				GameObject gameObject = cookingEffects.Fetch(component.transform.position, Quaternion.identity);
				cookableCookingEffects.Add(gameObject);
				gameObject.SetActive(isOn);
			}
			else
			{
				Debug.LogWarning("Tried to add cookable " + component.gameObject.name + " to cooking surface more than once.");
			}
		}
		else if (!ignoreRBs.Contains(r) && !base.transform.IsChildOf(r.transform))
		{
			if (!normalObjectsOnSurface.Contains(r))
			{
				normalObjectsOnSurface.Add(r);
				GameObject gameObject2 = cookingEffects.Fetch(r.transform.position, Quaternion.identity);
				normalObjectCookingEffects.Add(gameObject2);
				gameObject2.SetActive(isOn);
			}
			else
			{
				Debug.LogWarning("Tried to add rigidbody " + r.gameObject.name + " to cooking surface more than once.");
			}
		}
	}

	private void RigidbodyExited(Rigidbody r)
	{
		CookableItem component = r.GetComponent<CookableItem>();
		if (component != null)
		{
			RemoveCookable(component);
		}
		else if (normalObjectsOnSurface.Contains(r))
		{
			int index = normalObjectsOnSurface.IndexOf(r);
			normalObjectsOnSurface.RemoveAt(index);
			cookingEffects.Release(normalObjectCookingEffects[index]);
			normalObjectCookingEffects.RemoveAt(index);
		}
		else
		{
			Debug.LogWarning("Tried to remove rigidbody " + r.gameObject.name + " from cooking surface, but it was not in the list.");
		}
	}

	private void RemoveCookable(CookableItem c)
	{
		if (cookablesOnSurface.Contains(c))
		{
			int index = cookablesOnSurface.IndexOf(c);
			c.OnCooked = (Action<CookableItem>)Delegate.Remove(c.OnCooked, new Action<CookableItem>(SomethingWasCooked));
			c.OnBurned = (Action<CookableItem>)Delegate.Remove(c.OnBurned, new Action<CookableItem>(SomethingWasBurned));
			cookablesOnSurface.RemoveAt(index);
			cookingEffects.Release(cookableCookingEffects[index]);
			cookableCookingEffects.RemoveAt(index);
		}
		else
		{
			Debug.LogWarning("Tried to remove cookable " + c.gameObject.name + " from cooking surface, but it was not in the list.");
		}
	}

	private void OnDrawGizmos()
	{
		if (effectsSurface != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(effectsSurface.position, effectsPositionClampSize * 2f);
		}
	}
}
