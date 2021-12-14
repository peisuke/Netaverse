using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Tutorial{
public class Icons : MonoBehaviour
{
	public static  Icons instance;
    public GameObject panel;

    // Start is called before the first frame update
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
        hide();
    }

    public void hide() {
        panel.SetActive(false);
    }

    public void show() {
        panel.SetActive(true);
    }

    public void Logout() {
        PhotonNetwork.LeaveRoom();  
    }
}
}