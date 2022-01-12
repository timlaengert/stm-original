namespace ShootClient
{
    partial class frm_gamefinder
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_gameslist = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numPlayers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmd_join = new System.Windows.Forms.Button();
            this.cmd_host = new System.Windows.Forms.Button();
            this.dgvPlayersOnline = new System.Windows.Forms.DataGridView();
            this.Username = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clientMainBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_gameslist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayersOnline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientMainBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_gameslist
            // 
            this.dgv_gameslist.AllowUserToAddRows = false;
            this.dgv_gameslist.AllowUserToDeleteRows = false;
            this.dgv_gameslist.AllowUserToResizeColumns = false;
            this.dgv_gameslist.AllowUserToResizeRows = false;
            this.dgv_gameslist.BackgroundColor = System.Drawing.Color.Gray;
            this.dgv_gameslist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_gameslist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.name,
            this.numPlayers});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_gameslist.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_gameslist.GridColor = System.Drawing.Color.Gray;
            this.dgv_gameslist.Location = new System.Drawing.Point(12, 12);
            this.dgv_gameslist.MultiSelect = false;
            this.dgv_gameslist.Name = "dgv_gameslist";
            this.dgv_gameslist.ReadOnly = true;
            this.dgv_gameslist.RowHeadersVisible = false;
            this.dgv_gameslist.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_gameslist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_gameslist.Size = new System.Drawing.Size(248, 387);
            this.dgv_gameslist.TabIndex = 0;
            this.dgv_gameslist.DoubleClick += new System.EventHandler(this.dgv_gameslist_DoubleClick);
            this.dgv_gameslist.SelectionChanged += new System.EventHandler(this.dgv_gameslist_SelectionChanged);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // name
            // 
            this.name.HeaderText = "Game Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 194;
            // 
            // numPlayers
            // 
            this.numPlayers.HeaderText = "Players";
            this.numPlayers.Name = "numPlayers";
            this.numPlayers.ReadOnly = true;
            this.numPlayers.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.numPlayers.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.numPlayers.Width = 50;
            // 
            // cmd_join
            // 
            this.cmd_join.Location = new System.Drawing.Point(12, 405);
            this.cmd_join.Name = "cmd_join";
            this.cmd_join.Size = new System.Drawing.Size(121, 28);
            this.cmd_join.TabIndex = 1;
            this.cmd_join.Text = "Join Game";
            this.cmd_join.UseVisualStyleBackColor = true;
            this.cmd_join.Click += new System.EventHandler(this.cmd_join_Click);
            // 
            // cmd_host
            // 
            this.cmd_host.Location = new System.Drawing.Point(139, 405);
            this.cmd_host.Name = "cmd_host";
            this.cmd_host.Size = new System.Drawing.Size(121, 28);
            this.cmd_host.TabIndex = 2;
            this.cmd_host.Text = "New Game";
            this.cmd_host.UseVisualStyleBackColor = true;
            this.cmd_host.Click += new System.EventHandler(this.cmd_host_Click);
            // 
            // dgvPlayersOnline
            // 
            this.dgvPlayersOnline.AllowUserToAddRows = false;
            this.dgvPlayersOnline.AllowUserToDeleteRows = false;
            this.dgvPlayersOnline.AllowUserToResizeColumns = false;
            this.dgvPlayersOnline.AllowUserToResizeRows = false;
            this.dgvPlayersOnline.BackgroundColor = System.Drawing.Color.Gray;
            this.dgvPlayersOnline.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlayersOnline.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Username});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPlayersOnline.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPlayersOnline.GridColor = System.Drawing.Color.Gray;
            this.dgvPlayersOnline.Location = new System.Drawing.Point(266, 12);
            this.dgvPlayersOnline.Name = "dgvPlayersOnline";
            this.dgvPlayersOnline.ReadOnly = true;
            this.dgvPlayersOnline.RowHeadersVisible = false;
            this.dgvPlayersOnline.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPlayersOnline.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlayersOnline.Size = new System.Drawing.Size(194, 387);
            this.dgvPlayersOnline.TabIndex = 3;
            this.dgvPlayersOnline.TabStop = false;
            // 
            // Username
            // 
            this.Username.Frozen = true;
            this.Username.HeaderText = "Players Online";
            this.Username.Name = "Username";
            this.Username.ReadOnly = true;
            this.Username.Width = 190;
            // 
            // clientMainBindingSource
            // 
            this.clientMainBindingSource.DataSource = typeof(ShootClient.ClientMain);
            // 
            // frm_gamefinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(472, 445);
            this.Controls.Add(this.dgvPlayersOnline);
            this.Controls.Add(this.cmd_host);
            this.Controls.Add(this.cmd_join);
            this.Controls.Add(this.dgv_gameslist);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_gamefinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shoot the Moon - Choose a Game";
            this.VisibleChanged += new System.EventHandler(this.frm_gamefinder_VisibleChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_gamefinder_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_gameslist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayersOnline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientMainBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_gameslist;
        private System.Windows.Forms.Button cmd_join;
        private System.Windows.Forms.Button cmd_host;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn numPlayers;
        private System.Windows.Forms.DataGridView dgvPlayersOnline;
        private System.Windows.Forms.BindingSource clientMainBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Username;
    }
}