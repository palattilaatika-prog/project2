using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public class SalesHistoryForm : Form
    {
        private DataGridView dgvSales;

        public SalesHistoryForm()
        {
            this.Text = "Sales History";
            this.Size = new System.Drawing.Size(650, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            this.Controls.Add(dgvSales);

            LoadSalesHistoryAsync();
        }

        private async void LoadSalesHistoryAsync()
        {
            var history = await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    return db.SalesHistories.Select(s => new
                    {
                        s.Id,
                        s.ProductId,
                        s.ProductName,
                        s.CategoryName,
                        s.Quantity,
                        s.SaleDate
                    }).OrderByDescending(s => s.SaleDate).ToList();
                }
            });

            dgvSales.DataSource = history;
        }
    }
}
