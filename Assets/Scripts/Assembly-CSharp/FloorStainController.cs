using System;
using System.Collections;
using UnityEngine;

public class FloorStainController : MonoBehaviour
{
	[SerializeField]
	private Transform stain;

	[SerializeField]
	private ParticleSystem bubbleFx;

	[SerializeField]
	private Collider col;

	public Action OnDeactivate;

	public void Reset()
	{
		base.transform.localScale = Vector3.one;
		col.enabled = true;
		stain.gameObject.SetActive(true);
	}

	public void Hit()
	{
		Debug.Log("Hit!");
		StartCoroutine(hitRoutine());
	}

	private IEnumerator hitRoutine()
	{
		col.enabled = false;
		bubbleFx.Play();
		if (OnDeactivate != null)
		{
			OnDeactivate();
		}
		while (stain.localScale.x > 0.01f)
		{
			stain.localScale = Vector3.Lerp(stain.localScale, Vector3.zero, Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		stain.gameObject.SetActive(false);
	}
}
