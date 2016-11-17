using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace ShoppingCart.DAL.NHibernate
{
    public class DateTimeAsLong : IUserType
    {
        bool IUserType.Equals(object x, object y)
        {
            return Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x?.GetHashCode() ?? 0;
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

        public SqlType[] SqlTypes => new[] { SqlTypeFactory.Int64 };

        public Type ReturnedType => typeof(DateTime);

        public bool IsMutable => false;

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                var dateTimeValue = (DateTime)value;
                ((IDataParameter)cmd.Parameters[index]).Value = dateTimeValue.Ticks;
            }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var r = (long)rs[names[0]];
            return new DateTime(r);
        }
    }
}
