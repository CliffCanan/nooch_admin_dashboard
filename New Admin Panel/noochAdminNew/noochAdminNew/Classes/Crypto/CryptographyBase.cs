
namespace noochAdminNew.Classes.Crypto
{
    public abstract class CryptographyBase
    {
        public abstract string Encrypt(string plainString, string cryptographyKey);

        public abstract string Decrypt(string encryptedString, string cryptographyKey);
    }
}