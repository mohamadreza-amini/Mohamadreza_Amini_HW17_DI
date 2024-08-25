using Core.Entites;
using Core.Interfaces;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;



namespace DataAccess;

public class DataAccess:IDataAccess
{
    private readonly string _stringConnection;

    public DataAccess(IConfiguration config)
    {
        _stringConnection = config.GetConnectionString("DefaultConnection");
    }
    
   
    public List<Staff> GetStaffList(string storeName, int staffId)
    {
        List<Staff> staffList = new List<Staff>();

        using (var conn = new SqlConnection(_stringConnection))
        {
            var cmd = StaffCommandGenerate(storeName, staffId, conn);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var staff = new Staff();
                staff.StaffId = Convert.ToInt32(reader["staff_id"]);
                staff.FirstName = reader["first_name"].ToString() ?? "";
                staff.LastName = reader["last_name"].ToString() ?? "";
                staff.Email = reader["email"].ToString() ?? "";
                staff.Phone = reader["phone"].ToString() ?? "";
                staff.StoreId = Convert.ToInt32(reader["store_id"]);
                staff.StoreName = reader["store_name"].ToString() ?? "";
                staff.Manager = reader["manager"].ToString() == "" ? "senior manager" : reader["manager"].ToString() ?? "";
                staffList.Add(staff);
            }
            conn.Close();
        }
        return staffList;
    }


    public Order GetOrder(int orderId)
    {
        Order order = new Order();

        using (SqlConnection conn = new SqlConnection(_stringConnection))
        {
            SqlCommand cmd = new SqlCommand("select o.order_id,c.first_name,c.last_name,c.[state]+' '+c.city+' '+c.street as [address], c.phone,o.order_date," +
                "o.required_date,o.shipped_date,s.first_name+' '+s.last_name as staff_name from sales.orders o inner join sales.customers c on c.customer_id=o.customer_id" +
                " inner join sales.staffs s on s.staff_id = o.staff_id where o.order_id = @orderId ", conn);

            cmd.Parameters.Add("@orderId", System.Data.SqlDbType.Int).Value = orderId;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                order.OrderId = Convert.ToInt32(reader["order_id"]);
                order.FirstName = reader["first_name"].ToString() ?? "";
                order.LastName = reader["last_name"].ToString() ?? "";
                order.Address = reader["address"].ToString() ?? "";
                order.Phone = reader["phone"].ToString() == "" ? "---" : reader["phone"].ToString() ?? "";
                order.Order_Date = Convert.ToDateTime(reader["order_date"]);
                order.Required_Date = Convert.ToDateTime(reader["required_date"]);
                if (reader["shipped_date"] != DBNull.Value)
                {
                    order.Shipped_Date = Convert.ToDateTime(reader["shipped_date"]);
                }
                order.StaffName = reader["staff_name"].ToString() ?? "";

            }
            conn.Close();
        }
        return order;
    }


    public void GetOrderDetail(Order order)
    {
        order.OrderDetail = new List<OrderDetail>();

        using (SqlConnection conn = new SqlConnection(_stringConnection))
        {
            SqlCommand cmd = new SqlCommand("select o.order_id, p.product_id,p.product_name,o.quantity,o.list_price,o.discount from sales.order_items o " +
                "inner join production.products p on o.product_id=p.product_id where o.order_id = @order_Id", conn);

            cmd.Parameters.AddWithValue("@order_Id", order.OrderId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var orderDetail = new OrderDetail();
                orderDetail.OrderId = Convert.ToInt32(reader["order_id"]);
                orderDetail.ProductId = Convert.ToInt32(reader["product_id"]);
                orderDetail.ProductName = reader["product_name"].ToString() ?? "";
                orderDetail.Quantity = Convert.ToInt32(reader["quantity"]);
                orderDetail.ListPrice = Convert.ToDecimal(reader["list_price"]);
                orderDetail.Discount = Convert.ToDecimal(reader["discount"]);

                order.OrderDetail.Add(orderDetail);
            }
            conn.Close();
        }
    }

    private SqlCommand StaffCommandGenerate(string storeName, int staffId, SqlConnection conn)
    {
        var cmd = new SqlCommand("select sf.staff_id,sf.first_name,sf.last_name,sf.email,sf.phone,sf.store_id, st.store_name,stf.first_name+ ' ' + " +
               "stf.last_name as manager from sales.staffs sf left join sales.staffs stf on sf.manager_id=stf.staff_id" +
               " inner join sales.stores st on sf.store_id = st.store_id", conn);

        if (storeName != null || staffId > 0)
        {
            cmd.CommandText += " where ";
            bool flagAnd = false;
            if (staffId > 0)
            {
                cmd.CommandText += " sf.staff_id = @staffid ";

                cmd.Parameters.Add("@staffid", System.Data.SqlDbType.Int).Value = staffId;
                flagAnd = true;
            }
            if (storeName != null)
            {
                if (storeName.Trim() != "")
                {
                    storeName = storeName.Trim();

                    if (flagAnd)
                    {
                        cmd.CommandText += " and ";
                    }
                    cmd.CommandText += " st.store_name like @storename ";
                    cmd.Parameters.Add("@storename", System.Data.SqlDbType.VarChar, 255).Value = storeName + "%";
                }
            }
        }
        return cmd;
    }


}
