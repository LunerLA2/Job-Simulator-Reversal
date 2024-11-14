using UnityEngine;

public class BakeOutParticles : MonoBehaviour
{
	public ParticleSystem m_System;

	public ParticleSystem.Particle[] m_Particles;

	private int numParticlesAlive;

	public GameObject particlePrefab;

	private void Start()
	{
		m_System.Play();
	}

	private void LateUpdate()
	{
		InitializeIfNeeded();
		if (Input.GetKeyDown(KeyCode.A))
		{
			Bakeparticles();
			m_System.Stop();
			m_System.Clear();
		}
	}

	private void Bakeparticles()
	{
		numParticlesAlive = m_System.GetParticles(m_Particles);
		for (int i = 0; i < numParticlesAlive; i++)
		{
			GameObject gameObject = Object.Instantiate(particlePrefab);
			gameObject.transform.position = m_Particles[i].position;
			gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position, Vector3.up);
		}
	}

	private void InitializeIfNeeded()
	{
		if (m_System == null)
		{
			m_System = GetComponent<ParticleSystem>();
		}
		if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
		{
			m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
		}
	}
}
