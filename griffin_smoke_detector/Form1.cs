using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace griffin_smoke_detector
{
    public partial class Form1 : Form
    {
        // მონაცემების და ლოგიკის ცვლადები
        private string filePath = "users.json";
        private List<UserData> usersList = new List<UserData>();
        public string[] text = new string[20];
        public int attempts = 0;
        public int lockoutTime = 60;
        public int secondsRemaining = 0;
        private System.Windows.Forms.Timer lockoutTimer;

        // ახალი ცვლადები სიმულაციისთვის
        private System.Windows.Forms.Timer simulationTimer; // ტაიმერი კვამლის დონის ცვლილებისთვის
        private Random rnd = new Random();
        private List<DeviceUI> deviceRows = new List<DeviceUI>(); // მოწყობილობების ელემენტების სია რეალურ დროში განახლებისთვის

        // დინამიური კონტროლები - ავტორიზაცია (Login)
        private GroupBox gb_login;
        private Label lb_signin, lb_signin1, lb_signin_email, lb_signin_pasw, lb_error;
        private TextBox tb_email, tb_pasw;
        private Button bt_signin, bt_signin_register;

        // დინამიური კონტროლები - რეგისტრაცია (Register)
        private GroupBox gb_reg;
        private Label lb_reg, lb_reginfo, lb_regmail, lb_regpasw, lb_regpaswconf, lb_regerror;
        private TextBox tb_regmail, tb_regpasw, tb_regpaswconf;
        private Button bt_reg;

        // დინამიური კონტროლები - Dashboard (მთავარი პანელი)
        private GroupBox gb_dashboard;
        private Label lb_user_email, lb_dashboard_title;
        private Button bt_logout;
        private Panel pnl_cards_container;

        public Form1()
        {
            // ფანჯრის საწყისი პარამეტრები
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadUsers();
            InitializeLanguageArray();
            CreateDynamicControls();

            // ტაიმერის კონფიგურაცია დაბლოკვისთვის
            lockoutTimer = new System.Windows.Forms.Timer();
            lockoutTimer.Interval = 1000;
            lockoutTimer.Tick += LockoutTimer_Tick;

            // სიმულაციის ტაიმერის გამართვა (ყოველ 2 წამში მონაცემების შესაცვლელად)
            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = 2000;
            simulationTimer.Tick += SimulationTimer_Tick;

            // ფანჯრის ზომის ცვლილებისას ელემენტების გასწორება
            this.Resize += Form1_Resize;
        }

        private void InitializeLanguageArray()
        {
            text[0] = "ავტორიზაცია";
            text[1] = "შეიყვანეთ თქვენი მონაცემები რათა შეხვიდეთ პროგრამაში";
            text[2] = "ე-მაილი:";
            text[3] = "პაროლი";
            text[4] = "შესვლა";
            text[5] = "არ გაქვს ანგარიში? რეგისტრაცია";
            text[6] = "სწორია";
            text[7] = "პაროლი ან ე-მაილი არასწორია";
            text[8] = "რეგისტრაცია";
            text[9] = "შეიყვანეთ თქვენი სასურველი მონაცემები რათა დარეგისტრირდეთ";
            text[10] = "ე-მაილი:";
            text[11] = "პაროლი";
            text[12] = "პაროლის დადასტურება";
            text[13] = "რეგისტრაცია";
            text[14] = "შეყვანილი მეილი გამოყენებულია ან არასწორად გაქვთ გამეორებული პაროლი";
            text[15] = "ავტორიზაციაზე დაბრუნება";
        }

        private void CreateDynamicControls()
        {
            this.BackColor = Color.FromArgb(9, 13, 27);
            this.Text = "Griffin Smoke Detector";

            int controlWidth = 370;
            int leftPadding = 40;

            // --- DASHBOARD GROUPBOX (მთავარი ინტერფეისი) ---
            gb_dashboard = new GroupBox { Size = new Size(900, 650), Text = "", Visible = false, FlatStyle = FlatStyle.Flat };
            gb_dashboard.BackColor = Color.FromArgb(15, 18, 34);
            gb_dashboard.ForeColor = Color.White;

            lb_dashboard_title = new Label { Text = "🔥 GriffinSmokeDetector", ForeColor = Color.FromArgb(0, 255, 149), Font = new Font("Segoe UI", 14, FontStyle.Bold), Top = 20, Left = 25, AutoSize = true };
            lb_user_email = new Label { Text = "", ForeColor = Color.LightGray, Top = 25, Left = 600, AutoSize = true, Font = new Font("Segoe UI", 9) };

            bt_logout = new Button { Text = "Logout ↪", Top = 20, Left = 800, Width = 80, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9) };
            bt_logout.FlatAppearance.BorderSize = 0;
            bt_logout.Click += (s, e) => { gb_dashboard.Visible = false; gb_login.Visible = true; simulationTimer.Stop(); };

            pnl_cards_container = new Panel { Top = 70, Left = 20, Size = new Size(860, 550), BackColor = Color.Transparent };

            gb_dashboard.Controls.AddRange(new Control[] { lb_dashboard_title, lb_user_email, bt_logout, pnl_cards_container });
            this.Controls.Add(gb_dashboard);

            AddDashboardUIElements();

            // --- ავტორიზაციის ჯგუფი (LOGIN GROUPBOX) ---
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

            // --- რეგისტრაციის ჯგუფი (REGISTER GROUPBOX) ---
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

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            CenterControl(gb_login);
            CenterControl(gb_reg);
            CenterControl(gb_dashboard);

            // Login-ის შიდა ელემენტების ცენტრირება
            lb_signin.Left = (gb_login.Width - lb_signin.Width) / 2;
            lb_signin1.Left = (gb_login.Width - lb_signin1.Width) / 2;

            // Register-ის შიდა ელემენტების ცენტრირება
            lb_reg.Left = (gb_reg.Width - lb_reg.Width) / 2;
            lb_reginfo.Left = (gb_reg.Width - lb_reginfo.Width) / 2;
        }

        private void AddDashboardUIElements()
        {
            CreateStatCard("Total Devices", "3", Color.White, 0);
            CreateStatCard("System Health", "Good", Color.FromArgb(0, 255, 149), 210);

            Label lb_list_title = new Label { Text = "Real-Time Room Monitoring (%)", Top = 120, Left = 5, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            pnl_cards_container.Controls.Add(lb_list_title);

            // ოთახების დინამიური სტრიქონების შექმნა
            CreateDeviceRow("Kitchen Smoke Level", 160);
            CreateDeviceRow("Living Room Smoke Level", 220);
            CreateDeviceRow("Bedroom Smoke Level", 280);
        }

        private void CreateStatCard(string title, string value, Color valColor, int xPos)
        {
            Panel card = new Panel { Size = new Size(190, 80), Left = xPos, Top = 10, BackColor = Color.FromArgb(25, 30, 50) };
            Label lb_title = new Label { Text = title, Top = 15, Left = 15, ForeColor = Color.Gray, AutoSize = true, Font = new Font("Segoe UI", 8) };
            Label lb_val = new Label { Text = value, Top = 35, Left = 15, ForeColor = valColor, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true };
            card.Controls.AddRange(new Control[] { lb_title, lb_val });
            pnl_cards_container.Controls.Add(card);
        }

        private void CreateDeviceRow(string roomName, int yPos)
        {
            Panel row = new Panel { Size = new Size(820, 50), Left = 5, Top = yPos, BackColor = Color.FromArgb(20, 25, 45) };
            Label lb_name = new Label { Text = roomName, Top = 15, Left = 15, ForeColor = Color.White, AutoSize = true, Width = 200 };

            // პროგრეს ბარი (ვიზუალური %-ისთვის)
            Panel pnl_progress_bg = new Panel { Size = new Size(400, 10), Left = 220, Top = 20, BackColor = Color.FromArgb(40, 45, 65) };
            Panel pnl_progress_fill = new Panel { Size = new Size(0, 10), Left = 0, Top = 0, BackColor = Color.FromArgb(0, 255, 149) };
            pnl_progress_bg.Controls.Add(pnl_progress_fill);

            Label lb_pct = new Label { Text = "0%", Top = 15, Left = 650, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            Label lb_status = new Label { Text = "SAFE", Top = 15, Left = 730, ForeColor = Color.Green, AutoSize = true };

            row.Controls.AddRange(new Control[] { lb_name, pnl_progress_bg, lb_pct, lb_status });
            pnl_cards_container.Controls.Add(row);

            // ვინახავთ ელემენტებს სიაში, რომ მერე ტაიმერით განვაახლოთ
            deviceRows.Add(new DeviceUI { RowPanel = row, FillPanel = pnl_progress_fill, PctLabel = lb_pct, StatusLabel = lb_status });
        }

        // სიმულაციის ლოგიკა - აქ ხდება "ბზუილი" და %-ების ცვლილება
        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            foreach (var device in deviceRows)
            {
                int smokeLevel = rnd.Next(0, 100); // ვიგონებთ შემთხვევით %-ს 0-დან 100-მდე
                device.PctLabel.Text = smokeLevel + "%";
                device.FillPanel.Width = (int)(smokeLevel * 4); // ვაახლებთ პროგრეს ბარს

                if (smokeLevel > 50) // DANGER %
                {
                    device.StatusLabel.Text = "DANGER!";
                    device.StatusLabel.ForeColor = Color.Red;
                    device.FillPanel.BackColor = Color.Red;

                    // ვიზუალური ბზუილი (ციმციმი)
                    device.RowPanel.BackColor = (device.RowPanel.BackColor == Color.DarkRed) ? Color.FromArgb(20, 25, 45) : Color.DarkRed;
                }
                else if (smokeLevel > 25) // WARNING %
                {
                    device.StatusLabel.Text = "WARNING";
                    device.StatusLabel.ForeColor = Color.Orange;
                    device.FillPanel.BackColor = Color.Orange;
                    device.RowPanel.BackColor = Color.FromArgb(20, 25, 45); // ჩვეულებრივი ფონი
                }
                else // SAFE %
                {
                    device.StatusLabel.Text = "SAFE";
                    device.StatusLabel.ForeColor = Color.FromArgb(0, 255, 149);
                    device.FillPanel.BackColor = Color.FromArgb(0, 255, 149);
                    device.RowPanel.BackColor = Color.FromArgb(20, 25, 45);
                }
            }
        }

        private void CenterControl(Control ctrl)
        {
            if (ctrl != null && this.ClientSize.Width > 0)
            {
                ctrl.Left = (this.ClientSize.Width - ctrl.Width) / 2;
                ctrl.Top = (this.ClientSize.Height - ctrl.Height) / 2;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            RefreshLayout();
        }

        private void bt_signin_Click(object sender, EventArgs e)
        {
            var foundUser = usersList.Find(u => u.Email == tb_email.Text && u.Password == tb_pasw.Text);
            if (foundUser != null)
            {
                attempts = 0;
                lb_error.Text = "";
                lb_user_email.Text = foundUser.Email;
                lb_user_email.Left = gb_dashboard.Width - lb_user_email.Width - 110;

                gb_login.Visible = false;
                gb_dashboard.Visible = true;
                CenterControl(gb_dashboard);

                simulationTimer.Start(); // ვიწყებთ სიმულაციას სისტემაში შესვლისას
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

        private void StartLockout()
        {
            bt_signin.Enabled = false; tb_email.Enabled = false; tb_pasw.Enabled = false;
            secondsRemaining = lockoutTime; lockoutTimer.Start(); lockoutTime = 180;
        }

        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            if (secondsRemaining > 0) { secondsRemaining--; lb_error.Text = $"სისტემა დაბლოკილია! ცადეთ {secondsRemaining / 60}:{secondsRemaining % 60:D2} წუთში"; }
            else { lockoutTimer.Stop(); bt_signin.Enabled = true; tb_email.Enabled = true; tb_pasw.Enabled = true; lb_error.Text = "შეგიძლიათ ისევ სცადოთ"; lb_error.ForeColor = Color.Orange; attempts = 2; }
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
        }

        private void bt_signin_register_Click(object sender, EventArgs e) { gb_login.Visible = false; gb_reg.Visible = true; CenterControl(gb_reg); RefreshLayout(); }

        private void bt_reg_Click(object sender, EventArgs e)
        {
            bool exists = usersList.Exists(u => u.Email == tb_regmail.Text);
            bool isPasswordValid = ValidatePassword(tb_regpasw.Text);

            // მეილის ვალიდაცია
            bool isEmailValid = tb_regmail.Text.Contains("@");

            if (isEmailValid && tb_regpasw.Text == tb_regpaswconf.Text && !string.IsNullOrEmpty(tb_regmail.Text) && !exists && isPasswordValid)
            {
                usersList.Add(new UserData { Email = tb_regmail.Text, Password = tb_regpasw.Text });
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

        private bool ValidatePassword(string password) { return password.Any(char.IsUpper) && password.Any(char.IsDigit) && password.Any(ch => !char.IsLetterOrDigit(ch)); }
        private void SaveUsers() { File.WriteAllText(filePath, JsonSerializer.Serialize(usersList)); }
        private void LoadUsers()
        {
            if (File.Exists(filePath)) usersList = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(filePath)) ?? new List<UserData>();
            else { usersList.Add(new UserData { Email = "admin@iot.com", Password = "Password1!" }); SaveUsers(); }
        }
    }

    // დამხმარე კლასი ინტერფეისის ელემენტების სამართავად
    public class DeviceUI
    {
        public Panel RowPanel { get; set; }
        public Panel FillPanel { get; set; }
        public Label PctLabel { get; set; }
        public Label StatusLabel { get; set; }
    }

    public class UserData { public string Email { get; set; } public string Password { get; set; } }
}