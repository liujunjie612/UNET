using Message_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    public InputField nameInput;
    public InputField psdInput;
    public Button loginBtn;
    public Text errorTxt;

    void Start()
    {
        Client.conn.RegisterHandler(MessageType.LoginRsp, __onLoginRsp);

        loginBtn.onClick.AddListener(this.__onLoginClick);
    }

    private void __onLoginClick()
    {
        if(string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(psdInput.text))
        {
            errorTxt.gameObject.SetActive(true);
            errorTxt.text = "昵称或密码不能为空";
            return;
        }
        else
        {
            errorTxt.gameObject.SetActive(false);
        }

        LoginReq req = new LoginReq();
        req.name = nameInput.text;
        req.psd = psdInput.text;

        Client.conn.Send(MessageType.LoginReq, req);
    }

    private void __onLoginRsp(NetworkMessage msg)
    {
        LoginRsp rsp = msg.ReadMessage<LoginRsp>();

        if (string.IsNullOrEmpty(rsp.error))
        {
            errorTxt.gameObject.SetActive(false);
            Log.Instance.Info("登录Ok");

            MasterServerRsp r = new MasterServerRsp();
            Client.conn.Send(MessageType.MasterServerRsp, r);
            Client.loadingObj.SetActive(true);
            Log.Instance.Info("send MasterServerRsp");
        }
        else
        {
            errorTxt.gameObject.SetActive(true);
            errorTxt.text = rsp.error
;
        }
    }
}
