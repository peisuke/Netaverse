using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace Tutorial{
[RequireComponent(typeof(PhotonView))]
public class ChatCanvasManager : MonoBehaviourPun
{
    public bool snapped = true;
    public static  ChatCanvasManager instance;

    private Vector2 initialButtonAnchorMin;
    private Vector2 initialButtonAnchorMax;
    int IsVisible;

    public GameObject panelObject;
    public GameObject buttunObject;

	public TMP_InputField inputText;
    private List<string> messages = new List<string>();
    [SerializeField] private ScrollRect scrollRect = null;

    public GameObject content;

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
        RectTransform buttonRectTransform = buttunObject.GetComponent<RectTransform>();
        initialButtonAnchorMin = new Vector2(buttonRectTransform.anchorMin.x, buttonRectTransform.anchorMin.y);
        initialButtonAnchorMax = new Vector2(buttonRectTransform.anchorMax.x, buttonRectTransform.anchorMax.y);

        Initialize();
    }

    public void Initialize() {        
        IsVisible = -1;
        gameObject.SetActive(false);

        TMP_Text textObject = content.GetComponent<TMP_Text>();
        textObject.text = "";

        inputText.text = "";
        messages = new List<string>();
    }

    public void OnLogin() {
        gameObject.SetActive(true);
        hide();
    }

    private void hide() {        
        RectTransform buttonRectTransform = buttunObject.GetComponent<RectTransform>();
        panelObject.SetActive(false);
        buttonRectTransform.anchorMin = new Vector2(0.01f, 0.045f);
        buttonRectTransform.anchorMax = new Vector2(0.05f, 0.145f);
    }

    private void show() {        
        RectTransform buttonRectTransform = buttunObject.GetComponent<RectTransform>();

        panelObject.SetActive(true);
        buttonRectTransform.anchorMin = initialButtonAnchorMin;
        buttonRectTransform.anchorMax = initialButtonAnchorMax;
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

    // Update is called once per frame
    void Update()
    {
    }

    public void SendChat()
    {
        //chatRPC
        photonView.RPC("Chat", RpcTarget.All, inputText.text);

        //送信後、入力欄を空にし、スクロール最下位置に移動
        inputText.text = "";
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        if (messages.Count > 100)          //チャットログが多くなって来たらログを削除してから受信
        {
            messages.RemoveRange(0, messages.Count - 100);
        }

        //chat受信
        ReceiveChat(newLine, mi);

        //受信したときはスクロール最下位置
        //scrollPos.y = Mathf.Infinity;
    }

    void ReceiveChat(string _newLine, PhotonMessageInfo _mi)
    {
        //送信者の名前用変数
        string senderName = "anonymous";
        if (_mi.Sender != null)
        {
            //送信者の名前があれば
            if (!string.IsNullOrEmpty(_mi.Sender.NickName))
            {
                senderName = _mi.Sender.NickName;
            }
            else
            {
                senderName = "player " + _mi.Sender.UserId;
            }
        }
        //受信したチャットをログに追加
        if (_newLine.Length > 0) {
            messages.Add(senderName + ": " + _newLine);

            TMP_Text textObject = content.GetComponent<TMP_Text>();
            textObject.text = string.Join("\n", messages.ToArray());

            StartCoroutine(ForceScrollDown());
            Canvas.ForceUpdateCanvases();
        }
    }

    IEnumerator ForceScrollDown ()
    {
        yield return new WaitForEndOfFrame ();
        Canvas.ForceUpdateCanvases ();
        scrollRect.gameObject.SetActive(true);
        scrollRect.verticalNormalizedPosition = 0f;
        scrollRect.verticalScrollbar.value = 0;
        Canvas.ForceUpdateCanvases ();
    }
}
}