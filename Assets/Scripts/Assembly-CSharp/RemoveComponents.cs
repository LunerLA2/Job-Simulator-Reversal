using UnityEngine;

[ExecuteInEditMode]
public class RemoveComponents : MonoBehaviour
{
	public void RemoveAllComponents()
	{
		MonoBehaviour[] componentsInChildren = GetComponentsInChildren<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in componentsInChildren)
		{
			if (!(monoBehaviour.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(monoBehaviour);
			}
		}
	}

	public void RemoveAllRigidbodies()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			if (!(rigidbody.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(rigidbody);
			}
		}
	}

	public void RemoveAllColliders()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (!(collider.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(collider);
			}
		}
	}

	public void RemoveAllCameras()
	{
		Camera[] componentsInChildren = GetComponentsInChildren<Camera>();
		foreach (Camera camera in componentsInChildren)
		{
			if (!(camera.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(camera);
			}
		}
	}

	public void RemoveAllLights()
	{
		Light[] componentsInChildren = GetComponentsInChildren<Light>();
		foreach (Light light in componentsInChildren)
		{
			if (!(light.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(light);
			}
		}
	}

	public void RemoveAllAudioSource()
	{
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audioSource in componentsInChildren)
		{
			if (!(audioSource.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(audioSource);
			}
		}
	}

	public void RemoveAllAnimations()
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			if (!(animation.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(animation);
			}
		}
	}

	public void RemoveAllAnimators()
	{
		Animator[] componentsInChildren = GetComponentsInChildren<Animator>();
		foreach (Animator animator in componentsInChildren)
		{
			if (!(animator.gameObject == base.gameObject))
			{
				Object.DestroyImmediate(animator);
			}
		}
	}

	public void BreakAllPrefabReferences()
	{
	}

	public void StaticToDynamic()
	{
		foreach (Transform item in base.transform)
		{
			if (!(item.gameObject == base.gameObject))
			{
				item.gameObject.isStatic = false;
			}
		}
	}

	public void DefaultLayer()
	{
		foreach (Transform item in base.transform)
		{
			if (!(item.gameObject == base.gameObject))
			{
				item.gameObject.layer = 0;
			}
		}
	}

	public void Untag()
	{
		foreach (Transform item in base.transform)
		{
			if (!(item.gameObject == base.gameObject))
			{
				item.gameObject.tag = "Untagged";
			}
		}
	}
}
