﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Events;
using TechnomediaLabs;

namespace Zetcil
{
	public class DataSyncronize : MonoBehaviour
	{

        [Space(10)]
        public bool isEnabled;

        [Header("Config Settings")]
        public VarConfig SessionConfig;
        public VarString SessionName;
		
        [Header("HighScore Settings")]
        public VarString ConnectURL;
		public string RequestOutput;
		string SubmitURL;
		string UserName;
		string LevelName;
		string SubLevel;
		string scoreURL;
		string starURL;
		string lockURL;

		string _scoreURL;
		string _starURL;
		string _lockURL;

		string ScorePathFile;
		string StarPathFile;
		string StatusPathFile;
		
		string LocalScore;
		string LocalStar;
		string LocalLock;
		UnityWebRequest scoreRequest, starRequest, lockRequest;
		
		bool isDownload = true;

        public string GetDirectory(string aDirectoryName)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + aDirectoryName + "/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + aDirectoryName + "/");
            }
            return Application.persistentDataPath + "/" + aDirectoryName + "/";
        }

        public void InvokeSyncronization()
        {
            StartCoroutine(StartSyncronize());
        }
		
		IEnumerator StartSyncronize()
		{
			void BeginSyncronizeChunk(string UserName, string LevelName, string SubLevel)
			{
				scoreURL = "/gameuser/get_scorestatus/"+UserName+"/"+LevelName+"."+SubLevel;
				starURL = "/gameuser/get_starstatus/"+UserName+"/"+LevelName+"."+SubLevel;
				lockURL = "/gameuser/get_lockstatus/"+UserName+"/"+LevelName+"."+SubLevel;

				_scoreURL = "/gameuser/set_scorestatus/"+UserName+"/"+LevelName+"."+SubLevel;
				_starURL = "/gameuser/set_starstatus/"+UserName+"/"+LevelName+"."+SubLevel;
				_lockURL = "/gameuser/set_lockstatus/"+UserName+"/"+LevelName+"."+SubLevel;
			}
			
			void EndSyncronizeChunk(string UserName, string LevelName, string SubLevel)
			{
				if (scoreRequest.isNetworkError)
				{
					Debug.Log(scoreRequest.downloadHandler.text);
				}
				else
				{
					RequestOutput = scoreRequest.downloadHandler.text;
				}
				if (RequestOutput != "")
				{
					isDownload = true;
					//======================================================================================== DOWNLOAD
					ScorePathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Score.xml";
					StarPathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Star.xml";
					StatusPathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Status.xml";
					
					SaveXML(ScorePathFile, scoreRequest.downloadHandler.text);
					SaveXML(StarPathFile, starRequest.downloadHandler.text);
					SaveXML(StatusPathFile, lockRequest.downloadHandler.text);
				}	
				else 
				{
					isDownload = false;
					//======================================================================================== UPLOAD
					ScorePathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Score.xml";
					StarPathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Star.xml";
					StatusPathFile = SessionConfig.GetDataSessionDirectory() + UserName + "." + LevelName + "." + SubLevel + "." + "Status.xml";
					
					LocalScore = LoadXML(ScorePathFile);
					LocalStar = LoadXML(StarPathFile);
					LocalLock = LoadXML(StatusPathFile);
					
					if (LocalScore !=""){

						SubmitURL = ConnectURL.CurrentValue + _scoreURL + "/" + LocalScore;
						scoreRequest = UnityWebRequest.Get(SubmitURL);
						scoreRequest.SendWebRequest();
						
						SubmitURL = ConnectURL.CurrentValue + _starURL + "/" + LocalStar;
						starRequest = UnityWebRequest.Get(SubmitURL);
						starRequest.SendWebRequest();
						
						SubmitURL = ConnectURL.CurrentValue + _lockURL + "/" + LocalLock;
						lockRequest = UnityWebRequest.Get(SubmitURL);
						lockRequest.SendWebRequest();
					}
					
				}
			}
			
			string LoadXML(string FullPathFile)
			{
				string result = "";
				
				if (File.Exists(FullPathFile))
				{
					string tempxml = System.IO.File.ReadAllText(FullPathFile);
	
					XmlDocument xmldoc;
					XmlNodeList xmlnodelist;
					XmlNode xmlnode;
					xmldoc = new XmlDocument();
					xmldoc.LoadXml(tempxml);

					xmlnodelist = xmldoc.GetElementsByTagName("SessionValue");
					result = xmlnodelist.Item(0).InnerText.Trim();
				}
				
				return result;
			}			
			
			void SaveXML(string FullPathFile, string aSessionValue)
			{
				string header = "<SessionData>\n";
				string footer = "</SessionData>";
				string result = "";

				string singleValue = SessionConfig.SetXMLValueSingle("SessionValue", aSessionValue);
				result = header + singleValue + footer;
				
				string DirName = SessionConfig.GetDataSessionDirectory();
				var sr = File.CreateText(FullPathFile);
				sr.WriteLine(result);
				sr.Flush();
				sr.Close();
			}			
			
			char GetChar(int aIndex)
			{
				return (char)aIndex;
			}
			
			for (int x = 65; x <= 71; x++)
			{
			
			for (int i=1; i<=10; i++){
				if (i == 10){
					BeginSyncronizeChunk(SessionName.CurrentValue, "LV"+GetChar(x), "Play" + i.ToString());
				} else {
					BeginSyncronizeChunk(SessionName.CurrentValue, "LV"+GetChar(x), "Play0" + i.ToString());
				}
				
					SubmitURL = ConnectURL.CurrentValue + scoreURL;
					scoreRequest = UnityWebRequest.Get(SubmitURL);
					yield return scoreRequest.SendWebRequest();
				
					SubmitURL = ConnectURL.CurrentValue + starURL;
					starRequest = UnityWebRequest.Get(SubmitURL);
					yield return starRequest.SendWebRequest();
				
					SubmitURL = ConnectURL.CurrentValue + lockURL;
					lockRequest = UnityWebRequest.Get(SubmitURL);
					yield return lockRequest.SendWebRequest();
					
				if (i == 10){
					EndSyncronizeChunk(SessionName.CurrentValue, "LV"+GetChar(x), "Play"+ i.ToString());
				} else {
					EndSyncronizeChunk(SessionName.CurrentValue, "LV"+GetChar(x), "Play0"+ i.ToString());
				}
			}
			
			}
			
		}
		
		// Start is called before the first frame update
		void Start()
		{
			ConnectURL = ConnectURL.Replace("/connect", "");
		}
	
		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
