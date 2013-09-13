using System.Data;
using MonoTest.Models;

namespace MonoTest.Infrastructure
{
    public class ItemStore
    {
        private readonly DatabaseFactory _dbFactory;

        public ItemStore(DatabaseFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public Item Add(Item item)
        {
            using (IDbConnection con = _dbFactory.OpenDatabase())
            {
                con.Execute("INSERT INTO Item (Name,Description) VALUES (?,?)", new {item.Name, item.Description});
                item.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM Item");
            }

            return item;
        }
    }
}