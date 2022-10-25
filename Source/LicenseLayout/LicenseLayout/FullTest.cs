using System;
using System.Windows.Forms;
using LicenseKeyBuilder;

namespace LicenseLayout
{
    /// <summary>
    /// Summary description for FullTest.
    /// </summary>
    public class FullTest : System.Windows.Forms.Form
    {
        string [] hstr = {"Character", "Type", "Initial Value", "Decoded Value"};

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmboType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtIValue;
        private System.Windows.Forms.TextBox txtKeyInput;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdobtnBytes;
        private System.Windows.Forms.RadioButton rdobtnBits;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdobtn16;
        private System.Windows.Forms.RadioButton rdobtn10;
        private System.Windows.Forms.TextBox txtLicKey;
        private System.Windows.Forms.Button btnEncode;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdoBtnChksum2;
        private System.Windows.Forms.RadioButton rdoBtnChksum1;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public FullTest()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIValue = new System.Windows.Forms.TextBox();
            this.txtChar = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmboType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnEncode = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtKeyInput = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLicKey = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdobtnBytes = new System.Windows.Forms.RadioButton();
            this.rdobtnBits = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdobtn16 = new System.Windows.Forms.RadioButton();
            this.rdobtn10 = new System.Windows.Forms.RadioButton();
            this.btnDecode = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdoBtnChksum2 = new System.Windows.Forms.RadioButton();
            this.rdoBtnChksum1 = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(26, 123);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(435, 283);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(26, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(691, 24);
            this.label1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(26, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(473, 49);
            this.label2.TabIndex = 3;
            // 
            // txtIValue
            // 
            this.txtIValue.Location = new System.Drawing.Point(666, 197);
            this.txtIValue.Name = "txtIValue";
            this.txtIValue.Size = new System.Drawing.Size(204, 27);
            this.txtIValue.TabIndex = 12;
            // 
            // txtChar
            // 
            this.txtChar.Location = new System.Drawing.Point(666, 98);
            this.txtChar.Name = "txtChar";
            this.txtChar.Size = new System.Drawing.Size(204, 27);
            this.txtChar.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(525, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 25);
            this.label3.TabIndex = 11;
            this.label3.Text = "Initial Value";
            // 
            // cmboType
            // 
            this.cmboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboType.Location = new System.Drawing.Point(666, 148);
            this.cmboType.Name = "cmboType";
            this.cmboType.Size = new System.Drawing.Size(204, 27);
            this.cmboType.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(525, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 24);
            this.label4.TabIndex = 9;
            this.label4.Text = "Type";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(525, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 24);
            this.label5.TabIndex = 7;
            this.label5.Text = "Character";
            // 
            // btnEncode
            // 
            this.btnEncode.Location = new System.Drawing.Point(294, 615);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(128, 37);
            this.btnEncode.TabIndex = 13;
            this.btnEncode.Text = "Encode Key";
            this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(614, 615);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 37);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(653, 246);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(89, 37);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtKeyInput
            // 
            this.txtKeyInput.Location = new System.Drawing.Point(26, 468);
            this.txtKeyInput.Name = "txtKeyInput";
            this.txtKeyInput.Size = new System.Drawing.Size(883, 27);
            this.txtKeyInput.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(13, 431);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(141, 24);
            this.label6.TabIndex = 16;
            this.label6.Text = "Key Template";
            // 
            // txtLicKey
            // 
            this.txtLicKey.Location = new System.Drawing.Point(26, 554);
            this.txtLicKey.Name = "txtLicKey";
            this.txtLicKey.Size = new System.Drawing.Size(883, 27);
            this.txtLicKey.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(13, 517);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "Key";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdobtnBytes);
            this.groupBox2.Controls.Add(this.rdobtnBits);
            this.groupBox2.Location = new System.Drawing.Point(602, 308);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(128, 110);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Size";
            // 
            // rdobtnBytes
            // 
            this.rdobtnBytes.Location = new System.Drawing.Point(26, 25);
            this.rdobtnBytes.Name = "rdobtnBytes";
            this.rdobtnBytes.Size = new System.Drawing.Size(89, 24);
            this.rdobtnBytes.TabIndex = 1;
            this.rdobtnBytes.Text = "Bytes";
            // 
            // rdobtnBits
            // 
            this.rdobtnBits.Location = new System.Drawing.Point(26, 62);
            this.rdobtnBits.Name = "rdobtnBits";
            this.rdobtnBits.Size = new System.Drawing.Size(76, 24);
            this.rdobtnBits.TabIndex = 0;
            this.rdobtnBits.Text = "Bits";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdobtn16);
            this.groupBox1.Controls.Add(this.rdobtn10);
            this.groupBox1.Location = new System.Drawing.Point(486, 308);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(103, 110);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base";
            // 
            // rdobtn16
            // 
            this.rdobtn16.Location = new System.Drawing.Point(13, 62);
            this.rdobtn16.Name = "rdobtn16";
            this.rdobtn16.Size = new System.Drawing.Size(77, 24);
            this.rdobtn16.TabIndex = 1;
            this.rdobtn16.Text = "16";
            // 
            // rdobtn10
            // 
            this.rdobtn10.Location = new System.Drawing.Point(13, 25);
            this.rdobtn10.Name = "rdobtn10";
            this.rdobtn10.Size = new System.Drawing.Size(77, 24);
            this.rdobtn10.TabIndex = 0;
            this.rdobtn10.Text = "10";
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(448, 615);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(128, 37);
            this.btnDecode.TabIndex = 22;
            this.btnDecode.Text = "Decode Key";
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdoBtnChksum2);
            this.groupBox3.Controls.Add(this.rdoBtnChksum1);
            this.groupBox3.Location = new System.Drawing.Point(755, 308);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(141, 110);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Type";
            // 
            // rdoBtnChksum2
            // 
            this.rdoBtnChksum2.Location = new System.Drawing.Point(13, 62);
            this.rdoBtnChksum2.Name = "rdoBtnChksum2";
            this.rdoBtnChksum2.Size = new System.Drawing.Size(102, 24);
            this.rdoBtnChksum2.TabIndex = 1;
            this.rdoBtnChksum2.Text = "Type 2";
            // 
            // rdoBtnChksum1
            // 
            this.rdoBtnChksum1.Location = new System.Drawing.Point(13, 25);
            this.rdoBtnChksum1.Name = "rdoBtnChksum1";
            this.rdoBtnChksum1.Size = new System.Drawing.Size(102, 24);
            this.rdoBtnChksum1.TabIndex = 0;
            this.rdoBtnChksum1.Text = "Type 1";
            // 
            // FullTest
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 20);
            this.ClientSize = new System.Drawing.Size(968, 615);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnDecode);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLicKey);
            this.Controls.Add(this.txtKeyInput);
            this.Controls.Add(this.txtIValue);
            this.Controls.Add(this.txtChar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEncode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmboType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FullTest";
            this.ShowInTaskbar = false;
            this.Text = "FullTest";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    
        protected override void OnLoad(EventArgs e)
        {
            int toph;
            int i;

            base.OnLoad (e);

            label1.Text = "Define characters and initial values for template. ";
            label2.Text = "x = randon numbers\nc = Checksum";
            // show the gridlines, do a details view, and allow full row select
            // we do not want the user to sort the list
            listView1.GridLines = true;
            listView1.View = View.Details;
            listView1.FullRowSelect = false;
            listView1.Sorting = SortOrder.None;

            // get the length of the list of the column headers
            toph = hstr.Length;

            // for each column header add the header and align the text
            for (i = 0; i < toph; i++) 
            {
                ColumnHeader ch = new ColumnHeader();
                ch.Text = hstr[i];
                ch.TextAlign = HorizontalAlignment.Center;
                //ch.Width = -1
                //ListView1.Columns.Add(ch, -2, HorizontalAlignment.Center)
                listView1.Columns.Add(ch);
                //ch.Width = -2
            }

            // add some entries into our combo box. 
            cmboType.Items.Add("Numeric");
            cmboType.Items.Add("Date");
            cmboType.Items.Add("Character");

            // Initialize the radio buttons
            rdobtn10.Checked = true;
            rdobtnBytes.Checked = true;
            rdoBtnChksum1.Checked = true;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            string	fldivaluestr;
            int		fldivalueint;

            if ( txtChar.Text.Length == 0 ) 
            {
                MessageBox.Show("Enter a token Character", "Layout");
                txtChar.Focus();
                return;
            }
            if ( cmboType.Text.Length == 0 ) 
            {
                MessageBox.Show("Enter a token Type", "Layout");
                cmboType.Focus();
                return;
            }
            if ( txtIValue.Text.Length == 0 ) 
            {
                MessageBox.Show("Enter a field Length", "Layout");
                txtIValue.Focus();
                return;
            }
            fldivaluestr = txtIValue.Text;

            switch ( cmboType.Text ) 
            {
                case "Numeric":
                    try 
                    {
                        if (rdobtn10.Checked ) 
                        {
                            fldivalueint = int.Parse(fldivaluestr);
                        }
                        else 
                        {
                            fldivalueint = int.Parse(fldivaluestr,System.Globalization.NumberStyles.AllowHexSpecifier);
                        }
                    }
                    catch 
                    {
                        MessageBox.Show("Enter a Numeric field Length", "Layout");
                        txtIValue.Focus();
                        return;
                    }
                    break;
                case "Character":
                    break;
                case "Date":
                    break;
            }

            // create a new item for this token
            ListViewItem lvi = new ListViewItem(txtChar.Text);
            lvi.SubItems.Add(cmboType.Text);	// add the data type
            lvi.SubItems.Add(txtIValue.Text);	// add the initial value
            lvi.SubItems.Add("");				// add the decoded value
            listView1.Items.Add(lvi);
        }

        private void btnEncode_Click(object sender, System.EventArgs e)
        {
            LicenseKeyGenerator gkey;
            int		numtokens;
            int		lop;

            if ( txtKeyInput.Text.Length == 0 ) 
            {
                MessageBox.Show("Enter a Key Template", "Layout");
                txtKeyInput.Focus();
                return;
            }

            gkey = new LicenseKeyGenerator();

            // set the base type. 
            if ( rdobtn10.Checked ) 
            {
                // use base 10
                gkey.UseBase10 = true;
            }
            else 
            {
                // use base 16
                gkey.UseBase10 = false;
            }
                
            // set the token type. 
            if ( rdobtnBytes.Checked ) 
            {
                // use bytes
                gkey.UseBytes = true;
            }
            else 
            {
                // use bits
                gkey.UseBytes = false;
            }

                
            // Set the number of tokens. 
            numtokens = listView1.Items.Count;
            if ( numtokens > 0 ) 
            {
                gkey.MaxTokens = numtokens;
                lop = 0; 
                foreach (ListViewItem item in listView1.Items) 
                {
                    LicenseKeyGenerator.TokenTypes	ttypes;
                    string		tempstr;

                    tempstr = item.SubItems[1].Text;
                    ttypes = LicenseKeyGenerator.TokenTypes.NUMBER; // default junk
                    switch (tempstr) 
                    {
                        case "numeric":
                        case "Numeric":
                            ttypes = LicenseKeyGenerator.TokenTypes.NUMBER;
                            break;
                        case "date":
                        case "Date":
                            ttypes = LicenseKeyGenerator.TokenTypes.DATE;
                            break;
                        case "character":
                        case "Character":
                            ttypes = LicenseKeyGenerator.TokenTypes.CHARACTER;
                            break;
                        default:
                            MessageBox.Show("Illegal Token type in switch", "Generatekey");
                            break;
                    } 
                    
                    try 
                    {
                        gkey.AddToken(lop, item.SubItems[0].Text, ttypes, item.SubItems[2].Text);
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("Error is " + ex.InnerException);
                        btnAdd.Focus();
                        return;
                    }
                    lop++;
                }
                gkey.LicenseTemplate = txtKeyInput.Text;
                try 
                {
                    gkey.CreateKey();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Error exception is: " + ex.Message);
                    return;
                }
                txtLicKey.Text = gkey.GetLicenseKey();
            }
        }

        private void btnDecode_Click(object sender, System.EventArgs e)
        {
            LicenseKeyGenerator gkey;
            int		numtokens;
            int		lop;
            string	result;
            string	tokenvalue= "";


            if ( txtKeyInput.Text.Length == 0 ) 
            {
                MessageBox.Show("Enter a Key Template", "Layout");
                txtKeyInput.Focus();
                return;
            }

            gkey = new LicenseKeyGenerator();

            // set the base type. 
            if ( rdobtn10.Checked ) 
            {
                // use base 10
                gkey.UseBase10 = true;
            }
            else 
            {
                // use base 16
                gkey.UseBase10 = false;
            }
                
            // set the token type. 
            if ( rdobtnBytes.Checked ) 
            {
                // use bytes
                gkey.UseBytes = true;
            }
            else 
            {
                // use bits
                gkey.UseBytes = false;
            }

            // set the Checksum type. 
            if ( rdoBtnChksum1.Checked ) 
            {
                // use Algorithm 1
                gkey.ChecksumAlgorithm = Checksum.ChecksumType.Type1;
            }
            else 
            {
                // use Algorithm 2
                gkey.ChecksumAlgorithm = Checksum.ChecksumType.Type2;
            }

                
            // Set the number of tokens. 
            numtokens = listView1.Items.Count;
            if ( numtokens > 0 ) 
            {
                gkey.MaxTokens = numtokens;
                lop = 0; 
                foreach (ListViewItem item in listView1.Items) 
                {
                    LicenseKeyGenerator.TokenTypes ttypes;
                    string		tempstr;

                    tempstr = item.SubItems[1].Text;
                    ttypes = LicenseKeyGenerator.TokenTypes.NUMBER; // default junk
                    switch (tempstr) 
                    {
                        case "numeric":
                        case "Numeric":
                            ttypes = LicenseKeyGenerator.TokenTypes.NUMBER;
                            break;
                        case "date":
                        case "Date":
                            ttypes = LicenseKeyGenerator.TokenTypes.DATE;
                            break;
                        case "character":
                        case "Character":
                            ttypes = LicenseKeyGenerator.TokenTypes.CHARACTER;
                            break;
                        default:
                            MessageBox.Show("Illegal Token type in switch", "Generatekey");
                            break;
                    } 
                    try 
                    {
                        gkey.AddToken(lop, item.SubItems[0].Text, ttypes, item.SubItems[2].Text);
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("Error is " + ex.InnerException);
                        btnAdd.Focus();
                        return;
                    }
                    lop++;
                }
                gkey.LicenseTemplate = txtKeyInput.Text;
                // first create the key 
                try 
                {
                    gkey.CreateKey();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Error exception is: " + ex.Message);
                    return;
                }
                txtLicKey.Text = gkey.GetLicenseKey();
                foreach (ListViewItem item in listView1.Items) 
                {
                    tokenvalue = item.SubItems[0].Text;	
                    // now decode the key
                    try 
                    {
                        result = gkey.DisassembleKey(tokenvalue);
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("Error exception is: " + ex.Message);
                        return;
                    }
                    item.SubItems[3].Text = result;	
                }
            }
        }
    }
}
