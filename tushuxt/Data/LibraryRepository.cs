using System;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace tushuxt
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly string dataFilePath;

        public LibraryRepository(string filePath = null)
        {
            dataFilePath = string.IsNullOrWhiteSpace(filePath)
                ? Path.Combine(Application.StartupPath, "library-data.xml")
                : filePath;
        }

        public LibraryData Load()
        {
            if (File.Exists(dataFilePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LibraryData));
                    using (FileStream stream = new FileStream(dataFilePath, FileMode.Open))
                    {
                        return serializer.Deserialize(stream) as LibraryData;
                    }
                }
                catch
                {
                    LibraryData fallback = CreateSeedData();
                    Save(fallback);
                    return fallback;
                }
            }

            LibraryData seed = CreateSeedData();
            Save(seed);
            return seed;
        }

        public void Save(LibraryData data)
        {
            if (data == null)
            {
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(LibraryData));
            using (FileStream stream = new FileStream(dataFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }

        private LibraryData CreateSeedData()
        {
            LibraryData seed = new LibraryData();

            seed.Admins.Add(new AdminAccount { Account = "admin", Name = "系统管理员", Password = "123456" });
            seed.Admins.Add(new AdminAccount { Account = "manager01", Name = "馆藏管理员", Password = "123456" });

            seed.Readers.Add(new Reader { StudentId = "2023001", Name = "张三", Department = "计算机学院", Phone = "13800000001", Password = "123456" });
            seed.Readers.Add(new Reader { StudentId = "2023002", Name = "李四", Department = "软件学院", Phone = "13800000002", Password = "123456" });
            seed.Readers.Add(new Reader { StudentId = "2023003", Name = "王五", Department = "信息学院", Phone = "13800000003", Password = "123456" });

            seed.Books.Add(new Book { Id = 1, Title = "C# 程序设计", Author = "王老师", ISBN = "978730000001", Category = "程序设计", Location = "A-101", TotalStock = 8, AvailableStock = 6, BorrowCount = 2 });
            seed.Books.Add(new Book { Id = 2, Title = "数据库系统概论", Author = "陈老师", ISBN = "978730000002", Category = "数据库", Location = "A-102", TotalStock = 6, AvailableStock = 5, BorrowCount = 1 });
            seed.Books.Add(new Book { Id = 3, Title = "数据结构", Author = "严老师", ISBN = "978730000003", Category = "计算机基础", Location = "B-201", TotalStock = 10, AvailableStock = 8, BorrowCount = 2 });
            seed.Books.Add(new Book { Id = 4, Title = "软件工程导论", Author = "周老师", ISBN = "978730000004", Category = "软件工程", Location = "B-202", TotalStock = 5, AvailableStock = 5, BorrowCount = 0 });
            seed.Books.Add(new Book { Id = 5, Title = "操作系统原理", Author = "刘老师", ISBN = "978730000005", Category = "系统原理", Location = "C-105", TotalStock = 7, AvailableStock = 7, BorrowCount = 0 });

            seed.BorrowRecords.Add(new BorrowRecord
            {
                RecordId = 1,
                BookId = 1,
                BookTitle = "C# 程序设计",
                ReaderStudentId = "2023001",
                ReaderName = "张三",
                BorrowDate = DateTime.Today.AddDays(-5),
                DueDate = DateTime.Today.AddDays(25),
                IsReturned = false,
                ReturnDateText = string.Empty
            });
            seed.BorrowRecords.Add(new BorrowRecord
            {
                RecordId = 2,
                BookId = 3,
                BookTitle = "数据结构",
                ReaderStudentId = "2023002",
                ReaderName = "李四",
                BorrowDate = DateTime.Today.AddDays(-40),
                DueDate = DateTime.Today.AddDays(-10),
                IsReturned = false,
                ReturnDateText = string.Empty
            });
            seed.BorrowRecords.Add(new BorrowRecord
            {
                RecordId = 3,
                BookId = 2,
                BookTitle = "数据库系统概论",
                ReaderStudentId = "2023003",
                ReaderName = "王五",
                BorrowDate = DateTime.Today.AddDays(-25),
                DueDate = DateTime.Today.AddDays(-2),
                IsReturned = true,
                ReturnDateText = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")
            });

            return seed;
        }
    }
}

