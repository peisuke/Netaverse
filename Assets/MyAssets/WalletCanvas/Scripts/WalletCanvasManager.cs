using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace Tutorial{

public static class GameObjectExtensions
{
    /// <summary>
    /// 深い階層まで子オブジェクトを名前で検索して GameObject 型で取得します
    /// </summary>
    /// <param name="self">GameObject 型のインスタンス</param>
    /// <param name="name">検索するオブジェクトの名前</param>
    /// <param name="includeInactive">非アクティブなオブジェクトも検索する場合 true</param>
    /// <returns>子オブジェクト</returns>
    public static GameObject FindDeep( 
        this GameObject self, 
        string name, 
        bool includeInactive = false )
    {
        var children = self.GetComponentsInChildren<Transform>( includeInactive );
        foreach ( var transform in children )
        {
            if ( transform.name == name )
            {
                return transform.gameObject;
            }
        }
        return null;
    }
}


public class WalletCanvasManager : MonoBehaviour
{
    public static  WalletCanvasManager instance;

    int IsVisible;

    public GameObject panelObject;
    public GameObject buttonObject;
    public GameObject transferPanel;
    public GameObject swapPanel;
    
    [SerializeField] private TMP_Dropdown dropdownObject;

	// Use this for initialization
	void Start () {
		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
        Initialize();
    }

    public void Initialize()
    {
        IsVisible = -1;
        gameObject.SetActive(false);
    }

    public void SetPanel() {
        if (dropdownObject.captionText.text == "Transfer") {    
            transferPanel.SetActive(true);        
            swapPanel.SetActive(false);        
        } else {            
            transferPanel.SetActive(false);        
            swapPanel.SetActive(true);        
        }
    }
    public void OnLogin() {
        gameObject.SetActive(true);
        SetPanel();
        hide();
    }

    private void hide() {
        panelObject.SetActive(false);
    }

    private void show() {        
        panelObject.SetActive(true);
    }

    public void OnClickVisibleButton()
    {
        if (IsVisible == 1) {
            hide();
            IsVisible = 0;
        } else {
            show();
            IsVisible = 1;
        }
    }


	[System.Serializable]
	public class SendTransferParameter : CallbackParameter
	{
        public string name;
        public string tokenAddress;
        public int value;
	}

	[System.Serializable]
	public class RequestSwapParameter : CallbackParameter
	{
        public string name;
        public string openTokenAddress;
        public int openValue;
        public string closeTokenAddress;
        public int closeValue;
	}

    public void OnSendTransfer()
    {
        string userName = transferPanel.FindDeep("Name").GetComponent<TMP_InputField>().text;
        string tokenAddress = transferPanel.FindDeep("TokenAddress").GetComponent<TMP_InputField>().text;
        string value = transferPanel.FindDeep("Value").GetComponent<TMP_InputField>().text;

        /*
        bool found = false;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == userName) {
                found = true;
            }
        }
        if (!found) {
            return;
        }
        */

#if UNITY_WEBGL && !UNITY_EDITOR
		var callbackParameter = new SendTransferParameter
		{
			callbackGameObjectName = gameObject.name,
			callbackFunctionName = "CallbackSendTransfer",
            name = userName,
            tokenAddress = tokenAddress,
            value = int.Parse(value)
		};
		var parameterJson = JsonUtility.ToJson(callbackParameter);
		JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();
		jscall.Execute("SendTransfer", parameterJson);
#else
        Debug.Log("OnSendTransfer" + userName + "," + tokenAddress + "," + value);
        CallbackSendTransfer();
#endif
    }

    public void CallbackSendTransfer()
    {
        Debug.Log("Done");
    }

    public void OnSendSwap()
    {
        string userName = swapPanel.FindDeep("Name").GetComponent<TMP_InputField>().text;
        string openTokenAddress = swapPanel.FindDeep("OpenTokenAddress").GetComponent<TMP_InputField>().text;
        string openValue = swapPanel.FindDeep("OpenValue").GetComponent<TMP_InputField>().text;
        string closeTokenAddress = swapPanel.FindDeep("CloseTokenAddress").GetComponent<TMP_InputField>().text;
        string closeValue = swapPanel.FindDeep("CloseValue").GetComponent<TMP_InputField>().text;

        /*
        bool found = false;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == userName) {
                found = true;
            }
        }
        if (!found) {
            return;
        }
        */

#if UNITY_WEBGL && !UNITY_EDITOR
		var callbackParameter = new RequestSwapParameter
		{
			callbackGameObjectName = gameObject.name,
			callbackFunctionName = "CallbackRequestSwap",
            name = userName,
            openTokenAddress = openTokenAddress,
            openValue = int.Parse(openValue),
            closeTokenAddress = closeTokenAddress,
            closeValue = int.Parse(closeValue)
		};
		var parameterJson = JsonUtility.ToJson(callbackParameter);
		JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();
		jscall.Execute("RequestSwap", parameterJson);
#else
        Debug.Log("OnSendTransfer" + userName + "," + 
            openTokenAddress + "," + openValue + "," + closeTokenAddress + "," + closeValue);
        CallbackRequestSwap();
#endif
    }

    public void CallbackRequestSwap()
    {
        Debug.Log("Done");
    }
}
}