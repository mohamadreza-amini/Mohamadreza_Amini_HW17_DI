using  Core.Entites;
namespace Core.Interfaces
{
    public interface IDataAccess
    {
        List<Staff> GetStaffList(string storeName, int staffId);
        Order GetOrder(int orderId);
        void GetOrderDetail(Order order);
    }
}
