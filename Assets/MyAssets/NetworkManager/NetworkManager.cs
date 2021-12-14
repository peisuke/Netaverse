using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

namespace Tutorial{
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance; //useful for any gameObject to access this class without the need of instances her or you declare it

	[Header("Local Player Prefab")]
	public GameObject localPlayerPrefab; //store the local player prefabs

	[Header("Spawn Points")]
	public Transform[] spawnPoints; //stores the spawn points

    [Header("Camera Rig Prefab")]
	public GameObject camRigPref;

    [HideInInspector]
	public GameObject camRig;

    [Header("EventSystem")]
	public EventSystem eventSystem;

	static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator

    // Start is called before the first frame update
    void Start()
    {           
    }

    public void EmitJoin() {
		PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();         
    }

	public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnLeftRoom()
    {
        Destroy(camRig);
        
        CanvasManager.instance.Initialize();
        ChatCanvasManager.instance.Initialize();
        WalletCanvasManager.instance.Initialize();
        RecieveCanvas.instance.Initialize();
        ExpireCanvas.instance.Initialize();
        SettingCanvasManager.instance.Initialize();
        Icons.instance.hide();
        
        PhotonNetwork.Disconnect();
    }

	// 入室に失敗した場合に呼ばれるコールバック
    // １人目は部屋がないため必ず失敗するので部屋を作成する
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PlayerManager newPlayer;
        PhotonNetwork.LocalPlayer.NickName = CanvasManager.instance.inputLogin.text;
		
		int index = Random.Range (0, spawnPoints.Length);

        newPlayer = PhotonNetwork.Instantiate(
            "player",
            new Vector3(spawnPoints[index].position.x, 
                spawnPoints[index].position.y, 
                spawnPoints[index].position.z),
            Quaternion.identity,
            0
        ).GetComponent<PlayerManager> ();

        newPlayer.SetEvent(eventSystem);

        camRig = GameObject.Instantiate (camRigPref, new Vector3 (0f, 0f, 0f), Quaternion.identity);
        camRig.GetComponent<CameraFollow>().SetTarget(newPlayer.transform, newPlayer.cameraToTarget);

        CanvasManager.instance.OpenScreen(1);
        ChatCanvasManager.instance.OnLogin();
        WalletCanvasManager.instance.OnLogin();
        RecieveCanvas.instance.OnLogin();
        ExpireCanvas.instance.OnLogin();
        SettingCanvasManager.instance.OnLogin();
        Icons.instance.show();

        eventSystem.SetSelectedGameObject(null);
    }
}
}