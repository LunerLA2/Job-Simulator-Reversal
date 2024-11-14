using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class SaveLoad
{
	private const string SAVESTATE_XMLSTRING_PLAYER_PREFS_KEY = "SaveStateXMLString";

	private const string XML_EXTENSION = ".xml";

	private const string ENCRYPTED_EXTENSION = ".save";

	private const string SAVE_FILENAME = "JobSimulatorSave";

	private const string TESTING_SAVE_FILENAME = "TestJobSimulatorSave";

	private const string STANDALONE_SAVE_FOLDER_NAME = "User";

	private const string EDITOR_SAVE_FOLDER_NAME = "Editor";

	private const string TEMP_SAVE_FOLDER_NAME = "Temp";

	private const string BACKUP_COMPLETED_SAVE_GAME = "Backup";

	internal const string Inputkey = "560A11CD-6346-3CF0-C2E8-671F9B2B9EA1";

	internal const string Salt = "Lots of text goes here in this place";

	private static bool isEncypted = !Application.isEditor;

	private static RijndaelManaged crypto = null;

	public static bool IsEncypted
	{
		get
		{
			return isEncypted;
		}
		set
		{
			if (Application.isEditor)
			{
				isEncypted = value;
			}
		}
	}

	public static string GetCurrentFilePath(string fileNameAppend)
	{
		return BuildSaveFilePath(GetFullSaveFileName(fileNameAppend));
	}

	public static string GetTestingCurrentFilePath()
	{
		return BuildSaveFilePath(GetFullTestingSaveFileName());
	}

	public static string GetCompletedGameSaveCurrentFilePath(string fileNameAppendment)
	{
		return BuildSaveFilePath("Backup/JobSimulatorSave" + fileNameAppendment + GetCurrentExtension());
	}

	public static string GetTestingCurrentFilePathForceUnencrypted()
	{
		return BuildSaveFilePath(GetUnencyptedFullTestingSaveFileName());
	}

	public static string GetTempUnencryptedSaveFilePath()
	{
		string text = Application.persistentDataPath + "/";
		text += "Temp/";
		return text + "JobSimulatorSave.xml";
	}

	public static string GetTempEncryptedSaveFilePath()
	{
		string text = Application.persistentDataPath + "/";
		text += "Temp/";
		return text + "JobSimulatorSave.save";
	}

	public static string GetFullSaveFileName(string fileNameAppend)
	{
		return "JobSimulatorSave" + fileNameAppend + GetCurrentExtension();
	}

	public static string GetFullTestingSaveFileName()
	{
		return "TestJobSimulatorSave" + GetCurrentExtension();
	}

	public static string GetUnencyptedFullSaveFileName()
	{
		return "JobSimulatorSave.xml";
	}

	public static string GetEncryptedFullSaveFileName()
	{
		return "JobSimulatorSave.save";
	}

	public static string GetUnencyptedFullTestingSaveFileName()
	{
		return "TestJobSimulatorSave.xml";
	}

	private static string BuildSaveFilePath(string fileNameWithExtension)
	{
		string text = Application.persistentDataPath + "/";
		text += "User/";
		return text + fileNameWithExtension;
	}

	private static string GetCurrentExtension()
	{
		if (isEncypted)
		{
			return ".save";
		}
		return ".xml";
	}

	public static void SaveToPlayerPrefs(SaveStateData saveStateData)
	{
		SaveStatePlayerPrefsSerializer.Save(saveStateData);
	}

	public static SaveStateData LoadFromPlayerPrefs(GameStateData protoGameState)
	{
		return SaveStatePlayerPrefsSerializer.Load(protoGameState);
	}

	public static void Save(SaveStateData data)
	{
		Save(GetCurrentFilePath(string.Empty), data);
	}

	public static void Save(string filePath, SaveStateData data)
	{
		SaveAsXML(filePath, data, isEncypted);
	}

	private static void SaveAsXML(string filePath, SaveStateData data, bool applyEncyption)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveStateData));
		string directoryName = Path.GetDirectoryName(filePath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (applyEncyption && crypto == null)
		{
			crypto = NewRijndaelManaged();
		}
		using (Stream stream = new FileStream(filePath, FileMode.Create))
		{
			if (applyEncyption)
			{
				using (CryptoStream stream2 = new CryptoStream(stream, crypto.CreateEncryptor(), CryptoStreamMode.Write))
				{
					using (StreamWriter textWriter = new StreamWriter(stream2, Encoding.UTF8))
					{
						xmlSerializer.Serialize(textWriter, data);
						return;
					}
				}
			}
			using (StreamWriter textWriter2 = new StreamWriter(stream, Encoding.UTF8))
			{
				xmlSerializer.Serialize(textWriter2, data);
			}
		}
	}

	public static SaveStateData Load()
	{
		Debug.Log(GetCurrentFilePath(string.Empty));
		return LoadFullPath(GetCurrentFilePath(string.Empty));
	}

	public static SaveStateData LoadFullPath(string fullFilePath)
	{
		Debug.Log("Save:" + fullFilePath);
		return LoadAsXML(fullFilePath, isEncypted);
	}

	private static SaveStateData LoadAsXML(string filePath, bool useEncyption)
	{
		SaveStateData result = new SaveStateData();
		if (File.Exists(filePath))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveStateData));
			if (useEncyption && crypto == null)
			{
				crypto = NewRijndaelManaged();
			}
			using (Stream stream = new FileStream(filePath, FileMode.Open))
			{
				if (useEncyption)
				{
					using (CryptoStream stream2 = new CryptoStream(stream, crypto.CreateDecryptor(), CryptoStreamMode.Read))
					{
						result = xmlSerializer.Deserialize(stream2) as SaveStateData;
					}
				}
				else
				{
					result = xmlSerializer.Deserialize(stream) as SaveStateData;
				}
			}
		}
		return result;
	}

	public static bool DoesSaveGameExist()
	{
		return File.Exists(GetCurrentFilePath(string.Empty));
	}

	public static string GetFullPathWithoutExtension(string path)
	{
		return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
	}

	public static void TestingDecryptSaveFile(string currFilePath)
	{
		if (File.Exists(currFilePath))
		{
			SaveStateData saveStateData = LoadAsXML(currFilePath, true);
			if (saveStateData != null)
			{
				SaveAsXML("Decrypt-" + currFilePath, saveStateData, false);
			}
			else
			{
				Debug.LogError("Unable to load existing save file");
			}
		}
		else
		{
			Debug.LogError("No save file found, unable to load");
		}
	}

	public static void TestingEncryptSaveFile(string currFilePath)
	{
	}

	public static void DeleteExistingTempUnencryptedSave()
	{
		string tempUnencryptedSaveFilePath = GetTempUnencryptedSaveFilePath();
		if (File.Exists(tempUnencryptedSaveFilePath))
		{
			File.Delete(tempUnencryptedSaveFilePath);
		}
	}

	public static void WriteTestingSaveDirectlyFromEncryptedSave(byte[] bytes)
	{
		string tempEncryptedSaveFilePath = GetTempEncryptedSaveFilePath();
		File.WriteAllBytes(tempEncryptedSaveFilePath, bytes);
		SaveStateData data = LoadAsXML(tempEncryptedSaveFilePath, true);
		string testingCurrentFilePathForceUnencrypted = GetTestingCurrentFilePathForceUnencrypted();
		Save(testingCurrentFilePathForceUnencrypted, data);
		File.Delete(tempEncryptedSaveFilePath);
	}

	public static void WriteTestingSaveDirectly(string text)
	{
		string testingCurrentFilePathForceUnencrypted = GetTestingCurrentFilePathForceUnencrypted();
		string directoryName = Path.GetDirectoryName(testingCurrentFilePathForceUnencrypted);
		if (!Directory.Exists(directoryName))
		{
			Debug.Log("Path did not exists");
			Directory.CreateDirectory(directoryName);
		}
		else
		{
			Debug.Log("Path did exist:" + directoryName);
		}
		StreamWriter streamWriter = new StreamWriter(GetTestingCurrentFilePathForceUnencrypted());
		streamWriter.Write(text);
		streamWriter.Close();
		if (IsEncypted)
		{
			SaveStateData saveStateData = LoadAsXML(GetTestingCurrentFilePathForceUnencrypted(), false);
			if (saveStateData != null)
			{
				Save(GetTestingCurrentFilePath(), saveStateData);
			}
			else
			{
				Debug.LogError("Error in loading xml");
			}
			File.Delete(GetTestingCurrentFilePathForceUnencrypted());
		}
	}

	public static SaveStateData LoadTestingSave()
	{
		return LoadFullPath(GetTestingCurrentFilePath());
	}

	public static void Delete()
	{
		string currentFilePath = GetCurrentFilePath(string.Empty);
		string directoryName = Path.GetDirectoryName(currentFilePath);
		if (Directory.Exists(directoryName))
		{
			Directory.Delete(Path.GetDirectoryName(currentFilePath), true);
		}
	}

	private static RijndaelManaged NewRijndaelManaged()
	{
		byte[] bytes = Encoding.ASCII.GetBytes("Lots of text goes here in this place");
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes("560A11CD-6346-3CF0-C2E8-671F9B2B9EA1", bytes);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		return rijndaelManaged;
	}
}
