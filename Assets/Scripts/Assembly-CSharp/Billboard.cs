using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Camera m_Camera;

	public bool amActive;

	public bool autoInit;

	private GameObject myContainer;

	private void Awake()
	{
		if (autoInit)
		{
			m_Camera = Camera.main;
			amActive = true;
		}
		myContainer = new GameObject();
		myContainer.name = "GRP_" + base.transform.gameObject.name;
		myContainer.transform.position = base.transform.position;
		base.transform.parent = myContainer.transform;
	}

	private void Update()
	{
		if (amActive)
		{
			myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
		}
	}
}
