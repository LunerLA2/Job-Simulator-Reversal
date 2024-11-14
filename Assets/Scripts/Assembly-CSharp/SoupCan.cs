using System.Collections;
using TMPro;
using UnityEngine;

public class SoupCan : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private TextMeshPro labelTMPro;

	[SerializeField]
	private Camera labelCamera;

	[SerializeField]
	private MeshRenderer labelMesh;

	private bool hasBeenSetup;

	private void Start()
	{
		if (!hasBeenSetup)
		{
			SetLabel(string.Empty);
		}
	}

	private IEnumerator WaitAndCheckIfSetup()
	{
		yield return new WaitForSeconds(0.25f);
		SetLabel(string.Empty);
	}

	public void SetupSoupCan(Material material)
	{
		meshRenderer.sharedMaterial = material;
	}

	public void SetLabel(string labelText)
	{
		if (!hasBeenSetup)
		{
			hasBeenSetup = true;
			StartCoroutine(SetLabelAsync(labelText));
		}
	}

	private IEnumerator SetLabelAsync(string labelText)
	{
		RenderTexture labelTex = new RenderTexture(512, 256, 0)
		{
			wrapMode = TextureWrapMode.Clamp
		};
		labelTMPro.text = labelText;
		labelCamera.gameObject.SetActive(true);
		labelCamera.targetTexture = labelTex;
		labelCamera.Render();
		labelCamera.gameObject.SetActive(false);
		labelMesh.material.mainTexture = labelTex;
		yield return null;
		labelCamera.targetTexture = null;
		Object.Destroy(labelCamera.gameObject);
	}
}
