using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService  {

	public SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);     

	}

//Change Below
	public static bool TableExists(SQLiteConnection connection,string tablename)
	{    
    	const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
    	var cmd = connection.CreateCommand (cmdText, tablename);
    	return cmd.ExecuteScalar<string> () != null;
	}

	public void CreateDB(){
		if(!TableExists(_connection,"Character")){
			Debug.Log("Creating Database");
			_connection.DropTable<Character> ();
			_connection.CreateTable<Character>();
			_connection.InsertAll (new[]{
			new Character{
				Id = 1,
				Name = "Myuu",
				Rarity = 1,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 2,
				Name = "Bayashiko",
				Rarity = 10,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 3,
				Name = "Futoren",
				Rarity = 1,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 4,
				Name = "Kajimoto",
				Rarity = 100,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 5,
				Name = "Liberal",
				Rarity = 1,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 6,
				Name = "SEED",
				Rarity = 10,
				Visual = 3,
				Vocal = 3,
				Dance = 4
			},
			new Character{
				Id = 7,
				Name = "I.TK",
				Rarity = 1,
				Visual = 3,
				Vocal = 3,
				Dance = 4
				}
			});
		}
	}

	public IEnumerable<Character> GetCharacters(){
		return _connection.Table<Character>();
	}

	public Character GetCharacter(string name){
		return _connection.Table<Character>().Where(x => x.Name == name).FirstOrDefault();
	}

	public Character CreateCharacter(string name,int rarity,int visual,int vocal,int dance){
		var c = new Character{
				Name = name,
				Rarity = rarity,
				Visual = visual,
				Vocal = vocal,
				Dance = dance
		};
		_connection.Insert (c);
		return c;
	}
}