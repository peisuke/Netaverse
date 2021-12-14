using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace Tutorial {
public class SettingCanvasManager : MonoBehaviour
{
    public static  SettingCanvasManager instance;
    public GameObject panel;
    private bool isVisible;
	public TMP_InputField inputName;

    void Start()
    {
		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
        isVisible = false;
        panel.SetActive(isVisible);
    }

    public void Initialize()
    {
        isVisible = false;
        panel.SetActive(isVisible);
    }

    public void OnLogin()
    {        
        isVisible = false;
        panel.SetActive(isVisible);
    }

    public void hide()
    {
        isVisible = false;
        panel.SetActive(isVisible);        
    }

    // Update is called once per frame
    public void OnClickSetting()
    {
        isVisible = !isVisible;
        panel.SetActive(isVisible);
    }
    
	[System.Serializable]
	public class SetUserParameter : CallbackParameter
	{
		public string nickName;
	}

    public void OnChangeName()
    {
        string newName = inputName.text;

#if UNITY_WEBGL && !UNITY_EDITOR
        var callbackParameter = new SetUserParameter
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "GetCurrentName",
            nickName = newName
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

        jscall.Execute("SetUsername", parameterJson);
#else
        ChangedName(newName);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void GetCurrentName()
    {
        var callbackParameter = new CallbackParameter
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "ChangedName"
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

        jscall.Execute("GetUsername", parameterJson);
    }
#endif

    public void OnDeleteAccount()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var callbackParameter = new CallbackParameter
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "Logout"
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

        jscall.Execute("RemoveUsername", parameterJson);
#else
        Logout();
#endif
    }

    public void ChangedName(string newName)
    {
        Debug.Log(newName);
        PhotonNetwork.LocalPlayer.NickName = newName;

        hide();
    }

    public void Logout()
    {
        hide();
        PhotonNetwork.LeaveRoom();
    }
}
}