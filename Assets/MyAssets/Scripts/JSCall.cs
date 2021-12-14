using UnityEngine;
using System.Runtime.InteropServices;

namespace Tutorial{
public class JSCall : MonoBehaviour
{
    public static JSCall instance;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void execute(string methodName, string parameter);
#endif

    public void Execute(string methodName, string parameter = "{}")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        execute(methodName, parameter);
#else
        Debug.Log($"call native method: {methodName}, parameter : {parameter}");
#endif
    }

    public void Start()
    {
        if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}       
    }
}
}