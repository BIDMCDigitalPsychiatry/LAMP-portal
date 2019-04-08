using Microsoft.Practices.EnterpriseLibrary.Data;

namespace LAMP.DataAccess
{    
    public class DataAccessBase
    {
        private Database _database;

        public Database MyDatabase
        {
            get { return _database; }
            set { _database = value; }
        }

        public DataAccessBase()
        {
           // _database = new DatabaseProviderFactory().Create("LAMPEntities");
        }

    }
}
