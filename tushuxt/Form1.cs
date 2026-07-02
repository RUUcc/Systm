using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace tushuxt
{
    public partial class Form1 : Form
    {
        private readonly LibraryService service;

        private LoginUser currentUser;

        private Panel loginPanel;
        private Label lblLoginRole;
        private Label lblLoginAccount;
        private Label lblLoginPassword;
        private ComboBox cmbLoginRole;
        private TextBox txtLoginAccount;
        private TextBox txtLoginPassword;
        private Button btnLogin;
        private Button btnLogout;
        private Label lblCurrentUser;

        private TabControl mainTabs;

        private TabPage tabAdminBooks;
        private TabPage tabAdminReaders;
        private TabPage tabAdminRecords;
        private TabPage tabAdminAccounts;
        private TabPage tabAdminStats;

        private TabPage tabStudentBooks;
        private TabPage tabStudentRecords;
        private TabPage tabStudentProfile;

        private DataGridView dgvAdminBooks;
        private TextBox txtBookId;
        private TextBox txtBookTitle;
        private TextBox txtBookAuthor;
        private TextBox txtBookIsbn;
        private TextBox txtBookCategory;
        private TextBox txtBookLocation;
        private NumericUpDown numBookTotal;
        private NumericUpDown numBookAvailable;
        private TextBox txtBookSearchTitle;
        private TextBox txtBookSearchAuthor;
        private TextBox txtBookSearchCategory;

        private DataGridView dgvReaders;
        private TextBox txtReaderStudentId;
        private TextBox txtReaderName;
        private TextBox txtReaderDepartment;
        private TextBox txtReaderPhone;
        private TextBox txtReaderPassword;
        private TextBox txtReaderSearchKeyword;

        private DataGridView dgvRecords;
        private ComboBox cmbBorrowReader;
        private ComboBox cmbBorrowBook;
        private NumericUpDown numBorrowDays;
        private TextBox txtRecordSearchBook;
        private TextBox txtRecordSearchStudentId;
        private ComboBox cmbRecordStatus;
        private NumericUpDown numExtendDays;

        private DataGridView dgvAdmins;
        private TextBox txtAdminAccount;
        private TextBox txtAdminName;
        private TextBox txtAdminPassword;

        private Label lblStatsSummary;
        private DataGridView dgvHotBooks;
        private DataGridView dgvCategoryStats;
        private DataGridView dgvOverdueStats;

        private DataGridView dgvStudentBooks;
        private TextBox txtStudentBookSearchTitle;
        private TextBox txtStudentBookSearchAuthor;
        private TextBox txtStudentBookSearchIsbn;
        private TextBox txtStudentBookSearchCategory;

        private DataGridView dgvStudentRecords;
        private Label lblStudentProfile;
        private TextBox txtStudentOldPassword;
        private TextBox txtStudentNewPassword;
        private TextBox txtStudentConfirmPassword;

        public Form1()
        {
            InitializeComponent();
            service = new LibraryService(new MySqlRepository());
            BuildInterface();
            UpdateUserState(null);
            RefreshAllViews();
        }

        private void BuildInterface()
        {
            Text = "高校图书借阅管理系统";
            Width = 1380;
            Height = 860;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Microsoft YaHei UI", 9F);
            BackColor = Color.FromArgb(245, 248, 252);

            Controls.Clear();

            loginPanel = new Panel();
            loginPanel.Dock = DockStyle.Top;
            loginPanel.Height = 70;
            loginPanel.BackColor = Color.FromArgb(33, 64, 120);

            Label lblTitle = new Label();
            lblTitle.Text = "高校图书借阅管理系统";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 18);
            lblTitle.AutoSize = true;
            loginPanel.Controls.Add(lblTitle);

            lblLoginRole = CreateLabel("身份", 340, 24, Color.White);
            loginPanel.Controls.Add(lblLoginRole);

            cmbLoginRole = new ComboBox();
            cmbLoginRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoginRole.Items.AddRange(new object[] { "管理员", "学生" });
            cmbLoginRole.SelectedIndex = 0;
            cmbLoginRole.Location = new Point(382, 20);
            cmbLoginRole.Width = 90;
            loginPanel.Controls.Add(cmbLoginRole);

            lblLoginAccount = CreateLabel("账号", 488, 24, Color.White);
            loginPanel.Controls.Add(lblLoginAccount);

            txtLoginAccount = new TextBox();
            txtLoginAccount.Location = new Point(530, 20);
            txtLoginAccount.Width = 120;
            loginPanel.Controls.Add(txtLoginAccount);

            lblLoginPassword = CreateLabel("密码", 666, 24, Color.White);
            loginPanel.Controls.Add(lblLoginPassword);

            txtLoginPassword = new TextBox();
            txtLoginPassword.Location = new Point(708, 20);
            txtLoginPassword.Width = 120;
            txtLoginPassword.PasswordChar = '*';
            loginPanel.Controls.Add(txtLoginPassword);

            btnLogin = new Button();
            btnLogin.Text = "登录";
            btnLogin.Location = new Point(848, 18);
            btnLogin.Size = new Size(72, 30);
            btnLogin.Click += BtnLogin_Click;
            loginPanel.Controls.Add(btnLogin);

            btnLogout = new Button();
            btnLogout.Text = "退出登录";
            btnLogout.Location = new Point(928, 18);
            btnLogout.Size = new Size(90, 30);
            btnLogout.Click += BtnLogout_Click;
            loginPanel.Controls.Add(btnLogout);

            lblCurrentUser = new Label();
            lblCurrentUser.ForeColor = Color.White;
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Location = new Point(1040, 25);
            loginPanel.Controls.Add(lblCurrentUser);

            mainTabs = new TabControl();
            mainTabs.Dock = DockStyle.Fill;
            Controls.Add(mainTabs);
            Controls.Add(loginPanel);

            BuildAdminTabs();
            BuildStudentTabs();
        }

        private void BuildAdminTabs()
        {
            tabAdminBooks = new TabPage("图书信息管理");
            tabAdminReaders = new TabPage("读者信息管理");
            tabAdminRecords = new TabPage("借阅与归还管理");
            tabAdminAccounts = new TabPage("管理员账号管理");
            tabAdminStats = new TabPage("数据统计");

            BuildAdminBookPage();
            BuildReaderPage();
            BuildRecordPage();
            BuildAdminAccountPage();
            BuildStatisticsPage();
        }

        private void BuildStudentTabs()
        {
            tabStudentBooks = new TabPage("图书查询与借阅");
            tabStudentRecords = new TabPage("我的借阅记录");
            tabStudentProfile = new TabPage("个人中心");

            BuildStudentBookPage();
            BuildStudentRecordPage();
            BuildStudentProfilePage();
        }

        private void BuildAdminBookPage()
        {
            Panel topPanel = CreatePageTopPanel(tabAdminBooks, 240);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            txtBookId = CreateTextBox(0, 0, 80, true);
            txtBookTitle = CreateTextBox(0, 0, 120, false);
            txtBookAuthor = CreateTextBox(0, 0, 100, false);
            txtBookIsbn = CreateTextBox(0, 0, 110, false);
            txtBookCategory = CreateTextBox(0, 0, 90, false);
            txtBookLocation = CreateTextBox(0, 0, 90, false);
            numBookTotal = CreateNumeric(0, 0, 70, 1, 10000, 1);
            numBookAvailable = CreateNumeric(0, 0, 70, 0, 10000, 1);

            txtBookSearchTitle = CreateTextBox(0, 0, 120, false);
            txtBookSearchAuthor = CreateTextBox(0, 0, 100, false);
            txtBookSearchCategory = CreateTextBox(0, 0, 100, false);

            AddToolbarField(layout, "编号", txtBookId);
            AddToolbarField(layout, "书名", txtBookTitle);
            AddToolbarField(layout, "作者", txtBookAuthor);
            AddToolbarField(layout, "ISBN", txtBookIsbn);
            AddToolbarField(layout, "分类", txtBookCategory);
            AddToolbarField(layout, "位置", txtBookLocation);
            AddToolbarField(layout, "总库存", numBookTotal);
            AddToolbarField(layout, "可借", numBookAvailable);

            AddToolbarButton(layout, "新增图书", 88, BtnAddBook_Click);
            AddToolbarButton(layout, "修改图书", 88, BtnUpdateBook_Click);
            AddToolbarButton(layout, "删除图书", 88, BtnDeleteBook_Click);
            AddToolbarButton(layout, "清空表单", 92, BtnClearBookForm_Click);

            AddToolbarField(layout, "查询书名", txtBookSearchTitle);
            AddToolbarField(layout, "查询作者", txtBookSearchAuthor);
            AddToolbarField(layout, "查询分类", txtBookSearchCategory);
            AddToolbarButton(layout, "查询", 72, BtnSearchAdminBooks_Click);
            AddToolbarButton(layout, "重置", 72, BtnResetAdminBookFilters_Click);
            AdjustPageTopHeight(tabAdminBooks, layout, 240);

            dgvAdminBooks = CreateGrid();
            dgvAdminBooks.SelectionChanged += DgvAdminBooks_SelectionChanged;
            BindGridToPage(tabAdminBooks, dgvAdminBooks);
        }

        private void BuildReaderPage()
        {
            Panel topPanel = CreatePageTopPanel(tabAdminReaders, 200);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            txtReaderStudentId = CreateTextBox(0, 0, 110, false);
            txtReaderName = CreateTextBox(0, 0, 100, false);
            txtReaderDepartment = CreateTextBox(0, 0, 120, false);
            txtReaderPhone = CreateTextBox(0, 0, 110, false);
            txtReaderPassword = CreateTextBox(0, 0, 110, false);
            txtReaderSearchKeyword = CreateTextBox(0, 0, 170, false);

            AddToolbarField(layout, "学号", txtReaderStudentId);
            AddToolbarField(layout, "姓名", txtReaderName);
            AddToolbarField(layout, "院系", txtReaderDepartment);
            AddToolbarField(layout, "电话", txtReaderPhone);
            AddToolbarField(layout, "密码", txtReaderPassword);
            AddToolbarButton(layout, "新增读者", 88, BtnAddReader_Click);
            AddToolbarButton(layout, "修改读者", 88, BtnUpdateReader_Click);
            AddToolbarButton(layout, "删除读者", 88, BtnDeleteReader_Click);
            AddToolbarButton(layout, "清空表单", 92, BtnClearReaderForm_Click);
            AddToolbarField(layout, "关键字", txtReaderSearchKeyword);
            AddToolbarButton(layout, "查询", 72, BtnSearchReaders_Click);
            AddToolbarButton(layout, "重置", 72, BtnResetReaderFilters_Click);
            AdjustPageTopHeight(tabAdminReaders, layout, 200);

            dgvReaders = CreateGrid();
            dgvReaders.SelectionChanged += DgvReaders_SelectionChanged;
            BindGridToPage(tabAdminReaders, dgvReaders);
        }

        private void BuildRecordPage()
        {
            Panel topPanel = CreatePageTopPanel(tabAdminRecords, 220);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            cmbBorrowReader = new ComboBox();
            cmbBorrowReader.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBorrowReader.Width = 160;
            AddToolbarField(layout, "借阅读者", cmbBorrowReader);

            cmbBorrowBook = new ComboBox();
            cmbBorrowBook.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBorrowBook.Width = 240;
            AddToolbarField(layout, "图书", cmbBorrowBook);

            numBorrowDays = CreateNumeric(592, 14, 70, 1, 90, 30);
            AddToolbarField(layout, "借阅天数", numBorrowDays);

            AddToolbarButton(layout, "新增借阅", 90, BtnAddBorrow_Click);

            numExtendDays = CreateNumeric(846, 14, 70, 1, 60, 7);
            AddToolbarField(layout, "续借天数", numExtendDays);

            AddToolbarButton(layout, "登记归还", 90, BtnReturnBook_Click);
            AddToolbarButton(layout, "修改到期", 90, BtnExtendBorrow_Click);
            AddToolbarButton(layout, "删除记录", 90, BtnDeleteRecord_Click);

            txtRecordSearchBook = CreateTextBox(48, 56, 160, false);
            AddToolbarField(layout, "书名", txtRecordSearchBook);

            txtRecordSearchStudentId = CreateTextBox(256, 56, 120, false);
            AddToolbarField(layout, "学号", txtRecordSearchStudentId);

            cmbRecordStatus = new ComboBox();
            cmbRecordStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecordStatus.Items.AddRange(new object[] { "全部", "借阅中", "已归还", "超期" });
            cmbRecordStatus.SelectedIndex = 0;
            cmbRecordStatus.Width = 100;
            AddToolbarField(layout, "状态", cmbRecordStatus);

            AddToolbarButton(layout, "查询", 72, BtnSearchRecords_Click);
            AddToolbarButton(layout, "重置", 72, BtnResetRecordFilters_Click);
            AdjustPageTopHeight(tabAdminRecords, layout, 220);

            dgvRecords = CreateGrid();
            BindGridToPage(tabAdminRecords, dgvRecords);
        }

        private void BuildAdminAccountPage()
        {
            Panel topPanel = CreatePageTopPanel(tabAdminAccounts, 170);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            txtAdminAccount = CreateTextBox(58, 14, 120, false);
            AddToolbarField(layout, "账号", txtAdminAccount);

            txtAdminName = CreateTextBox(240, 14, 120, false);
            AddToolbarField(layout, "姓名", txtAdminName);

            txtAdminPassword = CreateTextBox(422, 14, 120, false);
            AddToolbarField(layout, "密码", txtAdminPassword);

            AddToolbarButton(layout, "新增管理员", 96, BtnAddAdmin_Click);
            AddToolbarButton(layout, "修改管理员", 96, BtnUpdateAdmin_Click);
            AddToolbarButton(layout, "删除管理员", 96, BtnDeleteAdmin_Click);
            AddToolbarButton(layout, "清空表单", 92, BtnClearAdminForm_Click);
            AdjustPageTopHeight(tabAdminAccounts, layout, 170);

            dgvAdmins = CreateGrid();
            dgvAdmins.SelectionChanged += DgvAdmins_SelectionChanged;
            BindGridToPage(tabAdminAccounts, dgvAdmins);
        }

        private void BuildStatisticsPage()
        {
            Panel topPanel = CreatePageTopPanel(tabAdminStats, 160);

            lblStatsSummary = new Label();
            lblStatsSummary.Location = new Point(12, 12);
            lblStatsSummary.Size = new Size(1280, 110);
            lblStatsSummary.Font = new Font("Microsoft YaHei UI", 10F);
            lblStatsSummary.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Controls.Add(lblStatsSummary);

            SplitContainer splitMain = new SplitContainer();
            splitMain.Dock = DockStyle.Fill;
            splitMain.Orientation = Orientation.Horizontal;
            splitMain.SplitterDistance = 250;
            BindFillContentToPage(tabAdminStats, splitMain);

            SplitContainer splitTop = new SplitContainer();
            splitTop.Dock = DockStyle.Fill;
            splitTop.SplitterDistance = 600;
            splitMain.Panel1.Controls.Add(splitTop);

            dgvHotBooks = CreateGrid();
            dgvCategoryStats = CreateGrid();
            dgvOverdueStats = CreateGrid();

            splitTop.Panel1.Controls.Add(dgvHotBooks);
            splitTop.Panel2.Controls.Add(dgvCategoryStats);
            splitMain.Panel2.Controls.Add(dgvOverdueStats);
        }

        private void BuildStudentBookPage()
        {
            Panel topPanel = CreatePageTopPanel(tabStudentBooks, 170);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            txtStudentBookSearchTitle = CreateTextBox(48, 14, 130, false);
            AddToolbarField(layout, "书名", txtStudentBookSearchTitle);

            txtStudentBookSearchAuthor = CreateTextBox(226, 14, 110, false);
            AddToolbarField(layout, "作者", txtStudentBookSearchAuthor);

            txtStudentBookSearchIsbn = CreateTextBox(392, 14, 120, false);
            AddToolbarField(layout, "ISBN", txtStudentBookSearchIsbn);

            txtStudentBookSearchCategory = CreateTextBox(560, 14, 110, false);
            AddToolbarField(layout, "分类", txtStudentBookSearchCategory);

            AddToolbarButton(layout, "查询图书", 90, BtnSearchStudentBooks_Click);
            AddToolbarButton(layout, "重置", 72, BtnResetStudentBookFilters_Click);
            AddToolbarButton(layout, "借阅选中图书", 110, BtnStudentBorrow_Click);
            AdjustPageTopHeight(tabStudentBooks, layout, 170);

            dgvStudentBooks = CreateGrid();
            BindGridToPage(tabStudentBooks, dgvStudentBooks);
        }

        private void BuildStudentRecordPage()
        {
            Panel topPanel = CreatePageTopPanel(tabStudentRecords, 120);
            FlowLayoutPanel layout = CreateToolbarTable();
            topPanel.Controls.Add(layout);

            AddToolbarButton(layout, "刷新记录", 90, BtnRefreshStudentRecords_Click);
            AddToolbarButton(layout, "归还选中图书(还书)", 140, BtnStudentReturn_Click);
            AdjustPageTopHeight(tabStudentRecords, layout, 120);

            dgvStudentRecords = CreateGrid();
            BindGridToPage(tabStudentRecords, dgvStudentRecords);
        }

        private void BuildStudentProfilePage()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            tabStudentProfile.Controls.Add(panel);

            lblStudentProfile = new Label();
            lblStudentProfile.Location = new Point(20, 20);
            lblStudentProfile.Size = new Size(700, 120);
            lblStudentProfile.BorderStyle = BorderStyle.FixedSingle;
            lblStudentProfile.Font = new Font("Microsoft YaHei UI", 10F);
            panel.Controls.Add(lblStudentProfile);

            panel.Controls.Add(CreateLabel("原密码", 20, 170));
            txtStudentOldPassword = CreateTextBox(90, 166, 180, false);
            txtStudentOldPassword.PasswordChar = '*';
            panel.Controls.Add(txtStudentOldPassword);

            panel.Controls.Add(CreateLabel("新密码", 20, 210));
            txtStudentNewPassword = CreateTextBox(90, 206, 180, false);
            txtStudentNewPassword.PasswordChar = '*';
            panel.Controls.Add(txtStudentNewPassword);

            panel.Controls.Add(CreateLabel("确认密码", 6, 250));
            txtStudentConfirmPassword = CreateTextBox(90, 246, 180, false);
            txtStudentConfirmPassword.PasswordChar = '*';
            panel.Controls.Add(txtStudentConfirmPassword);

            Button btnChange = CreateButton("修改密码", 90, 290, 90, BtnChangeStudentPassword_Click);
            panel.Controls.Add(btnChange);
        }

        // ==================== 用户状态管理 ====================

        private void UpdateUserState(LoginUser user)
        {
            currentUser = user;
            txtLoginPassword.Text = string.Empty;

            bool isLoggedIn = currentUser != null;
            lblLoginRole.Visible = !isLoggedIn;
            lblLoginAccount.Visible = !isLoggedIn;
            lblLoginPassword.Visible = !isLoggedIn;
            cmbLoginRole.Visible = !isLoggedIn;
            txtLoginAccount.Visible = !isLoggedIn;
            txtLoginPassword.Visible = !isLoggedIn;
            btnLogin.Visible = !isLoggedIn;

            btnLogout.Visible = isLoggedIn;
            btnLogout.Location = isLoggedIn ? new Point(1180, 18) : new Point(928, 18);
            lblCurrentUser.Location = isLoggedIn ? new Point(930, 25) : new Point(1040, 25);

            if (currentUser == null)
            {
                lblCurrentUser.Text = "当前未登录";
                btnLogout.Enabled = false;
                txtLoginAccount.Enabled = true;
                txtLoginPassword.Enabled = true;
                cmbLoginRole.Enabled = true;

                mainTabs.TabPages.Clear();
                return;
            }

            btnLogout.Enabled = true;
            txtLoginAccount.Enabled = false;
            txtLoginPassword.Enabled = false;
            cmbLoginRole.Enabled = false;
            lblCurrentUser.Text = string.Format("当前用户：{0}（{1}）", currentUser.DisplayName, currentUser.Role == UserRole.Admin ? "管理员" : "学生");

            mainTabs.TabPages.Clear();

            if (currentUser.Role == UserRole.Admin)
            {
                mainTabs.TabPages.Add(tabAdminBooks);
                mainTabs.TabPages.Add(tabAdminReaders);
                mainTabs.TabPages.Add(tabAdminRecords);
                mainTabs.TabPages.Add(tabAdminAccounts);
                mainTabs.TabPages.Add(tabAdminStats);
            }
            else
            {
                mainTabs.TabPages.Add(tabStudentBooks);
                mainTabs.TabPages.Add(tabStudentRecords);
                mainTabs.TabPages.Add(tabStudentProfile);
            }
        }

        private void ShowSuccessAndRefresh(string message)
        {
            RefreshAllViews();
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== 视图刷新 ====================

        private void RefreshAllViews()
        {
            RefreshBookCombos();
            RefreshAdminBooksGrid();
            RefreshReadersGrid();
            RefreshRecordsGrid();
            RefreshAdminsGrid();
            RefreshStatistics();
            RefreshStudentBooksGrid();
            RefreshStudentRecordGrid();
            RefreshStudentProfile();
        }

        private void RefreshBookCombos()
        {
            if (cmbBorrowBook != null)
            {
                cmbBorrowBook.DataSource = service.Data.Books
                    .OrderBy(b => b.Title)
                    .Select(b => new ComboItem
                    {
                        Value = b.Id.ToString(),
                        Display = string.Format("{0}（库存 {1}/{2}）", b.Title, b.AvailableStock, b.TotalStock)
                    })
                    .ToList();
                cmbBorrowBook.DisplayMember = "Display";
                cmbBorrowBook.ValueMember = "Value";
            }

            if (cmbBorrowReader != null)
            {
                cmbBorrowReader.DataSource = service.Data.Readers
                    .OrderBy(r => r.StudentId)
                    .Select(r => new ComboItem
                    {
                        Value = r.StudentId,
                        Display = string.Format("{0} - {1}", r.StudentId, r.Name)
                    })
                    .ToList();
                cmbBorrowReader.DisplayMember = "Display";
                cmbBorrowReader.ValueMember = "Value";
            }
        }

        private void RefreshAdminBooksGrid()
        {
            if (dgvAdminBooks == null) return;

            IEnumerable<Book> query = service.Data.Books;

            if (!string.IsNullOrWhiteSpace(txtBookSearchTitle.Text))
                query = query.Where(b => b.Title.IndexOf(txtBookSearchTitle.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtBookSearchAuthor.Text))
                query = query.Where(b => b.Author.IndexOf(txtBookSearchAuthor.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtBookSearchCategory.Text))
                query = query.Where(b => b.Category.IndexOf(txtBookSearchCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);

            dgvAdminBooks.DataSource = query
                .OrderBy(b => b.Id)
                .Select(b => new
                {
                    b.Id,
                    书名 = b.Title,
                    作者 = b.Author,
                    ISBN = b.ISBN,
                    分类 = b.Category,
                    馆藏位置 = b.Location,
                    总库存 = b.TotalStock,
                    可借库存 = b.AvailableStock,
                    已借次数 = b.BorrowCount,
                    馆藏状态 = b.AvailableStock > 0 ? "在馆可借" : "已借完"
                })
                .ToList();
        }

        private void RefreshReadersGrid()
        {
            if (dgvReaders == null) return;

            IEnumerable<Reader> query = service.Data.Readers;
            if (!string.IsNullOrWhiteSpace(txtReaderSearchKeyword.Text))
            {
                string keyword = txtReaderSearchKeyword.Text.Trim();
                query = query.Where(r =>
                    r.StudentId.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.Department.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            dgvReaders.DataSource = query
                .OrderBy(r => r.StudentId)
                .Select(r => new
                {
                    学号 = r.StudentId,
                    姓名 = r.Name,
                    院系 = r.Department,
                    电话 = r.Phone,
                    当前借阅册数 = service.GetActiveBorrowCount(r.StudentId)
                })
                .ToList();
        }

        private void RefreshRecordsGrid()
        {
            if (dgvRecords == null) return;

            IEnumerable<BorrowRecord> query = service.Data.BorrowRecords;

            if (!string.IsNullOrWhiteSpace(txtRecordSearchBook.Text))
                query = query.Where(r => r.BookTitle.IndexOf(txtRecordSearchBook.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtRecordSearchStudentId.Text))
                query = query.Where(r => r.ReaderStudentId.IndexOf(txtRecordSearchStudentId.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);

            string status = cmbRecordStatus == null ? "全部" : Convert.ToString(cmbRecordStatus.SelectedItem);
            if (status == "借阅中")
                query = query.Where(r => !r.IsReturned);
            else if (status == "已归还")
                query = query.Where(r => r.IsReturned);
            else if (status == "超期")
                query = query.Where(r => !r.IsReturned && r.DueDate.Date < DateTime.Today);

            dgvRecords.DataSource = query
                .OrderByDescending(r => r.BorrowDate)
                .Select(r => new
                {
                    记录编号 = r.RecordId,
                    图书编号 = r.BookId,
                    书名 = r.BookTitle,
                    学号 = r.ReaderStudentId,
                    读者 = r.ReaderName,
                    借阅日期 = r.BorrowDate.ToString("yyyy-MM-dd"),
                    应还日期 = r.DueDate.ToString("yyyy-MM-dd"),
                    归还日期 = r.IsReturned ? r.ReturnDateText : "-",
                    借阅状态 = r.IsReturned ? "已归还" : (r.DueDate.Date < DateTime.Today ? "超期" : "借阅中")
                })
                .ToList();
        }

        private void RefreshAdminsGrid()
        {
            if (dgvAdmins == null) return;

            dgvAdmins.DataSource = service.Data.Admins
                .OrderBy(a => a.Account)
                .Select(a => new
                {
                    账号 = a.Account,
                    姓名 = a.Name
                })
                .ToList();
        }

        private void RefreshStatistics()
        {
            if (lblStatsSummary == null) return;

            int totalBooks = service.Data.Books.Sum(b => b.TotalStock);
            int availableBooks = service.Data.Books.Sum(b => b.AvailableStock);
            int borrowedBooks = totalBooks - availableBooks;
            double borrowRate = totalBooks == 0 ? 0 : (borrowedBooks * 100.0 / totalBooks);
            int activeRecords = service.Data.BorrowRecords.Count(r => !r.IsReturned);
            int returnedRecords = service.Data.BorrowRecords.Count(r => r.IsReturned);
            int overdueRecords = service.Data.BorrowRecords.Count(r => !r.IsReturned && r.DueDate.Date < DateTime.Today);

            lblStatsSummary.Text =
                "集中数据概况\r\n" +
                string.Format("图书总册数：{0}    当前可借：{1}    当前借出：{2}    借阅率：{3:F2}%\r\n", totalBooks, availableBooks, borrowedBooks, borrowRate) +
                string.Format("读者总数：{0}    管理员总数：{1}    借阅记录总数：{2}\r\n", service.Data.Readers.Count, service.Data.Admins.Count, service.Data.BorrowRecords.Count) +
                string.Format("当前借阅中：{0}    已归还：{1}    超期未还：{2}\r\n", activeRecords, returnedRecords, overdueRecords) +
                string.Format("最近数据更新时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            dgvHotBooks.DataSource = service.Data.Books
                .OrderByDescending(b => b.BorrowCount)
                .ThenBy(b => b.Title)
                .Take(10)
                .Select(b => new
                {
                    书名 = b.Title,
                    作者 = b.Author,
                    分类 = b.Category,
                    借阅次数 = b.BorrowCount,
                    可借库存 = b.AvailableStock
                })
                .ToList();

            dgvCategoryStats.DataSource = service.Data.Books
                .GroupBy(b => b.Category)
                .Select(g => new
                {
                    分类 = g.Key,
                    图书种数 = g.Count(),
                    馆藏总量 = g.Sum(x => x.TotalStock),
                    在馆数量 = g.Sum(x => x.AvailableStock)
                })
                .OrderByDescending(x => x.馆藏总量)
                .ToList();

            dgvOverdueStats.DataSource = service.Data.BorrowRecords
                .Where(r => !r.IsReturned && r.DueDate.Date < DateTime.Today)
                .OrderBy(r => r.DueDate)
                .Select(r => new
                {
                    记录编号 = r.RecordId,
                    书名 = r.BookTitle,
                    学号 = r.ReaderStudentId,
                    读者 = r.ReaderName,
                    应还日期 = r.DueDate.ToString("yyyy-MM-dd"),
                    超期天数 = (DateTime.Today - r.DueDate.Date).Days
                })
                .ToList();
        }

        private void RefreshStudentBooksGrid()
        {
            if (dgvStudentBooks == null) return;

            IEnumerable<Book> query = service.Data.Books;

            if (!string.IsNullOrWhiteSpace(txtStudentBookSearchTitle.Text))
                query = query.Where(b => b.Title.IndexOf(txtStudentBookSearchTitle.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtStudentBookSearchAuthor.Text))
                query = query.Where(b => b.Author.IndexOf(txtStudentBookSearchAuthor.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtStudentBookSearchIsbn.Text))
                query = query.Where(b => b.ISBN.IndexOf(txtStudentBookSearchIsbn.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(txtStudentBookSearchCategory.Text))
                query = query.Where(b => b.Category.IndexOf(txtStudentBookSearchCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);

            dgvStudentBooks.DataSource = query
                .OrderBy(b => b.Title)
                .Select(b => new
                {
                    b.Id,
                    书名 = b.Title,
                    作者 = b.Author,
                    ISBN = b.ISBN,
                    分类 = b.Category,
                    馆藏位置 = b.Location,
                    可借库存 = b.AvailableStock,
                    状态 = b.AvailableStock > 0 ? "可借" : "不可借"
                })
                .ToList();
        }

        private void RefreshStudentRecordGrid()
        {
            if (dgvStudentRecords == null) return;

            if (currentUser == null || currentUser.Role != UserRole.Student)
            {
                dgvStudentRecords.DataSource = new List<object>();
                return;
            }

            dgvStudentRecords.DataSource = service.Data.BorrowRecords
                .Where(r => r.ReaderStudentId == currentUser.Account)
                .OrderByDescending(r => r.BorrowDate)
                .Select(r => new
                {
                    记录编号 = r.RecordId,
                    图书编号 = r.BookId,
                    书名 = r.BookTitle,
                    借阅日期 = r.BorrowDate.ToString("yyyy-MM-dd"),
                    应还日期 = r.DueDate.ToString("yyyy-MM-dd"),
                    归还日期 = r.IsReturned ? r.ReturnDateText : "-",
                    状态 = r.IsReturned ? "已归还" : (r.DueDate.Date < DateTime.Today ? "超期" : "借阅中")
                })
                .ToList();
        }

        private void RefreshStudentProfile()
        {
            if (lblStudentProfile == null) return;

            if (currentUser == null || currentUser.Role != UserRole.Student)
            {
                lblStudentProfile.Text = "请先登录学生账号。";
                return;
            }

            Reader reader = service.Data.Readers.FirstOrDefault(r => r.StudentId == currentUser.Account);
            if (reader == null)
            {
                lblStudentProfile.Text = "未找到当前学生信息。";
                return;
            }

            lblStudentProfile.Text =
                string.Format("学号：{0}\r\n", reader.StudentId) +
                string.Format("姓名：{0}\r\n", reader.Name) +
                string.Format("院系：{0}\r\n", reader.Department) +
                string.Format("电话：{0}\r\n", reader.Phone) +
                string.Format("当前借阅册数：{0}", service.GetActiveBorrowCount(reader.StudentId));
        }

        // ==================== 登录 / 退出 ====================

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string account = txtLoginAccount.Text.Trim();
            string password = txtLoginPassword.Text.Trim();
            UserRole role = (UserRole)cmbLoginRole.SelectedIndex;

            string error;
            LoginUser user = service.Login(account, password, role, out error);
            if (error != null)
            {
                ShowWarning(error);
                return;
            }

            UpdateUserState(user);
            RefreshAllViews();
            MessageBox.Show("登录成功。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            UpdateUserState(null);
            txtLoginAccount.Text = string.Empty;
            RefreshAllViews();
        }

        // ==================== 管理员 - 图书管理 ====================

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            string error = service.AddBook(
                txtBookTitle.Text.Trim(),
                txtBookAuthor.Text.Trim(),
                txtBookIsbn.Text.Trim(),
                txtBookCategory.Text.Trim(),
                txtBookLocation.Text.Trim(),
                Convert.ToInt32(numBookTotal.Value),
                Convert.ToInt32(numBookAvailable.Value));

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("图书新增成功。");
            ClearBookForm();
        }

        private void BtnUpdateBook_Click(object sender, EventArgs e)
        {
            int bookId;
            if (!int.TryParse(txtBookId.Text.Trim(), out bookId)) { ShowWarning("请先选择需要修改的图书。"); return; }

            string error = service.UpdateBook(
                bookId,
                txtBookTitle.Text.Trim(),
                txtBookAuthor.Text.Trim(),
                txtBookIsbn.Text.Trim(),
                txtBookCategory.Text.Trim(),
                txtBookLocation.Text.Trim(),
                Convert.ToInt32(numBookTotal.Value),
                Convert.ToInt32(numBookAvailable.Value));

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("图书信息修改成功。");
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            int bookId;
            if (!int.TryParse(txtBookId.Text.Trim(), out bookId)) { ShowWarning("请先选择需要删除的图书。"); return; }

            if (service.Data.BorrowRecords.Any(r => r.BookId == bookId && !r.IsReturned))
            {
                ShowWarning("该图书仍存在未归还记录，不能删除。");
                return;
            }

            if (MessageBox.Show("确认删除该图书吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            service.DeleteBook(bookId);
            ShowSuccessAndRefresh("图书删除成功。");
            ClearBookForm();
        }

        private void BtnSearchAdminBooks_Click(object sender, EventArgs e) { RefreshAdminBooksGrid(); }

        private void BtnResetAdminBookFilters_Click(object sender, EventArgs e)
        {
            txtBookSearchTitle.Text = string.Empty;
            txtBookSearchAuthor.Text = string.Empty;
            txtBookSearchCategory.Text = string.Empty;
            RefreshAdminBooksGrid();
        }

        private void BtnClearBookForm_Click(object sender, EventArgs e) { ClearBookForm(); }

        // ==================== 管理员 - 读者管理 ====================

        private void BtnAddReader_Click(object sender, EventArgs e)
        {
            string error = service.AddReader(
                txtReaderStudentId.Text.Trim(),
                txtReaderName.Text.Trim(),
                txtReaderDepartment.Text.Trim(),
                txtReaderPhone.Text.Trim(),
                txtReaderPassword.Text.Trim());

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("读者新增成功。");
            ClearReaderForm();
        }

        private void BtnUpdateReader_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReaderStudentId.Text)) { ShowWarning("请先选择读者。"); return; }

            string error = service.UpdateReader(
                txtReaderStudentId.Text.Trim(),
                txtReaderName.Text.Trim(),
                txtReaderDepartment.Text.Trim(),
                txtReaderPhone.Text.Trim(),
                txtReaderPassword.Text.Trim());

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("读者信息修改成功。");
        }

        private void BtnDeleteReader_Click(object sender, EventArgs e)
        {
            string studentId = txtReaderStudentId.Text.Trim();
            if (string.IsNullOrWhiteSpace(studentId)) { ShowWarning("请先选择读者。"); return; }

            if (service.Data.BorrowRecords.Any(r => r.ReaderStudentId == studentId && !r.IsReturned))
            {
                ShowWarning("该读者仍有未归还图书，不能删除。");
                return;
            }

            if (MessageBox.Show("确认删除该读者吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            service.DeleteReader(studentId);
            ShowSuccessAndRefresh("读者删除成功。");
            ClearReaderForm();
        }

        private void BtnSearchReaders_Click(object sender, EventArgs e) { RefreshReadersGrid(); }

        private void BtnResetReaderFilters_Click(object sender, EventArgs e)
        {
            txtReaderSearchKeyword.Text = string.Empty;
            RefreshReadersGrid();
        }

        private void BtnClearReaderForm_Click(object sender, EventArgs e) { ClearReaderForm(); }

        // ==================== 管理员 - 借阅管理 ====================

        private void BtnAddBorrow_Click(object sender, EventArgs e)
        {
            if (cmbBorrowReader.SelectedValue == null || cmbBorrowBook.SelectedValue == null)
            {
                ShowWarning("请先选择读者和图书。");
                return;
            }

            string studentId = Convert.ToString(cmbBorrowReader.SelectedValue);
            int bookId = Convert.ToInt32(cmbBorrowBook.SelectedValue);

            string error = service.BorrowBook(studentId, bookId, Convert.ToInt32(numBorrowDays.Value));
            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("借阅登记成功。");
        }

        private void BtnReturnBook_Click(object sender, EventArgs e)
        {
            int recordId = GetSelectedIntValue(dgvRecords, "记录编号");
            if (recordId <= 0) { ShowWarning("请先选择借阅记录。"); return; }

            BorrowRecord record = service.Data.BorrowRecords.FirstOrDefault(r => r.RecordId == recordId);
            if (record == null) { ShowWarning("未找到借阅记录。"); return; }
            if (record.IsReturned) { ShowWarning("该记录已归还。"); return; }

            if (MessageBox.Show("确认登记归还吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            service.ReturnBook(recordId);
            ShowSuccessAndRefresh("图书归还成功。");
        }

        private void BtnExtendBorrow_Click(object sender, EventArgs e)
        {
            int recordId = GetSelectedIntValue(dgvRecords, "记录编号");
            if (recordId <= 0) { ShowWarning("请先选择借阅记录。"); return; }

            string error = service.ExtendBorrow(recordId, Convert.ToInt32(numExtendDays.Value));
            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("到期日期修改成功。");
        }

        private void BtnDeleteRecord_Click(object sender, EventArgs e)
        {
            int recordId = GetSelectedIntValue(dgvRecords, "记录编号");
            if (recordId <= 0) { ShowWarning("请先选择借阅记录。"); return; }

            if (MessageBox.Show("确认删除该借阅记录吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string error = service.DeleteRecord(recordId);
            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("借阅记录删除成功。");
        }

        private void BtnSearchRecords_Click(object sender, EventArgs e) { RefreshRecordsGrid(); }

        private void BtnResetRecordFilters_Click(object sender, EventArgs e)
        {
            txtRecordSearchBook.Text = string.Empty;
            txtRecordSearchStudentId.Text = string.Empty;
            cmbRecordStatus.SelectedIndex = 0;
            RefreshRecordsGrid();
        }

        // ==================== 管理员 - 管理员账号管理 ====================

        private void BtnAddAdmin_Click(object sender, EventArgs e)
        {
            string error = service.AddAdmin(
                txtAdminAccount.Text.Trim(),
                txtAdminName.Text.Trim(),
                txtAdminPassword.Text.Trim());

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("管理员账号新增成功。");
            ClearAdminForm();
        }

        private void BtnUpdateAdmin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdminAccount.Text)) { ShowWarning("请先选择管理员账号。"); return; }

            string error = service.UpdateAdmin(
                txtAdminAccount.Text.Trim(),
                txtAdminName.Text.Trim(),
                txtAdminPassword.Text.Trim());

            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("管理员账号修改成功。");
        }

        private void BtnDeleteAdmin_Click(object sender, EventArgs e)
        {
            string account = txtAdminAccount.Text.Trim();
            if (string.IsNullOrWhiteSpace(account)) { ShowWarning("请先选择管理员账号。"); return; }

            if (account == "admin") { ShowWarning("默认管理员账号不允许删除。"); return; }

            if (MessageBox.Show("确认删除该管理员账号吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            service.DeleteAdmin(account);
            ShowSuccessAndRefresh("管理员账号删除成功。");
            ClearAdminForm();
        }

        private void BtnClearAdminForm_Click(object sender, EventArgs e) { ClearAdminForm(); }

        // ==================== 学生 - 图书查询与借阅 ====================

        private void BtnStudentBorrow_Click(object sender, EventArgs e)
        {
            if (currentUser == null || currentUser.Role != UserRole.Student)
            {
                ShowWarning("请先登录学生账号。");
                return;
            }

            int bookId = GetSelectedIntValue(dgvStudentBooks, "Id");
            if (bookId <= 0) { ShowWarning("请先选择图书。"); return; }

            string error = service.BorrowBook(currentUser.Account, bookId, 30);
            if (error != null) { ShowWarning(error); return; }
            ShowSuccessAndRefresh("借阅登记成功。");
        }

        private void BtnSearchStudentBooks_Click(object sender, EventArgs e) { RefreshStudentBooksGrid(); }

        private void BtnResetStudentBookFilters_Click(object sender, EventArgs e)
        {
            txtStudentBookSearchTitle.Text = string.Empty;
            txtStudentBookSearchAuthor.Text = string.Empty;
            txtStudentBookSearchIsbn.Text = string.Empty;
            txtStudentBookSearchCategory.Text = string.Empty;
            RefreshStudentBooksGrid();
        }

        // ==================== 学生 - 借阅记录 ====================

        private void BtnStudentReturn_Click(object sender, EventArgs e)
        {
            if (currentUser == null || currentUser.Role != UserRole.Student)
            {
                ShowWarning("请先登录学生账号。");
                return;
            }

            int recordId = GetSelectedIntValue(dgvStudentRecords, "记录编号");
            if (recordId <= 0) { ShowWarning("请先选择借阅记录。"); return; }

            BorrowRecord record = service.Data.BorrowRecords.FirstOrDefault(r => r.RecordId == recordId);
            if (record == null || record.ReaderStudentId != currentUser.Account)
            {
                ShowWarning("只能归还自己的图书。");
                return;
            }
            if (record.IsReturned) { ShowWarning("该记录已归还。"); return; }

            if (MessageBox.Show("确认登记归还吗？", "二次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            service.ReturnBook(recordId);
            ShowSuccessAndRefresh("图书归还成功。");
        }

        private void BtnRefreshStudentRecords_Click(object sender, EventArgs e) { RefreshStudentRecordGrid(); }

        // ==================== 学生 - 个人中心 ====================

        private void BtnChangeStudentPassword_Click(object sender, EventArgs e)
        {
            if (currentUser == null || currentUser.Role != UserRole.Student)
            {
                ShowWarning("请先登录学生账号。");
                return;
            }

            if (txtStudentNewPassword.Text != txtStudentConfirmPassword.Text)
            {
                ShowWarning("两次输入的新密码不一致。");
                return;
            }

            string error = service.ChangePassword(
                currentUser.Account,
                txtStudentOldPassword.Text.Trim(),
                txtStudentNewPassword.Text.Trim());

            if (error != null) { ShowWarning(error); return; }

            ShowSuccessAndRefresh("密码修改成功。");
            txtStudentOldPassword.Text = string.Empty;
            txtStudentNewPassword.Text = string.Empty;
            txtStudentConfirmPassword.Text = string.Empty;
        }

        // ==================== Grid 选择事件 ====================

        private void DgvAdminBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAdminBooks.CurrentRow == null) return;

            int bookId = GetSelectedIntValue(dgvAdminBooks, "Id");
            Book book = service.Data.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return;

            txtBookId.Text = book.Id.ToString();
            txtBookTitle.Text = book.Title;
            txtBookAuthor.Text = book.Author;
            txtBookIsbn.Text = book.ISBN;
            txtBookCategory.Text = book.Category;
            txtBookLocation.Text = book.Location;
            numBookTotal.Value = book.TotalStock;
            numBookAvailable.Value = book.AvailableStock;
        }

        private void DgvReaders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReaders.CurrentRow == null) return;

            string studentId = GetSelectedStringValue(dgvReaders, "学号");
            Reader reader = service.Data.Readers.FirstOrDefault(r => r.StudentId == studentId);
            if (reader == null) return;

            txtReaderStudentId.Text = reader.StudentId;
            txtReaderName.Text = reader.Name;
            txtReaderDepartment.Text = reader.Department;
            txtReaderPhone.Text = reader.Phone;
            txtReaderPassword.Text = reader.Password;
        }

        private void DgvAdmins_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAdmins.CurrentRow == null) return;

            string account = GetSelectedStringValue(dgvAdmins, "账号");
            AdminAccount admin = service.Data.Admins.FirstOrDefault(a => a.Account == account);
            if (admin == null) return;

            txtAdminAccount.Text = admin.Account;
            txtAdminName.Text = admin.Name;
            txtAdminPassword.Text = admin.Password;
        }

        // ==================== 表单清空 ====================

        private void ClearBookForm()
        {
            txtBookId.Text = string.Empty;
            txtBookTitle.Text = string.Empty;
            txtBookAuthor.Text = string.Empty;
            txtBookIsbn.Text = string.Empty;
            txtBookCategory.Text = string.Empty;
            txtBookLocation.Text = string.Empty;
            numBookTotal.Value = 1;
            numBookAvailable.Value = 1;
        }

        private void ClearReaderForm()
        {
            txtReaderStudentId.Text = string.Empty;
            txtReaderName.Text = string.Empty;
            txtReaderDepartment.Text = string.Empty;
            txtReaderPhone.Text = string.Empty;
            txtReaderPassword.Text = string.Empty;
        }

        private void ClearAdminForm()
        {
            txtAdminAccount.Text = string.Empty;
            txtAdminName.Text = string.Empty;
            txtAdminPassword.Text = string.Empty;
        }

        // ==================== UI 通用工具方法 ====================

        private int GetSelectedIntValue(DataGridView grid, string columnName)
        {
            if (grid == null || grid.CurrentRow == null || !grid.Columns.Contains(columnName))
                return 0;

            object value = grid.CurrentRow.Cells[columnName].Value;
            int result;
            return value != null && int.TryParse(value.ToString(), out result) ? result : 0;
        }

        private string GetSelectedStringValue(DataGridView grid, string columnName)
        {
            if (grid == null || grid.CurrentRow == null || !grid.Columns.Contains(columnName))
                return string.Empty;

            object value = grid.CurrentRow.Cells[columnName].Value;
            return value == null ? string.Empty : value.ToString();
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return CreateLabel(text, x, y, Color.Black);
        }

        private Label CreateLabel(string text, int x, int y, Color color)
        {
            Label label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.ForeColor = color;
            label.Location = new Point(x, y);
            return label;
        }

        private TextBox CreateTextBox(int x, int y, int width, bool readOnly)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Width = width;
            textBox.ReadOnly = readOnly;
            return textBox;
        }

        private NumericUpDown CreateNumeric(int x, int y, int width, int min, int max, int value)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Location = new Point(x, y);
            numeric.Width = width;
            numeric.Minimum = min;
            numeric.Maximum = max;
            numeric.Value = value;
            return numeric;
        }

        private Button CreateButton(string text, int x, int y, int width, EventHandler handler)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, 30);
            button.Click += handler;
            return button;
        }

        private Panel CreateTopPanel()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Height = 110;
            panel.BackColor = Color.White;
            panel.Padding = new Padding(8);
            panel.AutoScroll = true;
            return panel;
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.ScrollBars = ScrollBars.Both;
            return grid;
        }

        private Panel CreatePageTopPanel(TabPage page, int topHeight)
        {
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.FixedPanel = FixedPanel.Panel1;
            split.IsSplitterFixed = true;
            split.SplitterWidth = 1;
            split.Panel2MinSize = 100;
            split.Tag = "PageSplitHost";
            page.Controls.Add(split);
            split.Panel1MinSize = 0;
            split.HandleCreated += (sender, args) => SetSafeSplitterDistance(split, topHeight, 0);
            split.SizeChanged += (sender, args) => SetSafeSplitterDistance(split, topHeight, 0);

            Panel topPanel = CreateTopPanel();
            split.Panel1.Controls.Add(topPanel);
            return topPanel;
        }

        private void BindGridToPage(TabPage page, DataGridView grid)
        {
            SplitContainer split = GetPageSplitHost(page);
            if (split != null)
            {
                split.Panel2.Controls.Clear();
                split.Panel2.Controls.Add(grid);
            }
        }

        private void BindFillContentToPage(TabPage page, Control control)
        {
            SplitContainer split = GetPageSplitHost(page);
            if (split != null)
            {
                split.Panel2.Controls.Clear();
                split.Panel2.Controls.Add(control);
            }
        }

        private SplitContainer GetPageSplitHost(TabPage page)
        {
            foreach (Control control in page.Controls)
            {
                if (control is SplitContainer)
                {
                    SplitContainer split = control as SplitContainer;
                    if (split != null && Equals(split.Tag, "PageSplitHost"))
                        return split;
                }
            }

            return null;
        }

        private FlowLayoutPanel CreateToolbarTable()
        {
            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = true;
            flow.AutoScroll = false;
            flow.Padding = new Padding(4);
            flow.Margin = new Padding(0);
            return flow;
        }

        private void AdjustPageTopHeight(TabPage page, FlowLayoutPanel layout, int minHeight)
        {
            if (page == null || layout == null) return;

            layout.PerformLayout();
            int requiredHeight = Math.Max(minHeight, layout.PreferredSize.Height + 20);

            SplitContainer split = GetPageSplitHost(page);
            if (split == null) return;

            SetSafeSplitterDistance(split, requiredHeight, minHeight);
        }

        private void SetSafeSplitterDistance(SplitContainer split, int desiredHeight, int minHeight)
        {
            if (split == null || split.IsDisposed) return;

            int axisLength = split.Orientation == Orientation.Horizontal ? split.ClientSize.Height : split.ClientSize.Width;
            int maxDistance = axisLength - split.SplitterWidth - split.Panel2MinSize;
            if (maxDistance < 0) return;

            int minAllowed = Math.Max(0, Math.Min(minHeight, maxDistance));
            int target = Math.Max(minAllowed, Math.Min(desiredHeight, maxDistance));

            try
            {
                split.Panel1MinSize = minAllowed;
                split.SplitterDistance = target;
            }
            catch (InvalidOperationException)
            {
                // Ignore transient layout timing issues; SizeChanged will retry.
            }
        }

        private void AddToolbarField(FlowLayoutPanel table, string labelText, Control inputControl)
        {
            FlowLayoutPanel field = new FlowLayoutPanel();
            field.AutoSize = true;
            field.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            field.WrapContents = false;
            field.FlowDirection = FlowDirection.LeftToRight;
            field.Margin = new Padding(6, 6, 10, 6);

            Label label = new Label();
            label.Text = labelText;
            label.AutoSize = true;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(0, 8, 4, 0);

            inputControl.Margin = new Padding(0, 4, 0, 0);

            field.Controls.Add(label);
            field.Controls.Add(inputControl);
            table.Controls.Add(field);
        }

        private void AddToolbarButton(FlowLayoutPanel table, string text, int width, EventHandler handler)
        {
            Button button = new Button();
            button.Text = text;
            button.Width = width;
            button.Height = 30;
            button.Margin = new Padding(0, 6, 8, 6);
            button.Click += handler;
            table.Controls.Add(button);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
