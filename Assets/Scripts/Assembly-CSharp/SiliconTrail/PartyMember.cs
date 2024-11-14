using System.Collections.Generic;
using UnityEngine;

namespace SiliconTrail
{
	public class PartyMember
	{
		private const int INITIAL_HEALTH = 1024;

		public string Name;

		public int Health = 1024;

		private HashSet<Virus> viruses = new HashSet<Virus>();

		public int NumViruses
		{
			get
			{
				return viruses.Count;
			}
		}

		public bool IsDead
		{
			get
			{
				return Health <= 0;
			}
		}

		public bool IsAlive
		{
			get
			{
				return Health > 0;
			}
		}

		public void IncrementHealth(int increment)
		{
			Health = Mathf.Min(Health + increment, 1024);
		}

		public void DecrementHealth(int decrement)
		{
			Health = Mathf.Max(Health - decrement, 0);
		}

		public bool HasVirus(Virus virus)
		{
			return viruses.Contains(virus);
		}

		public void CatchVirus(Virus virus)
		{
			if (!viruses.Contains(virus))
			{
				viruses.Add(virus);
			}
		}

		public void RemoveVirus(Virus virus)
		{
			viruses.Remove(virus);
		}

		public void ClearViruses()
		{
			viruses.Clear();
		}

		public void TakeViralDamage()
		{
			foreach (Virus virus in viruses)
			{
				DecrementHealth(virus.DailyDamage);
			}
		}
	}
}
