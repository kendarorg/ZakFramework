namespace ZakCmsSetup
{
	partial class ZakCmsSetup
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
			this.tbDbConnectionString = new System.Windows.Forms.TextBox();
			this.lbDbConnectionString = new System.Windows.Forms.Label();
			this.cbCreateTables = new System.Windows.Forms.CheckBox();
			this.cbDropTables = new System.Windows.Forms.CheckBox();
			this.cbEmptyTables = new System.Windows.Forms.CheckBox();
			this.cbFillTables = new System.Windows.Forms.CheckBox();
			this.lbDbDataDirectory = new System.Windows.Forms.Label();
			this.tbDbDataDirectory = new System.Windows.Forms.TextBox();
			this.btRunSetup = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbDbConnectionString
			// 
			this.tbDbConnectionString.Location = new System.Drawing.Point(136, 12);
			this.tbDbConnectionString.Name = "tbDbConnectionString";
			this.tbDbConnectionString.Size = new System.Drawing.Size(307, 20);
			this.tbDbConnectionString.TabIndex = 0;
			// 
			// lbDbConnectionString
			// 
			this.lbDbConnectionString.AutoSize = true;
			this.lbDbConnectionString.Location = new System.Drawing.Point(12, 15);
			this.lbDbConnectionString.Name = "lbDbConnectionString";
			this.lbDbConnectionString.Size = new System.Drawing.Size(108, 13);
			this.lbDbConnectionString.TabIndex = 1;
			this.lbDbConnectionString.Text = "Db Connection String";
			// 
			// cbCreateTables
			// 
			this.cbCreateTables.AutoSize = true;
			this.cbCreateTables.Checked = true;
			this.cbCreateTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbCreateTables.Location = new System.Drawing.Point(136, 67);
			this.cbCreateTables.Name = "cbCreateTables";
			this.cbCreateTables.Size = new System.Drawing.Size(92, 17);
			this.cbCreateTables.TabIndex = 3;
			this.cbCreateTables.Text = "Create Tables";
			this.cbCreateTables.UseVisualStyleBackColor = true;
			// 
			// cbDropTables
			// 
			this.cbDropTables.AutoSize = true;
			this.cbDropTables.Checked = true;
			this.cbDropTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbDropTables.Location = new System.Drawing.Point(15, 67);
			this.cbDropTables.Name = "cbDropTables";
			this.cbDropTables.Size = new System.Drawing.Size(84, 17);
			this.cbDropTables.TabIndex = 4;
			this.cbDropTables.Text = "Drop Tables";
			this.cbDropTables.UseVisualStyleBackColor = true;
			// 
			// cbEmptyTables
			// 
			this.cbEmptyTables.AutoSize = true;
			this.cbEmptyTables.Checked = true;
			this.cbEmptyTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbEmptyTables.Location = new System.Drawing.Point(247, 67);
			this.cbEmptyTables.Name = "cbEmptyTables";
			this.cbEmptyTables.Size = new System.Drawing.Size(90, 17);
			this.cbEmptyTables.TabIndex = 5;
			this.cbEmptyTables.Text = "Empty Tables";
			this.cbEmptyTables.UseVisualStyleBackColor = true;
			// 
			// cbFillTables
			// 
			this.cbFillTables.AutoSize = true;
			this.cbFillTables.Checked = true;
			this.cbFillTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFillTables.Location = new System.Drawing.Point(353, 67);
			this.cbFillTables.Name = "cbFillTables";
			this.cbFillTables.Size = new System.Drawing.Size(73, 17);
			this.cbFillTables.TabIndex = 6;
			this.cbFillTables.Text = "Fill Tables";
			this.cbFillTables.UseVisualStyleBackColor = true;
			// 
			// lbDbDataDirectory
			// 
			this.lbDbDataDirectory.AutoSize = true;
			this.lbDbDataDirectory.Location = new System.Drawing.Point(12, 44);
			this.lbDbDataDirectory.Name = "lbDbDataDirectory";
			this.lbDbDataDirectory.Size = new System.Drawing.Size(75, 13);
			this.lbDbDataDirectory.TabIndex = 8;
			this.lbDbDataDirectory.Text = "Data Directory";
			// 
			// tbDbDataDirectory
			// 
			this.tbDbDataDirectory.Location = new System.Drawing.Point(136, 41);
			this.tbDbDataDirectory.Name = "tbDbDataDirectory";
			this.tbDbDataDirectory.Size = new System.Drawing.Size(307, 20);
			this.tbDbDataDirectory.TabIndex = 7;
			// 
			// btRunSetup
			// 
			this.btRunSetup.Location = new System.Drawing.Point(164, 90);
			this.btRunSetup.Name = "btRunSetup";
			this.btRunSetup.Size = new System.Drawing.Size(100, 23);
			this.btRunSetup.TabIndex = 9;
			this.btRunSetup.Text = "Run Setup";
			this.btRunSetup.UseVisualStyleBackColor = true;
			this.btRunSetup.Click += new System.EventHandler(this.BtRunSetupClick);
			// 
			// ZakCmsSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(455, 124);
			this.Controls.Add(this.btRunSetup);
			this.Controls.Add(this.lbDbDataDirectory);
			this.Controls.Add(this.tbDbDataDirectory);
			this.Controls.Add(this.cbFillTables);
			this.Controls.Add(this.cbEmptyTables);
			this.Controls.Add(this.cbDropTables);
			this.Controls.Add(this.cbCreateTables);
			this.Controls.Add(this.lbDbConnectionString);
			this.Controls.Add(this.tbDbConnectionString);
			this.Name = "ZakCmsSetup";
			this.Text = "ZakCms Setup Application";
			this.Load += new System.EventHandler(this.ZakCmsSetupLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbDbConnectionString;
		private System.Windows.Forms.Label lbDbConnectionString;
		private System.Windows.Forms.CheckBox cbCreateTables;
		private System.Windows.Forms.CheckBox cbDropTables;
		private System.Windows.Forms.CheckBox cbEmptyTables;
		private System.Windows.Forms.CheckBox cbFillTables;
		private System.Windows.Forms.Label lbDbDataDirectory;
		private System.Windows.Forms.TextBox tbDbDataDirectory;
		private System.Windows.Forms.Button btRunSetup;
	}
}

