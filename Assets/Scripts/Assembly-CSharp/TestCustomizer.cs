using UnityEngine;

public class TestCustomizer : ObjectCustomizer<TestPreset>
{
	[SerializeField]
	private MeshFilter meshFilter;

	public override void Customize(TestPreset preset)
	{
		meshFilter.mesh = preset.Mesh;
	}
}
