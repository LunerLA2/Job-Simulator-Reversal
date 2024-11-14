using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace OwlchemyTests
{
	public class Test_SaveBug : MonoBehaviour
	{
		private const string FileSavePath = "C://Users/kode80/Desktop/TestSave/Test.save";

		private const string NonSaveFilePath = "C://Users/kode80/Desktop/TestSave/NotASaveFile.txt";

		private const string CorruptedSaveFilePath = "C://Users/kode80/Desktop/TestSave/CorruptedSave.save";

		private const string NullFieldSaveFilePath = "C://Users/kode80/Desktop/TestSave/NullFieldSave.save";

		private const string CoroutineCancelSaveFilePath = "C://Users/kode80/Desktop/TestSave/CoroutineCancelSave.save";

		internal const string SaveLoad_Inputkey = "560A11CD-6346-3CF0-C2E8-671F9B2B9EA1";

		internal const string SaveLoad_Salt = "Lots of text goes here in this place";

		private static RijndaelManaged SaveLoad_crypto;

		private void Start()
		{
			TestSaveLoad();
		}

		private void Update()
		{
		}

		private void TestKeyBlockSizes()
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			for (int i = 0; i < 1000; i++)
			{
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				hashSet.Add(rijndaelManaged.KeySize);
				hashSet2.Add(rijndaelManaged.BlockSize);
			}
			Debug.Log("=== Testing key/block sizes for " + 1000 + " instantiations ===");
			Debug.Log("Total unique key sizes:" + hashSet.Count);
			Debug.Log("Total unique block sizes:" + hashSet.Count);
		}

		private void TestKeyAndIVAreConsistant()
		{
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			for (int i = 0; i < 1000; i++)
			{
				RijndaelManaged rijndaelManaged = SaveLoad_NewRijndaelManaged();
				hashSet.Add(Encoding.ASCII.GetString(rijndaelManaged.Key));
				hashSet2.Add(Encoding.ASCII.GetString(rijndaelManaged.IV));
			}
			Debug.Log("=== Testing key/IV generation for " + 1000 + " instantiations ===");
			Debug.Log("Total unique keys:" + hashSet.Count);
			Debug.Log("Total unique IVs:" + hashSet2.Count);
		}

		private void TestSaveLoad()
		{
			SaveStateData data = BuildFullSaveState();
			SaveLoad_SaveAsXML("C://Users/kode80/Desktop/TestSave/Test.save", data, true);
			SaveStateData saveStateData = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/Test.save", true);
		}

		private void TestNonSaveFileLoad()
		{
			File.WriteAllText("C://Users/kode80/Desktop/TestSave/NotASaveFile.txt", "This is not a save file", Encoding.UTF8);
			SaveStateData saveStateData = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/NotASaveFile.txt", true);
		}

		private void TestCorruptedLoad()
		{
			SaveStateData data = BuildFullSaveState();
			SaveLoad_SaveAsXML("C://Users/kode80/Desktop/TestSave/CorruptedSave.save", data, true);
			byte[] array = File.ReadAllBytes("C://Users/kode80/Desktop/TestSave/CorruptedSave.save");
			for (int i = 1; i < 10; i++)
			{
				int num = array.Length - i;
				array[num] ^= byte.MaxValue;
			}
			File.WriteAllBytes("C://Users/kode80/Desktop/TestSave/CorruptedSave.save", array);
			SaveStateData saveStateData = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/CorruptedSave.save", true);
		}

		private void TestSaveStateDataNullsLoad()
		{
			SaveStateData saveStateData = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/NullFieldSave.save", true);
			SaveStateData saveStateData2 = new SaveStateData();
			List<JobSaveData> list = new List<JobSaveData>();
			JobSaveData jobSaveData = new JobSaveData();
			jobSaveData.SetID("TestJob\nBlah");
			List<TaskSaveData> list2 = new List<TaskSaveData>();
			TaskSaveData taskSaveData = new TaskSaveData();
			taskSaveData.SetID("\"\\>");
			CustomActionSaveData customActionSaveData = new CustomActionSaveData();
			customActionSaveData.SetCustomActionData("CustomActionasd", "custom data \f asdasdasd");
			taskSaveData.SetCustomData(customActionSaveData);
			list2.Add(taskSaveData);
			jobSaveData.SetTasks(list2);
			list.Add(jobSaveData);
			list.Add(null);
			list.Add(null);
			list.Add(null);
			list.Add(jobSaveData);
			saveStateData2.SetJobsData(list);
			SaveLoad_SaveAsXML("C://Users/kode80/Desktop/TestSave/NullFieldSave.save", saveStateData2, true);
			SaveStateData saveStateData3 = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/NullFieldSave.save", true);
		}

		private void Update_TestCoroutineCancel()
		{
			StartCoroutine("LoadCoroutine");
			StartCoroutine("SaveCoroutine");
			StartCoroutine("CancelSaveCoroutine");
		}

		private float RandomDelay()
		{
			return Random.Range(0.001f, 0.1f);
		}

		private IEnumerator LoadCoroutine()
		{
			yield return new WaitForSeconds(RandomDelay());
			Debug.Log("LoadCoroutine...");
			SaveStateData previousRunLoadedData = SaveLoad_LoadAsXML("C://Users/kode80/Desktop/TestSave/CoroutineCancelSave.save", true);
		}

		private IEnumerator SaveCoroutine()
		{
			yield return new WaitForSeconds(RandomDelay());
			Debug.Log("SaveCoroutine saving...");
			SaveStateData data = BuildFullSaveState();
			SaveLoad_SaveAsXML("C://Users/kode80/Desktop/TestSave/CoroutineCancelSave.save", data, true);
		}

		private IEnumerator CancelSaveCoroutine()
		{
			yield return new WaitForSeconds(RandomDelay());
			Debug.Log("CancelSaveCoroutine...");
			StopAllCoroutines();
		}

		private SaveStateData BuildFullSaveState()
		{
			SaveStateData saveStateData = new SaveStateData();
			GameStateData gameStateData = GameStateController_BuildGameState();
			List<JobSaveData> list = new List<JobSaveData>();
			for (int i = 0; i < gameStateData.JobsData.Count; i++)
			{
				JobStateData jobStateData = gameStateData.JobsData[i];
				JobSaveData jobSaveData = new JobSaveData();
				jobSaveData.SetID(jobStateData.ID);
				List<TaskSaveData> list2 = new List<TaskSaveData>();
				for (int j = 0; j < jobStateData.TasksData.Count; j++)
				{
					TaskStateData taskStateData = jobStateData.TasksData[j];
					TaskSaveData taskSaveData = new TaskSaveData();
					taskSaveData.SetID(taskStateData.ID);
					list2.Add(taskSaveData);
				}
				jobSaveData.SetTasks(list2);
				list.Add(jobSaveData);
			}
			saveStateData.SetJobsData(list);
			saveStateData.SetHasSeenGameComplete(gameStateData.HasSeenGameComplete);
			return saveStateData;
		}

		private static void SaveLoad_SaveAsXML(string filePath, SaveStateData data, bool applyEncyption)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveStateData));
			string directoryName = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			Stream stream = new FileStream(filePath, FileMode.Create);
			Stream stream2;
			if (applyEncyption)
			{
				if (SaveLoad_crypto == null)
				{
					SaveLoad_crypto = SaveLoad_NewRijndaelManaged();
				}
				stream2 = new CryptoStream(stream, SaveLoad_crypto.CreateEncryptor(), CryptoStreamMode.Write);
			}
			else
			{
				stream2 = stream;
			}
			using (StreamWriter textWriter = new StreamWriter(stream2, Encoding.UTF8))
			{
				xmlSerializer.Serialize(textWriter, data);
			}
			stream2.Close();
			stream.Close();
		}

		private static SaveStateData SaveLoad_LoadAsXML(string filePath, bool useEncyption)
		{
			SaveStateData result = new SaveStateData();
			if (File.Exists(filePath))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveStateData));
				Stream stream = new FileStream(filePath, FileMode.Open);
				Stream stream2;
				if (useEncyption)
				{
					if (SaveLoad_crypto == null)
					{
						SaveLoad_crypto = SaveLoad_NewRijndaelManaged();
					}
					stream2 = new CryptoStream(stream, SaveLoad_crypto.CreateDecryptor(), CryptoStreamMode.Read);
				}
				else
				{
					stream2 = stream;
				}
				result = xmlSerializer.Deserialize(stream2) as SaveStateData;
				stream2.Close();
				stream.Close();
				Debug.Log("Stream!");
			}
			return result;
		}

		private static RijndaelManaged SaveLoad_NewRijndaelManaged()
		{
			byte[] bytes = Encoding.ASCII.GetBytes("Lots of text goes here in this place");
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes("560A11CD-6346-3CF0-C2E8-671F9B2B9EA1", bytes);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
			rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
			return rijndaelManaged;
		}

		private static GameStateData GameStateController_BuildGameState()
		{
			GameStateData gameStateData = new GameStateData();
			List<JobStateData> list = new List<JobStateData>();
			JobLevelsListData jobLevelsListData = Resources.Load<JobLevelsListData>("Data/JobLevelsList/JobLevelsListData");
			for (int i = 0; i < jobLevelsListData.ActiveJobs.Count; i++)
			{
				if (jobLevelsListData.ActiveJobs[i] != null)
				{
					JobLevelData jobLevelData = jobLevelsListData.ActiveJobs[i];
					List<TaskStateData> list2 = new List<TaskStateData>();
					for (int j = 0; j < jobLevelData.JobData.Tasks.Count; j++)
					{
						TaskStateData item = new TaskStateData(jobLevelData.JobData.Tasks[j].ID);
						list2.Add(item);
					}
					JobStateData item2 = new JobStateData(jobLevelData, list2, 0);
					list.Add(item2);
				}
			}
			gameStateData.SetJobStateData(list);
			gameStateData.SetHasSeenGameComplete(false);
			return gameStateData;
		}
	}
}
