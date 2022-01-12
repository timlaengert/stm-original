namespace ShootClient
{
    partial class frm_gametable
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_score = new System.Windows.Forms.Label();
            this.lbl_tricks = new System.Windows.Forms.Label();
            this.cmd_seat0 = new System.Windows.Forms.Button();
            this.cmd_seat5 = new System.Windows.Forms.Button();
            this.cmd_seat4 = new System.Windows.Forms.Button();
            this.cmd_seat3 = new System.Windows.Forms.Button();
            this.cmd_seat2 = new System.Windows.Forms.Button();
            this.cmd_seat1 = new System.Windows.Forms.Button();
            this.pnl_seat2 = new System.Windows.Forms.Panel();
            this.pb_p2 = new System.Windows.Forms.PictureBox();
            this.lbl_status2 = new System.Windows.Forms.Label();
            this.lbl_seat2 = new System.Windows.Forms.Label();
            this.pnl_seat1 = new System.Windows.Forms.Panel();
            this.pb_p1 = new System.Windows.Forms.PictureBox();
            this.lbl_status1 = new System.Windows.Forms.Label();
            this.lbl_seat1 = new System.Windows.Forms.Label();
            this.pnl_seat0 = new System.Windows.Forms.Panel();
            this.pb_p0 = new System.Windows.Forms.PictureBox();
            this.lbl_status0 = new System.Windows.Forms.Label();
            this.lbl_seat0 = new System.Windows.Forms.Label();
            this.pnl_seat5 = new System.Windows.Forms.Panel();
            this.pb_p5 = new System.Windows.Forms.PictureBox();
            this.lbl_status5 = new System.Windows.Forms.Label();
            this.lbl_seat5 = new System.Windows.Forms.Label();
            this.pnl_seat4 = new System.Windows.Forms.Panel();
            this.pb_p4 = new System.Windows.Forms.PictureBox();
            this.lbl_status4 = new System.Windows.Forms.Label();
            this.lbl_seat4 = new System.Windows.Forms.Label();
            this.pnl_seat3 = new System.Windows.Forms.Panel();
            this.pb_p3 = new System.Windows.Forms.PictureBox();
            this.lbl_status3 = new System.Windows.Forms.Label();
            this.lbl_seat3 = new System.Windows.Forms.Label();
            this.txt_chatbox = new System.Windows.Forms.RichTextBox();
            this.txt_sendMessage = new System.Windows.Forms.RichTextBox();
            this.pnl_bid = new System.Windows.Forms.Panel();
            this.cmd_bid = new System.Windows.Forms.Button();
            this.lbl_bid = new System.Windows.Forms.Label();
            this.rdo_diamonds = new System.Windows.Forms.RadioButton();
            this.rdo_spades = new System.Windows.Forms.RadioButton();
            this.rdo_clubs = new System.Windows.Forms.RadioButton();
            this.rdo_high = new System.Windows.Forms.RadioButton();
            this.rdo_low = new System.Windows.Forms.RadioButton();
            this.rdo_hearts = new System.Windows.Forms.RadioButton();
            this.tb_bid = new System.Windows.Forms.TrackBar();
            this.chk_ready = new System.Windows.Forms.CheckBox();
            this.pb_cardSurface = new System.Windows.Forms.PictureBox();
            this.pnl_seat2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p2)).BeginInit();
            this.pnl_seat1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p1)).BeginInit();
            this.pnl_seat0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p0)).BeginInit();
            this.pnl_seat5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p5)).BeginInit();
            this.pnl_seat4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p4)).BeginInit();
            this.pnl_seat3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p3)).BeginInit();
            this.pnl_bid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_bid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_cardSurface)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_score
            // 
            this.lbl_score.AutoSize = true;
            this.lbl_score.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_score.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_score.Location = new System.Drawing.Point(728, 9);
            this.lbl_score.Name = "lbl_score";
            this.lbl_score.Size = new System.Drawing.Size(56, 22);
            this.lbl_score.TabIndex = 2;
            this.lbl_score.Text = "Score:";
            // 
            // lbl_tricks
            // 
            this.lbl_tricks.AutoSize = true;
            this.lbl_tricks.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_tricks.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_tricks.Location = new System.Drawing.Point(728, 31);
            this.lbl_tricks.Name = "lbl_tricks";
            this.lbl_tricks.Size = new System.Drawing.Size(58, 22);
            this.lbl_tricks.TabIndex = 3;
            this.lbl_tricks.Text = "Tricks:";
            // 
            // cmd_seat0
            // 
            this.cmd_seat0.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat0.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat0.Name = "cmd_seat0";
            this.cmd_seat0.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat0.TabIndex = 5;
            this.cmd_seat0.Text = "I\'ll Sit Here";
            this.cmd_seat0.UseVisualStyleBackColor = true;
            this.cmd_seat0.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // cmd_seat5
            // 
            this.cmd_seat5.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat5.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat5.Name = "cmd_seat5";
            this.cmd_seat5.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat5.TabIndex = 6;
            this.cmd_seat5.Text = "Here, Please";
            this.cmd_seat5.UseVisualStyleBackColor = true;
            this.cmd_seat5.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // cmd_seat4
            // 
            this.cmd_seat4.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmd_seat4.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat4.Name = "cmd_seat4";
            this.cmd_seat4.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat4.TabIndex = 7;
            this.cmd_seat4.Text = "How About Here?";
            this.cmd_seat4.UseVisualStyleBackColor = true;
            this.cmd_seat4.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // cmd_seat3
            // 
            this.cmd_seat3.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat3.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat3.Name = "cmd_seat3";
            this.cmd_seat3.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat3.TabIndex = 8;
            this.cmd_seat3.Text = "Here Looks Good";
            this.cmd_seat3.UseVisualStyleBackColor = true;
            this.cmd_seat3.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // cmd_seat2
            // 
            this.cmd_seat2.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat2.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat2.Name = "cmd_seat2";
            this.cmd_seat2.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat2.TabIndex = 9;
            this.cmd_seat2.Text = "Why Not Here?";
            this.cmd_seat2.UseVisualStyleBackColor = true;
            this.cmd_seat2.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // cmd_seat1
            // 
            this.cmd_seat1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_seat1.Location = new System.Drawing.Point(43, 144);
            this.cmd_seat1.Name = "cmd_seat1";
            this.cmd_seat1.Size = new System.Drawing.Size(140, 34);
            this.cmd_seat1.TabIndex = 10;
            this.cmd_seat1.Text = "Or Maybe Here?";
            this.cmd_seat1.UseVisualStyleBackColor = true;
            this.cmd_seat1.Click += new System.EventHandler(this.cmd_seat_Click);
            // 
            // pnl_seat2
            // 
            this.pnl_seat2.Controls.Add(this.pb_p2);
            this.pnl_seat2.Controls.Add(this.lbl_status2);
            this.pnl_seat2.Controls.Add(this.lbl_seat2);
            this.pnl_seat2.Controls.Add(this.cmd_seat2);
            this.pnl_seat2.Location = new System.Drawing.Point(24, 214);
            this.pnl_seat2.Name = "pnl_seat2";
            this.pnl_seat2.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat2.TabIndex = 11;
            // 
            // pb_p2
            // 
            this.pb_p2.Location = new System.Drawing.Point(2, 37);
            this.pb_p2.Name = "pb_p2";
            this.pb_p2.Size = new System.Drawing.Size(264, 108);
            this.pb_p2.TabIndex = 25;
            this.pb_p2.TabStop = false;
            this.pb_p2.Visible = false;
            this.pb_p2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status2
            // 
            this.lbl_status2.AutoSize = true;
            this.lbl_status2.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status2.Location = new System.Drawing.Point(90, 104);
            this.lbl_status2.Name = "lbl_status2";
            this.lbl_status2.Size = new System.Drawing.Size(46, 18);
            this.lbl_status2.TabIndex = 11;
            this.lbl_status2.Text = "READY";
            // 
            // lbl_seat2
            // 
            this.lbl_seat2.AutoSize = true;
            this.lbl_seat2.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat2.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_seat2.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat2.Name = "lbl_seat2";
            this.lbl_seat2.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat2.TabIndex = 10;
            this.lbl_seat2.Text = "Player 2";
            // 
            // pnl_seat1
            // 
            this.pnl_seat1.Controls.Add(this.pb_p1);
            this.pnl_seat1.Controls.Add(this.lbl_status1);
            this.pnl_seat1.Controls.Add(this.lbl_seat1);
            this.pnl_seat1.Controls.Add(this.cmd_seat1);
            this.pnl_seat1.Location = new System.Drawing.Point(80, 417);
            this.pnl_seat1.Name = "pnl_seat1";
            this.pnl_seat1.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat1.TabIndex = 12;
            // 
            // pb_p1
            // 
            this.pb_p1.Location = new System.Drawing.Point(3, 37);
            this.pb_p1.Name = "pb_p1";
            this.pb_p1.Size = new System.Drawing.Size(264, 108);
            this.pb_p1.TabIndex = 13;
            this.pb_p1.TabStop = false;
            this.pb_p1.Visible = false;
            this.pb_p1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status1
            // 
            this.lbl_status1.AutoSize = true;
            this.lbl_status1.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status1.Location = new System.Drawing.Point(90, 104);
            this.lbl_status1.Name = "lbl_status1";
            this.lbl_status1.Size = new System.Drawing.Size(46, 18);
            this.lbl_status1.TabIndex = 12;
            this.lbl_status1.Text = "READY";
            // 
            // lbl_seat1
            // 
            this.lbl_seat1.AutoSize = true;
            this.lbl_seat1.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_seat1.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat1.Name = "lbl_seat1";
            this.lbl_seat1.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat1.TabIndex = 11;
            this.lbl_seat1.Text = "Player 1";
            // 
            // pnl_seat0
            // 
            this.pnl_seat0.Controls.Add(this.pb_p0);
            this.pnl_seat0.Controls.Add(this.lbl_status0);
            this.pnl_seat0.Controls.Add(this.lbl_seat0);
            this.pnl_seat0.Controls.Add(this.cmd_seat0);
            this.pnl_seat0.Location = new System.Drawing.Point(425, 417);
            this.pnl_seat0.Name = "pnl_seat0";
            this.pnl_seat0.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat0.TabIndex = 13;
            // 
            // pb_p0
            // 
            this.pb_p0.Location = new System.Drawing.Point(2, 37);
            this.pb_p0.Name = "pb_p0";
            this.pb_p0.Size = new System.Drawing.Size(264, 108);
            this.pb_p0.TabIndex = 25;
            this.pb_p0.TabStop = false;
            this.pb_p0.Visible = false;
            this.pb_p0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status0
            // 
            this.lbl_status0.AutoSize = true;
            this.lbl_status0.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status0.Location = new System.Drawing.Point(90, 104);
            this.lbl_status0.Name = "lbl_status0";
            this.lbl_status0.Size = new System.Drawing.Size(46, 18);
            this.lbl_status0.TabIndex = 13;
            this.lbl_status0.Text = "READY";
            // 
            // lbl_seat0
            // 
            this.lbl_seat0.AutoSize = true;
            this.lbl_seat0.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat0.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_seat0.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat0.Name = "lbl_seat0";
            this.lbl_seat0.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat0.TabIndex = 12;
            this.lbl_seat0.Text = "Player 0";
            // 
            // pnl_seat5
            // 
            this.pnl_seat5.Controls.Add(this.pb_p5);
            this.pnl_seat5.Controls.Add(this.lbl_status5);
            this.pnl_seat5.Controls.Add(this.lbl_seat5);
            this.pnl_seat5.Controls.Add(this.cmd_seat5);
            this.pnl_seat5.Location = new System.Drawing.Point(481, 214);
            this.pnl_seat5.Name = "pnl_seat5";
            this.pnl_seat5.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat5.TabIndex = 14;
            // 
            // pb_p5
            // 
            this.pb_p5.Location = new System.Drawing.Point(2, 37);
            this.pb_p5.Name = "pb_p5";
            this.pb_p5.Size = new System.Drawing.Size(264, 108);
            this.pb_p5.TabIndex = 25;
            this.pb_p5.TabStop = false;
            this.pb_p5.Visible = false;
            this.pb_p5.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status5
            // 
            this.lbl_status5.AutoSize = true;
            this.lbl_status5.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status5.Location = new System.Drawing.Point(90, 104);
            this.lbl_status5.Name = "lbl_status5";
            this.lbl_status5.Size = new System.Drawing.Size(46, 18);
            this.lbl_status5.TabIndex = 14;
            this.lbl_status5.Text = "READY";
            // 
            // lbl_seat5
            // 
            this.lbl_seat5.AutoSize = true;
            this.lbl_seat5.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat5.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_seat5.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat5.Name = "lbl_seat5";
            this.lbl_seat5.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat5.TabIndex = 13;
            this.lbl_seat5.Text = "Player 5";
            // 
            // pnl_seat4
            // 
            this.pnl_seat4.Controls.Add(this.pb_p4);
            this.pnl_seat4.Controls.Add(this.lbl_status4);
            this.pnl_seat4.Controls.Add(this.lbl_seat4);
            this.pnl_seat4.Controls.Add(this.cmd_seat4);
            this.pnl_seat4.Location = new System.Drawing.Point(425, 11);
            this.pnl_seat4.Name = "pnl_seat4";
            this.pnl_seat4.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat4.TabIndex = 15;
            // 
            // pb_p4
            // 
            this.pb_p4.Location = new System.Drawing.Point(2, 37);
            this.pb_p4.Name = "pb_p4";
            this.pb_p4.Size = new System.Drawing.Size(264, 108);
            this.pb_p4.TabIndex = 25;
            this.pb_p4.TabStop = false;
            this.pb_p4.Visible = false;
            this.pb_p4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status4
            // 
            this.lbl_status4.AutoSize = true;
            this.lbl_status4.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status4.Location = new System.Drawing.Point(90, 104);
            this.lbl_status4.Name = "lbl_status4";
            this.lbl_status4.Size = new System.Drawing.Size(46, 18);
            this.lbl_status4.TabIndex = 9;
            this.lbl_status4.Text = "READY";
            // 
            // lbl_seat4
            // 
            this.lbl_seat4.AutoSize = true;
            this.lbl_seat4.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat4.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_seat4.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat4.Name = "lbl_seat4";
            this.lbl_seat4.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat4.TabIndex = 8;
            this.lbl_seat4.Text = "Player 4";
            // 
            // pnl_seat3
            // 
            this.pnl_seat3.Controls.Add(this.pb_p3);
            this.pnl_seat3.Controls.Add(this.lbl_status3);
            this.pnl_seat3.Controls.Add(this.lbl_seat3);
            this.pnl_seat3.Controls.Add(this.cmd_seat3);
            this.pnl_seat3.Location = new System.Drawing.Point(80, 11);
            this.pnl_seat3.Name = "pnl_seat3";
            this.pnl_seat3.Size = new System.Drawing.Size(268, 181);
            this.pnl_seat3.TabIndex = 16;
            // 
            // pb_p3
            // 
            this.pb_p3.Location = new System.Drawing.Point(2, 37);
            this.pb_p3.Name = "pb_p3";
            this.pb_p3.Size = new System.Drawing.Size(264, 108);
            this.pb_p3.TabIndex = 25;
            this.pb_p3.TabStop = false;
            this.pb_p3.Visible = false;
            this.pb_p3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cardClicked);
            // 
            // lbl_status3
            // 
            this.lbl_status3.AutoSize = true;
            this.lbl_status3.ForeColor = System.Drawing.Color.Lime;
            this.lbl_status3.Location = new System.Drawing.Point(90, 104);
            this.lbl_status3.Name = "lbl_status3";
            this.lbl_status3.Size = new System.Drawing.Size(46, 18);
            this.lbl_status3.TabIndex = 10;
            this.lbl_status3.Text = "READY";
            // 
            // lbl_seat3
            // 
            this.lbl_seat3.AutoSize = true;
            this.lbl_seat3.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_seat3.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_seat3.Location = new System.Drawing.Point(69, 77);
            this.lbl_seat3.Name = "lbl_seat3";
            this.lbl_seat3.Size = new System.Drawing.Size(88, 27);
            this.lbl_seat3.TabIndex = 9;
            this.lbl_seat3.Text = "Player 3";
            // 
            // txt_chatbox
            // 
            this.txt_chatbox.Location = new System.Drawing.Point(731, 56);
            this.txt_chatbox.Name = "txt_chatbox";
            this.txt_chatbox.ReadOnly = true;
            this.txt_chatbox.Size = new System.Drawing.Size(181, 567);
            this.txt_chatbox.TabIndex = 17;
            this.txt_chatbox.Text = "";
            // 
            // txt_sendMessage
            // 
            this.txt_sendMessage.BackColor = System.Drawing.SystemColors.Control;
            this.txt_sendMessage.Location = new System.Drawing.Point(12, 601);
            this.txt_sendMessage.Name = "txt_sendMessage";
            this.txt_sendMessage.Size = new System.Drawing.Size(713, 23);
            this.txt_sendMessage.TabIndex = 18;
            this.txt_sendMessage.Text = "";
            this.txt_sendMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_sendMessage_KeyPress);
            // 
            // pnl_bid
            // 
            this.pnl_bid.Controls.Add(this.cmd_bid);
            this.pnl_bid.Controls.Add(this.lbl_bid);
            this.pnl_bid.Controls.Add(this.rdo_diamonds);
            this.pnl_bid.Controls.Add(this.rdo_spades);
            this.pnl_bid.Controls.Add(this.rdo_clubs);
            this.pnl_bid.Controls.Add(this.rdo_high);
            this.pnl_bid.Controls.Add(this.rdo_low);
            this.pnl_bid.Controls.Add(this.rdo_hearts);
            this.pnl_bid.Controls.Add(this.tb_bid);
            this.pnl_bid.Controls.Add(this.chk_ready);
            this.pnl_bid.Location = new System.Drawing.Point(266, 214);
            this.pnl_bid.Name = "pnl_bid";
            this.pnl_bid.Size = new System.Drawing.Size(199, 183);
            this.pnl_bid.TabIndex = 19;
            // 
            // cmd_bid
            // 
            this.cmd_bid.Location = new System.Drawing.Point(0, 154);
            this.cmd_bid.Name = "cmd_bid";
            this.cmd_bid.Size = new System.Drawing.Size(199, 29);
            this.cmd_bid.TabIndex = 28;
            this.cmd_bid.Text = "Place Bid (Pass)";
            this.cmd_bid.UseVisualStyleBackColor = true;
            this.cmd_bid.Visible = false;
            this.cmd_bid.Click += new System.EventHandler(this.cmd_bid_Click);
            // 
            // lbl_bid
            // 
            this.lbl_bid.AutoSize = true;
            this.lbl_bid.ForeColor = System.Drawing.Color.LimeGreen;
            this.lbl_bid.Location = new System.Drawing.Point(68, 1);
            this.lbl_bid.Name = "lbl_bid";
            this.lbl_bid.Size = new System.Drawing.Size(63, 18);
            this.lbl_bid.TabIndex = 27;
            this.lbl_bid.Text = "YOUR BID";
            this.lbl_bid.Visible = false;
            // 
            // rdo_diamonds
            // 
            this.rdo_diamonds.AutoSize = true;
            this.rdo_diamonds.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_diamonds.Location = new System.Drawing.Point(12, 50);
            this.rdo_diamonds.Name = "rdo_diamonds";
            this.rdo_diamonds.Size = new System.Drawing.Size(83, 22);
            this.rdo_diamonds.TabIndex = 26;
            this.rdo_diamonds.TabStop = true;
            this.rdo_diamonds.Text = "Diamonds";
            this.rdo_diamonds.UseVisualStyleBackColor = true;
            this.rdo_diamonds.Visible = false;
            this.rdo_diamonds.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // rdo_spades
            // 
            this.rdo_spades.AutoSize = true;
            this.rdo_spades.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_spades.Location = new System.Drawing.Point(115, 22);
            this.rdo_spades.Name = "rdo_spades";
            this.rdo_spades.Size = new System.Drawing.Size(67, 22);
            this.rdo_spades.TabIndex = 25;
            this.rdo_spades.TabStop = true;
            this.rdo_spades.Text = "Spades";
            this.rdo_spades.UseVisualStyleBackColor = true;
            this.rdo_spades.Visible = false;
            this.rdo_spades.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // rdo_clubs
            // 
            this.rdo_clubs.AutoSize = true;
            this.rdo_clubs.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_clubs.Location = new System.Drawing.Point(115, 50);
            this.rdo_clubs.Name = "rdo_clubs";
            this.rdo_clubs.Size = new System.Drawing.Size(57, 22);
            this.rdo_clubs.TabIndex = 24;
            this.rdo_clubs.TabStop = true;
            this.rdo_clubs.Text = "Clubs";
            this.rdo_clubs.UseVisualStyleBackColor = true;
            this.rdo_clubs.Visible = false;
            this.rdo_clubs.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // rdo_high
            // 
            this.rdo_high.AutoSize = true;
            this.rdo_high.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_high.Location = new System.Drawing.Point(12, 78);
            this.rdo_high.Name = "rdo_high";
            this.rdo_high.Size = new System.Drawing.Size(53, 22);
            this.rdo_high.TabIndex = 23;
            this.rdo_high.TabStop = true;
            this.rdo_high.Text = "High";
            this.rdo_high.UseVisualStyleBackColor = true;
            this.rdo_high.Visible = false;
            this.rdo_high.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // rdo_low
            // 
            this.rdo_low.AutoSize = true;
            this.rdo_low.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_low.Location = new System.Drawing.Point(115, 78);
            this.rdo_low.Name = "rdo_low";
            this.rdo_low.Size = new System.Drawing.Size(49, 22);
            this.rdo_low.TabIndex = 22;
            this.rdo_low.TabStop = true;
            this.rdo_low.Text = "Low";
            this.rdo_low.UseVisualStyleBackColor = true;
            this.rdo_low.Visible = false;
            this.rdo_low.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // rdo_hearts
            // 
            this.rdo_hearts.AutoSize = true;
            this.rdo_hearts.ForeColor = System.Drawing.Color.DarkKhaki;
            this.rdo_hearts.Location = new System.Drawing.Point(12, 22);
            this.rdo_hearts.Name = "rdo_hearts";
            this.rdo_hearts.Size = new System.Drawing.Size(65, 22);
            this.rdo_hearts.TabIndex = 21;
            this.rdo_hearts.TabStop = true;
            this.rdo_hearts.Text = "Hearts";
            this.rdo_hearts.UseVisualStyleBackColor = true;
            this.rdo_hearts.Visible = false;
            this.rdo_hearts.CheckedChanged += new System.EventHandler(this.rdo_bid_CheckedChanged);
            // 
            // tb_bid
            // 
            this.tb_bid.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.tb_bid.LargeChange = 1;
            this.tb_bid.Location = new System.Drawing.Point(0, 105);
            this.tb_bid.Maximum = 9;
            this.tb_bid.Name = "tb_bid";
            this.tb_bid.Size = new System.Drawing.Size(199, 48);
            this.tb_bid.TabIndex = 20;
            this.tb_bid.Visible = false;
            this.tb_bid.ValueChanged += new System.EventHandler(this.tb_bid_ValueChanged);
            // 
            // chk_ready
            // 
            this.chk_ready.AutoSize = true;
            this.chk_ready.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_ready.ForeColor = System.Drawing.Color.DarkKhaki;
            this.chk_ready.Location = new System.Drawing.Point(48, 78);
            this.chk_ready.Name = "chk_ready";
            this.chk_ready.Size = new System.Drawing.Size(103, 26);
            this.chk_ready.TabIndex = 0;
            this.chk_ready.Text = "I\'m Ready!";
            this.chk_ready.UseVisualStyleBackColor = true;
            this.chk_ready.Visible = false;
            this.chk_ready.CheckedChanged += new System.EventHandler(this.chk_ready_CheckedChanged);
            // 
            // pb_cardSurface
            // 
            this.pb_cardSurface.Location = new System.Drawing.Point(253, 196);
            this.pb_cardSurface.Name = "pb_cardSurface";
            this.pb_cardSurface.Size = new System.Drawing.Size(225, 216);
            this.pb_cardSurface.TabIndex = 20;
            this.pb_cardSurface.TabStop = false;
            this.pb_cardSurface.Visible = false;
            // 
            // frm_gametable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.ClientSize = new System.Drawing.Size(924, 636);
            this.Controls.Add(this.pb_cardSurface);
            this.Controls.Add(this.pnl_bid);
            this.Controls.Add(this.txt_sendMessage);
            this.Controls.Add(this.txt_chatbox);
            this.Controls.Add(this.lbl_tricks);
            this.Controls.Add(this.lbl_score);
            this.Controls.Add(this.pnl_seat2);
            this.Controls.Add(this.pnl_seat1);
            this.Controls.Add(this.pnl_seat0);
            this.Controls.Add(this.pnl_seat5);
            this.Controls.Add(this.pnl_seat4);
            this.Controls.Add(this.pnl_seat3);
            this.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_gametable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shoot The Moon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_gametable_FormClosing);
            this.pnl_seat2.ResumeLayout(false);
            this.pnl_seat2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p2)).EndInit();
            this.pnl_seat1.ResumeLayout(false);
            this.pnl_seat1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p1)).EndInit();
            this.pnl_seat0.ResumeLayout(false);
            this.pnl_seat0.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p0)).EndInit();
            this.pnl_seat5.ResumeLayout(false);
            this.pnl_seat5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p5)).EndInit();
            this.pnl_seat4.ResumeLayout(false);
            this.pnl_seat4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p4)).EndInit();
            this.pnl_seat3.ResumeLayout(false);
            this.pnl_seat3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_p3)).EndInit();
            this.pnl_bid.ResumeLayout(false);
            this.pnl_bid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_bid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_cardSurface)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_score;
        private System.Windows.Forms.Label lbl_tricks;
        private System.Windows.Forms.Panel pnl_seat2;
        private System.Windows.Forms.Panel pnl_seat1;
        private System.Windows.Forms.Panel pnl_seat0;
        private System.Windows.Forms.Panel pnl_seat5;
        private System.Windows.Forms.Panel pnl_seat4;
        private System.Windows.Forms.Panel pnl_seat3;
        public System.Windows.Forms.Button cmd_seat0;
        public System.Windows.Forms.Button cmd_seat5;
        public System.Windows.Forms.Button cmd_seat4;
        public System.Windows.Forms.Button cmd_seat3;
        public System.Windows.Forms.Button cmd_seat2;
        public System.Windows.Forms.Button cmd_seat1;
        public System.Windows.Forms.Label lbl_seat2;
        public System.Windows.Forms.Label lbl_seat0;
        public System.Windows.Forms.Label lbl_seat1;
        public System.Windows.Forms.Label lbl_seat4;
        public System.Windows.Forms.Label lbl_seat3;
        public System.Windows.Forms.Label lbl_seat5;
        private System.Windows.Forms.RichTextBox txt_chatbox;
        private System.Windows.Forms.RichTextBox txt_sendMessage;
        private System.Windows.Forms.Panel pnl_bid;
        public System.Windows.Forms.Label lbl_status4;
        public System.Windows.Forms.Label lbl_status2;
        public System.Windows.Forms.Label lbl_status1;
        public System.Windows.Forms.Label lbl_status0;
        public System.Windows.Forms.Label lbl_status5;
        public System.Windows.Forms.Label lbl_status3;
        public System.Windows.Forms.CheckBox chk_ready;
        public System.Windows.Forms.PictureBox pb_p1;
        public System.Windows.Forms.PictureBox pb_p2;
        public System.Windows.Forms.PictureBox pb_p0;
        public System.Windows.Forms.PictureBox pb_p3;
        public System.Windows.Forms.PictureBox pb_p5;
        public System.Windows.Forms.PictureBox pb_p4;
        public System.Windows.Forms.TrackBar tb_bid;
        public System.Windows.Forms.RadioButton rdo_diamonds;
        public System.Windows.Forms.RadioButton rdo_spades;
        public System.Windows.Forms.RadioButton rdo_clubs;
        public System.Windows.Forms.RadioButton rdo_high;
        public System.Windows.Forms.RadioButton rdo_low;
        public System.Windows.Forms.RadioButton rdo_hearts;
        public System.Windows.Forms.Button cmd_bid;
        public System.Windows.Forms.Label lbl_bid;
        public System.Windows.Forms.PictureBox pb_cardSurface;
    }
}