using System.Collections;
using UnityEngine;

namespace SiliconTrail
{
	public abstract class Task
	{
		protected SiliconTrailComputerProgram program;

		protected Campaign campaign;

		public event TaskFinishedHandler TaskFinished;

		protected virtual IEnumerator Execute()
		{
			yield break;
		}

		private IEnumerator ExecuteWrapper()
		{
			yield return program.StartCoroutine(Execute());
			if (this.TaskFinished != null)
			{
				this.TaskFinished(this);
			}
		}

		public Coroutine Execute(SiliconTrailComputerProgram program)
		{
			this.program = program;
			campaign = program.Campaign;
			return program.StartCoroutine(ExecuteWrapper());
		}

		public virtual void OnKeyPress(string code)
		{
		}

		public virtual void OnMouseMove(Vector2 cursorPos)
		{
		}

		public virtual void OnMouseClick(Vector2 cursorPos)
		{
		}
	}
}
