using RealEstateAgency.Model;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RealEstateAgency.ViewModel
{
    public class AccessViewModel : BindableBase
    {
        public AccessViewModel(IWindowController windowFactory)
        {
            this.windowFactory = windowFactory;
            RegistrationMode = false;
            dialogService = new DefaultDialogService();
            InitializeDB();
        }
        private async void InitializeDB()
        {
            ErrorText = "Установка связи с БД...";
            await Task.Run(() => SQLDb = new AgencySQLDb());
            ErrorText = "Загрузка данных...";
            UserData = SQLDb.GetLoginData();
            UserData.RowChanged += UserData_RowChanged;
            Login = "Admin";
            Password = "Admin";
            ErrorText = null;
        }
        private async void UserData_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (SQLDb.HaveEntry((int)e.Row["Id"]))
            {
                SQLDb.UpdateLoginTable(e.Row);
                await Task.Delay(500);
                UserData = SQLDb.GetLoginData();
                UserData.RowChanged += UserData_RowChanged;
            }
        }

        private AgencySQLDb SQLDb;
        private readonly IWindowController windowFactory;
        private readonly IDialogService dialogService;


        #region Propertyes
        private DataRow _SelectedLoginRow;
        public DataRow SelectedLoginRow
        {
            get => _SelectedLoginRow;
            set
            {
                _SelectedLoginRow = value;
                if (_SelectedLoginRow != null)
                    if (_SelectedLoginRow.ItemArray.Length == 0) MultyActionText = "Добавить";
                    else
                    {
                        int Id = (int)_SelectedLoginRow.ItemArray[0];
                        if (SQLDb.HaveEntry(Id)) MultyActionText = "Удалить";
                        else MultyActionText = "Добавить";
                    }
                OnPropertyChange("SelectedLoginRow");
            }
        }

        private DataTable userData;
        public DataTable UserData
        {
            get => userData;
            set
            {
                if (userData != null)
                    userData.Dispose();
                userData = value;
                OnPropertyChange("UserData");
            }
        }

        private string errorText;
        public string ErrorText
        {
            get => errorText;
            set
            {
                errorText = value;
                OnPropertyChange("ErrorText");
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                ErrorText = string.Empty;
                OnPropertyChange("Password");
            }
        }

        private string login;
        public string Login
        {
            get => login;
            set
            {
                login = value;
                ErrorText = string.Empty;
                OnPropertyChange("Login");
            }
        }

        private bool registrationMode;
        public bool RegistrationMode
        {
            get => registrationMode;
            set
            {
                registrationMode = value;
                Login = string.Empty;
                Password = string.Empty;
                ErrorText = string.Empty;
                OnPropertyChange("RegistrationMode");
                OnPropertyChange("EnterText");
            }
        }

        public string EnterText => RegistrationMode ? "Зарегистрировать и войти" : "Войти";

        private bool adminMode;
        public bool AdminMode
        {
            get => adminMode;
            set
            {
                adminMode = value;
                OnPropertyChange("AdminMode");
                UserData = SQLDb.GetLoginData();
                UserData.RowChanged += UserData_RowChanged;
            }
        }

        private string _MultyActionText;
        public string MultyActionText
        {
            get => _MultyActionText;
            set
            {
                _MultyActionText = value;
                OnPropertyChange("MultyActionText");
            }
        }

        #endregion

        #region Commands
        private RelayCommand generateUserData;
        public RelayCommand GenerateUserData
        {
            get
            {
                if (generateUserData == null)
                    generateUserData = new RelayCommand(generateUserDataMethod);
                return generateUserData;
            }
        }
        private void generateUserDataMethod()
        {
            Random rn = new Random();
            Password = NewPassword(rn);
            Login = "Nik" + rn.Next(10, 100);
        }
        private string NewPassword(Random r)
        {
            string pass = "";
            while (pass.Length < 10)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }

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
            Login = null;
            Password = null;
            AdminMode = false;
        }

        private RelayCommand _MultyActionCommand;
        public RelayCommand MultyActionCommand
        {
            get
            {
                if (_MultyActionCommand == null)
                    _MultyActionCommand = new RelayCommand(MultyActionCommandMethod);
                return _MultyActionCommand;
            }
        }
        private void MultyActionCommandMethod()
        {
            if (SelectedLoginRow.ItemArray.Length > 0)
            {
                int Id = (int)SelectedLoginRow.ItemArray[0];
                if (!SQLDb.HaveEntry(Id))
                {
                    if (!SelectedLoginRow["Function"].Equals("Admin"))
                        SelectedLoginRow["Function"] = User.Client;
                    if (SelectedLoginRow["Login"].ToString().Length > 0 &&
                        SelectedLoginRow["Password"].ToString().Length > 0)
                        SQLDb.InsertIntoLoginTable(SelectedLoginRow);
                    else dialogService.ShowMessage("Имя пользователя или пароль не введены!");
                }
                else SQLDb.DeleteLoginEntry(Id);

                UserData = SQLDb.GetLoginData();
                UserData.RowChanged += UserData_RowChanged;
            }
            else dialogService.ShowMessage("Имя пользователя или пароль не введены!");
        }

        private RelayCommand showSupplWindow;
        public RelayCommand ShowSupplyWindow
        {
            get
            {
                if (showSupplWindow == null)
                    showSupplWindow = new RelayCommand(supplywindow);
                return showSupplWindow;
            }
        }
        private void supplywindow()
        {
            windowFactory.CreateSupplyWindow(windowFactory, SQLDb, User.Admin, Login);
        }

        private RelayCommand enterLogic;
        public RelayCommand EnterLogic
        {
            get
            {
                if (enterLogic == null)
                    enterLogic = new RelayCommand(enterLogicMethod);
                return enterLogic;
            }
        }
        private void enterLogicMethod()
        {
            if (RegistrationMode)
            {
                if (SQLDb.TryRegistration(Login, Password)) windowFactory.CreateSupplyWindow(windowFactory, SQLDb, User.Client, Login);
                else ErrorText = "Такой пользователь уже зарегистрирован!";
            }
            else
            {
                User currentUser;
                if (SQLDb.TryEnter(Login, Password, out currentUser))
                {
                    if (currentUser == User.Admin) AdminMode = true;
                    else windowFactory.CreateSupplyWindow(windowFactory, SQLDb, currentUser, Login);
                }
                else
                {
                    ErrorText = "Неверное имя пользователя или пароль!";
                }
            }
        }

        private RelayCommand _showEditCommand;
        public RelayCommand ShowEditCommand
        {
            get
            {
                if (_showEditCommand == null)
                    _showEditCommand = new RelayCommand(ShowEdit);
                return _showEditCommand;
            }
        }
        private void ShowEdit()
        {
            windowFactory.CreateEditWindow(windowFactory, SQLDb);
        }
        #endregion




    }
}
