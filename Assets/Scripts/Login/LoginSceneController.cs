using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LoginWebClient))]
public class LoginSceneController : SceneVisor 
{
    [SerializeField] private LoginWebClient loginWebClient; 

    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField emailInputField;
    [SerializeField] InputField passwordInputField;

    [SerializeField] GameObject ConnectionSuccessDisplayGameObject;
    [SerializeField] Text ConnectionSuccessDisplayText;
    [SerializeField] GameObject ConnectionErrorDisplayGameObject;
    [SerializeField] Text ConnectionErrorDisplayText;

    private void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        string username = usernameInputField.text;
        string email = emailInputField.text;
        string password = passwordInputField.text;
        loginWebClient.SetData(username, email, password);
        yield return StartCoroutine(loginWebClient.Send());

        //処理
        if (loginWebClient.isSuccess==true && loginWebClient.isInProgress==false)
        {
            //成功した時
            //LoginWebClientはひとまずLoginResponseDataをdataに保存するとする 
            LoginWebClient.LoginResponseData lrd = (LoginWebClient.LoginResponseData)loginWebClient.data;
            Debug.Log("ParsedResponseData: \n"+lrd.ToString());
            ConnectionSuccessDisplayText.text = loginWebClient.message; 
            ShowForWhile(5.0f, ConnectionSuccessDisplayGameObject);
        }
        else
        {
            //失敗した時
            ConnectionErrorDisplayText.text = loginWebClient.message; 
            ShowForWhile(5.0f,ConnectionErrorDisplayGameObject);
        }
    }

    private void ShowForWhile(float duration, GameObject gm)
    {
        StartCoroutine(ShowForWhileCoroutine(duration, gm));
    }
    private IEnumerator ShowForWhileCoroutine(float duration,GameObject gm)
    {
        gm.SetActive(true);
        yield return new WaitForSeconds(duration);
        gm.SetActive(false);
        yield break;
    }
}
