namespace Core.Entites
{
    public class Order
    {
        public int OrderId { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateTime Order_Date { get; set; }
        public DateTime Required_Date { get; set; }

        public DateTime Shipped_Date { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public List<OrderDetail> OrderDetail { get; set; }
    }
}
