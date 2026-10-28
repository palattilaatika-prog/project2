using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public class SaleForm : Form
    {
        private Product _product;
        private NumericUpDown numSellQty;
        private Button btnSell = new Button { Text = "Confirm Sale", Top = 150, Left = 120, DialogResult = DialogResult.OK };

        public SaleForm(Product product)
        {
            _product = product;
            this.Text = $"Sell Product - {product.Name}";
            this.Size = new System.Drawing.Size(340, 240);
            this.StartPosition = FormStartPosition.CenterParent;

            string categoryName = product.Category != null ? product.Category.Name : "N/A";

            this.Controls.Add(new Label { Text = $"Product: {product.Name}", Top = 20, Left = 20, AutoSize = true });
            this.Controls.Add(new Label { Text = $"Category: {categoryName}", Top = 45, Left = 20, AutoSize = true });
            this.Controls.Add(new Label { Text = $"Available Quantity: {product.Quantity}", Top = 70, Left = 20, AutoSize = true });
            this.Controls.Add(new Label { Text = "Quantity to Sell:", Top = 100, Left = 20 });

            numSellQty = new NumericUpDown
            {
                Top = 100,
                Left = 120,
                Width = 100,
                Minimum = 1,
                Maximum = product.Quantity
            };
            this.Controls.Add(numSellQty);
            this.Controls.Add(btnSell);

            btnSell.Click += async (s, e) => await SellProductAsync();
        }

        private async Task SellProductAsync()
        {
            int quantityToSell = (int)numSellQty.Value;

            await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    var item = db.Products.Find(_product.Id);
                    if (item != null)
                    {
                        string categoryName = _product.Category != null ? _product.Category.Name : "N/A";

                        // Record sales history
                        var salesRecord = new SalesHistory
                        {
                            ProductId = item.Id,
                            ProductName = item.Name,
                            CategoryName = categoryName,
                            Quantity = quantityToSell,
                            SaleDate = DateTime.Now
                        };
                        db.SalesHistories.Add(salesRecord);

                        // Deduct stock quantity
                        item.Quantity -= quantityToSell;

                        // Delete if stock drops to 0
                        if (item.Quantity <= 0)
                        {
                            db.Products.Remove(item);
                        }

                        db.SaveChanges();
                    }
                }
            });
        }
    }
}
