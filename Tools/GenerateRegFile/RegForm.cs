using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using LicenseKeyBuilder;

namespace GenerateRegFile
{
    public partial class RegForm : Form
    {
        private const string PublicKey =
            @"<RSAKeyValue><Modulus>ruaEYJRk8DHvjyEzNuNZ9YNHdubbGJeWFc9SZeG3LvKegHOXg/f3cn88RjeRNNfPGbs/2byN7iCkBXp5Io6Ie4olNjEwyb3feJ4JVkjdJjsMraR7qFWrVweOA805ttnVhK8htEbCaiiHJZS0k93u+N5AAaNAgJPRd9dvt3DZAck=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string PrivateKey =
            @"<RSAKeyValue><Modulus>ruaEYJRk8DHvjyEzNuNZ9YNHdubbGJeWFc9SZeG3LvKegHOXg/f3cn88RjeRNNfPGbs/2byN7iCkBXp5Io6Ie4olNjEwyb3feJ4JVkjdJjsMraR7qFWrVweOA805ttnVhK8htEbCaiiHJZS0k93u+N5AAaNAgJPRd9dvt3DZAck=</Modulus><Exponent>AQAB</Exponent><P>7YYsCzVPnYjJsDeUr2W3BfW49NQxDHSGJS+S6fk1XjXrUToeiRi98cWz21cFwQ2uGUewnHDf5Ai1kQoj4DpdFQ==</P><Q>vIFRSgu46L+YSAyVYHxcIevJ8VfEFWEaXwI6ZeZFORbZGM4bHNf6OpyRHXwz+eHJTfHbOs7nulb7SglB9nBG5Q==</Q><DP>Ete1AMKe6UFjtp4CJpPsHAXtQUbgCTvpNGP8xKDncezXGR+CMqAK2XY0mW7i3vjF2e2M/iwaJs3mEXZ7eBtJLQ==</DP><DQ>Duwdx0LOTH5+S5XTbWPq1zSXM+fCuf6J2+ONQ+vKpw5L+U63jrz6LhGe2zqt0qYxDV6MAEfIyFOCaQX6lsukkQ==</DQ><InverseQ>fCRx8Usu4ZBzq8f4HsPdTrtdsqfztoA6c4VY6gEb+OISd7OARj5ICqLMhV0j8BjGy6Z3aO+PD/aoMFM0g5LoxQ==</InverseQ><D>TPHdCyA9x+4wFiflACDFUt2OcyDdAtStkqrC9U9354+Va61u2wAcPKL9QWbw2u6Wjhty27e4OSri/gYDhNBv+i4+5TTdJYPvVGdZdzsaOioUs5lGNvB29PSVfONo71nWD8BWlTxrBIKJNsddPOpovew/S1OUfcp9Y61icAQEogE=</D></RSAKeyValue>";

        private string _productId;       

        public RegForm()
        {
            InitializeComponent();
        }

        private void RegForm_Load(object sender, EventArgs e)
        {
            _productId = ConfigurationManager.AppSettings["ProductID"];
            if (String.IsNullOrEmpty(_productId))
            {
                MsgBoxHelper.ShowError("應用程式組態檔中沒有指定 ProductID。");
                Close();
            }
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = txtCustomerName.Text.Trim();
            txtContactName.Text = txtContactName.Text.Trim();
            txtEmail.Text = txtEmail.Text.Trim();
            txtTel.Text = txtTel.Text.Trim();
            txtAddress.Text = txtAddress.Text.Trim();

            if (string.IsNullOrEmpty(txtCustomerName.Text))
            {
                MsgBoxHelper.ShowInfo("請輸入公司／組織名稱!");
                txtCustomerName.Focus();
                return;
            }

            string licenseKey = txtLicenseKey.Text.Trim().Replace("-", "");


            // 尚未註冊過
            var regData = new UserRegData();
            regData.ProductID = _productId;
            regData.LicenseKey = licenseKey;
            regData.CustomerName = txtCustomerName.Text;
            regData.ContactName = txtContactName.Text;
            regData.Email = txtEmail.Text;
            regData.Tel = txtTel.Text;
            regData.Address = txtAddress.Text;
            regData.IPAddr = IPAddress.Parse("127.0.0.1");
            regData.VersionName = cboVersionName.Text;
            regData.ExpiredDate = dateExpired.Value;

            // 加密並簽章。
            var regCrypto = new UserRegCrypto(PublicKey, PrivateKey);
            var encryptedRegData = regCrypto.EncryptAndSign(regData);

            SaveRegData(txtFileName.Text, encryptedRegData);

            MsgBoxHelper.ShowInfo("成功!");
        }

        private void SaveRegData(string pathFileName, string text)
        {
            using (StreamWriter sw = new StreamWriter(pathFileName, false, Encoding.Default))
            {
                sw.Write(text);
                sw.Flush();
                sw.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtEmail_Enter(object sender, EventArgs e)
        {
            if (txtEmail.Text.IndexOf("電子郵件") >= 0)
            {
                txtEmail.Text = "";
            }
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (txtEmail.Text.Trim().Length == 0)
            {
                txtEmail.Text = "請填寫正確的電子郵件位址";
            }
        }


        private void btnGenKey_Click(object sender, EventArgs e)
        {
            var keyGen = new LicenseKeyGenerator();

            keyGen.LicenseTemplate = "xxxx-xxxx-xxxx-xxxx";
            keyGen.MaxTokens = 0;
            keyGen.UseBase10 = false;
            keyGen.UseBytes = true;

            keyGen.CreateKey();
            txtLicenseKey.Text = keyGen.GetLicenseKey();
        }

        private void btnDecryptRegFile_Click(object sender, EventArgs e)
        {
            var crypto = new UserRegCrypto(PublicKey, PrivateKey);
            var regData = crypto.DecryptRegDataFile(txtFileName.Text);

            try
            {
                cboVersionName.Text = regData.VersionName;
            }
            catch
            {
                cboVersionName.SelectedIndex = -1;
            }
            
            txtLicenseKey.Text = regData.LicenseKey;
            dateExpired.Value = regData.ExpiredDate;
            txtCustomerName.Text = regData.CustomerName;
            txtContactName.Text = regData.ContactName;
            txtEmail.Text = regData.Email;
            txtTel.Text = regData.Tel;
            txtAddress.Text = regData.Address;
            
            MsgBoxHelper.ShowInfo("註冊檔案反解成功，已將註冊資訊填入各欄位。");
        }
    }
}