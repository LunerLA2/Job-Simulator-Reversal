using UnityEngine;

public class RollableItem : MonoBehaviour
{
	[SerializeField]
	private Collider[] collidersToRemoveMaterial;

	[SerializeField]
	private RotateAtSpeed rotateAtSpeed;

	private PhysicMaterial ogMaterial;

	private bool isRotating;

	public bool IsRotating
	{
		get
		{
			return isRotating;
		}
	}

	private void Start()
	{
		ogMaterial = collidersToRemoveMaterial[collidersToRemoveMaterial.Length - 1].material;
	}

	public void SetRotationSpeed(float rotationSpeed)
	{
		if (rotationSpeed > 0f)
		{
			RemoveMaterials();
			isRotating = true;
		}
		else
		{
			isRotating = false;
			ResetMaterials();
		}
		rotateAtSpeed.SetSpeed(new Vector3(rotationSpeed, 0f, 0f));
	}

	private void SetPhysicsMaterial(PhysicMaterial newMat)
	{
		for (int i = 0; i < collidersToRemoveMaterial.Length; i++)
		{
			collidersToRemoveMaterial[i].material = newMat;
		}
	}

	private void RemoveMaterials()
	{
		SetPhysicsMaterial(null);
	}

	public void ResetMaterials()
	{
		SetPhysicsMaterial(ogMaterial);
	}
}
