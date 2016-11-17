using ShoppingCart.WebApi;

namespace ShoppingCart.BondingLayer
{
    public class Connector : IConnector
    {
        private readonly ISecurityContext _context;
        public Connector(ISecurityContext context)
        {
            _context = context;
        }
    }
}
