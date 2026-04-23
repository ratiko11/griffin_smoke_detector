using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace griffin_smoke_detector
{
    public partial class Form1 : Form
    {
        // --- მონაცემების და ლოგიკის ცვლადები ---
        private string filePath = "users.json"; // ფაილი, სადაც ინახება მომხმარებლების ინფორმაცია
        private List<UserData> usersList = new List<UserData>(); // ყველა რეგისტრირებული მომხმარებლის სია
        private UserData currentUser; // მიმდინარე ავტორიზებული მომხმარებელი
        public string[] text = new string[20]; // ენის მასივი ტექსტებისთვის
        public int attempts = 0; // არასწორი პაროლის შეყვანის მცდელობები
        public int lockoutTime = 60; // დაბლოკვის საწყისი დრო (წამებში)
        public int secondsRemaining = 0; // დარჩენილი დრო დაბლოკვის დასრულებამდე
        private System.Windows.Forms.Timer lockoutTimer; // ტაიმერი დაბლოკვის დროის სათვლელად

        // --- სიმულაციის ცვლადები ---
        private System.Windows.Forms.Timer simulationTimer; // ტაიმერი, რომელიც "აფუჭებს" კვამლის დონეს (იმიტაცია)
        private Random rnd = new Random(); // შემთხვევითი რიცხვების გენერატორი
        private List<DeviceUI> deviceRows = new List<DeviceUI>(); // ვიზუალური დეტექტორების სიის კონტროლერი
        private int detectorCount = 0; // დამატებული დეტექტორების რაოდენობა
        private const int MaxDetectors = 5; // მაქსიმუმ რამდენი დეტექტორის დამატება შეიძლება

        // --- დინამიური კონტროლები (UI) ---
        private GroupBox gb_login; // ავტორიზაციის კონტეინერი
        private Label lb_signin, lb_signin1, lb_signin_email, lb_signin_pasw, lb_error;
        private TextBox tb_email, tb_pasw;
        private Button bt_signin, bt_signin_register;

        private GroupBox gb_reg; // რეგისტრაციის კონტეინერი
        private Label lb_reg, lb_reginfo, lb_regmail, lb_regpasw, lb_regpaswconf, lb_regerror;
        private TextBox tb_regmail, tb_regpasw, tb_regpaswconf;
        private Button bt_reg;

        private GroupBox gb_dashboard; // მთავარი მართვის პანელი (ავტორიზაციის შემდეგ)
        private Label lb_user_email, lb_dashboard_title, lb_total_count_val;
        private Button bt_logout, bt_add_device;
        private Panel pnl_cards_container; // დეტექტორების ბარათების სათავსო

        public Form1()
        {
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadUsers(); // მომხმარებლების ჩატვირთვა JSON ფაილიდან
            InitializeLanguageArray(); // ტექსტების მომზადება
            CreateDynamicControls(); // ინტერფეისის აწყობა

            // დაბლოკვის ტაიმერის გამართვა
            lockoutTimer = new System.Windows.Forms.Timer();
            lockoutTimer.Interval = 1000;
            lockoutTimer.Tick += LockoutTimer_Tick;

            // სიმულაციის ტაიმერის გამართვა
            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = 2000; // ყოველ 2 წამში მონაცემების განახლება
            simulationTimer.Tick += SimulationTimer_Tick;

            this.Resize += Form1_Resize; // ფორმის ზომის შეცვლისას ცენტრირებისთვის
        }

        // ენის მასივის შევსება
        private void InitializeLanguageArray()
        {
            text[0] = "ავტორიზაცია";
            text[1] = "შეიყვანეთ თქვენი მონაცემები სისტემაში შესასვლელად";
            text[2] = "ელ-ფოსტა:";
            text[3] = "პაროლი:";
            text[4] = "შესვლა";
            text[5] = "არ გაქვთ ანგარიში? რეგისტრაცია";
            text[6] = "სწორია";
            text[7] = "პაროლი ან ელ-ფოსტა არასწორია";
            text[8] = "რეგისტრაცია";
            text[9] = "შეიყვანეთ მონაცემები ახალი ანგარიშის შესაქმნელად";
            text[10] = "ელ-ფოსტა:";
            text[11] = "პაროლი:";
            text[12] = "დაადასტურეთ პაროლი:";
            text[13] = "რეგისტრაცია";
            text[14] = "მონაცემები არასწორია ან მეილი უკვე დაკავებულია";
            text[15] = "უკან დაბრუნება";
        }

        // UI-ს შექმნა კოდით (დინამიურად)
        private void CreateDynamicControls()
        {
            this.BackColor = Color.FromArgb(9, 13, 27);
            this.Text = "Griffin კვამლის დეტექტორი";

            int controlWidth = 370;
            int leftPadding = 40;

            // --- Dashboard-ის (მთავარი პანელის) აწყობა ---
            gb_dashboard = new GroupBox { Size = new Size(900, 650), Text = "", Visible = false, FlatStyle = FlatStyle.Flat };
            gb_dashboard.BackColor = Color.FromArgb(15, 18, 34);
            gb_dashboard.ForeColor = Color.White;

            lb_dashboard_title = new Label { Text = "🔥 Griffin კვამლის დეტექტორი", ForeColor = Color.FromArgb(0, 255, 149), Font = new Font("Segoe UI", 14, FontStyle.Bold), Top = 20, Left = 25, AutoSize = true };
            lb_user_email = new Label { Text = "", ForeColor = Color.LightGray, Top = 25, Left = 600, AutoSize = true, Font = new Font("Segoe UI", 9) };

            bt_logout = new Button { Text = "გასვლა ↪", Top = 20, Left = 800, Width = 90, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9) };
            bt_logout.FlatAppearance.BorderSize = 0;
            bt_logout.Click += (s, e) => {
                gb_dashboard.Visible = false;
                gb_login.Visible = true;
                simulationTimer.Stop(); // სიმულაციის შეჩერება გასვლისას
                currentUser = null;
            };

            bt_add_device = new Button { Text = "+ დეტექტორის დამატება", Top = 18, Left = 380, Width = 180, Height = 35, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 255, 149), ForeColor = Color.FromArgb(32, 39, 60), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            bt_add_device.FlatAppearance.BorderSize = 0;
            bt_add_device.Click += bt_add_device_Click;

            pnl_cards_container = new Panel { Top = 70, Left = 20, Size = new Size(860, 550), BackColor = Color.Transparent, AutoScroll = true };

            gb_dashboard.Controls.AddRange(new Control[] { lb_dashboard_title, lb_user_email, bt_logout, bt_add_device, pnl_cards_container });
            this.Controls.Add(gb_dashboard);

            // --- ავტორიზაციის (Login) ფორმის აწყობა ---
            gb_login = new GroupBox { Size = new Size(450, 420), Text = "", FlatStyle = FlatStyle.Flat };
            gb_login.BackColor = Color.FromArgb(17, 23, 39);
            gb_login.ForeColor = Color.White;

            lb_signin = new Label { Text = text[0], Font = new Font("Segoe UI", 16, FontStyle.Bold), Top = 25, AutoSize = true };
            lb_signin1 = new Label { Text = text[1], Top = 65, AutoSize = true, ForeColor = Color.LightGray };
            lb_signin_email = new Label { Text = text[2], Top = 115, Left = leftPadding, AutoSize = true };
            tb_email = new TextBox { Top = 140, Left = leftPadding, Width = controlWidth, BackColor = Color.FromArgb(32, 39, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, Height = 30 };
            lb_signin_pasw = new Label { Text = text[3], Top = 185, Left = leftPadding, AutoSize = true };
            tb_pasw = new TextBox { Top = 210, Left = leftPadding, Width = controlWidth, BackColor = Color.FromArgb(32, 39, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, PasswordChar = '*', Height = 30 };
            lb_error = new Label { Text = "", Top = 255, AutoSize = true };
            bt_signin = new Button { Text = text[4], Top = 290, Left = leftPadding, Width = controlWidth, Height = 45, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 255, 149), ForeColor = Color.FromArgb(32, 39, 60), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            bt_signin.FlatAppearance.BorderSize = 0;
            bt_signin.Click += bt_signin_Click;
            bt_signin_register = new Button { Text = text[5], Top = 350, Left = leftPadding, Width = controlWidth, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
            bt_signin_register.FlatAppearance.BorderSize = 0;
            bt_signin_register.Click += bt_signin_register_Click;

            gb_login.Controls.AddRange(new Control[] { lb_signin, lb_signin1, lb_signin_email, tb_email, lb_signin_pasw, tb_pasw, lb_error, bt_signin, bt_signin_register });
            this.Controls.Add(gb_login);

            // --- რეგისტრაციის (Register) ფორმის აწყობა ---
            gb_reg = new GroupBox { Size = new Size(450, 500), Text = "", Visible = false, FlatStyle = FlatStyle.Flat };
            gb_reg.BackColor = Color.FromArgb(17, 23, 39);
            gb_reg.ForeColor = Color.White;

            lb_reg = new Label { Text = text[8], Font = new Font("Segoe UI", 16, FontStyle.Bold), Top = 25, AutoSize = true };
            lb_reginfo = new Label { Text = text[9], Top = 65, AutoSize = true, ForeColor = Color.LightGray };
            lb_regmail = new Label { Text = text[10], Top = 115, Left = leftPadding, AutoSize = true };
            tb_regmail = new TextBox { Top = 140, Left = leftPadding, Width = controlWidth, BackColor = Color.FromArgb(32, 39, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, Height = 30 };
            lb_regpasw = new Label { Text = text[11], Top = 190, Left = leftPadding, AutoSize = true };
            tb_regpasw = new TextBox { Top = 215, Left = leftPadding, Width = controlWidth, BackColor = Color.FromArgb(32, 39, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, PasswordChar = '*', Height = 30 };
            lb_regpaswconf = new Label { Text = text[12], Top = 265, Left = leftPadding, AutoSize = true };
            tb_regpaswconf = new TextBox { Top = 290, Left = leftPadding, Width = controlWidth, BackColor = Color.FromArgb(32, 39, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, PasswordChar = '*', Height = 30 };
            lb_regerror = new Label { Text = "", Top = 330, AutoSize = true };
            bt_reg = new Button { Text = text[13], Top = 370, Left = leftPadding, Width = controlWidth, Height = 45, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 255, 149), ForeColor = Color.FromArgb(32, 39, 60), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            bt_reg.FlatAppearance.BorderSize = 0;
            bt_reg.Click += bt_reg_Click;

            Button bt_reg_back = new Button { Text = "← " + text[15], Top = 430, Left = leftPadding, Width = controlWidth, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
            bt_reg_back.FlatAppearance.BorderSize = 0;
            bt_reg_back.Click += (s, e) => { gb_reg.Visible = false; gb_login.Visible = true; };

            gb_reg.Controls.AddRange(new Control[] { lb_reg, lb_reginfo, lb_regmail, tb_regmail, lb_regpasw, tb_regpasw, lb_regpaswconf, tb_regpaswconf, lb_regerror, bt_reg, bt_reg_back });
            this.Controls.Add(gb_reg);

            RefreshLayout(); // ფორმების ცენტრირება
        }

        // ელემენტების ცენტრირება ეკრანზე
        private void RefreshLayout()
        {
            CenterControl(gb_login);
            CenterControl(gb_reg);
            CenterControl(gb_dashboard);
            lb_signin.Left = (gb_login.Width - lb_signin.Width) / 2;
            lb_signin1.Left = (gb_login.Width - lb_signin1.Width) / 2;
            lb_reg.Left = (gb_reg.Width - lb_reg.Width) / 2;
            lb_reginfo.Left = (gb_reg.Width - lb_reginfo.Width) / 2;
        }

        // Dashboard-ის საინფორმაციო ბარათების დამატება
        private void AddDashboardUIElements()
        {
            CreateStatCard("დეტექტორების რაოდენობა", "0", Color.White, 0, true);
            // xPos შევცვალე 250-ზე, რადგან წინა ბარათი გაიზარდა
            CreateStatCard("სისტემის მდგომარეობა", "არაა დაკავშირებული", Color.Red, 300, false);

            Label lb_list_title = new Label { Name = "lblListTitle", Text = "ოთახების მონიტორინგი რეალურ დროში (%)", Top = 120, Left = 5, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            pnl_cards_container.Controls.Add(lb_list_title);
        }

        // პატარა საინფორმაციო ბარათის (Stat Card) შექმნა
        private void CreateStatCard(string title, string value, Color valColor, int xPos, bool isCounter)
        {
            // სიგანე გავზარდე 230-მდე, რომ ტექსტი ჩაეტიოს
            Panel card = new Panel { Size = new Size(280, 80), Left = xPos, Top = 10, BackColor = Color.FromArgb(25, 30, 50) };
            Label lb_title = new Label { Text = title, Top = 15, Left = 15, ForeColor = Color.Gray, AutoSize = true, Font = new Font("Segoe UI", 8) };
            // ფონტის ზომა ოდნავ შევამცირე (15-მდე), რომ გრძელი სიტყვა ჩაეტიოს
            Label lb_val = new Label { Text = value, Top = 35, Left = 15, ForeColor = valColor, Font = new Font("Segoe UI", 15, FontStyle.Bold), AutoSize = true };

            if (isCounter) lb_total_count_val = lb_val; // თუ მთვლელია, შევინახოთ ცვლადში რომ მერე განვაახლოთ

            card.Controls.AddRange(new Control[] { lb_title, lb_val });
            pnl_cards_container.Controls.Add(card);
        }

        // ავტორიზაციის ღილაკზე დაჭერა
        private void bt_signin_Click(object sender, EventArgs e)
        {
            // მომხმარებლის ძებნა სიაში
            var foundUser = usersList.Find(u => u.Email == tb_email.Text && u.Password == tb_pasw.Text);
            if (foundUser != null)
            {
                currentUser = foundUser;
                attempts = 0; lb_error.Text = "";
                lb_user_email.Text = currentUser.Email;
                lb_user_email.Left = gb_dashboard.Width - lb_user_email.Width - 110;

                // Dashboard-ის გასუფთავება და ჩატვირთვა
                pnl_cards_container.Controls.Clear();
                deviceRows.Clear();
                detectorCount = 0;
                bt_add_device.Enabled = true;

                AddDashboardUIElements();

                // თუ მომხმარებელს შენახული ჰქონდა დეტექტორები, აღვადგინოთ
                if (currentUser.UserDetectors != null)
                {
                    foreach (var room in currentUser.UserDetectors)
                    {
                        CreateDeviceRow(room, 160 + (detectorCount * 60));
                        detectorCount++;
                    }
                    lb_total_count_val.Text = detectorCount.ToString();
                    if (detectorCount >= MaxDetectors) bt_add_device.Enabled = false;
                }

                gb_login.Visible = false; gb_dashboard.Visible = true;
                simulationTimer.Start(); // სიმულაციის ჩართვა
            }
            else
            {
                attempts++;
                lb_error.ForeColor = Color.Red;
                if (attempts >= 3) { StartLockout(); lb_error.Text = "სისტემა დაბლოკილია!"; }
                else { lb_error.Text = text[7] + " (დაგრჩათ " + (3 - attempts) + " ცდა)"; }
                lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
            }
        }

        // ახალი დეტექტორის დამატების ლოგიკა
        private void bt_add_device_Click(object sender, EventArgs e)
        {
            if (detectorCount >= MaxDetectors)
            {
                MessageBox.Show("მაქსიმალური რაოდენობა (5 დეტექტორი) უკვე დამატებულია.");
                return;
            }

            // სახელი შევიყვანოთ InputBox-ით
            string roomName = Interaction.InputBox("შეიყვანეთ ოთახის ან დეტექტორის სახელი:", "ახალი დეტექტორის დამატება", "ოთახი " + (detectorCount + 1));

            if (!string.IsNullOrWhiteSpace(roomName))
            {
                // შენახვა მიმდინარე მომხმარებლის ობიექტში
                if (currentUser.UserDetectors == null) currentUser.UserDetectors = new List<string>();
                currentUser.UserDetectors.Add(roomName);
                SaveUsers(); // JSON-ში გადაწერა

                CreateDeviceRow(roomName, 160 + (detectorCount * 60));
                detectorCount++;
                lb_total_count_val.Text = detectorCount.ToString();

                if (detectorCount >= MaxDetectors) bt_add_device.Enabled = false;
            }
        }

        // დეტექტორის ერთი "სტრიქონის" (Row) შექმნა UI-ში
        private void CreateDeviceRow(string roomName, int yPos)
        {
            Panel row = new Panel { Size = new Size(820, 50), Left = 5, Top = yPos, BackColor = Color.FromArgb(20, 25, 45) };
            Label lb_name = new Label { Text = roomName, Top = 15, Left = 15, ForeColor = Color.White, AutoSize = true, Width = 200 };

            // პროგრეს ბარი (ProgressBar-ის ნაცვლად პანელებით)
            Panel pnl_progress_bg = new Panel { Size = new Size(400, 10), Left = 220, Top = 20, BackColor = Color.FromArgb(40, 45, 65) };
            Panel pnl_progress_fill = new Panel { Size = new Size(0, 10), Left = 0, Top = 0, BackColor = Color.FromArgb(0, 255, 149) };
            pnl_progress_bg.Controls.Add(pnl_progress_fill);

            Label lb_pct = new Label { Text = "0%", Top = 15, Left = 650, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            Label lb_status = new Label { Text = "აქტიური", Top = 15, Left = 730, ForeColor = Color.Gray, AutoSize = true };

            row.Controls.AddRange(new Control[] { lb_name, pnl_progress_bg, lb_pct, lb_status });
            pnl_cards_container.Controls.Add(row);

            // ობიექტის შენახვა სიაში, რომ მერე ტაიმერმა "ამოძრაოს"
            deviceRows.Add(new DeviceUI { RowPanel = row, FillPanel = pnl_progress_fill, PctLabel = lb_pct, StatusLabel = lb_status });
        }

        // სიმულაციის ლოგიკა - მონაცემების შემთხვევითი ცვლა
        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            foreach (var device in deviceRows)
            {
                int smokeLevel = rnd.Next(0, 100); // 0-დან 100-მდე შემთხვევითი "კვამლი"
                device.PctLabel.Text = smokeLevel + "%";
                device.FillPanel.Width = (int)(smokeLevel * 4); // 400 პიქსელია მაქსიმუმი

                // საფრთხის დონის მიხედვით ფერების შეცვლა
                if (smokeLevel > 50) // საშიში დონე
                {
                    device.StatusLabel.Text = "საშიშია!";
                    device.StatusLabel.ForeColor = Color.Red;
                    device.FillPanel.BackColor = Color.Red;
                    // მოციმციმე ეფექტი
                    device.RowPanel.BackColor = (device.RowPanel.BackColor == Color.DarkRed) ? Color.FromArgb(20, 25, 45) : Color.DarkRed;
                }
                else if (smokeLevel > 25) // ყურადღება
                {
                    device.StatusLabel.Text = "ყურადღება";
                    device.StatusLabel.ForeColor = Color.Orange;
                    device.FillPanel.BackColor = Color.Orange;
                    device.RowPanel.BackColor = Color.FromArgb(20, 25, 45);
                }
                else // უსაფრთხო
                {
                    device.StatusLabel.Text = "უსაფრთხო";
                    device.StatusLabel.ForeColor = Color.FromArgb(0, 255, 149);
                    device.FillPanel.BackColor = Color.FromArgb(0, 255, 149);
                    device.RowPanel.BackColor = Color.FromArgb(20, 25, 45);
                }
            }
        }

        // სისტემის დაბლოკვა
        private void StartLockout()
        {
            bt_signin.Enabled = false; tb_email.Enabled = false; tb_pasw.Enabled = false;
            secondsRemaining = lockoutTime;
            lockoutTimer.Start();
            lockoutTime = 180; // შემდეგი დაბლოკვა უფრო ხანგრძლივი იქნება
        }

        // დაბლოკვის ტაიმერი
        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            if (secondsRemaining > 0)
            {
                secondsRemaining--;
                lb_error.Text = $"სისტემა დაბლოკილია! ცადეთ {secondsRemaining / 60}:{secondsRemaining % 60:D2} წუთში";
            }
            else
            {
                lockoutTimer.Stop();
                bt_signin.Enabled = true; tb_email.Enabled = true; tb_pasw.Enabled = true;
                lb_error.Text = "შეგიძლიათ ისევ სცადოთ";
                lb_error.ForeColor = Color.Orange;
                attempts = 2; // ბოლო ცდა
            }
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
        }

        // რეგისტრაციის ლოგიკა
        private void bt_reg_Click(object sender, EventArgs e)
        {
            bool exists = usersList.Exists(u => u.Email == tb_regmail.Text);
            bool isPasswordValid = ValidatePassword(tb_regpasw.Text); // რთული პაროლის შემოწმება
            bool isEmailValid = tb_regmail.Text.Contains("@");

            if (isEmailValid && tb_regpasw.Text == tb_regpaswconf.Text && !string.IsNullOrEmpty(tb_regmail.Text) && !exists && isPasswordValid)
            {
                // ახალი მომხმარებლის დამატება
                usersList.Add(new UserData { Email = tb_regmail.Text, Password = tb_regpasw.Text, UserDetectors = new List<string>() });
                SaveUsers();
                gb_reg.Visible = false; gb_login.Visible = true;
                MessageBox.Show("რეგისტრაცია წარმატებულია!");
            }
            else
            {
                if (!isEmailValid) lb_regerror.Text = "არასწორი მეილი!";
                else if (!isPasswordValid) lb_regerror.Text = "პაროლი უნდა შეიცავდეს: 1 დიდ ასოს, 1 ციფრს და 1 სიმბოლოს!";
                else if (exists) lb_regerror.Text = "ეს მეილი უკვე დაკავებულია!";
                else lb_regerror.Text = text[14];
                lb_regerror.ForeColor = Color.Red;
                lb_regerror.Left = (gb_reg.Width - lb_regerror.Width) / 2;
            }
        }

        // დამხმარე მეთოდი კონტროლების ცენტრირებისთვის
        private void CenterControl(Control ctrl)
        {
            if (ctrl != null && this.ClientSize.Width > 0)
            {
                ctrl.Left = (this.ClientSize.Width - ctrl.Width) / 2;
                ctrl.Top = (this.ClientSize.Height - ctrl.Height) / 2;
            }
        }

        // სხვადასხვა დამხმარე ფუნქციები
        private void Form1_Resize(object sender, EventArgs e) { RefreshLayout(); }
        private void bt_signin_register_Click(object sender, EventArgs e) { gb_login.Visible = false; gb_reg.Visible = true; RefreshLayout(); }

        // პაროლის ვალიდაცია
        private bool ValidatePassword(string password) { return password.Any(char.IsUpper) && password.Any(char.IsDigit) && password.Any(ch => !char.IsLetterOrDigit(ch)); }

        // მონაცემების შენახვა JSON ფაილში
        private void SaveUsers() { File.WriteAllText(filePath, JsonSerializer.Serialize(usersList)); }

        // მონაცემების ჩატვირთვა ფაილიდან
        private void LoadUsers()
        {
            if (File.Exists(filePath))
                usersList = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(filePath)) ?? new List<UserData>();
            else
            {
                // პირველივე გაშვებისას ადმინის შექმნა
                usersList.Add(new UserData { Email = "admin@iot.com", Password = "Password1!", UserDetectors = new List<string>() });
                SaveUsers();
            }
        }
    }

    // კლასი დეტექტორის ვიზუალური ელემენტების სამართავად
    public class DeviceUI
    {
        public Panel RowPanel { get; set; }
        public Panel FillPanel { get; set; }
        public Label PctLabel { get; set; }
        public Label StatusLabel { get; set; }
    }

    // მომხმარებლის მონაცემთა მოდელი
    public class UserData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> UserDetectors { get; set; } = new List<string>(); // შენახული ოთახების სია
    }
}