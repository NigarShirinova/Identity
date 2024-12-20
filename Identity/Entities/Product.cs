namespace Identity.Entities
{
    public class Product : BaseEntity
    {
        public  string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string PhotoName { get; set; }

    }
}
