using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BotVOProfileData", menuName = "Bot VO Profile Data")]
public class BotVOProfileData : ScriptableObject
{
	[Serializable]
	public class VOLine
	{
		[SerializeField]
		public AudioClip clip;

		[SerializeField]
		private BotVOInfoData info;

		public VOLine()
		{
		}

		public VOLine(AudioClip _clip)
		{
			clip = _clip;
		}

		public VOLine(BotVOInfoData _info)
		{
			info = _info;
		}

		public VOContainer GetVOContainer(BotVoiceController.VOImportance importance = BotVoiceController.VOImportance.OverrideOnlySelf)
		{
			if (info != null && info.AudioClip != null)
			{
				return new VOContainer(info, importance);
			}
			if (clip != null)
			{
				return new VOContainer(clip, importance);
			}
			return null;
		}
	}

	[Serializable]
	public class VOPair
	{
		public VOLine prefixLine;

		public VOLine suffixLine;
	}

	[Serializable]
	public class VOClause
	{
		public enum RelationToSubject
		{
			before = 0,
			after = 1
		}

		public RelationToSubject relationToSubject;

		public VOPair pair;
	}

	[Serializable]
	public class VOPairGroup
	{
		public PageData[] pageDataReferences;

		public VOPair[] introPairs;

		public VOPair[] requestPairs;

		public VOClause[] requestClauses;

		public VOPair[] successPairs;

		public VOPair[] skipPairs;

		public VOPair[] failurePairs;
	}

	[SerializeField]
	private BotVoiceType voiceType;

	private Dictionary<List<int>, int> currentVOLine;

	[SerializeField]
	private VOLine[] introLines;

	private List<int> introLinesIndices;

	[SerializeField]
	private VOPairGroup[] pageSpecificLines;

	private List<List<int>> introPairsIndicies;

	private List<List<int>> requestPairsIndicies;

	private List<List<int>> requestClausesIndicies;

	private List<List<int>> successPairsIndicies;

	private List<List<int>> skipPairsIndicies;

	private List<List<int>> failurePairsIndicies;

	[SerializeField]
	private VOLine[] outroLines;

	private List<int> outroLinesIndices;

	[SerializeField]
	private VOLine[] taskSuccessLines;

	private List<int> taskSuccessLinesIndices;

	[SerializeField]
	private VOLine[] taskSkipLines;

	private List<int> taskSkipLinesIndices;

	[SerializeField]
	private VOLine[] pageCompleteLines;

	private List<int> pageCompleteLinesIndices;

	[SerializeField]
	private VOLine[] sentenceConjunctionLines;

	private List<int> sentenceConjunctionLinesIndices;

	[SerializeField]
	private VOLine[] finalSentenceConjunctionLines;

	private List<int> finalSentenceConjunctionLinesIndices;

	[SerializeField]
	private VOLine[] clauseConjunctionLines;

	private List<int> clauseConjunctionLinesIndices;

	[SerializeField]
	private BotCostumeData optionalCustomCostumeForCharacter;

	public BotVoiceType VoiceType
	{
		get
		{
			return voiceType;
		}
	}

	public BotCostumeData Costume
	{
		get
		{
			return optionalCustomCostumeForCharacter;
		}
	}

	public void Init()
	{
		introLinesIndices = InitIndices(introLines);
		outroLinesIndices = InitIndices(outroLines);
		taskSuccessLinesIndices = InitIndices(taskSuccessLines);
		taskSkipLinesIndices = InitIndices(taskSkipLines);
		pageCompleteLinesIndices = InitIndices(pageCompleteLines);
		sentenceConjunctionLinesIndices = InitIndices(sentenceConjunctionLines);
		finalSentenceConjunctionLinesIndices = InitIndices(finalSentenceConjunctionLines);
		clauseConjunctionLinesIndices = InitIndices(clauseConjunctionLines);
		currentVOLine = new Dictionary<List<int>, int>
		{
			{ introLinesIndices, 0 },
			{ outroLinesIndices, 0 },
			{ taskSuccessLinesIndices, 0 },
			{ taskSkipLinesIndices, 0 },
			{ pageCompleteLinesIndices, 0 },
			{ sentenceConjunctionLinesIndices, 0 },
			{ finalSentenceConjunctionLinesIndices, 0 },
			{ clauseConjunctionLinesIndices, 0 }
		};
		InitPageSpecificIndices();
	}

	private List<int> InitIndices(Array a)
	{
		List<int> list = Enumerable.Range(0, a.Length).ToList();
		int num = UnityEngine.Random.Range(1, 5);
		for (int i = 0; i < num; i++)
		{
			list.Shuffle();
		}
		return list;
	}

	private void InitPageSpecificIndices()
	{
		introPairsIndicies = new List<List<int>>();
		requestPairsIndicies = new List<List<int>>();
		successPairsIndicies = new List<List<int>>();
		skipPairsIndicies = new List<List<int>>();
		failurePairsIndicies = new List<List<int>>();
		requestClausesIndicies = new List<List<int>>();
		for (int i = 0; i < pageSpecificLines.Length; i++)
		{
			InitPairIndices(introPairsIndicies, pageSpecificLines[i].introPairs);
			InitPairIndices(requestPairsIndicies, pageSpecificLines[i].requestPairs);
			InitPairIndices(successPairsIndicies, pageSpecificLines[i].successPairs);
			InitPairIndices(skipPairsIndicies, pageSpecificLines[i].skipPairs);
			InitPairIndices(failurePairsIndicies, pageSpecificLines[i].failurePairs);
			InitPairIndices(requestClausesIndicies, pageSpecificLines[i].requestClauses);
		}
	}

	private void InitPairIndices(List<List<int>> pairsIndices, Array a)
	{
		List<int> list = Enumerable.Range(0, a.Length).ToList();
		pairsIndices.Add(list);
		currentVOLine.Add(list, 0);
		int num = UnityEngine.Random.Range(1, 5);
		for (int i = 0; i < num; i++)
		{
			list.Shuffle();
		}
	}

	private void ResetIndex(List<int> indicesList)
	{
		indicesList.Shuffle();
		currentVOLine[indicesList] = 0;
	}

	private int GetNextIndex(List<int> indicesList)
	{
		int result = indicesList[currentVOLine[indicesList]];
		Dictionary<List<int>, int> dictionary;
		Dictionary<List<int>, int> dictionary2 = (dictionary = currentVOLine);
		List<int> key;
		List<int> key2 = (key = indicesList);
		int num = dictionary[key];
		dictionary2[key2] = num + 1;
		if (currentVOLine[indicesList] >= indicesList.Count)
		{
			ResetIndex(indicesList);
		}
		return result;
	}

	public VOPair GetRandomIntroVOPairForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].introPairs.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].introPairs[GetNextIndex(introPairsIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public VOPair GetRandomRequestVOPairForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].requestPairs.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].requestPairs[GetNextIndex(requestPairsIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public VOClause GetRandomRequestVOClauseForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].requestClauses.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].requestClauses[GetNextIndex(requestClausesIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public VOPair GetRandomSuccessVOPairForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].successPairs.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].successPairs[GetNextIndex(successPairsIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public VOPair GetRandomSkipVOPairForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].skipPairs.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].skipPairs[GetNextIndex(skipPairsIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public VOPair GetRandomFailureVOPairForPage(string pageDataName)
	{
		int vOPairGroupIndexForPage = GetVOPairGroupIndexForPage(pageDataName);
		if (vOPairGroupIndexForPage != -1 && pageSpecificLines[vOPairGroupIndexForPage].failurePairs.Length > 0)
		{
			return pageSpecificLines[vOPairGroupIndexForPage].failurePairs[GetNextIndex(failurePairsIndicies[vOPairGroupIndexForPage])];
		}
		return null;
	}

	public int GetVOPairGroupIndexForPage(string pageDataName)
	{
		for (int i = 0; i < pageSpecificLines.Length; i++)
		{
			if (pageSpecificLines[i].pageDataReferences == null)
			{
				continue;
			}
			for (int j = 0; j < pageSpecificLines[i].pageDataReferences.Length; j++)
			{
				if (pageSpecificLines[i].pageDataReferences[j].name == pageDataName)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public VOLine GetRandomIntro()
	{
		return GetNextVOLine(introLines, introLinesIndices);
	}

	public VOLine GetRandomOutro(float chance = 0.5f)
	{
		return GetNextVOLine(outroLines, outroLinesIndices, chance);
	}

	public VOLine GetRandomSuccessLine()
	{
		return GetNextVOLine(taskSuccessLines, taskSuccessLinesIndices);
	}

	public VOLine GetRandomSkipLine()
	{
		return GetNextVOLine(taskSkipLines, taskSkipLinesIndices);
	}

	public VOLine GetRandomPageCompleteLine(float chance = 0.5f)
	{
		return GetNextVOLine(pageCompleteLines, pageCompleteLinesIndices, chance);
	}

	public VOLine GetRandomSentenceConjunctionLine()
	{
		return GetNextVOLine(sentenceConjunctionLines, sentenceConjunctionLinesIndices);
	}

	public VOLine GetRandomFinalSentenceConjunctionLine()
	{
		return GetNextVOLine(finalSentenceConjunctionLines, finalSentenceConjunctionLinesIndices);
	}

	public VOLine GetRandomClauseConjunctionLine()
	{
		return GetNextVOLine(clauseConjunctionLines, clauseConjunctionLinesIndices);
	}

	private VOLine GetNextVOLine(VOLine[] lines, List<int> indices, float chance = 1f)
	{
		if (lines != null && lines.Length > 0 && UnityEngine.Random.Range(0f, 1f) <= chance)
		{
			return lines[GetNextIndex(indices)];
		}
		return new VOLine();
	}
}
