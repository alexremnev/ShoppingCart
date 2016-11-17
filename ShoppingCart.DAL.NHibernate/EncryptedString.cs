using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using ShoppingCart.Business;
using Spring.Context.Support;

namespace ShoppingCart.DAL.NHibernate
{
    public class EncryptedString : IUserType
    {
        private static readonly ICryptoEngine CryptoEngine = new XmlApplicationContext("config://spring/objects", "assembly://ShoppingCart.DAL.NHibernate/ShoppingCart.DAL.NHibernate/config.xml").GetObject<CryptoEngine>();
        bool IUserType.Equals(object x, object y)
        {
            return Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var r = rs[names[0]];
            if (r == DBNull.Value)
                return null;
            var r1 = (string)rs[names[0]];
            return CryptoEngine.Decrypt(r1);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null) ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;

            else
            {
                var encryptedString = CryptoEngine.Encrypt((string)value);
                ((IDataParameter)cmd.Parameters[index]).Value = encryptedString;
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes => new SqlType[] { new StringSqlType() };

        public Type ReturnedType => typeof(string);

        public bool IsMutable => false;
    }
}
