using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqlLogin
{
    public static MySqlCommand Linq = SqlConnection.Linq;

    public static string GetAccountPassword(string name)
    {
        Linq.CommandText = "SELECT PasswordAc FROM accountlist WHERE AccountName = '" + name + "'";
        MySqlDataReader Reader = Linq.ExecuteReader();

        string psd = "";
        if(Reader.Read())
        {
            psd = Reader.GetString(0);
        }
        
        Reader.Close();

        return psd;
    }
}
