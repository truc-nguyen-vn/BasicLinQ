namespace BasicLinQ.Models.Product
{
    public class ProductRequestModel
    {
        public string? SearchTerm { get; set; }

        public bool IsSortSimple { get; set; }
        public bool IsSortUseThenBy { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
