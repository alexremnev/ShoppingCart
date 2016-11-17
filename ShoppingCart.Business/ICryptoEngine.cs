namespace ShoppingCart.Business
{
    public interface ICryptoEngine
    {
        /// <summary>
        /// Encrypt text.
        /// </summary>
        /// <param name="text">unencrypted text.</param>
        /// <returns>Encrypted text.</returns>
        string Encrypt(string text);
        /// <summary>
        /// Decrypt text.
        /// </summary>
        /// <param name="cipherText">cipher text.</param>
        /// <returns>Decrypted text.</returns>
        string Decrypt(string cipherText);
    }
}
