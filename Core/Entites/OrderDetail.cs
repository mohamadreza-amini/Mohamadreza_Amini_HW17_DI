namespace Core.Entites
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
