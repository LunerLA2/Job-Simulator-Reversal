using System.Collections;
using UnityEngine;

public class ShowerManager : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem shower;

	[SerializeField]
	private ParticleSystem eyewash;

	[SerializeField]
	private Collider myCollider;

	private bool showerActive;

	private void OnTriggerEnter(Collider col)
	{
		TransmutableObject component = col.gameObject.GetComponent<TransmutableObject>();
		if (component != null)
		{
			component.RecieveChemical(ChemLabManager.Chemicals.Water);
		}
		Debug.Log("Shower Applied Water to: " + col.gameObject.name);
	}

	public void ShowerButton()
	{
		if (!showerActive)
		{
			shower.Play();
			StartCoroutine(ShowerTimer());
		}
	}

	private IEnumerator ShowerTimer()
	{
		showerActive = true;
		myCollider.enabled = true;
		yield return new WaitForSeconds(5f);
		myCollider.enabled = false;
		showerActive = false;
	}

	public void EyewashButton()
	{
		eyewash.Play();
	}
}
