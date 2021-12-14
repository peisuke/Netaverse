using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tutorial{
public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	public Canvas pLobby;
	public TMP_InputField inputLogin;
	public GameObject lobbyCamera;
	public GameObject connectPanel;
	public GameObject inputPanel;
	public GameObject loadingPanel;
	public int currentMenu;

	[System.Serializable]
	public class SetUserParameter : CallbackParameter
	{
		public string nickName;
	}

	// Use this for initialization
	void Start () {
		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
			OpenScreen(0);
		}
		else
		{
			Destroy(this.gameObject);
		}
		Initialize();
	}

	public void Initialize() {	
		connectPanel.SetActive(true);	
		loadingPanel.SetActive(false);
		inputPanel.SetActive(false);

		OpenScreen(0);
	}

	public void OnClickConnect () {
#if UNITY_WEBGL && !UNITY_EDITOR
		var callbackParameter = new CallbackParameter
		{
			callbackGameObjectName = gameObject.name,
			callbackFunctionName = "OnConnected"
		};
		var parameterJson = JsonUtility.ToJson(callbackParameter);
		JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

		jscall.Execute("Connect", parameterJson);
#else
		OnConnected(-1);
#endif
	}

	void OnConnected(int isConnected) {
#if UNITY_WEBGL && !UNITY_EDITOR
		if (isConnected >= 0) {
			//	名前の取得チャレンジ
			connectPanel.SetActive(false);
			loadingPanel.SetActive(true);
			inputPanel.SetActive(false);

			var callbackParameter = new CallbackParameter
			{
				callbackGameObjectName = gameObject.name,
				callbackFunctionName = "Join"
			};
			var parameterJson = JsonUtility.ToJson(callbackParameter);
			JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

			jscall.Execute("GetUsername", parameterJson);
		}
#else
		connectPanel.SetActive(false);
		loadingPanel.SetActive(false);
		inputPanel.SetActive(true);
#endif
	}

	public void Join(string name) {
		if (name.Length == 0) {
			connectPanel.SetActive(false);
			loadingPanel.SetActive(false);
			inputPanel.SetActive(true);
		} else {
			inputLogin.text = name;
			NetworkManager mgr = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
			mgr.EmitJoin();
		}
	}

	public void Register() {
		string name = inputLogin.text;

#if UNITY_WEBGL && !UNITY_EDITOR
		var callbackParameter = new SetUserParameter
		{
			callbackGameObjectName = gameObject.name,
			callbackFunctionName = "OnConnected",
			nickName = name
		};
		var parameterJson = JsonUtility.ToJson(callbackParameter);
		JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

		jscall.Execute("SetUsername", parameterJson);
#else
		Join(name);
#endif
	}

	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(int _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case 0:
			currentMenu = _current;
			pLobby.enabled = true;
			lobbyCamera.GetComponent<Camera> ().enabled = true;
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
			pLobby.enabled = false;
			lobbyCamera.GetComponent<Camera> ().enabled = false;
			break;

		}

	}

}
}