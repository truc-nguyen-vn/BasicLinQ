namespace BasicLinQ.Models.Product
{
    public class ProductRequestModel
    {
        public string? SearchTerm { get; set; }

        public bool IsSortSimple { get; set; }
        public bool IsSortUseThenBy { get; set; }

        public bool IsFilterByWhere { get; set; }
        public bool IsFilterByOfType { get; set; }

        public bool IsProjectingBySelect { get; set; }
        public bool IsProjectingBySelectMany { get; set; }

        public bool IsPartitioningSimple { get; set; }
        public bool IsPartitioningChunk { get; set; }

        public bool IsJoinSimple { get; set; }
        public bool IsGroupJoinSimple { get; set; }

        public bool IsGroupBySimple { get; set; }


    }
}
