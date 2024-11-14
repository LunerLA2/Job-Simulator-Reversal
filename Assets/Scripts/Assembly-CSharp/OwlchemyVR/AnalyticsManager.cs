using System.Collections.Generic;
using UnityEngine.Analytics;

namespace OwlchemyVR
{
	public static class AnalyticsManager
	{
		public static readonly bool ANALYTICS_ENABLED = true;

		private static Dictionary<string, object> tempDictionary = new Dictionary<string, object>();

		public static void CustomEvent(string eventName, string eventDataName, string eventDataValue)
		{
			CustomEvent(eventName, eventDataName, (object)eventDataValue);
		}

		public static void CustomEvent(string eventName, string eventDataName, int eventDataValue)
		{
			CustomEvent(eventName, eventDataName, (object)eventDataValue);
		}

		public static void CustomEvent(string eventName, string eventDataName, float eventDataValue)
		{
			CustomEvent(eventName, eventDataName, (object)eventDataValue);
		}

		public static void CustomEvent(string eventName, string eventDataName, bool eventDataValue)
		{
			CustomEvent(eventName, eventDataName, (object)eventDataValue);
		}

		private static void CustomEvent(string eventName, string eventDataName, object eventDataValue)
		{
			if (ANALYTICS_ENABLED)
			{
				tempDictionary.Add(eventDataName, eventDataValue);
				Analytics.CustomEvent(eventName, tempDictionary);
				tempDictionary.Remove(eventDataName);
			}
		}

		public static void CustomEvent(string eventName, IDictionary<string, object> eventData)
		{
			if (ANALYTICS_ENABLED)
			{
				Analytics.CustomEvent(eventName, eventData);
			}
		}
	}
}
