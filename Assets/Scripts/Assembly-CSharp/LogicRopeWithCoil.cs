using UnityEngine;

public class LogicRopeWithCoil : MonoBehaviour
{
	public UltimateRope Rope;

	public float RopeExtensionSpeed;

	public Transform handle;

	private float m_fRopeExtension;

	private void Start()
	{
		m_fRopeExtension = ((!(Rope != null)) ? 0f : Rope.m_fCurrentExtension);
	}

	private void OnGUI()
	{
		LogicGlobal.GlobalGUI();
		GUILayout.Label("Rope test (Procedural rope with additional coil)");
		GUILayout.Label("Use the keys i and o to extend the rope");
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.O))
		{
			m_fRopeExtension += Time.deltaTime * RopeExtensionSpeed;
		}
		if (Input.GetKey(KeyCode.I))
		{
			m_fRopeExtension -= Time.deltaTime * RopeExtensionSpeed;
		}
		if (Rope != null)
		{
			m_fRopeExtension = Mathf.Clamp(m_fRopeExtension, 0f, Rope.ExtensibleLength);
			Rope.ExtendRope(UltimateRope.ERopeExtensionMode.LinearExtensionIncrement, m_fRopeExtension - Rope.m_fCurrentExtension);
		}
		Debug.Log("handle height: " + handle.position.y);
	}
}
