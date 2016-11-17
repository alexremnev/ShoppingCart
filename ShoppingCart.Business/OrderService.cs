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

        public IList<Order> List(string filter, string sortby, bool isAscending, int firstResult, int maxResults)
        {
            return _orderRepository.List(null, null, true, firstResult, maxResults);
        }

        public Order Get(int id)
        {
            return _orderRepository.Get(id);
        }

        public void Place(Order entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            PrepateEntity(entity);
            if (!entity.SaleDate.HasValue) entity.SaleDate = DateTime.Now;
            _orderRepository.Create(entity);
        }

        public void Update(Order entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            PrepateEntity(entity);
            _orderRepository.Update(entity);
        }

        public int Count(string filter, decimal maxPrice)
        {
            return _orderRepository.Count();
        }

        private void PrepateEntity(Order entity)
        {
            var products = entity.LineItems;
            decimal total = 0;
            foreach (var item in products)
            {
                var product = _productService.Get(item.ProductId);
                item.Name = product.Name;
                item.Price = product.Price;
                total += item.Quantity * item.Price;
                if (item.Quantity > product.Quantity) item.Quantity = product.Quantity;
                product.Quantity = product.Quantity - item.Quantity;
                _productService.Update(product);
            }
            entity.Total = total;
        }

        public void ReturnProducts(IList<LineItem> products)
        {
            foreach (var product in products)
            {
                var existPoduct = _productService.Get(product.ProductId);
                existPoduct.Quantity = existPoduct.Quantity + product.Quantity;
                _productService.Update(existPoduct);
            }
        }
    }
}
