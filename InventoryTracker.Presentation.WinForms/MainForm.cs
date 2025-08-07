using InventoryTracker.Application.Usecases.Inventory;
using System.Windows.Forms;

namespace InventoryTracker.Presentation.WinForms
{
    public partial class MainForm : Form
    {
        private readonly GetAllItemsUseCase _getAllItemsUseCase;
        private readonly AddItemUseCase _addItemUseCase;
        private readonly GetItemByIdUseCase _getItemByIdUseCase;

        public MainForm(
            GetAllItemsUseCase getAllItemsUseCase,
            AddItemUseCase addItemUseCase,
            GetItemByIdUseCase getItemByIdUseCase)
        {
            InitializeComponent();
            _getAllItemsUseCase = getAllItemsUseCase;
            _addItemUseCase = addItemUseCase;
            _getItemByIdUseCase = getItemByIdUseCase;
        }

        // Add your UI logic here (e.g., buttons to list/add/view items)
    }
}