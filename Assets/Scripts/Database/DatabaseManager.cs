using SQLite;
using System.IO;
using UnityEngine;

public class DatabaseManager
{
    private static string dbPath = Path.Combine(Application.persistentDataPath, "game.db");
    private static SQLiteConnection _db;

    public static void InitializeDatabase()
    {
        Debug.Log("DB Path:");
        Debug.Log(dbPath);
        _db = new SQLiteConnection(dbPath);

        if (!File.Exists(dbPath) || _db.GetTableInfo("completed_characters").Count == 0)
        {
            // データベースファイルが存在しない場合は、新しく作成し初期データを投入する
            // テーブルが存在しない場合は作成する
            _db.CreateTable<DendouModel>();
            ProgressService.InitData();

            Debug.Log("Database and tables created, and initial data inserted.");
        }
    }

    public static SQLiteConnection getDB()
    {
        return _db;
    }




    // public static void AddPlayer(Player player)
    // {
    //     using (var db = new SQLiteConnection(dbPath))
    //     {
    //         db.Insert(player);
    //     }
    // }

    // public static void UpdatePlayer(Player player)
    // {
    //     using (var db = new SQLiteConnection(dbPath))
    //     {
    //         db.Update(player);
    //     }
    // }

    // public static void DeletePlayer(Player player)
    // {
    //     using (var db = new SQLiteConnection(dbPath))
    //     {
    //         db.Delete(player);
    //     }
    // }
}
