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
        if(Reader.Read())
        {
            return Reader.GetString(0);
        }
        else
        {
            return null;
        }
    }
}
