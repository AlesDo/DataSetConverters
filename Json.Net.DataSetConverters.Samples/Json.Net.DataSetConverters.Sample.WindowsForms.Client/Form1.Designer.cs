namespace Json.Net.DataSetConverters.Sample.WindowsForms.Client
{
   partial class Form1
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
         this.dataGridView1 = new System.Windows.Forms.DataGridView();
         this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.customersBindingSource = new System.Windows.Forms.BindingSource(this.components);
         this.panel1 = new System.Windows.Forms.Panel();
         this.updateButton = new System.Windows.Forms.Button();
         this.refreshButton = new System.Windows.Forms.Button();
         this.clearButton = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.customersBindingSource)).BeginInit();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // dataGridView1
         // 
         this.dataGridView1.AutoGenerateColumns = false;
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FirstName,
            this.LastName});
         this.dataGridView1.DataSource = this.customersBindingSource;
         this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataGridView1.Location = new System.Drawing.Point(0, 0);
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.Size = new System.Drawing.Size(800, 403);
         this.dataGridView1.TabIndex = 0;
         // 
         // FirstName
         // 
         this.FirstName.DataPropertyName = "FirstName";
         this.FirstName.HeaderText = "First Name";
         this.FirstName.Name = "FirstName";
         // 
         // LastName
         // 
         this.LastName.DataPropertyName = "LastName";
         this.LastName.HeaderText = "Last Name";
         this.LastName.Name = "LastName";
         // 
         // customersBindingSource
         // 
         this.customersBindingSource.DataMember = "Customer";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.clearButton);
         this.panel1.Controls.Add(this.updateButton);
         this.panel1.Controls.Add(this.refreshButton);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.panel1.Location = new System.Drawing.Point(0, 403);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(800, 47);
         this.panel1.TabIndex = 1;
         // 
         // updateButton
         // 
         this.updateButton.Location = new System.Drawing.Point(713, 12);
         this.updateButton.Name = "updateButton";
         this.updateButton.Size = new System.Drawing.Size(75, 23);
         this.updateButton.TabIndex = 1;
         this.updateButton.Text = "Update";
         this.updateButton.UseVisualStyleBackColor = true;
         this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
         // 
         // refreshButton
         // 
         this.refreshButton.Location = new System.Drawing.Point(632, 12);
         this.refreshButton.Name = "refreshButton";
         this.refreshButton.Size = new System.Drawing.Size(75, 23);
         this.refreshButton.TabIndex = 0;
         this.refreshButton.Text = "Refresh";
         this.refreshButton.UseVisualStyleBackColor = true;
         this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
         // 
         // clearButton
         // 
         this.clearButton.Location = new System.Drawing.Point(551, 12);
         this.clearButton.Name = "clearButton";
         this.clearButton.Size = new System.Drawing.Size(75, 23);
         this.clearButton.TabIndex = 2;
         this.clearButton.Text = "Clear";
         this.clearButton.UseVisualStyleBackColor = true;
         this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.dataGridView1);
         this.Controls.Add(this.panel1);
         this.Name = "Form1";
         this.Text = "Form1";
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.customersBindingSource)).EndInit();
         this.panel1.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.DataGridView dataGridView1;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Button updateButton;
      private System.Windows.Forms.Button refreshButton;
      private System.Windows.Forms.BindingSource customersBindingSource;
      private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
      private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
      private System.Windows.Forms.Button clearButton;
   }
}

