//----------------------------------------------
// SQLiter
// Copyright � 2014 OuijaPaw Games LLC
//----------------------------------------------

using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;

namespace SQLiter
{
	/// <summary>
	/// The idea is that here is a bunch of the basics on using SQLite
	/// Nothing is some advanced course on doing joins and unions and trying to make your infinitely normalized schema work
	/// SQLite is simple.  Very simple.  
	/// Pros:
	/// - Very simple to use
	/// - Very small memory footprint
	/// 
	/// Cons:
	/// - It is a flat file database.  You can change the settings to make it run completely in memory, which will make it even
	/// faster; however, you cannot have separate threads interact with it -ever-, so if you plan on using SQLite for any sort
	/// of multiplayer game and want different Unity instances to interact/read data... they absolutely cannot.
	/// - Doesn't offer as many bells and whistles as other DB systems
	/// - It is awfully slow.  I mean dreadfully slow.  I know "slow" is a relative term, but unless the DB is all in memory, every
	/// time you do a write/delete/update/replace, it has to write to a physical file - since SQLite is just a file based DB.
	/// If you ever do a write and then need to read it shortly after, like .5 to 1 second after... there's a chance it hasn't been
	/// updated yet... and this is local.  So, just make sure you use a coroutine or whatever to make sure data is written before
	/// using it.
	/// 
	/// SQLite is nice for small games, high scores, simple saved, etc.  It is not very secure and not very fast, but it's cheap,
	/// simple, and useful at times.
	/// 
	/// Here are some starting tools and information.  Go explore.
	/// </summary>
	public class SQLite : MonoBehaviour
	{
		public static SQLite Instance = null;
		public bool DebugMode = false;

		// Location of database - this will be set during Awake as to stop Unity 5.4 error regarding initialization before scene is set
		// file should show up in the Unity inspector after a few seconds of running it the first time
		private static string _sqlDBLocation = "";

		/// <summary>
		/// Table name and DB actual file name -- this is the name of the actual file on the filesystem
		/// </summary>
		private const string SQL_DB_NAME = "PokemonDB";

		// table name
		private const string SQL_TABLE_NAME = "Pokemon";

		/// <summary>
		/// predefine columns here to there are no typos
		/// </summary>
		private const string COL_ID = "id";
		private const string COL_NAME = "name";  // using name as example of primary, unique, key
		private const string COL_SPECIES = "species";
		private const string COL_TYPE1 = "type1";
		private const string COL_TYPE2 = "type2";
		private const string COL_HEIGHT = "height";
		private const string COL_WEIGHT = "weight";

		/// <summary>
		/// DB objects
		/// </summary>
		private IDbConnection _connection = null;
		private IDbCommand _command = null;
		private IDataReader _reader = null;
		private string _sqlString;

		private bool _createNewTavle = false;

		/// <summary>
		/// Awake will initialize the connection.  
		/// RunAsyncInit is just for show.  You can do the normal SQLiteInit to ensure that it is
		/// initialized during the Awake() phase and everything is ready during the Start() phase
		/// </summary>
		void Awake()
		{
			if (DebugMode)
				Debug.Log("--- Awake ---");

			// here is where we set the file location
			// ------------ IMPORTANT ---------
			// - during builds, this is located in the project root - same level as Assets/Library/obj/ProjectSettings
			// - during runtime (Windows at least), this is located in the SAME directory as the executable
			// you can play around with the path if you like, but build-vs-run locations need to be taken into account
			_sqlDBLocation = "URI=file:" + SQL_DB_NAME + ".db";

			Debug.Log(_sqlDBLocation);
			Instance = this;
			SQLiteInit();
		}

		void Start()
		{
			if (DebugMode)
				Debug.Log("--- Start ---");

			// just for testing, comment/uncomment to play with it
			// note that it MUST be invoked after SQLite has initialized, 2-3 seconds later usually.  1 second is cutting it too close
			Invoke("Test", 3);
		}

		/// <summary>
		/// Just for testing, but you can see that GetAllPlayers is called -before- the insert player methods,
		/// and returns the data afterwards.
		/// </summary>
		void Test()
		{
			if (DebugMode)
				Debug.Log("--- Test Invoked ---");

			LoomManager.Loom.QueueOnMainThread(() =>
			{
				GetAllPokemon();
			});

			InsertPokemon("001", "Bulbasaur", "Seed Pokemon", "GRASS", "POISON", "2′4″ (0.71m)", "15.2 lbs (6.9 kg)");
			InsertPokemon("004", "Charmander", "Lizard Pokemon", "FIRE", null, "2′0″ (0.61m)", "18.7 lbs (8.5 kg)");
			InsertPokemon("007", "Squirtle", "Tiny Turtle Pokemon", "WATER", null, "1′8″ (0.51m)", "19.8 lbs (9.0 kg)");
			InsertPokemon("025", "Pikachu", "Mouse Pokemon", "ELECTRIC", null, "1′4″ (0.41m)", "13.2 lbs (6.0 kg)");
		}

		/// <summary>
		/// Uncomment if you want to see the time it takes to do things
		/// </summary>
		//void Update()
		//{
		//    Debug.Log(Time.time);
		//}

		/// <summary>
		/// Clean up SQLite Connections, anything else
		/// </summary>
		void OnDestroy()
		{
			SQLiteClose();
		}

		/// <summary>
		/// Example using the Loom to run an asynchronous method on another thread so SQLite lookups
		/// do not block the main Unity thread
		/// </summary>
		public void RunAsyncInit()
		{
			LoomManager.Loom.QueueOnMainThread(() =>
			{
				SQLiteInit();
			});
		}

		/// <summary>
		/// Basic initialization of SQLite
		/// </summary>
		private void SQLiteInit()
		{
			Debug.Log("SQLiter - Opening SQLite Connection" + _sqlDBLocation);
			_connection = new SqliteConnection(_sqlDBLocation);
			_command = _connection.CreateCommand();
			_connection.Open();

			// WAL = write ahead logging, very huge speed increase
			_command.CommandText = "PRAGMA journal_mode = WAL;";
			_command.ExecuteNonQuery();

			// journal mode = look it up on google, I don't remember
			_command.CommandText = "PRAGMA journal_mode";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
			_reader.Close();

			// more speed increases
			_command.CommandText = "PRAGMA synchronous = OFF";
			_command.ExecuteNonQuery();

			// and some more
			_command.CommandText = "PRAGMA synchronous";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
			_reader.Close();

			// here we check if the table you want to use exists or not.  If it doesn't exist we create it.
			_command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + SQL_TABLE_NAME + "'";
			_reader = _command.ExecuteReader();
			if (!_reader.Read())
			{
				Debug.Log("SQLiter - Could not find SQLite table " + SQL_TABLE_NAME);
				_createNewTavle = true;
			}
			_reader.Close();

			// create new table if it wasn't found
			if (_createNewTavle)
			{
				Debug.Log("SQLiter - Creating new SQLite table " + SQL_TABLE_NAME);

				// insurance policy, drop table
				_command.CommandText = "DROP TABLE IF EXISTS " + SQL_TABLE_NAME;
				_command.ExecuteNonQuery();

				// create new - SQLite recommendation is to drop table, not clear it
				_sqlString = "CREATE TABLE IF NOT EXISTS " + SQL_TABLE_NAME + " (" +
					COL_ID + " TEXT UNIQUE, " +
					COL_NAME + " TEXT, " +
					COL_SPECIES + " TEXT, " +
					COL_TYPE1 + " TEXT, " +
					COL_TYPE2 + " TEXT, " +
					COL_HEIGHT + " TEXT, " +
					COL_WEIGHT + " TEXT)";
				_command.CommandText = _sqlString;
				_command.ExecuteNonQuery();
			}
			else
			{
				if (DebugMode)
					Debug.Log("SQLiter - SQLite table " + SQL_TABLE_NAME + " was found");
			}

			// close connection
			_connection.Close();
		}
			

		#region Insert
		/// <summary>
		/// Inserts a player into the database
		/// http://www.sqlite.org/lang_insert.html
		/// name must be unique, it's our primary key
		/// </summary>
		/// <param name="name"></param>
		/// <param name="raceType"></param>
		/// <param name="classType"></param>
		/// <param name="gold"></param>
		/// <param name="login"></param>
		/// <param name="level"></param>
		/// <param name="xp"></param>
		public void InsertPokemon(string id, string name, string species, string type1, string type2, string height, string weight)
		{
			name = name.ToLower();

			// note - this will replace any item that already exists, overwriting them.  
			// normal INSERT without the REPLACE will throw an error if an item already exists
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TABLE_NAME
				+ " ("
				+ COL_ID + ","
				+ COL_NAME + ","
				+ COL_SPECIES + ","
				+ COL_TYPE1 + ","
				+ COL_TYPE2 + ","
				+ COL_HEIGHT + ","
				+ COL_WEIGHT
				+ ") VALUES ("
				+ "'" + id + "',"  // note that string values need quote or double-quote delimiters
				+ "'" + name + "',"
				+ "'" + species + "',"
				+ "'" + type1 + "',"
				+ "'" + type2 + "',"
				+ "'" + height + "',"
				+ "'" + weight
				+ "');";

			if (DebugMode)
				Debug.Log(_sqlString);
			ExecuteNonQuery(_sqlString);
		}

		#endregion

		#region Query Values

		/// <summary>
		/// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
		/// </summary>
		public void GetAllPokemon()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			_connection.Open();

			// if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
			_command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME;
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				// reuse same stringbuilder
				sb.Length = 0;
				sb.Append(_reader.GetString(0)).Append(" ");
				sb.Append(_reader.GetString(1)).Append(" ");
				sb.Append(_reader.GetString(2)).Append(" ");
				sb.Append(_reader.GetString(3)).Append(" ");
				sb.Append(_reader.GetString(4)).Append(" ");
				sb.Append(_reader.GetString(5)).Append(" ");
				sb.Append(_reader.GetString(6)).Append(" ");
				sb.AppendLine();

				// view our output
				if (DebugMode)
					Debug.Log(sb.ToString());
			}
			_reader.Close();
			_connection.Close();
		}

		/// <summary>
		/// Basic get, returning a value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetPokemon(string value)
		{
			return QueryString(COL_ID, value);
		}

		/// <summary>
		/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public string QueryString(string column, string value)
		{
			string text = "Not Found";
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				text = _reader.GetString(0);
			else
				Debug.Log("QueryString - nothing to read...");
			_reader.Close();
			_connection.Close();
			return text;
		}

		/// <summary>
		/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public int QueryInt(string column, string value)
		{
			int sel = -1;
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				sel = _reader.GetInt32(0);
			else
				Debug.Log("QueryInt - nothing to read...");
			_reader.Close();
			_connection.Close();
			return sel;
		}

		/// <summary>
		/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public short QueryShort(string column, string value)
		{
			short sel = -1;
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				sel = _reader.GetInt16(0);
			else
				Debug.Log("QueryShort - nothing to read...");
			_reader.Close();
			_connection.Close();
			return sel;
		}
		#endregion

		#region Update / Replace Values
		/// <summary>
		/// A 'Set' method that will set a column value for a specific player, using their name as the unique primary key
		/// to some value.  This currently just uses 'int' types, but you could modify this to use/do most anything.
		/// Remember strings need single/double quotes around their values
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(string column, int value, string name)
		{
			ExecuteNonQuery("UPDATE OR REPLACE " + SQL_TABLE_NAME + " SET " + column + "=" + value + " WHERE " + COL_NAME + "='" + name + "'");
		}

		#endregion

		#region Delete

		/// <summary>
		/// Basic delete, using the name primary key for the 
		/// </summary>
		/// <param name="nameKey"></param>
		public void DeletePlayer(string nameKey)
		{
			ExecuteNonQuery("DELETE FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + nameKey + "'");
		}
		#endregion

		/// <summary>
		/// Basic execute command - open, create command, execute, close
		/// </summary>
		/// <param name="commandText"></param>
		public void ExecuteNonQuery(string commandText)
		{
			_connection.Open();
			_command.CommandText = commandText;
			_command.ExecuteNonQuery();
			_connection.Close();
		}

		/// <summary>
		/// Clean up everything for SQLite
		/// </summary>
		private void SQLiteClose()
		{
			if (_reader != null && !_reader.IsClosed)
				_reader.Close();
			_reader = null;

			if (_command != null)
				_command.Dispose();
			_command = null;

			if (_connection != null && _connection.State != ConnectionState.Closed)
				_connection.Close();
			_connection = null;
		}
	}
}
