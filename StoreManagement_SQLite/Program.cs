using System;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Ensure SQLite database & tables exist
            StoreDbContext.InitializeDatabase();

            Application.Run(new MainForm());
        }
    }
}
