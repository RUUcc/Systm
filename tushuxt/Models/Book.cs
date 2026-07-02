using System;

namespace tushuxt
{
    [Serializable]
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public int TotalStock { get; set; }
        public int AvailableStock { get; set; }
        public int BorrowCount { get; set; }
    }
}

