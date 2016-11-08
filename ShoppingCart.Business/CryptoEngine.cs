namespace ShoppingCart.Business
{
   public class CryptoEngine
    {
        public static long Encrypt(long decryptedCard)
        {
            return (decryptedCard * 2);
        }

        public static long Decrypt(long encryptedCard)
        {
            return (encryptedCard / 2);
        }
    }
}
