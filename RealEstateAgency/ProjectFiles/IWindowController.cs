using RealEstateAgency.Model;

namespace RealEstateAgency.ViewModel
{
    public interface IWindowController
    {
        void CreateEditWindow(IWindowController controller, AgencySQLDb sqlDb);
        void CreateSupplyWindow(IWindowController controller, AgencySQLDb sqlDb, User user, string userName);
        void CreateAccessWindow(IWindowController controller);
    }
}
