using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using UnityEngine;

namespace TSW.Loca
{
	public class DirtyLoca
	{
		public static void Load(string langDefinitionFileName)
		{
			_instance._Load(langDefinitionFileName);
		}
		public static event System.Action OnLanguageChanged;
		public static string GetTextValue(string key)
		{
			return _instance._GetValue(key);
		}
		public static string GetValue(string key)
		{
			return _instance._GetValue(key);
		}
		public static void UseLanguage(string code)
		{
			_instance._UseLanguage(code);
		}
		public static string CurrentLanguageCode { get; private set; } = string.Empty;
		public static string[] AvailableLanguageCode => _instance._AvailableLanguageCode;

		private readonly Dictionary<string, string> _resxByLangCode = new Dictionary<string, string>();
		private readonly DirtyResxReader _reader = new DirtyResxReader();
		private Dictionary<string, string> _keyValueDict = null;
		private static readonly DirtyLoca _instance = new DirtyLoca();
		private string[] _AvailableLanguageCode => _resxByLangCode.Keys.ToArray();


		private void _Load(string langDefinitionFileName)
		{
			TextAsset txt = Resources.Load<TextAsset>(GetResourceName(langDefinitionFileName));
			if (txt == null)
			{
				Debug.LogError($"language definition {langDefinitionFileName} not found");
				return;
			}
			using (MemoryStream memStream = new MemoryStream(txt.bytes))
			{
				StreamReader reader = new StreamReader(memStream);
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					string[] tokens = line.Split(':');
					if (tokens.Length != 2)
					{
						throw new System.Exception($"Invalid token number on line:{line} file:{langDefinitionFileName}");
					}
					_resxByLangCode[tokens[0]] = tokens[1];
				}
			}
			LoadLanguage("en");

		}

		public static string GetResourceName(string name)
		{
			return $"LanguageData/{name}";
		}

		private string _GetValue(string key)
		{
			if (null == _keyValueDict)
			{
				return "..loca error..";
			}

			string result;
			if (_keyValueDict.TryGetValue(key, out result))
			{
				return result;
			}
			return "..loca not found..";
		}

		private void _UseLanguage(string code)
		{
			CurrentLanguageCode = code;
			LoadLanguage(code);
			OnLanguageChanged?.Invoke();
		}

		private void LoadLanguage(string code)
		{
			string resxFileName;
			if (!_resxByLangCode.TryGetValue(code, out resxFileName))
			{
				Debug.LogError($"No language found for code {code}");
				return;
			}
			try
			{
				_reader.Read(GetResourceName(resxFileName));
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Fail to load language {code} {ex.Message}");
				return;
			}
			_keyValueDict = _reader.Data;
		}
	}

	internal class DirtyResxReader
	{
		public Dictionary<string, string> Data
		{
			get
			{
				if (!_readComplete)
				{
					return null;
				}

				return _keyValueDict;
			}
		}
		private Dictionary<string, string> _keyValueDict;
		private bool _readComplete = false;

		public void Read(string fileName)
		{
			_readComplete = false;
			_keyValueDict = new Dictionary<string, string>();
			TextAsset txt = Resources.Load<TextAsset>(fileName);
			if (txt == null)
			{
				throw new System.Exception($"Could not load resource {fileName}");
			}
			using (MemoryStream memStream = new MemoryStream(txt.bytes))
			{
				using (XmlReader reader = XmlReader.Create(memStream))
				{
					string keyName = string.Empty;
					while (reader.Read())
					{
						if (reader.IsStartElement())
						{
							if (reader.Name == "data")
							{
								keyName = reader.GetAttribute("name");
							}
							if (reader.Name == "value" && !string.IsNullOrEmpty(keyName))
							{
								if (reader.Read())
								{
									_keyValueDict[keyName] = reader.Value;
									keyName = string.Empty;
								}
							}
						}
					}
				}
			}
			_readComplete = true;
		}
	}
}