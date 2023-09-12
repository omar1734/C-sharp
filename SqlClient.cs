///
///     2021
///
using Npgsql;
using UnityEngine;

public class SqlClient : MonoBehaviour
{
    public static SqlClient singleton;
    
    public static NpgsqlConnection Connection;
    
    public static bool IsConnected { get; private set; }
    
    public string host = "127.0.0.1";
    public string username = "postgres";
    public string password = "password";
    public string databaseName = "postgres";
    
    private string _connectString;

#if UNITY_EDITOR
    public void OnValidate()
    {
        _connectString = "Host=" + host + ";Username=" + username + ";Password=" + password +
                         ";Database=" + databaseName;
    }
#endif

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogWarning("There can only be one SqlConnectionManager component in the scene!");
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        try
        {
            Connection = new NpgsqlConnection(_connectString);
            Connection.Open();
            IsConnected = true; 
        }
        catch(System.Exception e)
        {
            Debug.LogWarning("Failed to establish sql connection with connection string: " + _connectString + " " + e.StackTrace);
            
            IsConnected = false;
        }
    }

    private void OnDisable()
    {
        Connection.Close();
    }
}
