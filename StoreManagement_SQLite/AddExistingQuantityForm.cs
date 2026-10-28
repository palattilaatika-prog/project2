using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public class AddExistingQuantityForm : Form
    {
        private int _productId;
        private NumericUpDown numAddQty = new NumericUpDown { Top = 60, Left = 130, Width = 120, Minimum = 1, Maximum = 10000 };
        private Button btnAdd = new Button { Text = "Add Stock", Top = 100, Left = 130, DialogResult = DialogResult.OK };

        public AddExistingQuantityForm(int productId)
        {
            _productId = productId;
            this.Text = "Add Quantity to Existing Product";
            this.Size = new System.Drawing.Size(300, 190);
            this.StartPosition = FormStartPosition.CenterParent;

            this.Controls.Add(new Label { Text = $"Product ID: {_productId}", Top = 20, Left = 20, AutoSize = true });
            this.Controls.Add(new Label { Text = "Add Quantity:", Top = 60, Left = 20 });
            this.Controls.Add(numAddQty);
            this.Controls.Add(btnAdd);

            btnAdd.Click += async (s, e) => await IncreaseQuantityAsync();
        }

        private async Task IncreaseQuantityAsync()
        {
            int qtyToAdd = (int)numAddQty.Value;

            await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    var product = db.Products.Find(_productId);
                    if (product != null)
                    {
                        product.Quantity += qtyToAdd;
                        db.SaveChanges();
                    }
                }
            });
        }
    }
}
