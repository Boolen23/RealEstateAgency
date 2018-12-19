using RealEstateAgency.Model;
using RealEstateAgency.View;
using RealEstateAgency.ViewModel;
using System.Windows;

namespace RealEstateAgency
{
    public class RealEstateWindowController : IWindowController
    {
        public RealEstateWindowController()
        {
        }
        private Window currentWindow;
        private Window CurrentWindow
        {
            get => currentWindow;
            set
            {
                if (currentWindow != null)
                    currentWindow.Close();
                currentWindow = value;
                currentWindow.Show();
            }
        }
        public void CreateSupplyWindow(IWindowController controller, AgencySQLDb dataBase, User user, string userName)
        {
            SupplyWindow window = new SupplyWindow()
            {
                DataContext = new SupplyViewModel(controller, dataBase, user, userName)
            };
            CurrentWindow = window;
        }
        public void CreateAccessWindow(IWindowController controller)
        {
            if (DBPath == string.Empty)
            {
                AccessWindow window = new AccessWindow()
                {
                    DataContext = new AccessViewModel(this)
                };
                CurrentWindow = window;
            }
            else
            {
                AccessWindow window = new AccessWindow()
                {
                    DataContext = new AccessViewModel(this, DBPath)
                };
                CurrentWindow = window;
            }
        }
        public void CreateEditWindow(IWindowController controller, AgencySQLDb sqlDb)
        {
            EditWindow window = new EditWindow()
            {
                DataContext = new EditViewModel(controller, sqlDb)
            };
            CurrentWindow = window;
        }
        public void CloseApp()
        {
            this.CurrentWindow.Close();
            Application.Current.Shutdown();
        }
        public void SetDBPath(string temppath)
        {
            DBPath = temppath;
        }
        private string DBPath = string.Empty;

    }
}
