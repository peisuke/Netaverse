using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial{
public class ExpireCanvas : MonoBehaviour
{
    public static  ExpireCanvas instance;
    public GameObject panel;
    public GameObject content;
    public GameObject recieveItem;
    private bool isVisible;

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
        Initialize();
    }

    public void Initialize()
    {
        RecieveItemControler[] items = content.GetComponentsInChildren<RecieveItemControler>();
        foreach(RecieveItemControler item in items) {
            item.Remove();
        }
        
        isVisible = false;
        panel.SetActive(isVisible);
    }

    public void OnLogin() 
    {
        RecieveItemControler[] items = content.GetComponentsInChildren<RecieveItemControler>();
        foreach(RecieveItemControler item in items) {
            item.Remove();
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        var callbackParameter = new CallbackParameter
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "Callback"
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

        jscall.Execute("GetRequestSwapFrom", parameterJson);
#else
        Callback("[{\"openTraderName\":\"peisuke\",\"closeTraderName\":\"nuko\",\"openValue\":1,\"openSymbol\":\"T1\",\"closeValue\":1,\"closeSymbol\":\"CRY\",\"swapID\":\"0x7aff009ec22bc6e49bbc58f22421630c40ecfea3fa896e5f78759e6d946241f7\"}]");
#endif
    }

    public void Callback(string ret) {
        JSONObject obj = JSONObject.Create(ret);
        foreach (JSONObject o in obj.list) {
            AddItem(o["swapID"].str, 
                o["closeTraderName"].str, o["openSymbol"].str, o["openValue"].ToString(), 
                o["closeSymbol"].str, o["closeValue"].ToString()
            );
        }
    }

    public void hide() {
        isVisible = false;
        panel.SetActive(isVisible);
    }

    public void ChangeVisible()
    {
        isVisible = !isVisible;
        if (isVisible) {
            RecieveCanvas.instance.hide();
        }
        panel.SetActive(isVisible);
    }

    public void AddItem(string swapID, string closeTraderName, string openSymbol, string openValue, string closeSymbol, string closeValue)
    {
        var item = GameObject.Instantiate(recieveItem);
        item.GetComponent<RecieveItemControler>().Initialze(swapID, "ExpireSwap", closeTraderName, openSymbol, openValue, closeSymbol, closeValue, "To:", "Expire");
        item.transform.SetParent(content.transform, false);
    }
}
}