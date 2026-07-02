using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace tushuxt
{
    public class MySqlRepository : ILibraryRepository
    {
        private readonly string connectionString;

        public MySqlRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["LibraryDb"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                // 默认连接字符串，以防 App.config 未配置
                connectionString = "Server=localhost;Database=library-mnaagement-system;Uid=root;Pwd=root;Charset=utf8mb4;";
            }
        }

        public LibraryData Load()
        {
            LibraryData data = new LibraryData();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 加载管理员
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Admins", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Admins.Add(new AdminAccount
                            {
                                Account = reader["Account"].ToString(),
                                Name = reader["Name"].ToString(),
                                Password = reader["Password"].ToString()
                            });
                        }
                    }

                    // 加载读者
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Readers", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Readers.Add(new Reader
                            {
                                StudentId = reader["StudentId"].ToString(),
                                Name = reader["Name"].ToString(),
                                Department = reader["Department"] == DBNull.Value ? "" : reader["Department"].ToString(),
                                Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                                Password = reader["Password"].ToString()
                            });
                        }
                    }

                    // 加载图书
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Books", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Books.Add(new Book
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"] == DBNull.Value ? "" : reader["Author"].ToString(),
                                ISBN = reader["ISBN"] == DBNull.Value ? "" : reader["ISBN"].ToString(),
                                Category = reader["Category"] == DBNull.Value ? "" : reader["Category"].ToString(),
                                Location = reader["Location"] == DBNull.Value ? "" : reader["Location"].ToString(),
                                TotalStock = Convert.ToInt32(reader["TotalStock"]),
                                AvailableStock = Convert.ToInt32(reader["AvailableStock"]),
                                BorrowCount = Convert.ToInt32(reader["BorrowCount"])
                            });
                        }
                    }

                    // 加载借阅记录
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM BorrowRecords", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.BorrowRecords.Add(new BorrowRecord
                            {
                                RecordId = Convert.ToInt32(reader["RecordId"]),
                                BookId = Convert.ToInt32(reader["BookId"]),
                                BookTitle = reader["BookTitle"] == DBNull.Value ? "" : reader["BookTitle"].ToString(),
                                ReaderStudentId = reader["ReaderStudentId"].ToString(),
                                ReaderName = reader["ReaderName"] == DBNull.Value ? "" : reader["ReaderName"].ToString(),
                                BorrowDate = Convert.ToDateTime(reader["BorrowDate"]),
                                DueDate = Convert.ToDateTime(reader["DueDate"]),
                                IsReturned = Convert.ToBoolean(reader["IsReturned"]),
                                ReturnDateText = reader["ReturnDateText"] == DBNull.Value ? "" : reader["ReturnDateText"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Load Error: " + ex.Message);
            }

            return data;
        }

        public void Save(LibraryData data)
        {
            if (data == null) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 禁用外键检查以允许重新同步所有表
                        new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0;", conn, trans).ExecuteNonQuery();

                        // 1. 同步管理员
                        new MySqlCommand("DELETE FROM Admins", conn, trans).ExecuteNonQuery();
                        foreach (var admin in data.Admins)
                        {
                            var cmd = new MySqlCommand("INSERT INTO Admins (Account, Name, Password) VALUES (@Account, @Name, @Password)", conn, trans);
                            cmd.Parameters.AddWithValue("@Account", admin.Account);
                            cmd.Parameters.AddWithValue("@Name", admin.Name);
                            cmd.Parameters.AddWithValue("@Password", admin.Password);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. 同步读者
                        new MySqlCommand("DELETE FROM Readers", conn, trans).ExecuteNonQuery();
                        foreach (var reader in data.Readers)
                        {
                            var cmd = new MySqlCommand("INSERT INTO Readers (StudentId, Name, Department, Phone, Password) VALUES (@StudentId, @Name, @Department, @Phone, @Password)", conn, trans);
                            cmd.Parameters.AddWithValue("@StudentId", reader.StudentId);
                            cmd.Parameters.AddWithValue("@Name", reader.Name);
                            cmd.Parameters.AddWithValue("@Department", reader.Department ?? "");
                            cmd.Parameters.AddWithValue("@Phone", reader.Phone ?? "");
                            cmd.Parameters.AddWithValue("@Password", reader.Password);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. 同步图书
                        new MySqlCommand("DELETE FROM Books", conn, trans).ExecuteNonQuery();
                        foreach (var book in data.Books)
                        {
                            var cmd = new MySqlCommand("INSERT INTO Books (Id, Title, Author, ISBN, Category, Location, TotalStock, AvailableStock, BorrowCount) VALUES (@Id, @Title, @Author, @ISBN, @Category, @Location, @TotalStock, @AvailableStock, @BorrowCount)", conn, trans);
                            cmd.Parameters.AddWithValue("@Id", book.Id);
                            cmd.Parameters.AddWithValue("@Title", book.Title);
                            cmd.Parameters.AddWithValue("@Author", book.Author ?? "");
                            cmd.Parameters.AddWithValue("@ISBN", book.ISBN ?? "");
                            cmd.Parameters.AddWithValue("@Category", book.Category ?? "");
                            cmd.Parameters.AddWithValue("@Location", book.Location ?? "");
                            cmd.Parameters.AddWithValue("@TotalStock", book.TotalStock);
                            cmd.Parameters.AddWithValue("@AvailableStock", book.AvailableStock);
                            cmd.Parameters.AddWithValue("@BorrowCount", book.BorrowCount);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. 同步借阅记录
                        new MySqlCommand("DELETE FROM BorrowRecords", conn, trans).ExecuteNonQuery();
                        foreach (var record in data.BorrowRecords)
                        {
                            var cmd = new MySqlCommand("INSERT INTO BorrowRecords (RecordId, BookId, BookTitle, ReaderStudentId, ReaderName, BorrowDate, DueDate, IsReturned, ReturnDateText) VALUES (@RecordId, @BookId, @BookTitle, @ReaderStudentId, @ReaderName, @BorrowDate, @DueDate, @IsReturned, @ReturnDateText)", conn, trans);
                            cmd.Parameters.AddWithValue("@RecordId", record.RecordId);
                            cmd.Parameters.AddWithValue("@BookId", record.BookId);
                            cmd.Parameters.AddWithValue("@BookTitle", record.BookTitle ?? "");
                            cmd.Parameters.AddWithValue("@ReaderStudentId", record.ReaderStudentId);
                            cmd.Parameters.AddWithValue("@ReaderName", record.ReaderName ?? "");
                            cmd.Parameters.AddWithValue("@BorrowDate", record.BorrowDate);
                            cmd.Parameters.AddWithValue("@DueDate", record.DueDate);
                            cmd.Parameters.AddWithValue("@IsReturned", record.IsReturned);
                            cmd.Parameters.AddWithValue("@ReturnDateText", record.ReturnDateText ?? "");
                            cmd.ExecuteNonQuery();
                        }

                        // 恢复外键检查
                        new MySqlCommand("SET FOREIGN_KEY_CHECKS = 1;", conn, trans).ExecuteNonQuery();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        System.Diagnostics.Debug.WriteLine("Database Save Error: " + ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}
