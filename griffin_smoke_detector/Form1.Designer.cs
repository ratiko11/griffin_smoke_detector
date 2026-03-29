namespace griffin_smoke_detector
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lb_signin = new Label();
            lb_signin1 = new Label();
            lb_signin_email = new Label();
            lb_signin_pasw = new Label();
            tb_email = new TextBox();
            tb_pasw = new TextBox();
            bt_signin = new Button();
            bt_signin_register = new Button();
            lb_error = new Label();
            gb_login = new GroupBox();
            gb_reg = new GroupBox();
            lb_regerror = new Label();
            bt_reg = new Button();
            tb_regpaswconf = new TextBox();
            lb_regpaswconf = new Label();
            tb_regpasw = new TextBox();
            lb_regpasw = new Label();
            tb_regmail = new TextBox();
            lb_regmail = new Label();
            lb_reginfo = new Label();
            lb_reg = new Label();
            gb_login.SuspendLayout();
            gb_reg.SuspendLayout();
            SuspendLayout();
            // 
            // lb_signin
            // 
            lb_signin.AutoSize = true;
            lb_signin.Location = new Point(221, 19);
            lb_signin.Name = "lb_signin";
            lb_signin.Size = new Size(38, 15);
            lb_signin.TabIndex = 0;
            lb_signin.Text = "label1";
            // 
            // lb_signin1
            // 
            lb_signin1.AutoSize = true;
            lb_signin1.Location = new Point(221, 64);
            lb_signin1.Name = "lb_signin1";
            lb_signin1.Size = new Size(38, 15);
            lb_signin1.TabIndex = 1;
            lb_signin1.Text = "label1";
            // 
            // lb_signin_email
            // 
            lb_signin_email.AutoSize = true;
            lb_signin_email.Location = new Point(56, 122);
            lb_signin_email.Name = "lb_signin_email";
            lb_signin_email.Size = new Size(38, 15);
            lb_signin_email.TabIndex = 2;
            lb_signin_email.Text = "label1";
            // 
            // lb_signin_pasw
            // 
            lb_signin_pasw.AutoSize = true;
            lb_signin_pasw.Location = new Point(56, 184);
            lb_signin_pasw.Name = "lb_signin_pasw";
            lb_signin_pasw.Size = new Size(38, 15);
            lb_signin_pasw.TabIndex = 3;
            lb_signin_pasw.Text = "label2";
            // 
            // tb_email
            // 
            tb_email.Location = new Point(56, 140);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(455, 23);
            tb_email.TabIndex = 4;
            // 
            // tb_pasw
            // 
            tb_pasw.Location = new Point(56, 202);
            tb_pasw.Name = "tb_pasw";
            tb_pasw.Size = new Size(455, 23);
            tb_pasw.TabIndex = 5;
            // 
            // bt_signin
            // 
            bt_signin.Location = new Point(127, 259);
            bt_signin.Name = "bt_signin";
            bt_signin.Size = new Size(302, 34);
            bt_signin.TabIndex = 6;
            bt_signin.Text = "button1";
            bt_signin.UseVisualStyleBackColor = true;
            bt_signin.Click += bt_signin_Click;
            // 
            // bt_signin_register
            // 
            bt_signin_register.Location = new Point(127, 310);
            bt_signin_register.Name = "bt_signin_register";
            bt_signin_register.Size = new Size(302, 23);
            bt_signin_register.TabIndex = 8;
            bt_signin_register.Text = "button2";
            bt_signin_register.UseVisualStyleBackColor = true;
            bt_signin_register.Click += bt_signin_register_Click;
            // 
            // lb_error
            // 
            lb_error.AutoSize = true;
            lb_error.Location = new Point(221, 112);
            lb_error.Name = "lb_error";
            lb_error.Size = new Size(38, 15);
            lb_error.TabIndex = 9;
            lb_error.Text = "label1";
            // 
            // gb_login
            // 
            gb_login.Controls.Add(gb_reg);
            gb_login.Controls.Add(lb_error);
            gb_login.Controls.Add(bt_signin_register);
            gb_login.Controls.Add(bt_signin);
            gb_login.Controls.Add(tb_pasw);
            gb_login.Controls.Add(tb_email);
            gb_login.Controls.Add(lb_signin_pasw);
            gb_login.Controls.Add(lb_signin_email);
            gb_login.Controls.Add(lb_signin1);
            gb_login.Controls.Add(lb_signin);
            gb_login.FlatStyle = FlatStyle.Flat;
            gb_login.Location = new Point(94, 42);
            gb_login.Name = "gb_login";
            gb_login.Size = new Size(539, 392);
            gb_login.TabIndex = 0;
            gb_login.TabStop = false;
            gb_login.Text = "groupBox1";
            // 
            // gb_reg
            // 
            gb_reg.Controls.Add(lb_regerror);
            gb_reg.Controls.Add(bt_reg);
            gb_reg.Controls.Add(tb_regpaswconf);
            gb_reg.Controls.Add(lb_regpaswconf);
            gb_reg.Controls.Add(tb_regpasw);
            gb_reg.Controls.Add(lb_regpasw);
            gb_reg.Controls.Add(tb_regmail);
            gb_reg.Controls.Add(lb_regmail);
            gb_reg.Controls.Add(lb_reginfo);
            gb_reg.Controls.Add(lb_reg);
            gb_reg.Location = new Point(0, 0);
            gb_reg.Name = "gb_reg";
            gb_reg.Size = new Size(586, 416);
            gb_reg.TabIndex = 1;
            gb_reg.TabStop = false;
            gb_reg.Text = "groupBox1";
            // 
            // lb_regerror
            // 
            lb_regerror.AutoSize = true;
            lb_regerror.Location = new Point(168, 377);
            lb_regerror.Name = "lb_regerror";
            lb_regerror.Size = new Size(38, 15);
            lb_regerror.TabIndex = 9;
            lb_regerror.Text = "label1";
            // 
            // bt_reg
            // 
            bt_reg.Location = new Point(142, 299);
            bt_reg.Name = "bt_reg";
            bt_reg.Size = new Size(260, 34);
            bt_reg.TabIndex = 8;
            bt_reg.Text = "button1";
            bt_reg.UseVisualStyleBackColor = true;
            bt_reg.Click += bt_reg_Click;
            // 
            // tb_regpaswconf
            // 
            tb_regpaswconf.Location = new Point(69, 246);
            tb_regpaswconf.Name = "tb_regpaswconf";
            tb_regpaswconf.Size = new Size(442, 23);
            tb_regpaswconf.TabIndex = 7;
            // 
            // lb_regpaswconf
            // 
            lb_regpaswconf.AutoSize = true;
            lb_regpaswconf.Location = new Point(69, 228);
            lb_regpaswconf.Name = "lb_regpaswconf";
            lb_regpaswconf.Size = new Size(38, 15);
            lb_regpaswconf.TabIndex = 6;
            lb_regpaswconf.Text = "label1";
            // 
            // tb_regpasw
            // 
            tb_regpasw.Location = new Point(69, 184);
            tb_regpasw.Name = "tb_regpasw";
            tb_regpasw.Size = new Size(442, 23);
            tb_regpasw.TabIndex = 5;
            // 
            // lb_regpasw
            // 
            lb_regpasw.AutoSize = true;
            lb_regpasw.Location = new Point(67, 166);
            lb_regpasw.Name = "lb_regpasw";
            lb_regpasw.Size = new Size(38, 15);
            lb_regpasw.TabIndex = 4;
            lb_regpasw.Text = "label1";
            // 
            // tb_regmail
            // 
            tb_regmail.Location = new Point(69, 130);
            tb_regmail.Name = "tb_regmail";
            tb_regmail.Size = new Size(442, 23);
            tb_regmail.TabIndex = 3;
            // 
            // lb_regmail
            // 
            lb_regmail.AutoSize = true;
            lb_regmail.Location = new Point(67, 109);
            lb_regmail.Name = "lb_regmail";
            lb_regmail.Size = new Size(38, 15);
            lb_regmail.TabIndex = 2;
            lb_regmail.Text = "label1";
            // 
            // lb_reginfo
            // 
            lb_reginfo.AutoSize = true;
            lb_reginfo.Location = new Point(236, 64);
            lb_reginfo.Name = "lb_reginfo";
            lb_reginfo.Size = new Size(38, 15);
            lb_reginfo.TabIndex = 1;
            lb_reginfo.Text = "label1";
            // 
            // lb_reg
            // 
            lb_reg.AutoSize = true;
            lb_reg.Location = new Point(236, 34);
            lb_reg.Name = "lb_reg";
            lb_reg.Size = new Size(38, 15);
            lb_reg.TabIndex = 0;
            lb_reg.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(749, 524);
            Controls.Add(gb_login);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            gb_login.ResumeLayout(false);
            gb_login.PerformLayout();
            gb_reg.ResumeLayout(false);
            gb_reg.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lb_signin;
        private Label lb_signin1;
        private Label lb_signin_email;
        private Label lb_signin_pasw;
        private TextBox tb_email;
        private TextBox tb_pasw;
        private Button bt_signin;
        private Button bt_signin_register;
        private Label lb_error;
        private GroupBox gb_login;
        private GroupBox gb_reg;
        private TextBox tb_regpaswconf;
        private Label lb_regpaswconf;
        private TextBox tb_regpasw;
        private Label lb_regpasw;
        private TextBox tb_regmail;
        private Label lb_regmail;
        private Label lb_reginfo;
        private Label lb_reg;
        private Button bt_reg;
        private Label lb_regerror;
    }
}
