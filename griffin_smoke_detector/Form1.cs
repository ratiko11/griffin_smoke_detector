using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics.Eventing.Reader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace griffin_smoke_detector
{
    public partial class Form1 : Form
    {
        public string[] text = new string[6];
        public string user = "test1";
        public string pasw = "abc";
        public Form1()

        {

            InitializeComponent();

            gb_reg.Parent = this;
            gb_login.Parent = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gb_reg.Visible = false;


            text[0] = "ავტორიზაცია";
            text[1] = "შეიყვანეთ თქვენი მონაცემები რათა შეხვიდეთ პროგრამაში";
            text[2] = "ე-მაილი:";
            text[3] = "პაროლი";
            text[4] = "შესვლა";
            text[5] = "არ გაქცს ანგარიში? რეგისტრაცია";


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
            gb_login.BackColor = System.Drawing.Color.FromArgb(17, 23, 39);
            gb_login.ForeColor = Color.White;
            tb_email.BackColor = System.Drawing.Color.FromArgb(32, 39, 60);
            tb_pasw.BackColor = System.Drawing.Color.FromArgb(32, 39, 60);
            tb_email.BorderStyle = BorderStyle.None;
            tb_pasw.BorderStyle = BorderStyle.None;
            bt_signin.BackColor = System.Drawing.Color.FromArgb(0, 255, 149);
            bt_signin.ForeColor = System.Drawing.Color.FromArgb(32, 39, 60);
            bt_signin.FlatStyle = FlatStyle.Flat;
            bt_signin.Font = new Font(bt_signin.Font, FontStyle.Bold);
            bt_signin_register.BackColor = System.Drawing.Color.FromArgb(17, 23, 39);
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

            if (user == tb_email.Text && pasw == tb_pasw.Text)
            {
                tb_email.Text = string.Empty;
                tb_pasw.Text = string.Empty;
                gb_login.Visible = false;
            }
            else
            {
                lb_error.Text = "არასწორი პაროლი ან ე-მაილი";
              
                lb_error.ForeColor = Color.Red;

            }
        }

        private void bt_signin_register_Click(object sender, EventArgs e)
        {
            lb_error.Text = string.Empty;
            tb_email.Text = string.Empty;
            tb_pasw.Text = string.Empty;

            gb_login.Visible = false;
            gb_reg.Visible = true;
            gb_reg.Location = new Point(gb_login.Location.X, gb_login.Location.Y);

            Array.Resize(ref text, text.Length + 7);




            text[6] = "რეგისტრაცია";
            text[7] = "შეიყვანეთ თქვენი სასურველი მონაცემები რათა დარეგისტრირდეთ";
            text[8] = "ე-მაილი:";
            text[9] = "პაროლი";
            text[10] = "პაროლის დადასტურება";
            text[11] = "რეგისტრაცია";
            text[12] = "შეყვანილი მეილი გამოყენებულია ან არასწორად გაქვთ გამეორებული პაროლი";



            lb_reg.Text = text[6];
            lb_reginfo.Text = text[7];
            lb_regmail.Text = text[8];
            lb_regpasw.Text = text[9];
            lb_regpaswconf.Text = text[10];
            bt_reg.Text = text[11];
            lb_regerror.Text = string.Empty;
            gb_reg.Text = string.Empty;
            



            gb_reg.Text = string.Empty;
            gb_reg.BackColor = Color.FromArgb(17, 23, 39);
            gb_reg.ForeColor = Color.White;
           



            lb_reg.Font = new Font(bt_signin.Font, FontStyle.Bold);
            lb_reginfo.Location = new Point(lb_signin1.Location.X, lb_signin1.Location.Y);
            lb_error.Location = new Point(lb_error.Location.X - lb_signin.Width + 40, lb_error.Location.Y);
            gb_reg.BackColor = System.Drawing.Color.FromArgb(17, 23, 39);
            gb_reg.ForeColor = Color.White;
            tb_regmail.BackColor = System.Drawing.Color.FromArgb(32, 39, 60);
            tb_regpasw.BackColor = System.Drawing.Color.FromArgb(32, 39, 60);
            tb_regpaswconf.BackColor = System.Drawing.Color.FromArgb(32, 39, 60);
            tb_regmail.BorderStyle = BorderStyle.None;
            tb_regpasw.BorderStyle = BorderStyle.None;
            tb_regpaswconf.BorderStyle = BorderStyle.None;
            bt_reg.BackColor = System.Drawing.Color.FromArgb(0, 255, 149);
            bt_reg.ForeColor = System.Drawing.Color.FromArgb(32, 39, 60);
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

            

            if(tb_regpasw.Text == tb_regpaswconf.Text)
            {
                tb_regmail.Text = string.Empty;
                tb_regpasw.Text = string.Empty;
                tb_regpaswconf.Text = string .Empty;

                gb_reg.Visible = false;
                gb_login.Visible = true;
            }
            else
            {
                lb_regerror.Text = text[12];
                lb_regerror.Location = new Point(bt_reg.Location.X - 95, lb_regerror.Location.Y);
                lb_regerror.ForeColor = Color.Red;
            }
        }
    }
}
