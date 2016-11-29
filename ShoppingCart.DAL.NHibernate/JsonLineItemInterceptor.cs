using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class JsonLineItemInterceptor : EmptyInterceptor
    {
        private const string Property = "JsonLineItems";

        public override int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
             IType[] types)
        {
            if (!(entity is IChangeable))
                return base.FindDirty(entity, id, currentState, previousState, propertyNames, types);
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] != Property) continue;
                currentState[i] = currentState[i + 1];
                break;
            }
            return base.FindDirty(entity, id, currentState, previousState, propertyNames, types);
        }
    }
}

