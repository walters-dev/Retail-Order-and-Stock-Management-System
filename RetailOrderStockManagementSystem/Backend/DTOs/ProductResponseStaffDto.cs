namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class ProductResponseStaffDto
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
