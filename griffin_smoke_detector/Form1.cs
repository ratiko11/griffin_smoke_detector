using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics.Eventing.Reader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.Timer;

namespace griffin_smoke_detector
{
    public partial class Form1 : Form
    {
        // ტექსტების მასივი, მომხმარებლის მონაცემები და მცდელობების მთვლელი
        public string[] text = new string[20];
        public string user = "test1";
        public string pasw = "abc";
        public int attempts = 0;

        //ტაიმერი
        public int lockoutTime = 60; // დაიწყოს 1 წუთით
        public int secondsRemaining = 0;
        private System.Windows.Forms.Timer lockoutTimer;

        public Form1()
        {
            InitializeComponent();
            // პანელების (GroupBox) მშობლად ფორმის მითითება სწორი განლაგებისთვის
            gb_reg.Parent = this;
            gb_login.Parent = this;

            // Initialize Timer
            lockoutTimer = new System.Windows.Forms.Timer();
            lockoutTimer.Interval = 1000; // 1 second intervals
            lockoutTimer.Tick += LockoutTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // რეგისტრაციის ფანჯრის დამალვა ჩართვისას
            gb_reg.Visible = false;

            // მასივის შევსება 
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

            // ავტორიზაციის ელემენტებისთვის ტექსტების მინიჭება
            lb_signin.Text = text[0];
            lb_signin1.Text = text[1];
            lb_signin_email.Text = text[2];
            lb_signin_pasw.Text = text[3];
            bt_signin.Text = text[4];
            bt_signin_register.Text = text[5];
            gb_login.Text = string.Empty;
            lb_error.Text = string.Empty;

            // ფორმის და ელემენტების ვიზუალური სტილი (ფერები, ფონტები, ჩარჩოები)
            this.BackColor = Color.FromArgb(9, 13, 27);
            lb_signin.Font = new Font(bt_signin.Font, FontStyle.Bold);

            // ელემენტების პოზიციონირება
            lb_signin1.Location = new Point(lb_signin1.Location.X - lb_signin.Width - 10, lb_signin1.Location.Y);
            lb_error.Location = new Point(lb_error.Location.X - lb_signin.Width + 40, lb_error.Location.Y);

            // ავტორიზაციის პანელის დიზაინი
            gb_login.BackColor = Color.FromArgb(17, 23, 39);
            gb_login.ForeColor = Color.White;
            tb_email.BackColor = Color.FromArgb(32, 39, 60);
            tb_pasw.BackColor = Color.FromArgb(32, 39, 60);
            tb_email.BorderStyle = BorderStyle.None;
            tb_pasw.BorderStyle = BorderStyle.None;

            // ღილაკის სტილი
            bt_signin.BackColor = Color.FromArgb(0, 255, 149);
            bt_signin.ForeColor = Color.FromArgb(32, 39, 60);
            bt_signin.FlatStyle = FlatStyle.Flat;
            bt_signin.Font = new Font(bt_signin.Font, FontStyle.Bold);

            // რეგისტრაციაზე გადასასვლელი ღილაკის სტილი
            bt_signin_register.BackColor = Color.FromArgb(17, 23, 39);
            bt_signin_register.FlatStyle = FlatStyle.Flat;
            bt_signin_register.FlatAppearance.BorderSize = 0;
            bt_signin_register.ForeColor = Color.Gray;

            tb_email.ForeColor = Color.White;
            tb_pasw.ForeColor = Color.White;
            tb_pasw.PasswordChar = '*'; // პაროლის სიმბოლოების დაფარვა
            lb_error.Location = new Point(lb_error.Location.X - lb_signin.Width + 120, lb_error.Location.Y);
        }

        private void bt_signin_Click(object sender, EventArgs e)
        {
            // მონაცემების შემოწმება
            if (user == tb_email.Text && pasw == tb_pasw.Text)
            {
                attempts = 0;
                lb_error.ForeColor = Color.Green;
                lb_error.Text = text[6]; // "სწორია"
                lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
                MessageBox.Show("ავტორიზაცია წარმატებულია");
                gb_login.Visible = false; // წარმატების შემთხვევაში ფანჯრის დამალვა
            }
            else
            {
                attempts++; // მცდელობების გაზრდა შეცდომისას
                lb_error.ForeColor = Color.Red;
                if (attempts >= 3)
                {
                    // 3 შეცდომის შემდეგ სისტემის დაბლოკვა
                    StartLockout();
                    bt_signin.Enabled = false;
                    tb_email.Enabled = false;
                    tb_pasw.Enabled = false;
                    lb_error.Text = "სისტემა დაბლოკილია!";
                }
                else
                {
                    // დარჩენილი მცდელობების ჩვენება
                    lb_error.Text = text[7] + " (დაგრჩათ " + (3 - attempts) + " ცდა)";
                }
            }
            // შეცდომის ტექსტის ცენტრირება
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
            lb_error.Top = bt_signin.Top - 30;
        }

        private void StartLockout()
        {
            // Disable inputs
            bt_signin.Enabled = false;
            tb_email.Enabled = false;
            tb_pasw.Enabled = false;

            secondsRemaining = lockoutTime;
            lockoutTimer.Start();

            // Set next lockout to 3 minutes (180s) for subsequent failures
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
                attempts = 2; // Next fail locks it again immediately
            }
            lb_error.Left = (gb_login.Width - lb_error.Width) / 2;
        }


        private void bt_signin_register_Click(object sender, EventArgs e)
        {
            // ველების გასუფთავება რეგისტრაციაზე გადასვლისას
            lb_error.Text = string.Empty;
            tb_email.Text = string.Empty;
            tb_pasw.Text = string.Empty;

            // ავტორიზაციის დამალვა და რეგისტრაციის ჩვენება იმავე ადგილზე
            gb_login.Visible = false;
            gb_reg.Visible = true;
            gb_reg.Location = new Point(gb_login.Location.X, gb_login.Location.Y);

            // რეგისტრაციის ტექსტების მინიჭება
            lb_reg.Text = text[8];
            lb_reginfo.Text = text[9];
            lb_regmail.Text = text[10];
            lb_regpasw.Text = text[11];
            lb_regpaswconf.Text = text[12];
            bt_reg.Text = text[13];
            lb_regerror.Text = string.Empty;

            // ინფო ლეიბლის ცენტრირება ტექსტის მინიჭების შემდეგ
            lb_reginfo.Location = new Point((gb_reg.Width - lb_reginfo.Width) / 2, lb_reg.Location.Y + 40);

            // რეგისტრაციის პანელის დიზაინი
            gb_reg.Text = string.Empty;
            gb_reg.BackColor = Color.FromArgb(17, 23, 39);
            gb_reg.ForeColor = Color.White;
            lb_reg.Font = new Font(bt_signin.Font, FontStyle.Bold);

            // რეგისტრაციის ტექსტბოქსების სტილი
            tb_regmail.BackColor = Color.FromArgb(32, 39, 60);
            tb_regpasw.BackColor = Color.FromArgb(32, 39, 60);
            tb_regpaswconf.BackColor = Color.FromArgb(32, 39, 60);
            tb_regmail.BorderStyle = BorderStyle.None;
            tb_regpasw.BorderStyle = BorderStyle.None;
            tb_regpaswconf.BorderStyle = BorderStyle.None;

            // რეგისტრაციის ღილაკის სტილი
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
            // პაროლების დამთხვევის და მეილის შევსების შემოწმება
            if (tb_regpasw.Text == tb_regpaswconf.Text && tb_regmail.Text != "")
            {
                // ახალი მონაცემების შენახვა ცვლადებში
                user = tb_regmail.Text;
                pasw = tb_regpasw.Text;

                // ველების გასუფთავება
                tb_regmail.Text = string.Empty;
                tb_regpasw.Text = string.Empty;
                tb_regpaswconf.Text = string.Empty;

                // უკან დაბრუნება ავტორიზაციის ფანჯარაზე
                gb_reg.Visible = false;
                gb_login.Visible = true;
                MessageBox.Show("რეგისტრაცია წარმატებულია!");
            }
            else
            {
                // შეცდომის ჩვენება რეგისტრაციისას
                lb_regerror.Text = text[14];
                lb_regerror.ForeColor = Color.Red;
                lb_regerror.Location = new Point((gb_reg.Width - lb_regerror.Width) / 2, lb_regerror.Location.Y);
            }
        }



    }
}