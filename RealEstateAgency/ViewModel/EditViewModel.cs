using RealEstateAgency.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateAgency.ViewModel
{
    public class EditViewModel : BindableBase
    {
        public EditViewModel(IWindowController controller, AgencySQLDb DataBase)
        {
            dialog = new DefaultDialogService();
            windowController = controller;
            SqlDataBase = DataBase;
            TransactionsEdit = new List<DataRow>();
            OfficeEmployeeEdit = new List<DataRow>();
            OfficeEdit = new List<DataRow>();
            SeekerEdit = new List<DataRow>();
            SaleEdit = new List<DataRow>();
            EmployeeEdit = new List<DataRow>();
            RefreshData();
        }

        private async void RefreshData()
        {
            Info = true;
            await Task.Run(() =>
            {
                lock (SqlDataBase)
                {

                    TransactionData = null;
                    TransactionData = SqlDataBase.GetTable(CurrentPurchase);
                    EmployeeData = SqlDataBase.GetTable("Employee");
                    SaleData = SqlDataBase.GetTable("ClientSale");
                    SeekerData = SqlDataBase.GetTable("ClientSeeker");
                    OfficeData = SqlDataBase.GetTable("Office");
                    OfficeEmployeeData = SqlDataBase.GetTable("OfficeEmployee");

                    TransactionData.RowChanged += TransactionData_RowChanged;
                    EmployeeData.RowChanged += EmployeeData_RowChanged;
                    SaleData.RowChanged += SaleData_RowChanged;
                    SeekerData.RowChanged += SeekerData_RowChanged;
                    OfficeData.RowChanged += OfficeData_RowChanged;
                    OfficeEmployeeData.RowChanged += OfficeEmployee_RowChanged;
                }
            });
            Info = false;
        }
        private async void TRefreshData()
        {
            Info = true;
            await Task.Run(() =>
            {
                lock (SqlDataBase)
                {
                    TransactionData = null;
                    TransactionData = SqlDataBase.GetTable(CurrentPurchase);
                    TransactionData.RowChanged += TransactionData_RowChanged;
                }
            });
            if (TransactionData != null)
                Info = false;
        }

        #region Properties
        public List<DataRow> TransactionsEdit { get; set; }
        public List<DataRow> OfficeEmployeeEdit { get; set; }
        public List<DataRow> OfficeEdit { get; set; }
        public List<DataRow> SeekerEdit { get; set; }
        public List<DataRow> SaleEdit { get; set; }
        public List<DataRow> EmployeeEdit { get; set; }

        private Purchase _currentPurchase;
        public Purchase CurrentPurchase
        {
            get => _currentPurchase;
            set
            {
                _currentPurchase = value;
                TRefreshData();
                OnPropertyChange("CurrentPurchase");
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

        private readonly IWindowController windowController;
        private readonly IDialogService dialog;
        private AgencySQLDb SqlDataBase;
        #endregion

        #region events

        private void OfficeEmployee_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (OfficeEmployeeEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                OfficeEmployeeEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            OfficeEmployeeEdit.Add(e.Row);
            CanAddOfficeEmployeeRow = true;
            OnPropertyChange("OfficeEmployeeEdit");
        }
        private void OfficeData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (OfficeEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                OfficeEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            OfficeEdit.Add(e.Row);
            OnPropertyChange("OfficeEdit");
        }
        private void SeekerData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (SeekerEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                SeekerEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            SeekerEdit.Add(e.Row);
            OnPropertyChange("SeekerEdit");
        }
        private void SaleData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (SaleEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                SaleEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            SaleEdit.Add(e.Row);
            OnPropertyChange("SaleEdit");
        }
        private void EmployeeData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (EmployeeEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                EmployeeEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            EmployeeEdit.Add(e.Row);
            OnPropertyChange("EmployeeEdit");
        }
        private void TransactionData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (TransactionsEdit.Where(i => (int)i[0] == (int)e.Row[0]).Count() > 0)
                TransactionsEdit.RemoveAll(i => (int)i[0] == (int)e.Row[0]);
            TransactionsEdit.Add(e.Row);
            CanAddTransactionRow = true;
            OnPropertyChange("TransactionsEdit");
        }
        #endregion

        #region dataTable
        private DataTable _officeData;
        private DataTable _transactionData;
        private DataTable _seekerData;
        private DataTable _employeeData;
        private DataTable _saleData;
        private DataTable _OfficeEmployeeData;
        public DataTable EmployeeData
        {
            get => _employeeData;
            set
            {
                _employeeData = value;
                OnPropertyChange("EmployeeData");
            }
        }
        public DataTable SaleData
        {
            get => _saleData;
            set
            {
                _saleData = value;
                OnPropertyChange("SaleData");
            }
        }
        public DataTable SeekerData
        {
            get => _seekerData;
            set
            {
                _seekerData = value;
                OnPropertyChange("SeekerData");
            }
        }
        public DataTable TransactionData
        {
            get => _transactionData;
            set
            {
                _transactionData = value;
                TransactionsEdit.Clear();
                OnPropertyChange("TransactionsEdit");
                OnPropertyChange("TransactionData");
            }
        }
        public DataTable OfficeData
        {
            get => _officeData;
            set
            {
                _officeData = value;
                OnPropertyChange("OfficeData");
            }
        }
        public DataTable OfficeEmployeeData
        {
            get => _OfficeEmployeeData;
            set
            {
                _OfficeEmployeeData = value;
                OnPropertyChange("OfficeEmployeeData");
            }
        }
        #endregion

        #region photo
        private ObjectRelayCommand<object> _newPhoto;
        public ObjectRelayCommand<object> NewPhoto
        {
            get
            {
                return _newPhoto ?? (_newPhoto = new ObjectRelayCommand<object>(param => NewPhotoInsert(param)));
            }
        }
        private void NewPhotoInsert(object param)
        {
            if (dialog.OpenFileDialog())
            {
                if (param is DataRowView)
                {
                    DataRow row = ((DataRowView)param).Row;
                    row["Image"] = dialog.FilePath.ToByteArray();
                }
                else
                {
                    DataRow newRow = TransactionData.NewRow();
                    newRow["Image"] = dialog.FilePath.ToByteArray();
                    newRow["ClientSaleId"] = 1;
                    newRow["EmployeeId"] = 1;
                    newRow["TypeTransaction"] = "Продам";
                    newRow["Date"] = DateTime.Now;
                    TransactionData.Rows.Add(newRow);
                }
            }
        }
        #endregion

        #region UpdateCommands
        private RelayCommand _updateTransactionData;
        public RelayCommand UpdateTransactionData => _updateTransactionData ?? (_updateTransactionData = new RelayCommand(UpdateTransactionDataMethod));
        private void UpdateTransactionDataMethod()
        {
            switch (CurrentPurchase)
            {
                case Purchase.Flat:
                    {
                        foreach (DataRow row in TransactionsEdit)
                            if (SqlDataBase.HaveEntryInTable(row, "ApartamentTransaction"))
                                SqlDataBase.UpdateApartamentTransaction(row);
                            else SqlDataBase.InsertIntoApartamentTransaction(row);
                        break;
                    }
                case Purchase.Home:
                    {
                        foreach (DataRow row in TransactionsEdit)
                            if (SqlDataBase.HaveEntryInTable(row, "HomeTransaction"))
                                SqlDataBase.UpdateHomeTransaction(row);
                            else SqlDataBase.InsertIntoHomeTransaction(row);
                        break;
                    }
                case Purchase.Site:
                    {
                        foreach (DataRow row in TransactionsEdit)
                            if (SqlDataBase.HaveEntryInTable(row, "SiteTransaction"))
                                SqlDataBase.UpdateSiteTransaction(row);
                            else SqlDataBase.InsertIntoSiteTransaction(row);
                        break;
                    }
                case Purchase.Commerce:
                    {
                        foreach (DataRow row in TransactionsEdit)
                            if (SqlDataBase.HaveEntryInTable(row, "CommerceTransaction"))
                                SqlDataBase.UpdateCommerceTransaction(row);
                            else SqlDataBase.InsertIntoCommerceTransaction(row);
                        break;
                    }
            }
            TransactionsEdit.Clear();
            OnPropertyChange("TransactionsEdit");
            TRefreshData();
        }
        private RelayCommand _resetTransactionData;
        public RelayCommand ResetTransactionData => _resetTransactionData ?? (_resetTransactionData = new RelayCommand(ResetTransactionDataMethod));
        private void ResetTransactionDataMethod()
        {
            TransactionsEdit.Clear();
            OnPropertyChange("TransactionsEdit");
            TRefreshData();
        }

        private RelayCommand _updateOfficeEmployeeData;
        public RelayCommand UpdateOfficeEmployeeData => _updateOfficeEmployeeData ?? (_updateOfficeEmployeeData = new RelayCommand(UpdateOfficeEmployeeDataMethod));
        private void UpdateOfficeEmployeeDataMethod()
        {
            foreach (DataRow row in OfficeEmployeeEdit)
                if (SqlDataBase.HaveEntryInTable(row, "OfficeEmployee"))
                    SqlDataBase.UpdateOfficeEmployee(row);
                else SqlDataBase.InsertIntoOfficeEmployee(row);
            OfficeEmployeeEdit.Clear();
            OnPropertyChange("OfficeEmployeeEdit");
            OfficeEmployeeData = SqlDataBase.GetTable("OfficeEmployee");
            OfficeEmployeeData.RowChanged += OfficeEmployee_RowChanged;
        }
        private RelayCommand _resetOfficeEmployeeData;
        public RelayCommand ResetOfficeEmployeeData => _resetOfficeEmployeeData ?? (_resetOfficeEmployeeData = new RelayCommand(ResetOfficeEmployeeDataMethod));
        private void ResetOfficeEmployeeDataMethod()
        {
            OfficeEmployeeEdit.Clear();
            OnPropertyChange("OfficeEmployeeEdit");
            OfficeEmployeeData = SqlDataBase.GetTable("OfficeEmployee");
            OfficeEmployeeData.RowChanged += OfficeEmployee_RowChanged;
        }

        private RelayCommand _updateSeekerData;
        public RelayCommand UpdateSeekerData => _updateSeekerData ?? (_updateSeekerData = new RelayCommand(UpdateSeekerDataMethod));
        private void UpdateSeekerDataMethod()
        {
            foreach (DataRow row in SeekerEdit)
                if (SqlDataBase.HaveEntryInTable(row, "ClientSeeker")) SqlDataBase.UpdateClientSeeker(row);
                else SqlDataBase.InsertIntoClientSeeker(row);
            SeekerEdit.Clear();
            OnPropertyChange("SeekerEdit");
            SeekerData = SqlDataBase.GetTable("ClientSeeker");
            SeekerData.RowChanged += SeekerData_RowChanged;
        }
        private RelayCommand _resetSeekerData;
        public RelayCommand ResetSeekerData => _resetSeekerData ?? (_resetSeekerData = new RelayCommand(ResetSeekerDataMethod));
        private void ResetSeekerDataMethod()
        {
            SeekerEdit.Clear();
            OnPropertyChange("SeekerEdit");
            SeekerData = SqlDataBase.GetTable("ClientSeeker");
            SeekerData.RowChanged += SeekerData_RowChanged;
        }

        private RelayCommand _updateOfficeData;
        public RelayCommand UpdateOfficeData => _updateOfficeData ?? (_updateOfficeData = new RelayCommand(UpdateOfficeDataMethod));
        private void UpdateOfficeDataMethod()
        {
            foreach (DataRow row in OfficeEdit)
                if (SqlDataBase.HaveEntryInTable(row, "Office"))
                    SqlDataBase.UpdateOffice(row);
                else SqlDataBase.InsertIntoOffice(row);
            OfficeEdit.Clear();
            OnPropertyChange("OfficeEdit");
            OfficeData = SqlDataBase.GetTable("Office");
            OfficeData.RowChanged += OfficeData_RowChanged;
        }
        private RelayCommand _resetOfficeData;
        public RelayCommand ResetOfficeData => _resetOfficeData ?? (_resetOfficeData = new RelayCommand(ResetOfficeDataMethod));
        private void ResetOfficeDataMethod()
        {
            OfficeEdit.Clear();
            OnPropertyChange("OfficeEdit");
            OfficeData = SqlDataBase.GetTable("Office");
            OfficeData.RowChanged += OfficeData_RowChanged;
        }

        private RelayCommand _updateSaleData;
        public RelayCommand UpdateSaleData => _updateSaleData ?? (_updateSaleData = new RelayCommand(UpdateSaleDataMethod));
        private void UpdateSaleDataMethod()
        {
            foreach (DataRow row in SaleEdit)
                if (SqlDataBase.HaveEntryInTable(row, "ClientSale"))
                    SqlDataBase.UpdateClientSale(row);
                else SqlDataBase.InsertIntoClientSale(row);
            SaleEdit.Clear();
            OnPropertyChange("SaleEdit");
            SaleData = SqlDataBase.GetTable("ClientSale");
            SaleData.RowChanged += SaleData_RowChanged;
        }
        private RelayCommand _resetSaleData;
        public RelayCommand ResetSaleData => _resetSaleData ?? (_resetSaleData = new RelayCommand(ResetSaleDataMethod));
        private void ResetSaleDataMethod()
        {
            SaleEdit.Clear();
            OnPropertyChange("SaleEdit");
            SaleData = SqlDataBase.GetTable("ClientSale");
            SaleData.RowChanged += SaleData_RowChanged;
        }

        private RelayCommand _updateEmployeeData;
        public RelayCommand UpdateEmployeeData => _updateEmployeeData ?? (_updateEmployeeData = new RelayCommand(UpdateEmployeeDataMethod));
        private void UpdateEmployeeDataMethod()
        {
            foreach (DataRow row in EmployeeEdit)
                if (SqlDataBase.HaveEntryInTable(row, "Employee"))
                    SqlDataBase.UpdateEmployee(row);
                else SqlDataBase.InsertIntoEmployee(row);
            EmployeeEdit.Clear();
            OnPropertyChange("EmployeeEdit");
            EmployeeData = SqlDataBase.GetTable("Employee");
            EmployeeData.RowChanged += EmployeeData_RowChanged;
        }
        private RelayCommand _resetEmployeeData;
        public RelayCommand ResetEmployeeData => _resetEmployeeData ?? (_resetEmployeeData = new RelayCommand(ResetEmployeeDataMethod));
        private void ResetEmployeeDataMethod()
        {
            EmployeeEdit.Clear();
            OnPropertyChange("EmployeeEdit");
            EmployeeData = SqlDataBase.GetTable("Employee");
            EmployeeData.RowChanged += EmployeeData_RowChanged;
        }
        #endregion

        #region DeleteCommands
        private ObjectRelayCommand<object> _TransactionDataDelete;
        public ObjectRelayCommand<object> TransactionDataDelete => _TransactionDataDelete ?? (_TransactionDataDelete = new ObjectRelayCommand<object>(TransactionDataDeleteMethod));
        private void TransactionDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                switch (CurrentPurchase)
                {
                    case Purchase.Flat:
                        {
                            foreach (DataRow row in views.Select(i => i.Row))
                                SqlDataBase.DeleteFromTable("ApartamentTransaction", row);
                            break;
                        }
                    case Purchase.Home:
                        {
                            foreach (DataRow row in views.Select(i => i.Row))
                                SqlDataBase.DeleteFromTable("HomeTransaction", row);
                            break;
                        }
                    case Purchase.Site:
                        {
                            foreach (DataRow row in views.Select(i => i.Row))
                                SqlDataBase.DeleteFromTable("SiteTransaction", row);
                            break;
                        }
                    case Purchase.Commerce:
                        {
                            foreach (DataRow row in views.Select(i => i.Row))
                                SqlDataBase.DeleteFromTable("CommerceTransaction", row);
                            break;
                        }
                }
                TRefreshData();
            }
        }

        private ObjectRelayCommand<object> _OfficeEmployeeDataDelete;
        public ObjectRelayCommand<object> OfficeEmployeeDataDelete => _OfficeEmployeeDataDelete ?? (_OfficeEmployeeDataDelete = new ObjectRelayCommand<object>(OfficeEmployeeDataDeleteMethod));
        private void OfficeEmployeeDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                foreach (DataRow row in views.Select(i => i.Row))
                    SqlDataBase.DeleteFromTable("OfficeEmployee", row);
                OfficeEmployeeData = SqlDataBase.GetTable("OfficeEmployee");
                OfficeEmployeeData.RowChanged += OfficeEmployee_RowChanged;
            }
        }

        private ObjectRelayCommand<object> _OfficeDataDelete;
        public ObjectRelayCommand<object> OfficeDataDelete => _OfficeDataDelete ?? (_OfficeDataDelete = new ObjectRelayCommand<object>(OfficeDataDeleteMethod));
        private void OfficeDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                foreach (DataRow row in views.Select(i => i.Row))
                    SqlDataBase.DeleteFromTable("Office", row);
                OfficeData = SqlDataBase.GetTable("Office");
                OfficeData.RowChanged += OfficeData_RowChanged;
            }
        }

        private ObjectRelayCommand<object> _SeekerDataDelete;
        public ObjectRelayCommand<object> SeekerDataDelete => _SeekerDataDelete ?? (_SeekerDataDelete = new ObjectRelayCommand<object>(SeekerDataDeleteMethod));
        private void SeekerDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                foreach (DataRow row in views.Select(i => i.Row))
                    SqlDataBase.DeleteFromTable("ClientSeeker", row);
                SeekerData = SqlDataBase.GetTable("ClientSeeker");
                SeekerData.RowChanged += SeekerData_RowChanged;
            }
        }

        private ObjectRelayCommand<object> _SaleDataDelete;
        public ObjectRelayCommand<object> SaleDataDelete => _SaleDataDelete ?? (_SaleDataDelete = new ObjectRelayCommand<object>(SaleDataDeleteMethod));
        private void SaleDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                foreach (DataRow row in views.Select(i => i.Row))
                    SqlDataBase.DeleteFromTable("ClientSale", row);
                SaleData = SqlDataBase.GetTable("ClientSale");
                SaleData.RowChanged += SaleData_RowChanged;
            }
        }

        private ObjectRelayCommand<object> _EmployeeDataDelete;
        public ObjectRelayCommand<object> EmployeeDataDelete => _EmployeeDataDelete ?? (_EmployeeDataDelete = new ObjectRelayCommand<object>(EmployeeDataDeleteMethod));
        private void EmployeeDataDeleteMethod(object o)
        {
            List<DataRowView> views = new List<DataRowView>();
            if (o is IEnumerable enumerable)
                foreach (object element in enumerable)
                    views.Add(element as DataRowView);
            if (dialog.QuestionToUser($"Удалить {views.Count} элементов?"))
            {
                foreach (DataRow row in views.Select(i => i.Row))
                    SqlDataBase.DeleteFromTable("Employee", row);
                EmployeeData = SqlDataBase.GetTable("Employee");
                EmployeeData.RowChanged += EmployeeData_RowChanged;
            }
        }
        #endregion

        #region Exit
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

        #region NewRow
        public object NewOfficeEmployeeRow
        {
            set
            {
                if (value != null)
                    if (value is DataRowView) return;
                    else if (CanAddOfficeEmployeeRow)
                    {
                        CanAddOfficeEmployeeRow = false;
                        DataRow row = OfficeEmployeeData.NewRow();
                        row["OfficeId"] = 1;
                        row["EmployeeId"] = 1;
                        OfficeEmployeeData.Rows.Add(row);
                    }
            }
        }
        private bool CanAddOfficeEmployeeRow = true;

        public object NewTransactionRow
        {
            set
            {
                if (value != null)
                    if (value is DataRowView) return;
                    else if (CanAddTransactionRow)
                    {
                        CanAddTransactionRow = false;
                        DataRow newRow = TransactionData.NewRow();
                        newRow["ClientSaleId"] = 1;
                        newRow["EmployeeId"] = 1;
                        newRow["TypeTransaction"] = "Продам";
                        newRow["Date"] = DateTime.Now;
                        TransactionData.Rows.Add(newRow);
                    }
            }
        }
        private bool CanAddTransactionRow = true;
        #endregion

    }
}
