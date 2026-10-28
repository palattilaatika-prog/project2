using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public class CategoriesForm : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtCategoryName;
        private Button btnAddCategory;

        public CategoriesForm()
        {
            this.Text = "Manage Product Categories";
            this.Size = new System.Drawing.Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            topPanel.Controls.Add(new Label { Text = "Category Name:", Top = 15, Left = 10, Width = 100 });
            txtCategoryName = new TextBox { Top = 12, Left = 115, Width = 160 };
            btnAddCategory = new Button { Text = "Add", Top = 10, Left = 285, Width = 80 };
            btnAddCategory.Click += async (s, e) => await AddCategoryAsync();

            topPanel.Controls.Add(txtCategoryName);
            topPanel.Controls.Add(btnAddCategory);
            this.Controls.Add(topPanel);

            dgvCategories = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            this.Controls.Add(dgvCategories);
            dgvCategories.BringToFront();

            LoadCategoriesAsync();
        }

        private async void LoadCategoriesAsync()
        {
            var list = await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    return db.Categories.Select(c => new { c.Id, c.Name }).ToList();
                }
            });

            dgvCategories.DataSource = list;
        }

        private async Task AddCategoryAsync()
        {
            string name = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a category name.");
                return;
            }

            await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    db.Categories.Add(new ProductCategory { Name = name });
                    db.SaveChanges();
                }
            });

            txtCategoryName.Clear();
            LoadCategoriesAsync();
        }
    }
}
