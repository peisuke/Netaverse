using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
namespace Tutorial{
public class PlayerManager : MonoBehaviourPun, IPunObservable {
	public Transform cameraToTarget;	
	public float verticalSpeed = 3.0f;
	public float rotateSpeed = 150f;

	float h, v;

	private PhotonView  m_photonView = null;
	//private Transform localTransform = null;

	[SerializeField] TextMeshPro userName;
	[SerializeField] EventSystem eventSystem;

    void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }

	public void Start()
	{
	}

	public void SetEvent(EventSystem _event)
	{
	   eventSystem = _event;
	}
	
	public void SetName(string name) {
		userName.text = name;
	}

	public string GetName() {
		return userName.text;
	}

	public void OnPhotonSerializeView( PhotonStream i_stream, PhotonMessageInfo i_info )
    {
        if( i_stream.IsWriting )
        {
            i_stream.SendNext(GetName());
        }
        else
        {
            string name = (string)i_stream.ReceiveNext();
			if (name != GetName()) {
				SetName(name);
			}
        }
	}

	// Update is called once per frame
	void FixedUpdate () {
		//eventSystem
		if( m_photonView.IsMine)
        {
			GameObject selectingObject = eventSystem.currentSelectedGameObject;
			if (selectingObject == null) {
				Move();
			}
			userName.text = PhotonNetwork.LocalPlayer.NickName;
			//localTransform = transform;
			//newPlayer.SetName(PhotonNetwork.LocalPlayer.NickName);
        } else {
		}

		//if (localTransform != null) {
		//	UpdateNameplate(localTransform);
		//}
	}

	void Move( )
	{
		 // Store the input axes.
        h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");

		var x = h* Time.deltaTime *  verticalSpeed;
		var y = h * Time.deltaTime * rotateSpeed;
		var z = v * Time.deltaTime * verticalSpeed;

		transform.Rotate (0, y, 0);
		transform.Translate (0, 0, z);
	}

	//public void UpdateNameplate(Transform tr)
	//{
	//	userName.transform.LookAt(2 * userName.transform.position - tr.position);
	//}
}
}