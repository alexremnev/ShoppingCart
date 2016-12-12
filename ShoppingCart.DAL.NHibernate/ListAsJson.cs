using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using ShoppingCart.Business;

namespace ShoppingCart.DAL.NHibernate
{
    public class ListAsJson : IUserType
    {
        private readonly IJsonService _jsonService;
        public ListAsJson(IJsonService jsonService)
        {
            _jsonService = jsonService;
        }

        bool IUserType.Equals(object x, object y)
        {
            return Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                ((IDataParameter)cmd.Parameters[index]).Value = _jsonService.Serialize(value);
            }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var json = (string)rs[names[0]];

            return _jsonService.Desirialize(json);
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

        public SqlType[] SqlTypes => new SqlType[] { new AnsiStringSqlType() };
        public Type ReturnedType => typeof(string);

        public bool IsMutable => false;
    }
}
