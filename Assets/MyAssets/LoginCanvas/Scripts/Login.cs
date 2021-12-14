using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject networkManager;

    // Start is called before the first frame update
    static string usernamePrefKey = "username";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void DoLogin()
    {
        networkManager.SendMessage("EmitJoin");
    }
}
