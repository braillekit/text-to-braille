using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Huanlin.Collections;
using Huanlin.Common.Cryptography;
using Huanlin.Common.Helpers;

namespace GenerateRegFile
{
    public class UserRegCrypto
    {
        private string m_PublicKey;
        private string m_PrivateKey;

        public UserRegCrypto()
        {
        }

        public UserRegCrypto(string publicKey, string privateKey)
        {
            m_PublicKey = publicKey;
            m_PrivateKey = privateKey;
        }

        /// <summary>
        /// 將使用者註冊資訊加密並簽章。
        /// </summary>
        /// <param name="regData"></param>
        /// <returns></returns>
        public string EncryptAndSign(UserRegData regData)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);
            rsa.FromXmlString(m_PrivateKey);
            /*
             * Note: 若呼叫 FromXmlString 時出現找不到檔案的錯誤，可以檢查看看以下資料夾的存取權限：
             *   C:\Documents and Settings\All Users\Application data\Microsoft\Crypto\RSA\MachineKeys
             * 或者 (Windows 7）：
             *   c:\ProgramData\Microsoft\Crypto\RSA\MachineKeys\
             * 應用程式的執行帳戶（例如 NETWORK SERVICE）要能對此目錄有存取權限。
             */

            string data = regData.ToString();
            string encrypted = EncryptRegData(data, regData.IPAddr);
            string signature = HashAndSign(data, m_PrivateKey);

            ByteArray signatureData = new ByteArray(Convert.FromBase64String(signature));
            rsa.FromXmlString(m_PublicKey);
            bool ok = rsa.VerifyData(Encoding.Default.GetBytes(data), new SHA1CryptoServiceProvider(), signatureData.Bytes);
            if (ok)
            {
                // 傳回公鑰、簽章、密文（包括密鑰、初始化向量、密文資料）。
                return m_PublicKey + ";" + signature + ";" + encrypted;
            }
            return "";
        }

        /// <summary>
        /// 使用對稱式加密方法將輸入的資料加密。
        /// </summary>
        /// <param name="regData"></param>
        /// <param name="ipAddr"></param>
        /// <returns>以 Base64 編碼的字串，且分成三段，第一段為密鑰，第二段為初始化向量，第三段為密文。各段之間以分號 (;) 區隔。</returns>
        private string EncryptRegData(string regData, IPAddress ipAddr)
        {
            string ipStr = ipAddr.ToString();
            byte[] key = ConvertHelper.StringToBytes(ipStr.Substring(0, 6)); // 取 IP 位址前六碼當作 key。
            byte[] data = ConvertHelper.StringToBytes(regData);

            SymmetricCrypto crypto = new SymmetricCrypto(SymmetricCrypto.Provider.Rijndael, false);
            byte[] encryptedData = crypto.Encrypt(data, key);

            StringBuilder sb = new StringBuilder();
            sb.Append(crypto.Key.Base64String);
            sb.Append(';');
            sb.Append(crypto.IntializationVector.Base64String);
            sb.Append(';');
            sb.Append(Convert.ToBase64String(encryptedData));

            return sb.ToString();
        }

        /// <summary>
        /// 產生簽章。
        /// </summary>
        /// <param name="data">資料。</param>
        /// <param name="privateKey">私鑰。</param>
        /// <returns>Base64 編碼的簽章字串。</returns>
        private string HashAndSign(string data, string privateKey)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);

            rsa.FromXmlString(privateKey);

            byte[] orgData = Encoding.Default.GetBytes(data);
            byte[] signature = rsa.SignData(orgData, new SHA1CryptoServiceProvider());

            return Convert.ToBase64String(signature);
        }


        /// <summary>
        /// 檢查 IP 位址是否合法。
        /// </summary>
        /// <param name="ipAddr"></param>
        private void CheckIPAddress(string ipAddr)
        {
            if (String.IsNullOrEmpty(ipAddr) || ipAddr.Length < 7)
            {
                throw new ArgumentException("參數 ipAddr 無效!");
            }

            int cnt = 0;
            for (int i = 0; i < ipAddr.Length; i++)
            {
                if (ipAddr[i] == '.')
                    cnt++;
            }
            if (cnt < 3)
            {
                throw new ArgumentException("參數 ipAddr 無效!");
            }
        }

        /// <summary>
        /// 檢查使用者註冊資訊檔是否遭到竄改。
        /// </summary>
        /// <returns></returns>
        public UserRegData DecryptRegDataFile(string filename)
        {
            if (!File.Exists(filename))
                return null;

            string regText;

            using (StreamReader sr = new StreamReader(filename, Encoding.Default))
            {
                regText = sr.ReadToEnd();
                sr.Close();
            }

            return DecryptRegData(regText);
        }

        public UserRegData DecryptRegData(string regText)
        {
            string[] parts = regText.Split(';');

			if (parts.Length < 5)
			{
				return null;
			}

            // 0: Public key (XML)
            // 1: 數位簽章 (Base64)
            // 2: 對稱式加密金鑰 (Base64)
            // 3: 初始化向量 (Base64)
            // 4: 密文 (Base64)

            // 解密
            SymmetricCrypto crypto = new SymmetricCrypto(SymmetricCrypto.Provider.Rijndael, false);
            byte[] key = Convert.FromBase64String(parts[2]);
            crypto.IntializationVector = new ByteArray(Convert.FromBase64String(parts[3]));
            byte[] decryptedData = crypto.Decrypt(Convert.FromBase64String(parts[4]), key);

            // 驗證簽章
            ByteArray signature = new ByteArray(Convert.FromBase64String(parts[1]));

            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);
            rsa.FromXmlString(parts[0]);
            bool valid = rsa.VerifyData(decryptedData, new SHA1CryptoServiceProvider(), signature.Bytes);

            if (!valid)
            {
                return null;
            }

            string decryptedText = Encoding.Default.GetString(decryptedData);
            UserRegData regData = UserRegData.Parse(decryptedText);

            return regData;
        }
    }
}
