using System;
using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;

        public OrderService(IOrderRepository orderRepository, IProductService productService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
        }

        public IList<Order> List(int firstResult, int maxResults)
        {
            return _orderRepository.List(firstResult, maxResults);
        }

        public Order Get(int id)
        {
            return _orderRepository.Get(id);
        }

        public void Place(Order entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var products = entity.LineItems;
            decimal total = 0;
            foreach (var item in products)
            {
                var product = _productService.Get(item.ProductId);
                item.Price = product.Price;
                total += item.Quantity * item.Price;
                if (item.Quantity > product.Quantity) item.Quantity = product.Quantity;
                product.Quantity = product.Quantity - item.Quantity;
                _productService.Update(product);
            }
            entity.Total = total;
            _orderRepository.Create(entity);
        }
    }
}
