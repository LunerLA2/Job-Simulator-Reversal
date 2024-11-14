using System;
using OwlchemyVR;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Replica))]
public class SculptureController : MonoBehaviour
{
	private const float HIT_TIMEOUT = 0.2f;

	public Action<SculptureController> OnLastHit;

	public Action<SculptureController, float> OnHit;

	[SerializeField]
	private GameObject[] shellStages;

	[SerializeField]
	private Transform[] shellStageTops;

	[SerializeField]
	private ParticleSystem[] hitParticles;

	[SerializeField]
	private AudioClip hitSound;

	[SerializeField]
	private AudioClip completionSound;

	[SerializeField]
	private TextMeshPro labelText;

	private int shellStageIndex;

	private float lastHitTime;

	public event Action<SculptureController> OnFirstHit;

	private void Awake()
	{
		shellStages[0].SetActive(true);
	}

	public void PickaxeHit()
	{
		if (shellStageIndex == shellStages.Length || base.transform.localScale != Vector3.one || Time.time - lastHitTime < 0.2f)
		{
			return;
		}
		lastHitTime = Time.time;
		if (shellStageIndex == 0 && this.OnFirstHit != null)
		{
			this.OnFirstHit(this);
		}
		shellStages[shellStageIndex].SetActive(false);
		for (int i = 0; i < hitParticles.Length; i++)
		{
			hitParticles[i].transform.position = shellStageTops[shellStageIndex].position;
			hitParticles[i].Play();
		}
		AudioManager.Instance.Play(shellStageTops[shellStageIndex].position, hitSound, 1f, 1f);
		shellStageIndex++;
		if (shellStageIndex == shellStages.Length)
		{
			GetComponent<PickupableItem>().enabled = true;
			for (int j = 0; j < shellStages.Length; j++)
			{
				UnityEngine.Object.Destroy(shellStages[j].gameObject);
			}
			for (int k = 0; k < hitParticles.Length; k++)
			{
				UnityEngine.Object.Destroy(hitParticles[k].gameObject, hitParticles[k].duration);
			}
			if (OnHit != null)
			{
				OnHit(this, 1f);
			}
			if (OnLastHit != null)
			{
				OnLastHit(this);
			}
		}
		else
		{
			shellStages[shellStageIndex].SetActive(true);
			if (OnHit != null)
			{
				OnHit(this, (float)shellStageIndex / (float)shellStages.Length);
			}
		}
	}

	public void Build(Transform modelBase, PickupableItem[] models)
	{
		if (models.Length == 0)
		{
			return;
		}
		AudioManager.Instance.Play(base.transform.position, completionSound, 1f, 1f);
		GetComponent<Replica>().CopyModels(modelBase, models);
		string text = string.Empty;
		bool flag = false;
		for (int i = 0; i < models.Length; i++)
		{
			if (i == 0)
			{
				text = models[i].InteractableItem.WorldItemData.ItemFullName;
			}
			else if (!flag)
			{
				text = text + ", " + models[i].InteractableItem.WorldItemData.ItemFullName;
				if (text.Length > 25 && i < models.Length - 1)
				{
					text += ", ...";
					flag = true;
				}
			}
		}
		labelText.text = text;
	}
}
