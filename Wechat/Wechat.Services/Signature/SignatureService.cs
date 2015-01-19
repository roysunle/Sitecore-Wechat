using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Wechat.Services.Signature
{
    public class SignatureService
    {
        public static Boolean CheckSignature(string signature, string timestamp, string nonce)
        {
            const string token = "20141120";
            var tmpArr = new ArrayList() { token, timestamp, nonce };
            tmpArr.Sort();
            var tmpStr = string.Join("", tmpArr.ToArray());
            //Log.Info("tmpStr1:" + tmpStr, typeof(SignatureService));
            SHA1 sha = new SHA1CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(tmpStr);
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            tmpStr = BitConverter.ToString(dataHashed).Replace("-", "");
            //Log.Info("tmpStr2:" + tmpStr, typeof(SignatureService));
            return tmpStr == signature;
        }
    }
}
