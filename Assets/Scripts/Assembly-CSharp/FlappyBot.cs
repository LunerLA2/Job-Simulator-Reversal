using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBot : MonoBehaviour
{
	private const float FLAP_UP_SPEED = 100f;

	private const float FLAP_COOLDOWN_DURATION = 0.15f;

	private const float DEATH_UP_SPEED = 100f;

	private const float DEATH_DURATION = 1.25f;

	[SerializeField]
	private AudioClip flapSound;

	[SerializeField]
	private AudioClip dieSound;

	private Collider2D col;

	private Rigidbody2D rb;

	private FlappyBotState state;

	private float flapCooldown;

	private HashSet<Collider2D> nonLethals;

	[NonSerialized]
	public ComputerController HostComputer;

	public FlappyBotState State
	{
		get
		{
			return state;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();
		nonLethals = new HashSet<Collider2D>();
	}

	private void Start()
	{
		state = FlappyBotState.Waiting;
		rb.isKinematic = true;
	}

	public void AddNonLethal(Collider2D nonLethal)
	{
		nonLethals.Add(nonLethal);
	}

	public void BeginFlapping()
	{
		state = FlappyBotState.Flapping;
		rb.isKinematic = false;
		Flap();
	}

	public void Flap()
	{
		if (state == FlappyBotState.Flapping && flapCooldown <= 0f)
		{
			HostComputer.PlaySound(flapSound, 1f, Mathf.Clamp(1f + base.transform.localPosition.y * 0.0027f, 0f, 3f));
			rb.velocity = Vector2.up * 100f;
			flapCooldown = 0.15f;
		}
	}

	private void Update()
	{
		if (state == FlappyBotState.Flapping)
		{
			base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, Mathf.Max(-15f, rb.velocity.y * 0.5f)), Time.deltaTime * 10f);
		}
		if (flapCooldown > 0f)
		{
			flapCooldown = Mathf.Max(0f, flapCooldown - Time.deltaTime);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (state == FlappyBotState.Flapping && !nonLethals.Contains(collision.collider))
		{
			StartCoroutine(DieAsync());
		}
	}

	private IEnumerator DieAsync()
	{
		state = FlappyBotState.Dead;
		HostComputer.PlaySound(dieSound);
		rb.velocity = Vector2.up * 100f;
		rb.angularVelocity = 300f;
		col.enabled = false;
		yield return new WaitForSeconds(1.25f);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
