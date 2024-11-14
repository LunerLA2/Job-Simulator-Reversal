using UnityEngine;

namespace OwlchemyVR
{
	public class ModelOutlineContainer : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter mf;

		[SerializeField]
		private Renderer modelRenderer;

		[SerializeField]
		private Material normalOutlineMaterial;

		[SerializeField]
		private Material specialOutlineMaterial;

		private bool isHighlighted;

		private bool isSpecial;

		public void SetMesh(Mesh mesh)
		{
			mf.mesh = mesh;
			if (mesh.subMeshCount > 1)
			{
				Debug.LogWarning(mf.transform.parent.gameObject.name + "'s renderer is using more than 1 material. The object should probably be 2 different meshes.", mf.gameObject);
				Material[] array = new Material[mesh.subMeshCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = normalOutlineMaterial;
				}
				modelRenderer.materials = array;
			}
		}

		public void SetEnableRenderer(bool isEnabled)
		{
			if (modelRenderer == null)
			{
				return;
			}
			if (isEnabled)
			{
				modelRenderer.enabled = true;
				if (modelRenderer.materials.Length > 1)
				{
					Material[] materials = modelRenderer.materials;
					for (int i = 0; i < materials.Length; i++)
					{
						materials[i] = normalOutlineMaterial;
					}
					modelRenderer.materials = materials;
				}
				else
				{
					modelRenderer.material = normalOutlineMaterial;
				}
			}
			else if (isSpecial)
			{
				modelRenderer.enabled = true;
				if (modelRenderer.materials.Length > 1)
				{
					Material[] materials2 = modelRenderer.materials;
					for (int j = 0; j < materials2.Length; j++)
					{
						materials2[j] = specialOutlineMaterial;
					}
					modelRenderer.materials = materials2;
				}
				else
				{
					modelRenderer.material = specialOutlineMaterial;
				}
			}
			else
			{
				modelRenderer.enabled = false;
			}
			isHighlighted = isEnabled;
		}

		public void SetSpecialHighlight(bool _isSpecial)
		{
			isSpecial = _isSpecial;
			SetEnableRenderer(isHighlighted);
		}
	}
}
