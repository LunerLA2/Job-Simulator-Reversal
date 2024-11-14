using UnityEngine;

public class FPSObjectShooter : MonoBehaviour
{
	public GameObject Element;

	public float InitialSpeed = 1f;

	public float MouseSpeed = 0.3f;

	public float Scale = 1f;

	public float Mass = 1f;

	public float Life = 10f;

	private Vector3 m_v3MousePosition;

	private void Start()
	{
		m_v3MousePosition = Input.mousePosition;
	}

	private void Update()
	{
		if (Element != null && Input.GetKeyDown(KeyCode.Space))
		{
			GameObject gameObject = Object.Instantiate(Element);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
			gameObject.GetComponent<Rigidbody>().mass = Mass;
			gameObject.GetComponent<Rigidbody>().solverIterations = 255;
			gameObject.GetComponent<Rigidbody>().AddForce(base.transform.forward * InitialSpeed, ForceMode.VelocityChange);
			DieTimer dieTimer = gameObject.AddComponent<DieTimer>();
			dieTimer.SecondsToDie = Life;
		}
		if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
		{
			base.transform.Rotate((0f - (Input.mousePosition.y - m_v3MousePosition.y)) * MouseSpeed, 0f, 0f);
			base.transform.RotateAround(base.transform.position, Vector3.up, (Input.mousePosition.x - m_v3MousePosition.x) * MouseSpeed);
		}
		m_v3MousePosition = Input.mousePosition;
	}
}
