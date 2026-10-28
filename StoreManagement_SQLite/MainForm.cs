using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagement_SQLite
{
    public partial class MainForm : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private Button btnRefresh;
        private MenuStrip menuStripMain;

        public MainForm()
        {
            InitializeComponentsManual();
            _=LoadDataAsync();
        }

        private void InitializeComponentsManual()
        {
            this.Text = "Store Management System (SQLite)";
            this.Width = 900;
            this.Height = 550;

            // Menu Strip
            menuStripMain = new MenuStrip();
            
            ToolStripMenuItem menuAdd = new ToolStripMenuItem("Add Products");
            ToolStripMenuItem menuAddNew = new ToolStripMenuItem("Add New Product", null, MenuAddNewProduct_Click);
            ToolStripMenuItem menuAddExisting = new ToolStripMenuItem("Add Existing Quantity", null, MenuAddExistingProduct_Click);
            menuAdd.DropDownItems.Add(menuAddNew);
            menuAdd.DropDownItems.Add(menuAddExisting);

            ToolStripMenuItem menuCategory = new ToolStripMenuItem("Categories");
            ToolStripMenuItem menuManageCat = new ToolStripMenuItem("Manage Categories", null, MenuManageCategories_Click);
            menuCategory.DropDownItems.Add(menuManageCat);

            ToolStripMenuItem menuSales = new ToolStripMenuItem("Sales History");
            ToolStripMenuItem menuViewSales = new ToolStripMenuItem("View Sales History", null, MenuViewSales_Click);
            menuSales.DropDownItems.Add(menuViewSales);

            ToolStripMenuItem menuSearch = new ToolStripMenuItem("Search");
            menuSearch.Click += (s, e) => txtSearch.Focus();

            menuStripMain.Items.Add(menuAdd);
            menuStripMain.Items.Add(menuCategory);
            menuStripMain.Items.Add(menuSales);
            menuStripMain.Items.Add(menuSearch);
            this.MainMenuStrip = menuStripMain;
            this.Controls.Add(menuStripMain);

            // Search Bar & Controls Panel
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
            Label lblSearch = new Label { Text = "Search:", Top = 10, Left = 10, Width = 60 };
            txtSearch = new TextBox { Top = 7, Left = 70, Width = 200 };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button { Text = "Refresh", Top = 6, Left = 280, Width = 80 };
            btnRefresh.Click += async (s, e) => await LoadDataAsync();

            topPanel.Controls.Add(lblSearch);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnRefresh);
            this.Controls.Add(topPanel);

            // DataGridView
            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
            this.Controls.Add(dgvProducts);
            dgvProducts.BringToFront();
        }

        // Asynchronous Data Loading using Task.Run on Thread Pool
        public async Task LoadDataAsync()
        {
            btnRefresh.Enabled = false;

            var productsList = await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    return db.Products.Include(p => p.Category)
                             .Select(p => new
                             {
                                 p.Id,
                                 p.Name,
                                 p.Description,
                                 Category = p.Category != null ? p.Category.Name : "N/A",
                                 p.EntryDate,
                                 p.ExpirationDate,
                                 p.Quantity
                             }).ToList();
                }
            });

            dgvProducts.DataSource = productsList;
            btnRefresh.Enabled = true;
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();

            var filteredList = await Task.Run(() =>
            {
                using (var db = new StoreDbContext())
                {
                    return db.Products.Include(p => p.Category)
                             .Where(p => p.Name.ToLower().Contains(searchTerm) ||
                                         p.Description.ToLower().Contains(searchTerm) ||
                                         p.Category.Name.ToLower().Contains(searchTerm))
                             .Select(p => new
                             {
                                 p.Id,
                                 p.Name,
                                 p.Description,
                                 Category = p.Category != null ? p.Category.Name : "N/A",
                                 p.EntryDate,
                                 p.ExpirationDate,
                                 p.Quantity
                             }).ToList();
                }
            });

            dgvProducts.DataSource = filteredList;
        }

        private async void MenuAddNewProduct_Click(object sender, EventArgs e)
        {
            AddProductForm addForm = new AddProductForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }

        private async void MenuAddExistingProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product from the grid first.", "Notice");
                return;
            }

            int selectedProductId = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
            AddExistingQuantityForm addQtyForm = new AddExistingQuantityForm(selectedProductId);

            if (addQtyForm.ShowDialog() == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }

        private async void MenuManageCategories_Click(object sender, EventArgs e)
        {
            CategoriesForm catForm = new CategoriesForm();
            catForm.ShowDialog();
            await LoadDataAsync();
        }

        private void MenuViewSales_Click(object sender, EventArgs e)
        {
            SalesHistoryForm salesForm = new SalesHistoryForm();
            salesForm.ShowDialog();
        }

        private async void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int selectedProductId = (int)dgvProducts.Rows[e.RowIndex].Cells["Id"].Value;

            using (var db = new StoreDbContext())
            {
                var product = await Task.Run(() => db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == selectedProductId));
                if (product != null)
                {
                    SaleForm saleForm = new SaleForm(product);
                    if (saleForm.ShowDialog() == DialogResult.OK)
                    {
                        await LoadDataAsync();
                    }
                }
            }
        }
    }
}
