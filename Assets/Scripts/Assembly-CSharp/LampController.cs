using System;
using UnityEngine;

public class LampController : MonoBehaviour
{
	[SerializeField]
	private AttachableObject lampPlug;

	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	[SerializeField]
	private Rigidbody[] rbsToLockDown;

	[SerializeField]
	private Light[] lights;

	[SerializeField]
	private GameObject[] ObjsToToggleWithLight;

	[SerializeField]
	private MeshRenderer bulbMesh;

	[SerializeField]
	private Material bulbLitMaterial;

	[SerializeField]
	private Material bulbUnlitMaterial;

	[SerializeField]
	private AudioClip onClip;

	[SerializeField]
	private AudioClip offClip;

	private void OnEnable()
	{
		AttachableObject attachableObject = lampPlug;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PluggedIn));
		AttachableObject attachableObject2 = lampPlug;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Unplugged));
		PlayerPartDetector obj = playerPartDetector;
		obj.OnFirstPartEntered = (Action<PlayerPartDetector>)Delegate.Combine(obj.OnFirstPartEntered, new Action<PlayerPartDetector>(FirstPartEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnLastPartExited = (Action<PlayerPartDetector>)Delegate.Combine(obj2.OnLastPartExited, new Action<PlayerPartDetector>(LastPartExited));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = lampPlug;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PluggedIn));
		AttachableObject attachableObject2 = lampPlug;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Unplugged));
		PlayerPartDetector obj = playerPartDetector;
		obj.OnFirstPartEntered = (Action<PlayerPartDetector>)Delegate.Remove(obj.OnFirstPartEntered, new Action<PlayerPartDetector>(FirstPartEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnLastPartExited = (Action<PlayerPartDetector>)Delegate.Remove(obj2.OnLastPartExited, new Action<PlayerPartDetector>(LastPartExited));
	}

	private void FirstPartEntered(PlayerPartDetector ppd)
	{
		for (int i = 0; i < rbsToLockDown.Length; i++)
		{
			rbsToLockDown[i].isKinematic = true;
		}
	}

	private void LastPartExited(PlayerPartDetector ppd)
	{
		for (int i = 0; i < rbsToLockDown.Length; i++)
		{
			rbsToLockDown[i].isKinematic = false;
		}
	}

	private void Awake()
	{
		Unplugged(null, null);
	}

	private void PluggedIn(AttachableObject o, AttachablePoint p)
	{
		bulbMesh.material = bulbLitMaterial;
		AudioManager.Instance.Play(base.transform.position, onClip, 1f, 1f);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = true;
		}
		for (int j = 0; j < ObjsToToggleWithLight.Length; j++)
		{
			ObjsToToggleWithLight[j].SetActive(true);
		}
	}

	private void Unplugged(AttachableObject o, AttachablePoint p)
	{
		bulbMesh.material = bulbUnlitMaterial;
		AudioManager.Instance.Play(base.transform.position, onClip, 1f, 1f);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = false;
		}
		for (int j = 0; j < ObjsToToggleWithLight.Length; j++)
		{
			ObjsToToggleWithLight[j].SetActive(false);
		}
	}
}
