using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShoppingCart.Business.Tests
{
    [TestClass]
    public class CryptoEngineTest
    {
        [TestMethod]
        public void can_encrypt_and_decrypt_card()
        {
            var r = new Random();
            var notEncryptedCard = r.Next(20, int.MaxValue);
            var encryptedCard = CryptoEngine.Encrypt(notEncryptedCard);
            Assert.AreNotEqual(notEncryptedCard,encryptedCard);
            var decodeCard = CryptoEngine.Decrypt(encryptedCard);
            Assert.AreEqual(notEncryptedCard,decodeCard);
        }
    }
}
