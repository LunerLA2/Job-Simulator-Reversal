using UnityEngine;

public class BotCostumeData : ScriptableObject
{
	[SerializeField]
	private Color mainScreenColor = Color.cyan;

	[SerializeField]
	private Texture mainTextureOverride;

	[SerializeField]
	[Range(0.5f, 2f)]
	private float masterScale = 1f;

	[SerializeField]
	private bool containsDynamicElements;

	[SerializeField]
	private AttachableObject glassesPrefab;

	[SerializeField]
	private AttachableObject hatPrefab;

	[SerializeField]
	private CostumePiece[] costumePieces;

	public Color MainScreenColor
	{
		get
		{
			return mainScreenColor;
		}
	}

	public Texture MainTextureOverride
	{
		get
		{
			return mainTextureOverride;
		}
	}

	public float MasterScale
	{
		get
		{
			return masterScale;
		}
	}

	public CostumePiece[] CostumePieces
	{
		get
		{
			return costumePieces;
		}
	}

	public AttachableObject GlassesPrefab
	{
		get
		{
			return glassesPrefab;
		}
	}

	public AttachableObject HatPrefab
	{
		get
		{
			return hatPrefab;
		}
	}

	public bool ContainsDynamicElements
	{
		get
		{
			return containsDynamicElements;
		}
	}

	public static BotCostumeData CreateInstance(CostumePiece[] costumeParts = null, AttachableObject glasses = null, AttachableObject hat = null, Color? screenColor = null, Texture textureOverride = null, float scale = 1f, bool hasDynamicElements = false)
	{
		BotCostumeData botCostumeData = ScriptableObject.CreateInstance<BotCostumeData>();
		botCostumeData.mainScreenColor = screenColor.GetValueOrDefault(Color.cyan);
		botCostumeData.mainTextureOverride = textureOverride;
		botCostumeData.masterScale = scale;
		botCostumeData.containsDynamicElements = hasDynamicElements;
		botCostumeData.glassesPrefab = glasses;
		botCostumeData.hatPrefab = hat;
		botCostumeData.costumePieces = costumeParts ?? new CostumePiece[0];
		return botCostumeData;
	}

	public void SetFaceColor(Color color)
	{
		mainScreenColor = color;
	}

	public void DuplicateData(BotCostumeData data)
	{
		mainScreenColor = data.MainScreenColor;
		mainTextureOverride = data.MainTextureOverride;
		masterScale = data.MasterScale;
		costumePieces = data.CostumePieces;
		glassesPrefab = data.GlassesPrefab;
		hatPrefab = data.HatPrefab;
		containsDynamicElements = data.ContainsDynamicElements;
	}
}
