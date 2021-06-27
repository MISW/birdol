using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExistingDBScript : MonoBehaviour {

	Text DebugText;
	
	// Use this for initialization
	void Start () {
		DebugText = GetComponent<Text>();
		var ds = new DataService ("existing.db");
		if(DataService.TableExists(ds._connection,"Character")){
			ToConsole("Table Exists");
		}
		ds.CreateDB();	

		var characters = ds.GetCharacters();
		foreach (Character character in characters)
		{
    		ToConsole(character.ToString());
		}

		ds.CreateCharacter("HarrisonKawagoe",1,3,3,4);
		ToConsole("New person has been created");
		var p = ds.GetCharacter("HarrisonKawagoe");
		ToConsole(p.ToString());

	}
	

	private void ToConsole(string msg){
		DebugText.text += System.Environment.NewLine + msg;
		Debug.Log (msg);
	}

}