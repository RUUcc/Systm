using System;

namespace tushuxt
{
    [Serializable]
    public class BorrowRecord
    {
        public int RecordId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string ReaderStudentId { get; set; }
        public string ReaderName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public string ReturnDateText { get; set; }
    }
}

