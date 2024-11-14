using System.Collections.Generic;
using UnityEngine;

namespace SiliconTrail
{
	public class Campaign
	{
		private const int NUM_PARTY_MEMBERS = 4;

		private const int NUM_PLACES = 32;

		private const int INITIAL_DAYS = 32;

		private const int INITIAL_MONEY = 32;

		private const int INITIAL_FOOD = 32;

		private const int DAILY_FOOD_CONSUMED_PER_MEMBER = 1;

		private const int STARVATION_DAMAGE = 64;

		private PartyMember[] partyMembers;

		private PartyMember[] livingPartyMembersYesterday;

		private PartyMember[] newDeadPartyMembers;

		private Place[] possiblePlaces;

		private Virus[] possibleViruses;

		private Place[] places;

		private int initialPartyHealth;

		private int daysLeft;

		private int food;

		private int money;

		private int placeIndex;

		public PartyMember[] PartyMembers
		{
			get
			{
				return partyMembers;
			}
		}

		public int NumPartyMembers
		{
			get
			{
				return 4;
			}
		}

		public int NumPlaces
		{
			get
			{
				return 32;
			}
		}

		public PartyMember[] LivingPartyMembers
		{
			get
			{
				return GetLivingPartyMembers();
			}
		}

		public int NumLivingPartyMembers
		{
			get
			{
				return LivingPartyMembers.Length;
			}
		}

		public int DaysLeft
		{
			get
			{
				return daysLeft;
			}
		}

		public int Food
		{
			get
			{
				return food;
			}
		}

		public int Money
		{
			get
			{
				return money;
			}
		}

		public Place[] Places
		{
			get
			{
				return places;
			}
		}

		public int PlaceIndex
		{
			get
			{
				return placeIndex;
			}
		}

		public Place CurrentPlace
		{
			get
			{
				return places[placeIndex];
			}
		}

		private PartyMember[] GetLivingPartyMembers()
		{
			List<PartyMember> list = new List<PartyMember>();
			for (int i = 0; i < partyMembers.Length; i++)
			{
				if (partyMembers[i].IsAlive)
				{
					list.Add(partyMembers[i]);
				}
			}
			return list.ToArray();
		}

		public void Reset()
		{
			partyMembers = new PartyMember[4]
			{
				new PartyMember
				{
					Name = "Happy_Bot"
				},
				new PartyMember
				{
					Name = "Nervous_Bot"
				},
				new PartyMember
				{
					Name = "Quiet_Bot"
				},
				new PartyMember
				{
					Name = "Crazy_Bot"
				}
			};
			livingPartyMembersYesterday = partyMembers;
			possiblePlaces = new Place[5]
			{
				new Place
				{
					Name = string.Empty,
					ProbabilityWeight = 1f,
					PossibleEvents = new CampaignEvent[6]
					{
						new VirusEvent
						{
							ProbabilityWeight = 0.5f
						},
						new PirateEvent
						{
							ProbabilityWeight = 0.5f
						},
						new DriverUpdateEvent
						{
							ProbabilityWeight = 0.5f
						},
						new CPUBoostEvent
						{
							ProbabilityWeight = 0.25f
						},
						new BitFlipEvent
						{
							ProbabilityWeight = 0.25f
						},
						new UneventfulEvent
						{
							ProbabilityWeight = 1f
						}
					}
				},
				new Place
				{
					Name = "Firewall",
					ProbabilityWeight = 0.2f,
					PossibleEvents = new CampaignEvent[1]
					{
						new FirewallEvent()
					}
				},
				new Place
				{
					Name = "Torrent",
					ProbabilityWeight = 0.2f,
					PossibleEvents = new CampaignEvent[1]
					{
						new TorrentEvent()
					}
				},
				new Place
				{
					Name = "AntiVirus",
					ProbabilityWeight = 0.2f,
					PossibleEvents = new CampaignEvent[1]
					{
						new AntiVirusEvent()
					}
				},
				new Place
				{
					Name = "Database",
					ProbabilityWeight = 0.2f,
					PossibleEvents = new CampaignEvent[1]
					{
						new DatabaseEvent()
					}
				}
			};
			possibleViruses = new Virus[3]
			{
				new Virus
				{
					Message = "{0} has some Malware.",
					ProbabilityWeight = 1f,
					DailyDamage = 64
				},
				new Virus
				{
					Message = "{0} got a Trojan Horse.",
					ProbabilityWeight = 0.5f,
					DailyDamage = 128
				},
				new Virus
				{
					Message = "{0} caught the Y2K virus.",
					ProbabilityWeight = 0.25f,
					DailyDamage = 512
				}
			};
			initialPartyHealth = GetPartyHealthSum();
			daysLeft = 32;
			food = 32;
			money = 32;
			GeneratePlaces();
			placeIndex = 0;
		}

		private int GetPartyHealthSum()
		{
			int num = 0;
			for (int i = 0; i < partyMembers.Length; i++)
			{
				num += partyMembers[i].Health;
			}
			return num;
		}

		private void GeneratePlaces()
		{
			places = new Place[32];
			places[0] = new Place
			{
				Name = "Start",
				PossibleEvents = new CampaignEvent[1]
				{
					new IntroEvent()
				}
			};
			for (int i = 1; i < 31; i++)
			{
				places[i] = SelectRandomPlace();
			}
			places[31] = new Place
			{
				Name = "Internet",
				PossibleEvents = new CampaignEvent[1]
				{
					new EndingEvent
					{
						EndingCondition = EndingCondition.Win
					}
				}
			};
		}

		public void SetDays(int days)
		{
			daysLeft = days;
		}

		public void IncrementDays(int increment)
		{
			daysLeft += increment;
		}

		public void DecrementDays(int decrement)
		{
			daysLeft = Mathf.Max(0, daysLeft - decrement);
		}

		public void SetPlace(int placeIndex)
		{
			this.placeIndex = placeIndex;
		}

		public void IncrementPlace(int increment)
		{
			placeIndex = Mathf.Min(placeIndex + increment, 31);
		}

		public void DecrementPlace(int decrement)
		{
			placeIndex = Mathf.Max(placeIndex - decrement, 0);
		}

		public void SetMoney(int money)
		{
			this.money = money;
		}

		public void IncrementMoney(int increment)
		{
			money += increment;
		}

		public void DecrementMoney(int decrement)
		{
			money = Mathf.Max(0, money - decrement);
		}

		public void SetFood(int food)
		{
			this.food = food;
		}

		public void IncrementFood(int increment)
		{
			food += increment;
		}

		public void DecrementFood(int decrement)
		{
			food = Mathf.Max(0, food - decrement);
		}

		public PartyMember GetPartyMember(int i)
		{
			return partyMembers[i];
		}

		public PartyMember[] GetNewDeadPartyMembers()
		{
			return newDeadPartyMembers;
		}

		public PartyHealth CalculatePartyHealth()
		{
			float num = (float)GetPartyHealthSum() / (float)initialPartyHealth;
			if (num > 0.75f)
			{
				return PartyHealth.Great;
			}
			if (num > 0.5f)
			{
				return PartyHealth.Good;
			}
			if (num > 0.25f)
			{
				return PartyHealth.Poor;
			}
			if (num > 0f)
			{
				return PartyHealth.Critical;
			}
			return PartyHealth.Dead;
		}

		public void ResolveDay()
		{
			PartyMember[] livingPartyMembers = LivingPartyMembers;
			foreach (PartyMember partyMember in livingPartyMembers)
			{
				partyMember.TakeViralDamage();
				if (food == 0)
				{
					partyMember.DecrementHealth(64);
				}
			}
			DecrementFood(NumLivingPartyMembers * 1);
			Dictionary<PartyMember, bool> dictionary = new Dictionary<PartyMember, bool>();
			for (int j = 0; j < LivingPartyMembers.Length; j++)
			{
				dictionary.Add(LivingPartyMembers[j], false);
				if (dictionary.ContainsKey(LivingPartyMembers[j]))
				{
					dictionary[LivingPartyMembers[j]] = true;
				}
				else
				{
					Debug.Log("Yesterday's list doesn't contain this party member from today's list: " + LivingPartyMembers[j]);
				}
			}
			List<PartyMember> list = new List<PartyMember>(dictionary.Keys);
			List<PartyMember> list2 = new List<PartyMember>();
			for (int k = 0; k < dictionary.Keys.Count; k++)
			{
				if (!dictionary[list[k]])
				{
					list2.Add(list[k]);
				}
			}
			newDeadPartyMembers = list2.ToArray();
			livingPartyMembersYesterday = LivingPartyMembers;
			DecrementDays(1);
		}

		public PartyMember SelectRandomPartyMember()
		{
			return partyMembers[Random.Range(0, partyMembers.Length)];
		}

		public PartyMember SelectRandomLivingPartyMember()
		{
			PartyMember[] livingPartyMembers = LivingPartyMembers;
			return livingPartyMembers[Random.Range(0, livingPartyMembers.Length)];
		}

		public Place SelectRandomPlace()
		{
			return possiblePlaces.SelectRandom();
		}

		public Virus SelectRandomVirus()
		{
			return possibleViruses.SelectRandom();
		}
	}
}
