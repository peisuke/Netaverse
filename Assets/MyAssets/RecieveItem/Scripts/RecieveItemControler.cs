using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial{
public class RecieveItemControler : MonoBehaviour
{
    private string swapID;
    private string contractFunction;
    public Text sideObject;
    public Text traderNameObject;
    public Text openSymbolObject;
    public Text openValueObject;
    public Text closeSymbolObject;
    public Text closeValueObject;
    public Text buttonTextObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Initialze(string _swapID, string func, 
                          string traderName, string openSymbol, string openValue, 
                          string closeSymbol, string closeValue, 
                          string sideText, string buttonText)
    {
        swapID = _swapID;
        contractFunction = func;
        traderNameObject.text = traderName;
        openSymbolObject.text = openSymbol;
        openValueObject.text = openValue;
        closeSymbolObject.text = closeSymbol;
        closeValueObject.text = closeValue;
        sideObject.text = sideText;
        buttonTextObject.text = buttonText;
    }

	[System.Serializable]
	public class SendApproveParameter : CallbackParameter
	{
        public string swapID;
	}

    public string GetSwapID() {
        return swapID;
    }
    
    public void OnClickApprove()
    {
        Debug.Log("OnClickApprove");
#if UNITY_WEBGL && !UNITY_EDITOR
        var callbackParameter = new SendApproveParameter
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "Callback",
            swapID = swapID
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        JSCall jscall = GameObject.Find("JSCall").GetComponent<JSCall>();

        jscall.Execute(contractFunction, parameterJson);
#else
        Callback(0);
#endif
    }

    public void Callback(int success) {
        if (success >= 0) {
            Remove();
        }
    }

    public void Remove() {
        Destroy(this.gameObject);
    }
}
}