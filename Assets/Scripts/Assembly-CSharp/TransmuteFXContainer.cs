using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TransmuteFXContainer : MonoBehaviour
{
	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private Material material;

	[SerializeField]
	private Material frozenMaterial;

	[SerializeField]
	private Material burningMaterial;

	[SerializeField]
	private ParticleSystem frozenParticleSystem;

	[SerializeField]
	private ParticleSystem burningParticleSystem;

	private bool rendererToggle;

	private bool particleToggle;

	private ParticleSystem frozenFX;

	private ParticleSystem burningFX;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = rendererToggle;
		frozenFX = Object.Instantiate(frozenParticleSystem);
		frozenFX.transform.SetParent(base.transform, false);
		burningFX = Object.Instantiate(burningParticleSystem);
		burningFX.transform.SetParent(base.transform, false);
		frozenFX.Stop();
		frozenFX.Clear();
		burningFX.Stop();
		burningFX.Clear();
	}

	public void SetMesh(MeshFilter targetMeshFilter)
	{
		meshFilter.mesh = targetMeshFilter.mesh;
	}

	public void SetState(ChemLabManager.ObjectStates targetState)
	{
		switch (targetState)
		{
		case ChemLabManager.ObjectStates.Frozen:
			material = frozenMaterial;
			rendererToggle = true;
			frozenFX.Play();
			burningFX.Stop();
			burningFX.Clear();
			break;
		case ChemLabManager.ObjectStates.Burning:
			material = burningMaterial;
			rendererToggle = true;
			burningFX.Play();
			frozenFX.Stop();
			frozenFX.Clear();
			break;
		case ChemLabManager.ObjectStates.Basic:
			material = null;
			rendererToggle = false;
			frozenFX.Stop();
			frozenFX.Clear();
			burningFX.Stop();
			burningFX.Clear();
			break;
		}
		meshRenderer.enabled = rendererToggle;
		meshRenderer.material = material;
	}
}
