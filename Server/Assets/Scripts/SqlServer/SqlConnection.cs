using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MySql.Data.MySqlClient;

public class SqlConnection : MonoBehaviour 
{
    public static MySqlConnection connection = new MySqlConnection();
    public static MySqlCommand Linq = new MySqlCommand();

    private static bool _sqlUp;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Log.Instance.Info(connection.State);
        }
    }

    public static  void SqlConnect()
    {
        if (!_sqlUp)
        {
            connection = new MySqlConnection();
            //connection.ConnectionString = "Server=" + "localhost" + ";Port=" + "3306" + ";UserID=" + "root" + ";Password=" + "admin" + ";";
            connection.ConnectionString = "Server=" + "localhost" + ";Port=" + "3306" + ";Database=" +
                    "sample" + ";UserID=" + "root" + ";Password=" + "admin" + ";";
            connection.Open();
            Linq.Connection = connection;
            _sqlUp = true;

            Log.Instance.Info("SQL connect");
        }
    }

    public static void GetAllDB()
    {
        if (_sqlUp)
        {
            MySqlCommand Linq = new MySqlCommand();
            Linq.Connection = connection;
            Linq.CommandText = "SHOW DATABASES";
            MySqlDataReader read = Linq.ExecuteReader();
            //DropDB.options.Clear();
            while (read.Read())
            {
               // Dropdown.OptionData dbname = new Dropdown.OptionData();
                //dbname.text = read.GetString(0);
               // DropDB.options.Add(dbname);
            }
            read.Close();
        }
    }
}
