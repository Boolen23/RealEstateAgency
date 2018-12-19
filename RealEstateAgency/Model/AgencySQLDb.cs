using RealEstateAgency.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RealEstateAgency.ViewModel
{
    public class AgencySQLDb
    {
        public AgencySQLDb()
        {
            conn = new SqlConnection(connStr);
        }
        public bool TryConnect()
        {
            try
            {
                conn.Open();
                #region forGenerateData
                ClientId = GetfromTable("Id", "ClientSale");
                EmployeeId = GetfromTable("Id", "Employee");
                OfficeId = GetfromTable("Id", "Office");
                rn = new Random();
                #endregion
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public bool TryOpen(string path)
        {
            try
            {
                string tempStr = string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", path);
                conn = new SqlConnection(tempStr);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async void createDb()
        {
            conn.Close();
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True");
            SqlCommand cmdCreateDataBase = new SqlCommand(string.Format(@"CREATE DATABASE [{0}]" , "AgencyDb1"), conn);
            conn.Open();
            cmdCreateDataBase.ExecuteNonQuery();
            await Task.Delay(200);
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True");
            conn.Open();
        }
        public event EventHandler DBChanged;
        string connStr = string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}\AgencyDb.mdf;Integrated Security=True", Directory.GetCurrentDirectory());

        //string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\OskarSparta\source\repos\RealEstateAgency\RealEstateAgency\AgencyDb.mdf;Integrated Security=True";
        private SqlConnection conn;
        public bool IsBusy { get; private set; } = false;

        #region LoginTable
        public DataTable GetLoginData()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LoginTable;", conn);
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            return dataTable;

        }
        public void InsertIntoLoginTable(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO LoginTable (Login, Password, [Function]) VALUES (@Login, @Password, @Function)", conn);
            cmd.Parameters.AddWithValue("@Login", row["Login"]);
            cmd.Parameters.AddWithValue("@Password", row["Password"]);
            cmd.Parameters.AddWithValue("@Function", row["Function"]);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }

        public bool TryEnter(string login, string password, out User curUser)
        {
            if (login == null || password == null)
            {
                curUser = User.Client;
                return false;
            }
            SqlCommand cmd = new SqlCommand("SELECT [Function] FROM LoginTable WHERE Login = @Login AND Password = @Password ;", conn);
            cmd.Parameters.AddWithValue("@Login", login);
            cmd.Parameters.AddWithValue("@Password", password);
            string user = (string)cmd.ExecuteScalar();
            if (user != null)
            {
                if (user.Equals(User.Admin.ToString())) curUser = User.Admin;
                else curUser = User.Client;
                return true;
            }
            else
            {
                curUser = User.Client;
                return false;
            }
        }
        public bool TryRegistration(string login, string password)
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(Login) FROM LoginTable WHERE Login = @Login AND Password = @Password ;", conn);
            cmd.Parameters.AddWithValue("@Login", login);
            cmd.Parameters.AddWithValue("@Password", password);
            int LoginCount = (int)cmd.ExecuteScalar();
            if (LoginCount > 0) return false;
            else
            {
                SqlCommand update = new SqlCommand("INSERT INTO LoginTable (Login, Password, [Function]) VALUES (@Login, @Password, 'Client');", conn);
                update.Parameters.AddWithValue("@Login", login);
                update.Parameters.AddWithValue("@Password", password);
                update.ExecuteNonQuery();
                return true;
            }
        }
        public bool HaveEntry(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(Id) FROM LoginTable WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            int enterys = (int)cmd.ExecuteScalar();
            if (enterys > 0) return true;
            else return false;
        }
        public void DeleteLoginEntry(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM LoginTable WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region GetMethods  
        public DataTable GetTable(string Table)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + Table + " ;", conn);
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            return dataTable;
        }
        public DataTable GetTable(Purchase purchase)
        {
            return GetTable(GetTableByPurchase(purchase));
        }

        private DataTable ClientApartamentTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "Floor AS 'Этаж',  " +
                "ObjectAdress AS 'Адрес', " +
                "NumberOfRooms AS 'Количество комнат', " +
                "Area AS 'Площадь', " +
                "Employee.Name + SPACE(1) + Employee.SurName AS 'Специалист', " +
                "Employee.PhoneNumber AS 'Телефон', " +
                "Office.Address AS 'Адрес офиса', " +
                "Image " +
                "FROM ApartamentTransaction " +
                "JOIN Employee ON Employee.Id = ApartamentTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search, conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }
        private DataTable AdminApartamentTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "Floor AS 'Этаж',  " +
                "ObjectAdress AS 'Адрес', " +
                "NumberOfRooms AS 'Количество комнат', " +
                "Area AS 'Площадь', " +
                "CONVERT(varchar, Date, 101) AS 'Дата объявления', " +
                "Image, " +
                "ClientSale.Name AS 'Имя хозяина', " +
                "ClientSale.SurName AS 'Фамилия хозяина', " +
                "ClientSale.PhoneNumber AS 'Телефон хозяина', " +
                "Employee.Name AS 'Имя работника', " +
                "Employee.SurName AS 'Фамилия работника', " +
                "Employee.PhoneNumber AS 'Телефон работника', " +
                "Office.Address AS 'Адрес офиса' " +
                "FROM ApartamentTransaction " +
                "JOIN ClientSale ON ClientSale.Id = ApartamentTransaction.ClientSaleId " +
                "JOIN Employee ON Employee.Id = ApartamentTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }

        private DataTable AdminHomeTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "FloorNumbers AS 'Количество этажей',  " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "CONVERT(varchar, Date, 101) AS 'Дата объявления', " +
                "Image, " +
                "ClientSale.Name AS 'Имя хозяина', " +
                "ClientSale.SurName AS 'Фамилия хозяина', " +
                "ClientSale.PhoneNumber AS 'Телефон хозяина', " +
                "Employee.Name AS 'Имя работника', " +
                "Employee.SurName AS 'Фамилия работника', " +
                "Employee.PhoneNumber AS 'Телефон работника', " +
                "Office.Address AS 'Адрес офиса' " +
                "FROM HomeTransaction " +
                "JOIN ClientSale ON ClientSale.Id = HomeTransaction.ClientSaleId " +
                "JOIN Employee ON Employee.Id = HomeTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }
        private DataTable ClientHomeTransaction(Transaction transaction, string search)

        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "FloorNumbers AS 'Количество этажей',  " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "Employee.Name + SPACE(1) + Employee.SurName AS 'Специалист', " +
                "Employee.PhoneNumber AS 'Телефон', " +
                "Office.Address AS 'Адрес офиса', " +
                "Image " +
                "FROM HomeTransaction " +
                "JOIN Employee ON Employee.Id = HomeTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }

        private DataTable AdminSiteTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "Purpose AS 'Назначение',  " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "CONVERT(varchar, Date, 101) AS 'Дата объявления', " +
                "Image, " +
                "ClientSale.Name AS 'Имя хозяина', " +
                "ClientSale.SurName AS 'Фамилия хозяина', " +
                "ClientSale.PhoneNumber AS 'Телефон хозяина', " +
                "Employee.Name AS 'Имя работника', " +
                "Employee.SurName AS 'Фамилия работника', " +
                "Employee.PhoneNumber AS 'Телефон работника', " +
                "Office.Address AS 'Адрес офиса' " +
                "FROM SiteTransaction " +
                "JOIN ClientSale ON ClientSale.Id = SiteTransaction.ClientSaleId " +
                "JOIN Employee ON Employee.Id = SiteTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }
        private DataTable ClientSiteTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "Purpose AS 'Назначение',  " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "Employee.Name + SPACE(1) + Employee.SurName AS 'Специалист', " +
                "Employee.PhoneNumber AS 'Телефон', " +
                "Office.Address AS 'Адрес офиса', " +
                "Image " +
                "FROM SiteTransaction " +
                 "JOIN Employee ON Employee.Id = SiteTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }

        private DataTable AdminCommerceTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "CONVERT(varchar, Date, 101) AS 'Дата объявления', " +
                "Image, " +
                "ClientSale.Name AS 'Имя хозяина', " +
                "ClientSale.SurName AS 'Фамилия хозяина', " +
                "ClientSale.PhoneNumber AS 'Телефон хозяина', " +
                "Employee.Name AS 'Имя работника', " +
                "Employee.SurName AS 'Фамилия работника', " +
                "Employee.PhoneNumber AS 'Телефон работника', " +
                "Office.Address AS 'Адрес офиса' " +
                "FROM CommerceTransaction " +
                "JOIN ClientSale ON ClientSale.Id = CommerceTransaction.ClientSaleId " +
                "JOIN Employee ON Employee.Id = CommerceTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }
        private DataTable ClientCommerceTransaction(Transaction transaction, string search)
        {
            SqlCommand cmd = new SqlCommand("SELECT " +
                "Price / (CONVERT(Real,1000)) AS 'Цена (тыс рублей)', " +
                "ObjectAdress AS 'Адрес', " +
                "Area AS 'Площадь', " +
                "Employee.Name + SPACE(1) + Employee.SurName AS 'Специалист', " +
                "Employee.PhoneNumber AS 'Телефон', " +
                "Office.Address AS 'Адрес офиса', " +
                "Image " +
                "FROM CommerceTransaction " +
                "JOIN Employee ON Employee.Id = CommerceTransaction.EmployeeId " +
                "JOIN Office ON Employee.Id = Office.Id " +
                "WHERE TypeTransaction = @transaction" + search
                , conn);
            cmd.Parameters.AddWithValue("@transaction", ConvertTransaction(transaction));
            IsBusy = true;
            var dataReader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            IsBusy = false;
            return dataTable;
        }

        public DataTable TransactionInfo(User user, Purchase purchase, Transaction transaction, SearchSet search)
        {
            string s = UserToString(user) + GetTableByPurchase(purchase);
            MethodInfo method = GetType().GetMethod(s, BindingFlags.Instance | BindingFlags.NonPublic);
            string searchSet = Parse(purchase, search);
            DataTable table = (DataTable)method.Invoke(this, new object[] { transaction, searchSet });
            return table;
        }
        #endregion

        #region Insert
        public void InsertIntoApartamentTransaction(int saleId, int employeeId, string address, string type, int numberOfRooms, int price, int floor, int area, DateTime date, string imgPath)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ApartamentTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, NumberOfRooms, Price, Floor, Area, Date, Image)" +
                "VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction, @NumberOfRooms," +
                " @Price, @Floor, @Area, @Date, @Image)", conn);

            ImageSource imageSource = new BitmapImage(new Uri(imgPath));
            MemoryStream ms = StreamFromBitmapSource(imageSource as BitmapSource);

            cmd.Parameters.AddWithValue("@ClientSaleId", saleId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@ObjectAdress", address);
            cmd.Parameters.AddWithValue("@TypeTransaction", type);
            cmd.Parameters.AddWithValue("@NumberOfRooms", numberOfRooms);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Floor", floor);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Image", ms.ToArray());
            ms.Dispose();
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoClientSale(string name, string surName, string phoneNumber)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ClientSale (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@SurName", surName);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoClientSeeker(string name, string surName, string phoneNumber)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ClientSeeker (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@SurName", surName);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoEmployee(string name, string surName, string phoneNumber)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Employee (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@SurName", surName);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoOffice(string adress)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Office (Address) VALUES (@Address)", conn);
            cmd.Parameters.AddWithValue("@Address", adress);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoOfficeEmployee(int officeId, int employeeId)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO OfficeEmployee (OfficeId, EmployeeId) VALUES" +
                "(@OfficeId, @EmployeeId)", conn);
            cmd.Parameters.AddWithValue("@OfficeId", officeId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoHomeTransaction(int saleId, int employeeId, string address, string type, int price, int floorNumbers, int area, DateTime date, string imgPath)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HomeTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, Price, FloorNumbers, Area, Date, Image)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction," +
                " @Price, @FloorNumbers, @Area, @Date, @Image)", conn);

            ImageSource imageSource = new BitmapImage(new Uri(imgPath));
            MemoryStream ms = StreamFromBitmapSource(imageSource as BitmapSource);

            cmd.Parameters.AddWithValue("@ClientSaleId", saleId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@ObjectAdress", address);
            cmd.Parameters.AddWithValue("@TypeTransaction", type);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@FloorNumbers", floorNumbers);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Image", ms.ToArray());
            ms.Dispose();
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoSiteTransaction(int saleId, int employeeId, string address, string type, int price, string purpose, int area, DateTime date, string imgPath)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO SiteTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "Area, Purpose, Price, Date, Image, TypeTransaction)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @Area," +
                " @Purpose, @Price, @Date, @Image, @TypeTransaction)", conn);

            ImageSource imageSource = new BitmapImage(new Uri(imgPath));
            MemoryStream ms = StreamFromBitmapSource(imageSource as BitmapSource);

            cmd.Parameters.AddWithValue("@ClientSaleId", saleId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@ObjectAdress", address);
            cmd.Parameters.AddWithValue("@TypeTransaction", type);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Purpose", purpose);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Image", ms.ToArray());
            ms.Dispose();
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoCommerceTransaction(int saleId, int employeeId, string address, string type, int price, int area, DateTime date, string imgPath)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO CommerceTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, Price, Area, Date, Image)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction," +
                " @Price, @Area, @Date, @Image)", conn);

            cmd.Parameters.AddWithValue("@ClientSaleId", saleId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@ObjectAdress", address);
            cmd.Parameters.AddWithValue("@TypeTransaction", type);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Image", imgPath.ToByteArray());
            cmd.ExecuteNonQuery();
        }

        public void InsertIntoApartamentTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ApartamentTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, NumberOfRooms, Price, Floor, Area, Date, Image)" +
                "VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction, @NumberOfRooms," +
                " @Price, @Floor, @Area, @Date, @Image)", conn);

            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@NumberOfRooms", row["NumberOfRooms"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Floor", row["Floor"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoClientSeeker(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ClientSeeker (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoHomeTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HomeTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, Price, FloorNumbers, Area, Date, Image)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction," +
                " @Price, @FloorNumbers, @Area, @Date, @Image)", conn);

            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@FloorNumbers", row["FloorNumbers"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoSiteTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO SiteTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "Area, Purpose, Price, Date, Image, TypeTransaction)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @Area," +
                " @Purpose, @Price, @Date, @Image, @TypeTransaction)", conn);

            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Purpose", row["Purpose"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoCommerceTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO CommerceTransaction (ClientSaleId, EmployeeId, ObjectAdress, " +
                "TypeTransaction, Price, Area, Date, Image)" +
                " VALUES (@ClientSaleId, @EmployeeId, @ObjectAdress, @TypeTransaction," +
                " @Price, @Area, @Date, @Image)", conn);

            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void InsertIntoClientSale(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO ClientSale (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoEmployee(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Employee (Name, SurName, PhoneNumber) VALUES " +
                "(@Name, @SurName, @PhoneNumber)", conn);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoOffice(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Office (Address) VALUES (@Address)", conn);
            cmd.Parameters.AddWithValue("@Address", row["Address"]);
            cmd.ExecuteNonQuery();
        }
        public void InsertIntoOfficeEmployee(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO OfficeEmployee (OfficeId, EmployeeId) VALUES" +
                "(@OfficeId, @EmployeeId)", conn);
            cmd.Parameters.AddWithValue("@OfficeId", row["OfficeId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region generator
        private Random rn;
        public List<int> ClientId;
        public List<int> EmployeeId;
        public List<int> OfficeId;
        private List<int> GetfromTable(string columnName, string tableName)
        {
            List<int> temp = new List<int>();
            string command = string.Format("SELECT {0} FROM {1}", columnName, tableName);
            SqlCommand cmd = new SqlCommand(command, conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    temp.Add((int)reader[columnName]);
                }
            }
            return temp;
        }
        public int RandomClient => ClientId[rn.Next(ClientId.Count)];
        public int RandomEmployee => EmployeeId[rn.Next(EmployeeId.Count)];
        public int RandomOffice => OfficeId[rn.Next(OfficeId.Count)];
        #endregion

        #region update
        public void UpdateApartamentTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE ApartamentTransaction " +
                "SET " +
                "ClientSaleId = @ClientSaleId, " +
                "EmployeeId = @EmployeeId, " +
                "ObjectAdress = @ObjectAdress, " +
                "TypeTransaction = @TypeTransaction, " +
                "NumberOfRooms = @NumberOfRooms, " +
                "Price = @Price, " +
                "Floor = @Floor, " +
                "Area = @Area, " +
                "Date = @Date," +
                "Image = @Image " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@NumberOfRooms", row["NumberOfRooms"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Floor", row["Floor"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateClientSale(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE ClientSale " +
                "SET " +
                "Name = @Name, " +
                "SurName = @SurName, " +
                "PhoneNumber = @PhoneNumber " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateClientSeeker(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE ClientSeeker " +
                "SET " +
                "Name = @Name, " +
                "SurName = @SurName, " +
                "PhoneNumber = @PhoneNumber " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void UpdateEmployee(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Employee " +
                "SET " +
                "Name = @Name, " +
                "SurName = @SurName, " +
                "PhoneNumber = @PhoneNumber " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@Name", row["Name"]);
            cmd.Parameters.AddWithValue("@SurName", row["SurName"]);
            cmd.Parameters.AddWithValue("@PhoneNumber", row["PhoneNumber"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateOffice(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Office " +
                "SET " +
                "Address = @Address " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@Address", row["Address"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateOfficeEmployee(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE OfficeEmployee " +
                "SET " +
                "OfficeId = @OfficeId, " +
                "EmployeeId = @EmployeeId " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@OfficeId", row["OfficeId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateLoginTable(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE LoginTable " +
    "SET " +
    "Login = @Login, " +
    "Password = @Password, " +
     "[Function] = @Function " +
    "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@Login", row["Login"]);
            cmd.Parameters.AddWithValue("@Password", row["Password"]);
            cmd.Parameters.AddWithValue("@Function", row["Function"]);
            cmd.ExecuteNonQuery();
            DBChanged?.Invoke(null, null);
        }
        public void UpdateHomeTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE HomeTransaction " +
                "SET " +
                "ClientSaleId = @ClientSaleId, " +
                "EmployeeId = @EmployeeId, " +
                "ObjectAdress = @ObjectAdress, " +
                "TypeTransaction = @TypeTransaction, " +
                "Price = @Price, " +
                "FloorNumbers = @FloorNumbers, " +
                "Area = @Area, " +
                "Date = @Date," +
                "Image = @Image " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@FloorNumbers", row["FloorNumbers"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateSiteTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE SiteTransaction " +
                "SET " +
                "ClientSaleId = @ClientSaleId, " +
                "EmployeeId = @EmployeeId, " +
                "ObjectAdress = @ObjectAdress, " +
                "Area = @Area, " +
                "Purpose = @Purpose, " +
                "Price = @Price, " +
                "Date = @Date," +
                "Image = @Image, " +
                "TypeTransaction = @TypeTransaction " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Purpose", row["Purpose"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        public void UpdateCommerceTransaction(DataRow row)
        {
            SqlCommand cmd = new SqlCommand("UPDATE CommerceTransaction " +
                "SET " +
                "ClientSaleId = @ClientSaleId, " +
                "EmployeeId = @EmployeeId, " +
                "ObjectAdress = @ObjectAdress, " +
                "TypeTransaction = @TypeTransaction, " +
                "Price = @Price, " +
                "Area = @Area, " +
                "Date = @Date," +
                "Image = @Image " +
                "WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.Parameters.AddWithValue("@ClientSaleId", row["ClientSaleId"]);
            cmd.Parameters.AddWithValue("@EmployeeId", row["EmployeeId"]);
            cmd.Parameters.AddWithValue("@ObjectAdress", row["ObjectAdress"]);
            cmd.Parameters.AddWithValue("@TypeTransaction", row["TypeTransaction"]);
            cmd.Parameters.AddWithValue("@Price", row["Price"]);
            cmd.Parameters.AddWithValue("@Area", row["Area"]);
            cmd.Parameters.AddWithValue("@Date", row["Date"]);
            cmd.Parameters.AddWithValue("@Image", row["Image"]);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region parse
        private string Parse(Purchase purchase, SearchSet set)
        {
            if (set.MinPrice == null) return string.Empty;
            switch (purchase)
            {
                case Purchase.Flat: return ParseFlatSet(set);
                case Purchase.Home: return ParseHomeSet(set);
                case Purchase.Site: return ParseSiteCommerceSet(set);
                case Purchase.Commerce: return ParseSiteCommerceSet(set);
            }
            throw new Exception("Something went wrong");
        }
        private string ParseFlatSet(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(And);
            sb.Append(ParsePrice(set));
            sb.Append(And);
            sb.Append(ParseArea(set));
            sb.Append(And);
            sb.Append(ParseFloor(set));
            sb.Append(And);
            sb.Append(ParseRooms(set));
            sb.Append(";");
            return sb.ToString();
        }
        private string ParseSiteCommerceSet(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(And);
            sb.Append(ParsePrice(set));
            sb.Append(And);
            sb.Append(ParseArea(set));
            sb.Append(";");
            return sb.ToString();
        }
        private string ParseHomeSet(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(And);
            sb.Append(ParsePrice(set));
            sb.Append(And);
            sb.Append(ParseArea(set));
            sb.Append(And);
            sb.Append(ParseFloorNumbers(set));
            sb.Append(";");
            return sb.ToString();
        }
        private string ParsePrice(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            if (set.MaxPrice != null)
            {
                if (set.MaxPrice == set.MinPrice) sb.Append("Price = " + set.MaxPrice * 1000);
                else sb.Append("Price >= " + set.MinPrice * 1000 + And + "Price <= " + set.MaxPrice * 1000);
            }
            else sb.Append("Price >= " + set.MinPrice * 1000);
            sb.Append(" ");
            return sb.ToString();
        }
        private string ParseArea(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            if (set.MaxArea != null)
            {
                if (set.MaxArea == set.MinArea) sb.Append("Area = " + set.MaxArea);
                else sb.Append("Area >= " + set.MinArea + And + "Area <= " + set.MaxArea);
            }
            else sb.Append("Area >= " + set.MinArea);
            sb.Append(" ");
            return sb.ToString();
        }
        private string ParseFloor(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            if (set.MaxFloor != null)
            {
                if (set.MaxFloor == set.MinFloor) sb.Append("Floor = " + set.MaxFloor);
                else sb.Append("Floor >= " + set.MinFloor + And + "Floor <= " + set.MaxFloor);
            }
            else sb.Append("Floor >= " + set.MinFloor);
            sb.Append(" ");
            return sb.ToString();
        }
        private string ParseRooms(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            if (set.MaxRooms != null)
            {
                if (set.MaxRooms == set.MinRooms) sb.Append("NumberOfRooms = " + set.MaxRooms);
                else sb.Append("NumberOfRooms >= " + set.MinRooms + And + "NumberOfRooms <= " + set.MaxRooms);
            }
            else sb.Append("NumberOfRooms >= " + set.MinRooms);
            sb.Append(" ");
            return sb.ToString();
        }
        private string ParseFloorNumbers(SearchSet set)
        {
            StringBuilder sb = new StringBuilder();
            if (set.MaxFloorNumbers != null)
            {
                if (set.MaxFloorNumbers == set.MinFloorNumbers) sb.Append("FloorNumbers = " + set.MaxFloorNumbers);
                else sb.Append("FloorNumbers >= " + set.MinFloorNumbers + And + "FloorNumbers <= " + set.MaxFloorNumbers);
            }
            else sb.Append("FloorNumbers >= " + set.MinFloorNumbers);
            sb.Append(" ");
            return sb.ToString();
        }
        private string And { get => " AND "; }
        #endregion

        #region OtherMethods
        private MemoryStream StreamFromBitmapSource(BitmapSource writeBmp)
        {
            MemoryStream bmp;
            using (bmp = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(writeBmp));
                enc.Save(bmp);
            }

            return bmp;
        }
        private string ConvertTransaction(Transaction transaction) => transaction.Equals(Transaction.Rent) ? "Сдам" : "Продам";
        private string UserToString(User user) => user.Equals(User.Admin) ? "Admin" : "Client";
        private string GetTableByPurchase(Purchase purchase)
        {
            switch (purchase)
            {
                case Purchase.Flat: return "ApartamentTransaction";
                case Purchase.Home: return "HomeTransaction";
                case Purchase.Site: return "SiteTransaction";
                case Purchase.Commerce: return "CommerceTransaction";
            }
            throw new Exception();
        }
        #endregion

        #region CheckMethods
        public bool Check(SqlCommand command)
        {
            int enterys = (int)command.ExecuteScalar();
            if (enterys > 0) return true;
            else return false;
        }
        public bool HaveEntryInTable(DataRow row, string Table)
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(Id) FROM " + Table + " WHERE Id = @Id ;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            return Check(cmd);
        }
        #endregion

        #region delete
        public void DeleteFromTable(string Table, DataRow row)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM " + Table + " WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", row["Id"]);
            cmd.ExecuteNonQuery();
        }
        #endregion

    }
}
