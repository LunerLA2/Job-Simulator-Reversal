using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuizComputerProgram : ComputerProgram
{
	[Serializable]
	public class Question
	{
		public string QuestionText;

		public string Response0;

		public string Response1;

		[Range(0f, 1f)]
		public int CorrectIndex;
	}

	private enum QuizState
	{
		Start = 0,
		Question = 1,
		Feedback = 2,
		Debrief = 3
	}

	private const float FEEDBACK_DURATION = 2f;

	private const float GOOD_SCORE_THRESHOLD = 0.8f;

	private const float BAD_SCORE_THRESHOLD = 0.5f;

	private const float SCORE_INCREASE_SPEED = 0.2f;

	private const float MIN_DEBRIEF_TIME = 1.5f;

	[SerializeField]
	private Question[] questions;

	[SerializeField]
	private GameObject startScreen;

	[SerializeField]
	private GameObject questionScreen;

	[SerializeField]
	private GameObject debriefScreen;

	[SerializeField]
	private Text questionText;

	[SerializeField]
	private Text responseText0;

	[SerializeField]
	private Text responseText1;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Image responseImage0;

	[SerializeField]
	private Image responseImage1;

	[SerializeField]
	private Gradient scoreGradient;

	[SerializeField]
	private GameObject confetti;

	[SerializeField]
	private AudioClip correctResponseSound;

	[SerializeField]
	private AudioClip incorrectResponseSound;

	[SerializeField]
	private AudioClip drumrollSound;

	[SerializeField]
	private AudioClip goodScoreSound;

	[SerializeField]
	private AudioClip badScoreSound;

	private QuizState state;

	private float feedbackTime;

	private int questionIndex;

	private float score;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Quiz;
		}
	}

	private void OnEnable()
	{
		if (state == QuizState.Debrief)
		{
			StopAllCoroutines();
			FinalizeDebrief();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Update()
	{
		if (state != QuizState.Feedback)
		{
			return;
		}
		feedbackTime += Time.deltaTime;
		if (feedbackTime >= 2f)
		{
			feedbackTime = 0f;
			if (questionIndex < questions.Length - 1)
			{
				questionIndex++;
				AskQuestion();
			}
			else
			{
				Debrief();
			}
		}
	}

	private void StartQuiz()
	{
		StopAllCoroutines();
		startScreen.SetActive(false);
		debriefScreen.SetActive(false);
		questionScreen.SetActive(true);
		score = 0f;
		questionIndex = 0;
		AskQuestion();
	}

	private void AskQuestion()
	{
		state = QuizState.Question;
		Question question = questions[questionIndex];
		questionText.text = question.QuestionText;
		responseText0.text = question.Response0;
		responseText1.text = question.Response1;
		responseImage0.color = Color.clear;
		responseImage1.color = Color.clear;
	}

	private void AnswerQuestion(int responseIndex)
	{
		Question question = questions[questionIndex];
		Image image = ((responseIndex != 0) ? responseImage1 : responseImage0);
		if (question.CorrectIndex == responseIndex)
		{
			image.color = Color.green;
			hostComputer.PlaySound(correctResponseSound);
			score += 1f / (float)questions.Length;
		}
		else
		{
			image.color = Color.red;
			hostComputer.PlaySound(incorrectResponseSound);
		}
		state = QuizState.Feedback;
		feedbackTime = 0f;
	}

	private void Debrief()
	{
		state = QuizState.Debrief;
		StopAllCoroutines();
		StartCoroutine(DebriefAsync());
	}

	private IEnumerator DebriefAsync()
	{
		questionScreen.SetActive(false);
		debriefScreen.SetActive(true);
		hostComputer.PlaySound(drumrollSound);
		float runningScore = 0f;
		float debriefTime = 0f;
		while (runningScore < score || debriefTime <= 1.5f)
		{
			if (runningScore < score)
			{
				runningScore = Mathf.Min(runningScore + Time.deltaTime * 0.2f, score);
				scoreText.text = Mathf.RoundToInt(runningScore * 100f) + "%";
				scoreText.color = scoreGradient.Evaluate(runningScore);
			}
			debriefTime += Time.deltaTime;
			yield return null;
		}
		if (score >= 0.8f)
		{
			hostComputer.PlaySound(goodScoreSound);
		}
		else if (score < 0.5f)
		{
			hostComputer.PlaySound(badScoreSound);
		}
		FinalizeDebrief();
	}

	private void FinalizeDebrief()
	{
		if (score >= 0.8f)
		{
			confetti.SetActive(true);
		}
		scoreText.text = Mathf.RoundToInt(score * 100f) + "%";
		scoreText.color = scoreGradient.Evaluate(score);
	}

	protected override bool OnKeyPress(string code)
	{
		if (state == QuizState.Question)
		{
			if (code == "0")
			{
				AnswerQuestion(0);
			}
			else if (code == "1")
			{
				AnswerQuestion(1);
			}
		}
		return true;
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable.name == "Start")
			{
				StartQuiz();
			}
			else if (clickable.name == "0")
			{
				AnswerQuestion(0);
			}
			else if (clickable.name == "1")
			{
				AnswerQuestion(1);
			}
			else if (clickable.name == "X" || clickable.name == "Quit")
			{
				Finish();
			}
			else if (clickable.name == "Retake")
			{
				StartQuiz();
			}
		}
	}
}
