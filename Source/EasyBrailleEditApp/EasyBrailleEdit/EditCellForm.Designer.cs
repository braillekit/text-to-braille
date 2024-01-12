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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditCellForm));
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            txtChar = new System.Windows.Forms.TextBox();
            cboPhCode = new System.Windows.Forms.ComboBox();
            txtBraille = new System.Windows.Forms.TextBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnPickBraille = new System.Windows.Forms.Button();
            errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(37, 29);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(54, 19);
            label1.TabIndex = 0;
            label1.Text = "明眼字";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(52, 121);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 19);
            label2.TabIndex = 4;
            label2.Text = "點字";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(37, 71);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(54, 19);
            label3.TabIndex = 2;
            label3.Text = "注音碼";
            // 
            // txtChar
            // 
            txtChar.Location = new System.Drawing.Point(98, 26);
            txtChar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtChar.MaxLength = 1;
            txtChar.Name = "txtChar";
            txtChar.Size = new System.Drawing.Size(78, 27);
            txtChar.TabIndex = 1;
            toolTip1.SetToolTip(txtChar, "只能輸入一個字元，若輸入空白字元則為空方。");
            txtChar.TextChanged += txtChar_TextChanged;
            txtChar.Validating += txtChar_Validating;
            // 
            // cboPhCode
            // 
            cboPhCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboPhCode.FormattingEnabled = true;
            cboPhCode.Location = new System.Drawing.Point(98, 68);
            cboPhCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboPhCode.Name = "cboPhCode";
            cboPhCode.Size = new System.Drawing.Size(149, 27);
            cboPhCode.TabIndex = 3;
            cboPhCode.SelectedIndexChanged += cboPhCode_SelectedIndexChanged;
            // 
            // txtBraille
            // 
            txtBraille.Font = new System.Drawing.Font("SimBraille", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtBraille.Location = new System.Drawing.Point(98, 110);
            txtBraille.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtBraille.MaxLength = 3;
            txtBraille.Name = "txtBraille";
            txtBraille.Size = new System.Drawing.Size(149, 39);
            txtBraille.TabIndex = 5;
            txtBraille.TextChanged += txtBraille_TextChanged;
            txtBraille.Validating += txtBraille_Validating;
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(78, 194);
            btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(84, 42);
            btnOk.TabIndex = 7;
            btnOk.Text = "確定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(168, 194);
            btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(84, 42);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnPickBraille
            // 
            btnPickBraille.Image = (System.Drawing.Image)resources.GetObject("btnPickBraille.Image");
            btnPickBraille.Location = new System.Drawing.Point(253, 107);
            btnPickBraille.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnPickBraille.Name = "btnPickBraille";
            btnPickBraille.Size = new System.Drawing.Size(36, 46);
            btnPickBraille.TabIndex = 6;
            btnPickBraille.UseVisualStyleBackColor = false;
            btnPickBraille.Click += btnPickBraille_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // EditCellForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(325, 252);
            Controls.Add(btnPickBraille);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtBraille);
            Controls.Add(cboPhCode);
            Controls.Add(txtChar);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditCellForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "修改點字";
            Load += EditCellForm_Load;
            KeyUp += EditCellForm_KeyUp;
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
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