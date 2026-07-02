using System;
using System.Collections.Generic;
using System.Linq;

namespace tushuxt
{
    public class LibraryService
    {
        private const int MaxBorrowCount = 5;

        private LibraryData data;
        private readonly ILibraryRepository repository;

        public LibraryData Data { get { return data; } }

        public LibraryService(ILibraryRepository repository)
        {
            this.repository = repository ?? new LibraryRepository();
            data = this.repository.Load();
        }

        public void Save()
        {
            repository.Save(data);
        }

        // ==================== 登录认证 ====================

        public LoginUser Login(string account, string password, UserRole role, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                error = "请输入账号和密码。";
                return null;
            }

            if (role == UserRole.Admin)
            {
                AdminAccount admin = data.Admins.FirstOrDefault(a => a.Account == account && a.Password == password);
                if (admin == null)
                {
                    error = "管理员账号或密码错误。";
                    return null;
                }

                return new LoginUser
                {
                    Account = admin.Account,
                    DisplayName = admin.Name,
                    Role = UserRole.Admin
                };
            }
            else
            {
                Reader reader = data.Readers.FirstOrDefault(r => r.StudentId == account && r.Password == password);
                if (reader == null)
                {
                    error = "学生学号或密码错误。";
                    return null;
                }

                return new LoginUser
                {
                    Account = reader.StudentId,
                    DisplayName = reader.Name,
                    Role = UserRole.Student
                };
            }
        }

        // ==================== 图书管理 ====================

        public string AddBook(string title, string author, string isbn, string category, string location, int total, int available)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
                return "请完整填写书名和作者。";

            if (data.Books.Any(b => b.ISBN == isbn && !string.IsNullOrWhiteSpace(isbn)))
                return "ISBN 已存在，请勿重复录入。";

            if (available > total)
                return "可借库存不能大于总库存。";

            int nextId = data.Books.Count == 0 ? 1 : data.Books.Max(b => b.Id) + 1;
            data.Books.Add(new Book
            {
                Id = nextId,
                Title = title,
                Author = author,
                ISBN = isbn,
                Category = category,
                Location = location,
                TotalStock = total,
                AvailableStock = available,
                BorrowCount = 0
            });

            Save();
            return null;
        }

        public string UpdateBook(int id, string title, string author, string isbn, string category, string location, int total, int available)
        {
            Book book = data.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return "未找到图书信息。";

            if (data.Books.Any(b => b.Id != id && b.ISBN == isbn && !string.IsNullOrWhiteSpace(isbn)))
                return "ISBN 已存在，请修改后重试。";

            if (available > total)
                return "可借库存不能大于总库存。";

            book.Title = title;
            book.Author = author;
            book.ISBN = isbn;
            book.Category = category;
            book.Location = location;
            book.TotalStock = total;
            book.AvailableStock = available;

            Save();
            return null;
        }

        public string DeleteBook(int id)
        {
            if (data.BorrowRecords.Any(r => r.BookId == id && !r.IsReturned))
                return "该图书仍存在未归还记录，不能删除。";

            Book book = data.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                data.Books.Remove(book);
                Save();
            }

            return null;
        }

        // ==================== 读者管理 ====================

        public string AddReader(string studentId, string name, string department, string phone, string password)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                return "请填写学号、姓名和密码。";

            if (data.Readers.Any(r => r.StudentId == studentId))
                return "学号已存在。";

            data.Readers.Add(new Reader
            {
                StudentId = studentId,
                Name = name,
                Department = department,
                Phone = phone,
                Password = password
            });

            Save();
            return null;
        }

        public string UpdateReader(string studentId, string name, string department, string phone, string password)
        {
            Reader reader = data.Readers.FirstOrDefault(r => r.StudentId == studentId);
            if (reader == null)
                return "未找到读者信息。";

            reader.Name = name;
            reader.Department = department;
            reader.Phone = phone;
            reader.Password = password;

            Save();
            return null;
        }

        public string DeleteReader(string studentId)
        {
            if (data.BorrowRecords.Any(r => r.ReaderStudentId == studentId && !r.IsReturned))
                return "该读者仍有未归还图书，不能删除。";

            Reader reader = data.Readers.FirstOrDefault(r => r.StudentId == studentId);
            if (reader != null)
            {
                data.Readers.Remove(reader);
                Save();
            }

            return null;
        }

        // ==================== 借阅管理 ====================

        public string BorrowBook(string studentId, int bookId, int days)
        {
            Reader reader = data.Readers.FirstOrDefault(r => r.StudentId == studentId);
            Book book = data.Books.FirstOrDefault(b => b.Id == bookId);

            if (reader == null || book == null)
                return "借阅对象或图书不存在。";

            if (book.AvailableStock <= 0)
                return "该图书当前不可借。";

            if (GetActiveBorrowCount(studentId) >= MaxBorrowCount)
                return string.Format("每位学生最多可借 {0} 本图书。", MaxBorrowCount);

            if (data.BorrowRecords.Any(r => !r.IsReturned && r.ReaderStudentId == studentId && r.BookId == bookId))
                return "同一本图书未归还前不能重复借阅。";

            BorrowRecord record = new BorrowRecord
            {
                RecordId = data.BorrowRecords.Count == 0 ? 1 : data.BorrowRecords.Max(r => r.RecordId) + 1,
                BookId = book.Id,
                BookTitle = book.Title,
                ReaderStudentId = reader.StudentId,
                ReaderName = reader.Name,
                BorrowDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(days),
                IsReturned = false,
                ReturnDateText = string.Empty
            };

            book.AvailableStock -= 1;
            book.BorrowCount += 1;
            data.BorrowRecords.Add(record);

            Save();
            return null;
        }

        public string ReturnBook(int recordId)
        {
            BorrowRecord record = data.BorrowRecords.FirstOrDefault(r => r.RecordId == recordId);
            if (record == null)
                return "未找到借阅记录。";

            if (record.IsReturned)
                return "该记录已归还。";

            record.IsReturned = true;
            record.ReturnDateText = DateTime.Today.ToString("yyyy-MM-dd");

            Book book = data.Books.FirstOrDefault(b => b.Id == record.BookId);
            if (book != null)
                book.AvailableStock += 1;

            Save();
            return null;
        }

        public string ExtendBorrow(int recordId, int days)
        {
            BorrowRecord record = data.BorrowRecords.FirstOrDefault(r => r.RecordId == recordId);
            if (record == null)
                return "未找到借阅记录。";

            if (record.IsReturned)
                return "已归还记录不能修改到期时间。";

            record.DueDate = record.DueDate.AddDays(days);

            Save();
            return null;
        }

        public string DeleteRecord(int recordId)
        {
            BorrowRecord record = data.BorrowRecords.FirstOrDefault(r => r.RecordId == recordId);
            if (record == null)
                return null;

            if (!record.IsReturned)
            {
                Book book = data.Books.FirstOrDefault(b => b.Id == record.BookId);
                if (book != null)
                    book.AvailableStock += 1;
            }

            data.BorrowRecords.Remove(record);

            Save();
            return null;
        }

        public int GetActiveBorrowCount(string studentId)
        {
            return data.BorrowRecords.Count(r => r.ReaderStudentId == studentId && !r.IsReturned);
        }

        // ==================== 管理员账号管理 ====================

        public string AddAdmin(string account, string name, string password)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                return "请填写管理员账号、姓名和密码。";

            if (data.Admins.Any(a => a.Account == account))
                return "管理员账号已存在。";

            data.Admins.Add(new AdminAccount
            {
                Account = account,
                Name = name,
                Password = password
            });

            Save();
            return null;
        }

        public string UpdateAdmin(string account, string name, string password)
        {
            AdminAccount admin = data.Admins.FirstOrDefault(a => a.Account == account);
            if (admin == null)
                return "未找到管理员账号。";

            admin.Name = name;
            admin.Password = password;

            Save();
            return null;
        }

        public string DeleteAdmin(string account)
        {
            if (account == "admin")
                return "默认管理员账号不允许删除。";

            AdminAccount admin = data.Admins.FirstOrDefault(a => a.Account == account);
            if (admin != null)
            {
                data.Admins.Remove(admin);
                Save();
            }

            return null;
        }

        // ==================== 学生操作 ====================

        public string ChangePassword(string studentId, string oldPassword, string newPassword)
        {
            Reader reader = data.Readers.FirstOrDefault(r => r.StudentId == studentId);
            if (reader == null)
                return "未找到学生信息。";

            if (reader.Password != oldPassword)
                return "原密码不正确。";

            if (string.IsNullOrWhiteSpace(newPassword))
                return "新密码不能为空。";

            reader.Password = newPassword;

            Save();
            return null;
        }
    }
}
