using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Authorization : WebClient
{
    [Header("Authorization")]
    [SerializeField] protected AuthRequest mAuthRequest;

    [Serializable]
    public struct AuthRequest {
        [SerializeField] public int user_id;
        [SerializeField] public string access_token;
        [SerializeField] public string device_id;

        public AuthRequest(int user_id, string access_token, string device_id) {
            this.user_id = user_id;
            this.access_token = access_token;
            this.device_id = device_id;
        }
    }

    [Serializable]
    public struct Response {
        [SerializeField] public string error;
        [SerializeField] public string result;
        [SerializeField] public string session_id;
    }

    public Authorization(HttpRequestMethod mRequestMethod, string path) : base(mRequestMethod, path){
        this.httpRequestMethod = mRequestMethod;
        this.path = path;
    }

    public override bool CheckRequestData() {
        throw new NotImplementedException();
    }

    protected override void HandleSetupWebRequestData(UnityWebRequest www) {
        throw new NotImplementedException();
    }

    protected override void HandleErrorData(string error) {
        throw new NotImplementedException();
    }

    protected override void HandleInProgressData() {
        throw new NotImplementedException();
    }

    protected override void HandleSuccessData(string response) {
        throw new NotImplementedException();
    }
}
