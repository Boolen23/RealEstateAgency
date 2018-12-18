using RealEstateAgency.Model;
using System;
using System.Data;
using System.Threading.Tasks;
using static RealEstateAgency.ViewModel.SQLDataGenerator;

namespace RealEstateAgency.ViewModel
{
    public class SupplyViewModel : BindableBase
    {
        public SupplyViewModel(IWindowController controller, AgencySQLDb sql, User user, string UserName)
        {
            CurrentUser = user;
            this.UserName = UserName;
            windowController = controller;
            db = sql;
            searchSet = new SearchSet();
            UpdateTransactionTable();
            searchSet.SearchSetChanged += SearchSet_SearchSetChanged;
        }

        private void SearchSet_SearchSetChanged(object sender, EventArgs e)
        {
            UpdateTransactionTable();
        }
        private async void UpdateTransactionTable()
        {
            Info = true;
            await Task.Run(() =>
             {
                 lock (db)
                 {
                     TransactionData = null;
                     TransactionData = db.TransactionInfo(CurrentUser, CurrentPurchase, CurrentTransaction, searchSet);
                 }
             });
            if (TransactionData != null)
                Info = false;
        }

        #region Add
        private RelayCommand add;
        public RelayCommand Add => add ?? (add = new RelayCommand(AddMethod));
        void AddMethod()
        {
            for (int i = 0; i < 222; i++)
                db.InsertIntoHomeTransaction(db.RandomClient, db.RandomEmployee, NextAdress, RandomTransaction, NextPrice, NextFloorNumbers, NextArea, RandomDate, NextHome);
            for (int i = 0; i < 222; i++)
                db.InsertIntoApartamentTransaction(db.RandomClient, db.RandomEmployee, NextAdress, RandomTransaction, NextNumberOfRooms, NextPrice, NextFloor, NextArea, RandomDate, NextFlat);
            for (int i = 0; i < 222; i++)
                db.InsertIntoSiteTransaction(db.RandomClient, db.RandomEmployee, NextAdress, RandomTransaction, NextPrice, NextPurpose, NextArea, RandomDate, NextSite);
            for (int i = 0; i <1; i++)
                db.InsertIntoCommerceTransaction(db.RandomClient, db.RandomEmployee, NextAdress, RandomTransaction, NextPrice, NextArea, RandomDate, NextCommerce);
            UpdateTransactionTable();
        }
        #endregion

        #region Properties
        private Transaction _currentTransaction;
        public Transaction CurrentTransaction
        {
            get => _currentTransaction;
            set
            {
                _currentTransaction = value;
                UpdateTransactionTable();
                OnPropertyChange("CurrentTransaction");
            }
        }

        private Purchase _currentPurchase;
        public Purchase CurrentPurchase
        {
            get => _currentPurchase;
            set
            {
                _currentPurchase = value;
                UpdateTransactionTable();
                OnPropertyChange("CurrentPurchase");
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChange("CurrentUser");
            }
        }

        private DataTable _TransactionData;
        public DataTable TransactionData
        {
            get => _TransactionData;
            set
            {
                _TransactionData = value;
                OnPropertyChange("TransactionData");
                OnPropertyChange("DataLenght");
            }
        }

        private bool _info;
        public bool Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChange("Info");
            }
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set { userName = value; OnPropertyChange("UserName"); }
        }

        public SearchSet searchSet { get; set; }

        public string DataLenght => TransactionData is null ? null : TransactionData.Rows.Count.ToString() + " предложений для Вас!";
        public AgencySQLDb db { get; private set; }
        private readonly IWindowController windowController;
        #endregion

        #region Commands
        private RelayCommand _ExitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                    _ExitCommand = new RelayCommand(ExitCommandMethod);
                return _ExitCommand;
            }
        }
        private void ExitCommandMethod()
        {
            windowController.CreateAccessWindow(windowController);
        }
        #endregion

    }
}
