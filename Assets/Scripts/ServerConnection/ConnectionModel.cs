using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サーバ<->クライアント間通信関連のモデル 
/// </summary>
public class ConnectionModel : MonoBehaviour
{
    public const int USERNAME_LENGTH_MIN = 1;
    public const int USERNAME_LENGTH_MAX = 16; //全角は2文字分
    public const int ACCOUNT_ID_LENGTH_MIN = 1;
    public const int ACCOUNT_ID_LENGTH_MAX = 256;
    public const int PASSWORD_LENGTH_MIN = 6;
    public const int PASSWORD_LENGTH_MAX = 256;

	/// <summary>
    /// 全角を2文字分として文字が何文字か数える
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
	public static int CountHalfWidthCharLength(string text)
	{
		int len =text.Length;
		foreach (char c in text)
        {
			if (!CheckHalfWidthChar(c)) len++;
		}
		return len;
		
    }
	/// <summary>
    /// 半角文字か否か
    /// 半角文字は、ひとまず「英数字(ASCII)」「半角カタカナ」「半角ハングル文字」「その他記号」としており、他は全角扱いにしている。
    /// </summary>
    /// <param name="c"></param>
    /// <returns>trueの場合、半角文字</returns>
	private static bool CheckHalfWidthChar(char c)
    {
		if (new System.Text.RegularExpressions.Regex(@"[(\u0020-\u007E)|(\uFF61-\uFF9F)|(\uFFE8-\uFFEE)]").IsMatch((c.ToString()))) return true; //半角文字
		else return false;	//全角文字
    }

	/// <summary>
    /// サーバからレスポンスとして帰ってくる文字列 
    /// </summary>
    public static class Response
    {
		// result strings
		public const string ResultOK = "ok";
		public const string ResultFail = "failed";
		public const string ResultNeedTokenRefresh = "need_refresh";
		public const string ResultRefreshSuccess = "refreshed";

		// error strings
		public const string ErrInvalidType = "invalid_content_type";
		public const string ErrAuthorizationFail = "fail_authorization";
		public const string ErrInvalidToken = "invalid_token";
		public const string ErrInvalidDevice = "invalid_device";
		public const string ErrFailParseXML = "fail_parsing_xml";
		public const string ErrInvalidSignature = "invalid_signature";
		public const string ErrNotLoggedIn = "not_logged_in";
		public const string ErrFailParseJSON = "fail_parsing_json";
		public const string ErrFailAccountCreation = "fail_account_creation";
		public const string ErrInvalidAccount = "invalid_account";
		public const string ErrPasswordExpire = "password_expired";
		public const string ErrInvalidPassword = "invalid_password";
		public const string ErrFailDataLink = "fail_datalink";
		public const string ErrFailSetPassword = "fail_set_password";
		public const string ErrFailUnlink = "fail_unlink";
		public const string ErrFailCreateSession = "fail_create_session";
		public const string ErrInvalidRefreshToken = "invalid_refresh_token";
		public const string ErrFailRefresh = "fail_refresh";
	}

	/// <summary>
    /// 通信から帰ってきたエラーメッセージに対応して、ユーザ(not 開発者)に表示するメッセージを返す。
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
	public static string ErrorMessage(string error="")
    {
		if (error == null) return "";
		Debug.LogError($"Error: {error}");
		string message="";
        switch (error)
        {
			case Response.ErrAuthorizationFail:
			case Response.ErrInvalidDevice:
			case Response.ErrInvalidSignature:
			case Response.ErrNotLoggedIn:
			case Response.ErrInvalidRefreshToken:
			case Response.ErrFailRefresh:
				message = "認証エラーが生じました。";
				break;
			case Response.ErrInvalidType:
			case Response.ErrFailParseJSON:
			case Response.ErrFailParseXML:
				message = "エラーが生じました。不適切なリクエストです。";
				break;
			case Response.ErrInvalidAccount:
			case Response.ErrPasswordExpire:
			case Response.ErrInvalidPassword:
				message = "このアカウントは存在しません。ユーザIDかパスワードが間違っています。";
				break;
			case Response.ErrFailAccountCreation:
				message = "アカウントの作成に失敗しました。";
				break;
			case Response.ErrFailDataLink:
				message = "アカウント連携に失敗しました。";
				break;
			case Response.ErrFailSetPassword:
				message = "パスワードの設定に失敗しました。";
				break;
			case Response.ErrFailUnlink:
				message = "ログアウトに失敗しました。";
				break;
			case Response.ErrFailCreateSession:
				message = "サーバでエラーが生じました。";
				break;
			default:
				Debug.LogError("予期せぬエラーがサーバから返ってきました。");
				break;
		}
		return message;
    }
}
