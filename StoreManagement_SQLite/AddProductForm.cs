using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public class AddProductForm : Form
    {
        private TextBox txtName = new TextBox { Top = 20, Left = 130, Width = 180, MaxLength = 30 };
        private TextBox txtDesc = new TextBox { Top = 60, Left = 130, Width = 180, Multiline = true, Height = 50, MaxLength = 500 };
        private ComboBox cbCategory = new ComboBox { Top = 120, Left = 130, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        private DateTimePicker dtpEntry = new DateTimePicker { Top = 160, Left = 130, Width = 180 };
        private DateTimePicker dtpExpiration = new DateTimePicker { Top = 200, Left = 130, Width = 180 };
        private NumericUpDown numQuantity = new NumericUpDown { Top = 240, Left = 130, Width = 180, Minimum = 1, Maximum = 10000 };
        private Button btnSave = new Button { Text = "Save", Top = 280, Left = 130, DialogResult = DialogResult.OK };

        public AddProductForm()
        {
            this.Text = "Add New Product";
            this.Size = new System.Drawing.Size(360, 370);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            this.Controls.Add(new Label { Text = "Name:", Top = 20, Left = 20 });
            this.Controls.Add(txtName);
            this.Controls.Add(new Label { Text = "Description:", Top = 60, Left = 20 });
            this.Controls.Add(txtDesc);
            this.Controls.Add(new Label { Text = "Category:", Top = 120, Left = 20 });
            this.Controls.Add(cbCategory);
            this.Controls.Add(new Label { Text = "Entry Date:", Top = 160, Left = 20 });
            this.Controls.Add(dtpEntry);
            this.Controls.Add(new Label { Text = "Expiration Date:", Top = 200, Left = 20 });
            this.Controls.Add(dtpExpiration);
            this.Controls.Add(new Label { Text = "Quantity:", Top = 240, Left = 20 });
            this.Controls.Add(numQuantity);
            this.Controls.Add(btnSave);

            LoadCategoriesAsync();

            btnSave.Click += async (s, e) => await SaveProductAsync();
        }

        private async void LoadCategoriesAsync()
        {
            var categories = await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    return db.Categories.ToList();
                }
            });

            cbCategory.DataSource = categories;
            cbCategory.DisplayMember = "Name";
            cbCategory.ValueMember = "Id";
        }

        private async Task SaveProductAsync()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Please fill in the product name and select a category.");
                this.DialogResult = DialogResult.None;
                return;
            }

            var newProduct = new Product
            {
                Name = txtName.Text.Trim(),
                Description = txtDesc.Text.Trim(),
                CategoryId = (int)cbCategory.SelectedValue,
                EntryDate = dtpEntry.Value,
                ExpirationDate = dtpExpiration.Value,
                Quantity = (int)numQuantity.Value
            };

            await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    db.Products.Add(newProduct);
                    db.SaveChanges();
                }
            });
        }
    }
}
