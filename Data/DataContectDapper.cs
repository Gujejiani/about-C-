


using System.Data;
using Microsoft.Data.SqlClient;


using Dapper;

namespace DotnetAPI {


    class DataContextDapper {
         private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config) {
            _config = config;
            Console.WriteLine("Connection string " + config.GetConnectionString("DefaultConnection"));
        }

        public IEnumerable<T> LoadData<T>(string sql) {
            
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return dbConnection.Query<T>(sql);
        }


         public T LoadDataSingle<T>(string sql) {
            
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return dbConnection.QuerySingle<T>(sql);
        }


        public  bool executeSql(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        
            return dbConnection.Execute(sql) > 0;
        }

        public int executeWithRowCount(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        
            return dbConnection.Execute(sql);
        } 

    }
}

