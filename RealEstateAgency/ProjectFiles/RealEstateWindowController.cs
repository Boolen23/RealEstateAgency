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
            AccessWindow window = new AccessWindow()
            {
                DataContext = new AccessViewModel(this)
            };
            CurrentWindow = window;
        }
        public void CreateEditWindow(IWindowController controller, AgencySQLDb sqlDb)
        {
            EditWindow window = new EditWindow()
            {
                DataContext = new EditViewModel(controller, sqlDb)
            };
            CurrentWindow = window;
        }

    }
}
