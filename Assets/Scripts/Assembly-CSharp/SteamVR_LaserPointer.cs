using UnityEngine;

public class SteamVR_LaserPointer : MonoBehaviour
{
	public bool active = true;

	public Color color;

	public float thickness = 0.002f;

	public GameObject holder;

	public GameObject pointer;

	private bool isActive;

	public bool addRigidBody;

	public Transform reference;

	private Transform previousContact;

	public event PointerEventHandler PointerIn;

	public event PointerEventHandler PointerOut;

	private void Start()
	{
		holder = new GameObject();
		holder.transform.parent = base.transform;
		holder.transform.localPosition = Vector3.zero;
		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pointer.transform.parent = holder.transform;
		pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
		pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		BoxCollider component = pointer.GetComponent<BoxCollider>();
		if (addRigidBody)
		{
			if ((bool)component)
			{
				component.isTrigger = true;
			}
			Rigidbody rigidbody = pointer.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
		}
		else if ((bool)component)
		{
			Object.Destroy(component);
		}
		Material material = new Material(Shader.Find("Unlit/Color"));
		material.SetColor("_Color", color);
		pointer.GetComponent<MeshRenderer>().material = material;
	}

	public virtual void OnPointerIn(PointerEventArgs e)
	{
		if (this.PointerIn != null)
		{
			this.PointerIn(this, e);
		}
	}

	public virtual void OnPointerOut(PointerEventArgs e)
	{
		if (this.PointerOut != null)
		{
			this.PointerOut(this, e);
		}
	}

	private void Update()
	{
		if (!isActive)
		{
			isActive = true;
			base.transform.GetChild(0).gameObject.SetActive(true);
		}
		float num = 100f;
		SteamVR_TrackedController component = GetComponent<SteamVR_TrackedController>();
		Ray ray = new Ray(base.transform.position, base.transform.forward);
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(ray, out hitInfo);
		if ((bool)previousContact && previousContact != hitInfo.transform)
		{
			PointerEventArgs e = default(PointerEventArgs);
			if (component != null)
			{
				e.controllerIndex = component.controllerIndex;
			}
			e.distance = 0f;
			e.flags = 0u;
			e.target = previousContact;
			OnPointerOut(e);
			previousContact = null;
		}
		if (flag && previousContact != hitInfo.transform)
		{
			PointerEventArgs e2 = default(PointerEventArgs);
			if (component != null)
			{
				e2.controllerIndex = component.controllerIndex;
			}
			e2.distance = hitInfo.distance;
			e2.flags = 0u;
			e2.target = hitInfo.transform;
			OnPointerIn(e2);
			previousContact = hitInfo.transform;
		}
		if (!flag)
		{
			previousContact = null;
		}
		if (flag && hitInfo.distance < 100f)
		{
			num = hitInfo.distance;
		}
		if (component != null && component.triggerPressed)
		{
			pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, num);
		}
		else
		{
			pointer.transform.localScale = new Vector3(thickness, thickness, num);
		}
		pointer.transform.localPosition = new Vector3(0f, 0f, num / 2f);
	}
}
