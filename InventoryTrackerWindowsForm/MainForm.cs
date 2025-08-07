using InventoryTracker.Application.Usecases.Inventory;
using InventoryTracker.Domain.Inventory;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryTrackerWindowsForm
{
    public partial class MainForm : Form
    {
        private readonly GetAllItemsUseCase _getAllItemsUseCase;
        private readonly AddItemUseCase _addItemUseCase;
        private readonly GetItemByIdUseCase _getItemByIdUseCase;
        private readonly DeleteItemByIdUseCase _deleteItemByIdUseCase;

        public MainForm(
            GetAllItemsUseCase getAllItemsUseCase,
            AddItemUseCase addItemUseCase,
            GetItemByIdUseCase getItemByIdUseCase,
            DeleteItemByIdUseCase deleteItemByIdUseCase)
        {
            InitializeComponent();
            _getAllItemsUseCase = getAllItemsUseCase;
            _addItemUseCase = addItemUseCase;
            _getItemByIdUseCase = getItemByIdUseCase;
            _deleteItemByIdUseCase = deleteItemByIdUseCase;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadItemsAsync();
        }

        private async Task LoadItemsAsync()
        {
            var items = (await _getAllItemsUseCase.Execute()).OrderBy(i => i.Price).ToList();
            dataGridView1.DataSource = items;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                !int.TryParse(txtQuantity.Text, out var quantity) ||
                !decimal.TryParse(txtPrice.Text, out var price))
            {
                MessageBox.Show("Please enter valid name, quantity, and price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await _addItemUseCase.Execute(
                txtName.Text.Trim(),
                quantity,
                price,
                txtCategory.Text.Trim()
            );

            MessageBox.Show("Item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadItemsAsync();
        }

        private async void btnViewById_Click(object sender, EventArgs e)
        {
            var id = txtId.Text.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Please enter an ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var item = await _getItemByIdUseCase.Execute(id);
            if (item == null)
            {
                MessageBox.Show("Item not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                $"ID: {item.Id}\nName: {item.Name}\nQuantity: {item.Quantity}\nPrice: {item.Price:C}\nCategory: {item.Category}\nCreated: {item.CreatedAt:g}\nUpdated: {item.UpdatedAt:g}",
                "Item Details",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private async void btnRemove_Click(object sender, EventArgs e)
        {
            var id = txtId.Text.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Please enter an ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                await _deleteItemByIdUseCase.Execute(id);
                MessageBox.Show("Item removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
