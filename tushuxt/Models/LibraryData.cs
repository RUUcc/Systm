using System;
using System.Collections.Generic;

namespace tushuxt
{
    [Serializable]
    public class LibraryData
    {
        public List<Book> Books { get; set; }
        public List<Reader> Readers { get; set; }
        public List<AdminAccount> Admins { get; set; }
        public List<BorrowRecord> BorrowRecords { get; set; }

        public LibraryData()
        {
            Books = new List<Book>();
            Readers = new List<Reader>();
            Admins = new List<AdminAccount>();
            BorrowRecords = new List<BorrowRecord>();
        }
    }
}

