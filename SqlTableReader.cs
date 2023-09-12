using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Npgsql;
using NpgsqlTypes;
using UnityEngine;
using UnityEngine.Events;




/// <summary>
/// Reads the rows of a table, 
/// then forwards column values as strings to other components.
///
/// Assumes table is structured as follows:
///     First col: id/primary key
///     Second -> Second to last col: Values
///
/// </summary>
public class SqlTableReader : MonoBehaviour
{
    public string tableName;

    public List<ColumnOutput> columnValueMappingEvents = new List<ColumnOutput>();

    public const string Schema = "public";
    
    public bool IsFinishedWithReplay { get; set; }
    
    private static readonly List<SqlTableReader> All = new List<SqlTableReader>();
    
    private static int currentReplaysCount = 0;
    
    private const float TickDelay = 1;
    
    private float _lastTickTime = float.NegativeInfinity;
    
    private readonly List<TableRow> _replayTableRows = new List<TableRow>();
    
    private Coroutine _replayCoroutine;

    private string _lastReadKey = string.Empty;
    
    public void Update()
    {
        if (!SqlClient.IsConnected)
        {
            Debug.Log("SQL Client not connected!");
            return;
        }

        if (Time.time - _lastTickTime < TickDelay)
        {
            return;
        }
        
        ReadLastRow();

        _lastTickTime = Time.time;
    } 

    //ID = YYMMDDhhmmss
    //WHERE log_time > (NOW() - interval '1 second')

    private void ReadLastRow()
    {
        string commandString = "SELECT * FROM " + Schema + "." + tableName + " ORDER BY ID DESC LIMIT 1";

        var command = new NpgsqlCommand(commandString, SqlClient.Connection);

        using var reader = command.ExecuteReader();
        
        // Read most recent row
        reader.Read();

        // Get columns from the row
        var columns = Enumerable.Range(0,reader.FieldCount).Select(ix => reader[ix].ToString());

        List<string> temp = new List<string>(columns);
        
        if (temp[0] == _lastReadKey)
        {
            // Table did not update
            return;
        }

        _lastReadKey = temp[0];
        
        // Remove id/primary key. Only need values.
        temp.RemoveAt(0);
            
        // Set new state for each column in row
        for (int i = 0; i < temp.Count; i++)
        {
            //Debug.Log(temp[i]);

            var values = temp[i].Split(',');

            for (int j = 0; j < values.Length; j++)
            {
                columnValueMappingEvents[i].commaSeperatedEvents[j].Invoke(values[j].Trim('[', ']'));
            }
        }
    }

    private struct TableRow
    {
        public readonly List<string> Columns;
        public DateTime TimeStamp;

        public TableRow(List<string> columns, DateTime timeStamp)
        {
            this.Columns = columns;
            this.TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            string s = string.Empty;
        
            foreach (var column in Columns)
            {
                s += column + ", ";
            }

            s += TimeStamp.ToString(CultureInfo.InvariantCulture);

            return s;
        }
    }
    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }
}


[System.Serializable]
public class ColumnOutput 
{
    public List<UnityEvent<string>> commaSeperatedEvents = new List<UnityEvent<string>>();
}
