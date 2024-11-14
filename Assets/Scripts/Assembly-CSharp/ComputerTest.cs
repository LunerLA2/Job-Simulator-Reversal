using UnityEngine;

public class ComputerTest : MonoBehaviour
{
	[SerializeField]
	private ComputerController computer;

	[SerializeField]
	private ComputerProgramID programToTest;

	private void Update()
	{
		if (computer.State == ComputerState.On && programToTest != 0)
		{
			computer.StartProgram(programToTest);
			Object.Destroy(base.gameObject);
		}
	}
}
