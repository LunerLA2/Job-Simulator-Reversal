using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class JobScoreBoard : MonoBehaviour
{
	private const int MAX_SCOREBOARD_SCORE = 9999999;

	private const string promotionText = "You Got Promoted to:\n\n{0}";

	private const float delayBeforShowingPromotionText = 4f;

	[SerializeField]
	private TextMeshPro HighScore;

	[SerializeField]
	private TextMeshPro CurrScore;

	[SerializeField]
	[Space]
	private AudioSourceHelper audioSrcHelper2d;

	[SerializeField]
	private AudioClip TickerClip;

	[SerializeField]
	private AudioClip FireWorkClip;

	[SerializeField]
	private AudioClip CelebrateClip;

	[SerializeField]
	private AudioClip[] promotionAudioClips;

	[SerializeField]
	private ParticleSystem Firework1;

	[SerializeField]
	private ParticleSystem Firework2;

	[SerializeField]
	private ParticleSystem BigFireWork;

	[SerializeField]
	private Animator Anim;

	private bool HasHighScored;

	private JobBoardManager jobManager;

	private int sessionScore;

	[SerializeField]
	private TextMeshPro rankText;

	private void Start()
	{
		HighScore.text = string.Empty;
		CurrScore.text = string.Empty;
		UpdateScoreImmediate();
	}

	private void OnEnable()
	{
		jobManager = JobBoardManager.instance;
		JobBoardManager jobBoardManager = jobManager;
		jobBoardManager.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(jobBoardManager.OnTaskComplete, new Action<TaskStatusController>(OnTaskCompleted));
	}

	private void OnDisable()
	{
		if (jobManager.isActiveAndEnabled)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskComplete, new Action<TaskStatusController>(OnTaskCompleted));
		}
	}

	private void OnTaskCompleted(TaskStatusController taskController)
	{
		if (!jobManager.EndlessModeStatusController.GetCurrentGoal().IsSkipped)
		{
			sessionScore++;
		}
		UpdateScores();
	}

	public void UpdateScores()
	{
		StopAllCoroutines();
		StartCoroutine(UpdateScoresSeq());
	}

	private IEnumerator UpdateScoresSeq()
	{
		yield return new WaitForSeconds(1f);
		if (CurrScore.text == string.Empty)
		{
			CurrScore.text = "0";
		}
		if (HighScore.text == string.Empty)
		{
			HighScore.text = "0";
		}
		if (GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks() == 0)
		{
			HasHighScored = true;
		}
		if (HighScore.text != string.Empty + Mathf.Clamp(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks(), 0, 9999999) && HighScore.text != string.Empty)
		{
			Anim.SetTrigger("TickedBest");
			AudioManager.Instance.Play(base.transform.position, TickerClip, 0.85f, 1f);
			StartCoroutine(Promotion());
			HasHighScored = true;
		}
		UpdateScoreImmediate();
	}

	private void UpdateScoreImmediate()
	{
		if (jobManager == null || jobManager.EndlessModeStatusController == null)
		{
			Invoke("UpdateScoreImmediate", 0.01f);
			return;
		}
		CurrScore.text = string.Empty + Mathf.Clamp(sessionScore, 0, 9999999);
		HighScore.text = string.Empty + Mathf.Clamp(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks(), 0, 9999999);
	}

	private IEnumerator FireWorks()
	{
		if (jobManager.EndlessModeStatusController.Score % 10 == 0)
		{
			yield return new WaitForSeconds(0.2f);
			AudioManager.Instance.Play(base.transform.position, CelebrateClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.4f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			Firework1.Play();
			yield return new WaitForSeconds(0.4f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			Firework2.Play();
		}
	}

	private IEnumerator BigFirework()
	{
		if (jobManager.EndlessModeStatusController.JobStateData.LongestShift % 20 == 0 && jobManager.EndlessModeStatusController.JobStateData.LongestShift != 0)
		{
			BigFireWork.Play();
			yield return new WaitForSeconds(0.6f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.2f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.1f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.2f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			AudioManager.Instance.Play(base.transform.position, CelebrateClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.1f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
		}
	}

	private IEnumerator Promotion()
	{
		if (jobManager.EndlessModeStatusController.ShouldGetPromotion())
		{
			StartCoroutine(RankTextUpdate());
			Firework1.Play();
			yield return new WaitForSeconds(0.2f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			yield return new WaitForSeconds(0.4f);
			AudioManager.Instance.Play(base.transform.position, FireWorkClip, 0.85f, 1f);
			Firework2.Play();
			PlayPromotionSound();
		}
	}

	private IEnumerator RankTextUpdate()
	{
		yield return new WaitForSeconds(4f);
		rankText.text = string.Format("You Got Promoted to:\n\n{0}", jobManager.EndlessModeStatusController.RankName);
		rankText.gameObject.SetActive(true);
		yield return new WaitForSeconds(jobManager.EndlessModeStatusController.Data.SecsOfBlankWhenPromoting);
		rankText.gameObject.SetActive(false);
	}

	private void PlayPromotionSound()
	{
		if (promotionAudioClips.Length > 0)
		{
			int score = JobBoardManager.instance.EndlessModeStatusController.Score;
			int secondaryRank = PromotionRankNameGenerator.GetSecondaryRank(score);
			secondaryRank %= promotionAudioClips.Length;
			audioSrcHelper2d.SetClip(promotionAudioClips[secondaryRank]);
			audioSrcHelper2d.Play();
		}
	}
}
