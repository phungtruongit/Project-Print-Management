namespace VCSLib.HMAC {
    public interface IHashCodeHMAC {
        public string ComputeHashCodeHMACSHA1(string content, string nonce, string timeStamp, string secretKey = "");
        public string ComputeHashCodeHMACSHA256(string content, string nonce, string timeStamp, string secretKey = "");
        public string ComputeSignatureHMACSHA256(string content, string userId,string machineName, string secretKey = "");
    }
}
