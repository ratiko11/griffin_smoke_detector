using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics.Eventing.Reader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.Timer;
// დამატებული ბიბლიოთეკები ფაილებთან და JSON-თან მუშაობისთვის
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace griffin_smoke_detector
{
    // კლასი მომხმარებლის მონაცემების სტრუქტურისთვის
    public class UserData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public partial class Form1 : Form
    {
        // JSON ფაილის სახელი და მომხმარებლების სია (ჩვენი "მონაცემთა ბაზა")
        private string filePath = "users.json";
        private List<UserData> usersList = new List<UserData>();

        public string[] text = new string[20];
        public int attempts = 0;
        public int lockoutTime = 60; 
        public int secondsRemaining = 0;
        private System.Windows.Forms.Timer lockoutTimer;

        public Form1()
        {
            InitializeComponent();
            
            // აპლიკაციის ჩართვისთანავე ვკითხულობთ ფაილს
            LoadUsers();

            gb_reg.Parent = this;
            gb_login.Parent = this;

            lockoutTimer = new System.Windows.Forms.Timer();
            lockoutTimer.Interval = 1000;
            lockoutTimer.Tick += LockoutTimer_Tick;
        }

        // ფუნქცია მონაცემების JSON ფაილში შესანახად
        private void SaveUsers()
        {
            string jsonString = JsonSerializer.Serialize(usersList);
            File.WriteAllText(filePath, jsonString);
        }

        // ფუნქცია JSON ფაილიდან მონაცემების წასაკითხად
        private void LoadUsers()
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                usersList = JsonSerializer.Deserialize<List<UserData>>(jsonString) ?? new List<UserData>();
            }
            else
            {
                // თუ ფაილი არ არსებობს, ვამატებთ ერთ სატესტო მომხმარებელს და ვქმნით ფაილს
                usersList.Add(new UserData { Email = "test1", Password = "abc" });
                SaveUsers();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Griffin Smoke Detector";
            gb_reg.Visible = false;

            // მასივის შევსება ტექსტებით
            text[0] = "ავტორიზაცია";
            text[1] = "შეიყვანეთ თქვენი მონაცემები რათა შეხვიდეთ პროგრამაში";
            text[2] = "ე-მაილი:";
            text[3] = "პაროლი";
            text[4] = "შესვლა";
            text[5] = "არ გაქცს ანგარიში? რეგისტრაცია";
            text[6] = "სწორია";
            text[7] = "პაროლი ან ე-მაილი არასწორია";
            text[8] = "რეგისტრაცია";
            text[9] = "შეიყვანეთ თქვენი სასურველი მონაცემები რათა დარეგისტრირდეთ";
            text[10] = "ე-მაილი:";
            text[11] = "პაროლი";
            text[12] = "პაროლის დადასტურება";
            text[13] = "რეგისტრაცია";
            text[14] = "შეყვანილი მეილი გამოყენებულია ან არასწორად გაქვთ გამეორებული პაროლი";

            // ვიზუალური ნაწილის დაყენება
            lb_signin.Text = text[0];
            lb_signin1.Text = text[1];
            lb_signin_email.Text = text[2];
            lb_signin_pasw.Text = text[3];
            bt_signin.Text = text[4];
            bt_signin_register.Text = text[5];
            gb_login.Text = string.Empty;
            lb_error.Text = string.Empty;

            this.BackColor = Color.FromArgb(9, 13, 27);
            lb_signin.Font = new Font(bt_signin.Font, FontStyle.Bold);
            lb_signin1.Location = new Point(lb_signin1.Location.X - lb_signin.Width - 10, lb_signin1.Location.Y);
            lb_error.Location = new Point(lb_error.Location.X - lb_signin.Width + 40, lb_error.Location.Y);
            gb_login.BackColor = Color.FromArgb(17, 23, 39);
            gb_login.ForeColor = Color.White;
            tb_email.BackColor = Color.FromArgb(32, 39, 60);
            tb_pasw.BackColor = Color.FromArgb(32, 39, 60);
            tb_email.BorderStyle = BorderStyle.None;
            tb_pasw.BorderStyle = BorderStyle.None;
            bt_signin.BackColor = Color.FromArgb(0, 255, 149);
            bt_signin.ForeColor = Color.FromArgb(32, 39, 60);
            bt_signin.FlatStyle = FlatStyle.Flat;
            bt_signin.Font = new Font(bt_signin.Font, FontStyle.Bold);
            bt_signin_register.BackColor = Color.FromArgb(17, 23, 39);
            bt_signin_register.FlatStyle = FlatStyle.Flat;
            bt_signin_register.FlatAppearance.BorderSize = 0;
            bt_signin_register.ForeColor = Color.Gray;
            tb_email.ForeColor = Color.White;
            tb_pasw.ForeColor = Color.White;
            tb_pasw.PasswordChar = '*';
            lb_error.Location = new Point(lb_error.Location.X - lb_signin.Width + 120, lb_error.Location.Y);
        }

        private void bt_signin_Click(object sender, EventArgs e)
        {
            // ვეძებთ მომხმარებელს სიაში (ბაზაში)
            var foundUser = usersList.Find(u => u.Email == tb_email.Text && u.Password == tb_pasw.Text);

            if (foundUser != null)
            {
                attempts = 0;
                lb_error.ForeColor = Color.Green;
                lb_error.Text = text[6]; 
                lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
                MessageBox.Show($"ავტორიზაცია წარმატებულია! მოგესალმებით {foundUser.Email}");
                gb_login.Visible = false;
            }
            else
            {
                attempts++;
                lb_error.ForeColor = Color.Red;
                if (attempts >= 3)
                {
                    StartLockout();
                    bt_signin.Enabled = false;
                    tb_email.Enabled = false;
                    tb_pasw.Enabled = false;
                    lb_error.Text = "სისტემა დაბლოკილია!";
                }
                else
                {
                    lb_error.Text = text[7] + " (დაგრჩათ " + (3 - attempts) + " ცდა)";
                }
            }
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
            lb_error.Top = bt_signin.Top - 30;
        }

        private void StartLockout()
        {
            bt_signin.Enabled = false;
            tb_email.Enabled = false;
            tb_pasw.Enabled = false;
            secondsRemaining = lockoutTime;
            lockoutTimer.Start();
            lockoutTime = 180;
        }

        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            if (secondsRemaining > 0)
            {
                secondsRemaining--;
                int mins = secondsRemaining / 60;
                int secs = secondsRemaining % 60;
                lb_error.Text = $"სისტემა დაბლოკილია! ცადეთ {mins}:{secs:D2} წუთში";
            }
            else
            {
                lockoutTimer.Stop();
                bt_signin.Enabled = true;
                tb_email.Enabled = true;
                tb_pasw.Enabled = true;
                lb_error.Text = "შეგიძლიათ ისევ სცადოთ";
                lb_error.ForeColor = Color.Orange;
                attempts = 2;
            }
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
        }

        private void bt_signin_register_Click(object sender, EventArgs e)
        {
            lb_error.Text = string.Empty;
            tb_email.Text = string.Empty;
            tb_pasw.Text = string.Empty;
            gb_login.Visible = false;
            gb_reg.Visible = true;
            gb_reg.Location = new Point(gb_login.Location.X, gb_login.Location.Y);

            lb_reg.Text = text[8];
            lb_reginfo.Text = text[9];
            lb_regmail.Text = text[10];
            lb_regpasw.Text = text[11];
            lb_regpaswconf.Text = text[12];
            bt_reg.Text = text[13];
            lb_regerror.Text = string.Empty;

            lb_reginfo.Location = new Point((gb_reg.Width - lb_reginfo.Width) / 2, lb_reg.Location.Y + 40);
            gb_reg.Text = string.Empty;
            gb_reg.BackColor = Color.FromArgb(17, 23, 39);
            gb_reg.ForeColor = Color.White;
            lb_reg.Font = new Font(bt_signin.Font, FontStyle.Bold);
            tb_regmail.BackColor = Color.FromArgb(32, 39, 60);
            tb_regpasw.BackColor = Color.FromArgb(32, 39, 60);
            tb_regpaswconf.BackColor = Color.FromArgb(32, 39, 60);
            tb_regmail.BorderStyle = BorderStyle.None;
            tb_regpasw.BorderStyle = BorderStyle.None;
            tb_regpaswconf.BorderStyle = BorderStyle.None;
            bt_reg.BackColor = Color.FromArgb(0, 255, 149);
            bt_reg.ForeColor = Color.FromArgb(32, 39, 60);
            bt_reg.FlatStyle = FlatStyle.Flat;
            bt_reg.Font = new Font(bt_signin.Font, FontStyle.Bold);
            tb_regmail.ForeColor = Color.White;
            tb_regpasw.ForeColor = Color.White;
            tb_regpasw.PasswordChar = '*';
            tb_regpaswconf.ForeColor = Color.White;
            tb_regpaswconf.PasswordChar = '*';
        }

        private void bt_reg_Click(object sender, EventArgs e)
        {
            // 1. ვამოწმებთ, უკვე არსებობს თუ არა ეს მეილი ბაზაში
            bool exists = usersList.Exists(u => u.Email == tb_regmail.Text);

            // 2. ვიძახებთ პაროლის ვალიდაციის ფუნქციას
            string password = tb_regpasw.Text;
            bool isPasswordValid = ValidatePassword(password);

            // 3. მთავარი შემოწმება: პაროლების დამთხვევა, ცარიელი მეილი, არსებული მეილი და პაროლის სირთულე
            if (tb_regpasw.Text == tb_regpaswconf.Text && !string.IsNullOrEmpty(tb_regmail.Text) && !exists && isPasswordValid)
            {
                usersList.Add(new UserData
                {
                    Email = tb_regmail.Text,
                    Password = tb_regpasw.Text
                });

                SaveUsers();

                tb_regmail.Text = string.Empty;
                tb_regpasw.Text = string.Empty;
                tb_regpaswconf.Text = string.Empty;

                gb_reg.Visible = false;
                gb_login.Visible = true;
                MessageBox.Show("რეგისტრაცია წარმატებულია!");
            }
            else
            {
                // შეცდომის შეტყობინების მორგება
                if (!isPasswordValid)
                {
                    lb_regerror.Text = "პაროლი უნდა შეიცავდეს: 1 დიდ ასოს, 1 ციფრს და 1 სიმბოლოს!";
                }
                else if (exists)
                {
                    lb_regerror.Text = "ეს მეილი უკვე დაკავებულია!";
                }
                else
                {
                    lb_regerror.Text = text[14]; // ძველი შეცდომის ტექსტი
                }

                lb_regerror.ForeColor = Color.Red;
                lb_regerror.Location = new Point((gb_reg.Width - lb_regerror.Width) / 2, lb_regerror.Location.Y);
            }
        }

        // ახალი ფუნქცია პაროლის შესამოწმებლად
        private bool ValidatePassword(string password)
        {
            // მინიმუმ 1 დიდი ასო, 1 ციფრი, 1 სპეციალური სიმბოლო
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasDigit && hasSpecial;
        }
    }
}
