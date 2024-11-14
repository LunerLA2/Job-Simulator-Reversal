using System.Collections;
using UnityEngine;

public class WorkerBotController : MonoBehaviour
{
	private const float PEEK_HEIGHT = 1.9f;

	private const float DEFAULT_HEIGHT = 1f;

	[SerializeField]
	private bool hideBotDuringIdle = true;

	private bool stopPeekingOnBotHit;

	[SerializeField]
	private bool voOnCubicleHit;

	[SerializeField]
	private CustomBot customBot;

	[SerializeField]
	private Vector3 idleLocalRotation;

	[SerializeField]
	private AudioClip hitImpactClip;

	private AudioClip[] hitVoClip;

	private ElementSequence<AudioClip> hitVoClipSequence;

	[SerializeField]
	private ParticleSystem hitParticles;

	private int hitCount;

	private bool botHit;

	private bool inTransit;

	private float awokeTime;

	private float lastHitTime;

	private Rigidbody myRigidbody;

	private float timeRequiredBetweenHits = 1.5f;

	private float myRandomFloatForBobing;

	private OfficeManager manager;

	private void Start()
	{
		manager = OfficeManager.Instance;
		awokeTime = Time.realtimeSinceStartup;
		myRigidbody = GetComponent<Rigidbody>();
		myRandomFloatForBobing = Random.Range(0f, 0.2f);
		if (!hideBotDuringIdle)
		{
			customBot.Appear();
			customBot.LookAt(BrainEffect.LookAtTypes.WorldAngle, string.Empty, idleLocalRotation.y);
		}
	}

	public void SetVoClipArray(AudioClip[] voClips)
	{
		hitVoClip = voClips;
		hitVoClipSequence = new ElementSequence<AudioClip>(hitVoClip);
	}

	public void SetVOMode(bool mode)
	{
		voOnCubicleHit = mode;
	}

	public void Setup(bool hideWhileIdle, bool voOnHit, Vector3 rot)
	{
		hideBotDuringIdle = hideWhileIdle;
		voOnCubicleHit = voOnHit;
		idleLocalRotation = rot;
	}

	public void SetCostume(BotCostumeData costume)
	{
		customBot.SetDefaultCostume(costume);
	}

	private void Update()
	{
		if (!inTransit && !hideBotDuringIdle)
		{
			Bob();
		}
	}

	private void Bob()
	{
		float num = Mathf.PingPong(Time.time / 20f, myRandomFloatForBobing);
		customBot.FloatingHeight(BrainEffect.FloatHeightTypes.WorldHeight, 1f + num, string.Empty);
	}

	public void Peek()
	{
		if (!(manager == null) && manager.currentAdjacentWorkerBots < manager.currentAdjacentWorkerBots + 1 && (!botHit || !stopPeekingOnBotHit) && !inTransit)
		{
			StartCoroutine(PeekingTween());
		}
	}

	private IEnumerator PeekingTween()
	{
		manager.currentAdjacentWorkerBots++;
		Go.killAllTweensWithTarget(base.transform);
		inTransit = true;
		if (hideBotDuringIdle)
		{
			customBot.Appear();
		}
		customBot.LookAt(BrainEffect.LookAtTypes.Player, string.Empty);
		customBot.FloatingHeight(BrainEffect.FloatHeightTypes.WorldHeight, 1.9f, string.Empty);
		yield return new WaitForSeconds(3.5f);
		customBot.FloatingHeight(BrainEffect.FloatHeightTypes.WorldHeight, 1f, string.Empty);
		yield return new WaitForSeconds(2f);
		customBot.LookAt(BrainEffect.LookAtTypes.WorldAngle, string.Empty, idleLocalRotation.y);
		customBot.Emote(BotFaceEmote.Idle);
		if (hideBotDuringIdle)
		{
			customBot.Disappear();
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
			{
				manager.currentJanitor = null;
			}
		}
		inTransit = false;
		manager.currentAdjacentWorkerBots--;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.attachedRigidbody == myRigidbody || col.attachedRigidbody == null || Time.realtimeSinceStartup - awokeTime < 1f || (col.attachedRigidbody != null && col.attachedRigidbody.GetComponent<Bot>() != null))
		{
			return;
		}
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
		{
			if (!(manager != null) || !(manager.currentJanitor == null))
			{
				return;
			}
			manager.currentJanitor = this;
		}
		if (lastHitTime + timeRequiredBetweenHits < Time.time)
		{
			lastHitTime = Time.time;
			hitCount++;
			if (!inTransit)
			{
				Peek();
			}
			if (voOnCubicleHit)
			{
				StartCoroutine(botHitVO());
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.rigidbody == myRigidbody || Time.realtimeSinceStartup - awokeTime < 1f)
		{
			return;
		}
		hitParticles.Play();
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
		{
			if (!(manager != null) || !(manager.currentJanitor == null))
			{
				return;
			}
			manager.currentJanitor = this;
		}
		if (lastHitTime + timeRequiredBetweenHits < Time.time)
		{
			lastHitTime = Time.time;
			botHit = true;
			hitCount++;
			Debug.Log("Bot Hit: " + col.rigidbody.name + " HitCount:" + hitCount + " this: " + base.name);
			if (voOnCubicleHit)
			{
				StartCoroutine(botHitVO());
			}
			if (!inTransit)
			{
				Peek();
			}
		}
	}

	private IEnumerator botHitVO()
	{
		yield return new WaitForSeconds(0.25f);
		if (hitVoClipSequence != null && customBot.IsActive)
		{
			customBot.Emote(BotFaceEmote.Angry);
			customBot.PlayVO(hitVoClipSequence.GetNext(), BotVoiceController.VOImportance.OverrideOnlySelf);
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			Color gray = Color.gray;
			gray.a = 0.2f;
			Gizmos.color = gray;
			Gizmos.DrawSphere(1.9f * Vector3.up, 0.1f);
		}
	}
}
