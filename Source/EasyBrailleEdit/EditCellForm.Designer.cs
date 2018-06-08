namespace EasyBrailleEdit
{
    partial class EditCellForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtChar = new System.Windows.Forms.TextBox();
            this.cboPhCode = new System.Windows.Forms.ComboBox();
            this.txtBraille = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPickBraille = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "明眼字";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "點字";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 80);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "注音碼";
            // 
            // txtChar
            // 
            this.txtChar.Location = new System.Drawing.Point(106, 38);
            this.txtChar.MaxLength = 1;
            this.txtChar.Name = "txtChar";
            this.txtChar.Size = new System.Drawing.Size(70, 29);
            this.txtChar.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtChar, "只能輸入一個字元，若輸入空白字元則為空方。");
            this.txtChar.TextChanged += new System.EventHandler(this.txtChar_TextChanged);
            this.txtChar.Validating += new System.ComponentModel.CancelEventHandler(this.txtChar_Validating);
            // 
            // cboPhCode
            // 
            this.cboPhCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhCode.FormattingEnabled = true;
            this.cboPhCode.Location = new System.Drawing.Point(106, 77);
            this.cboPhCode.Name = "cboPhCode";
            this.cboPhCode.Size = new System.Drawing.Size(133, 26);
            this.cboPhCode.TabIndex = 3;
            this.cboPhCode.SelectedIndexChanged += new System.EventHandler(this.cboPhCode_SelectedIndexChanged);
            // 
            // txtBraille
            // 
            this.txtBraille.Font = new System.Drawing.Font("SimBraille", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBraille.Location = new System.Drawing.Point(101, 116);
            this.txtBraille.MaxLength = 3;
            this.txtBraille.Name = "txtBraille";
            this.txtBraille.Size = new System.Drawing.Size(133, 47);
            this.txtBraille.TabIndex = 5;
            this.txtBraille.TextChanged += new System.EventHandler(this.txtBraille_TextChanged);
            this.txtBraille.Validating += new System.ComponentModel.CancelEventHandler(this.txtBraille_Validating);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(69, 194);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 33);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "確定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(150, 194);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 33);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnPickBraille
            // 
            this.btnPickBraille.Image = global::EasyBrailleEdit.Properties.Resources.PickBraille;
            this.btnPickBraille.Location = new System.Drawing.Point(240, 119);
            this.btnPickBraille.Name = "btnPickBraille";
            this.btnPickBraille.Size = new System.Drawing.Size(32, 36);
            this.btnPickBraille.TabIndex = 6;
            this.btnPickBraille.UseVisualStyleBackColor = false;
            this.btnPickBraille.Click += new System.EventHandler(this.btnPickBraille_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // EditCellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(303, 253);
            this.Controls.Add(this.btnPickBraille);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtBraille);
            this.Controls.Add(this.cboPhCode);
            this.Controls.Add(this.txtChar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditCellForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改點字";
            this.Load += new System.EventHandler(this.EditCellForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EditCellForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtChar;
        private System.Windows.Forms.ComboBox cboPhCode;
        private System.Windows.Forms.TextBox txtBraille;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPickBraille;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}