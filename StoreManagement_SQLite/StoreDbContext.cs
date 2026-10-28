using System.Data.Entity;
using System.Data.SQLite;

namespace StoreManagement_SQLite
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext() : base("name=StoreDbContext")
        {
            Database.SetInitializer<StoreDbContext>(null);
        }

        public DbSet<ProductCategory> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesHistory> SalesHistories { get; set; }

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=.\\store.db"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS ProductCategory (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Product (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Description TEXT,
                            EntryDate TEXT NOT NULL,
                            ExpirationDate TEXT NOT NULL,
                            Quantity INTEGER NOT NULL,
                            CategoryId INTEGER NOT NULL,
                            FOREIGN KEY(CategoryId) REFERENCES ProductCategory(Id)
                        );

                        CREATE TABLE IF NOT EXISTS SalesHistory (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductId INTEGER NOT NULL,
                            ProductName TEXT NOT NULL,
                            CategoryName TEXT NOT NULL,
                            Quantity INTEGER NOT NULL,
                            SaleDate TEXT NOT NULL
                        );
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
