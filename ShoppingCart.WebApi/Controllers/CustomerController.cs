﻿using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;

namespace ShoppingCart.WebApi.Controllers
{
    public class CustomerController : BaseController<Customer>
    {
        private const string Controller = "customer";
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private static readonly ILog Log = LogManager.GetLogger<CustomerController>();

        // GET api/customer
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public new IHttpActionResult List(int? page = null, int? pageSize = null)
        {
            try
            {
                return base.List(page, pageSize);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of customers", e);
                return InternalServerError();
            }
        }

        //POST api/customer
        [HttpPost]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Create([FromBody] Customer entity)
        {
            try
            {
                if (entity == null) return BadRequest("Entity is not valid");
                if (string.IsNullOrEmpty(entity.Name)) return BadRequest("Name is empty");
                var email = entity.Email;
                if (string.IsNullOrEmpty(email)) return BadRequest("Email is empty");
                if (entity.Card < 0) return BadRequest("Card is not valid");
                if (!IsValidEmail(email)) return BadRequest("Email is not correct");
                _customerService.Create(entity);
                return CreatedAtRoute(WebApiConfig.DefaultRoute, new { controller = Controller, id = entity.Id }, entity.Id);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to add a new customer", e);
                return InternalServerError();
            }
        }

        //PUT api/customer
        [HttpPut]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Update([FromBody] Customer customer)
        {
            try
            {
                if (customer == null) return BadRequest("Entity is not valid");
                var customerId = customer.Id;
                if (customerId < 0) return BadRequest("Id is not valid");
                if (string.IsNullOrEmpty(customer.Name)) return BadRequest("Name is empty");
                if (customer.Card < 0) return BadRequest("Card is not valid");
                var oldCustomer = _customerService.Get(customerId);
                if (oldCustomer == null) return BadRequest($"Product with id={customerId} not exist");
                if (!IsValidEmail(customer.Email)) return BadRequest("Email is not correct");
                oldCustomer.Name = customer.Name;
                oldCustomer.Email = customer.Email;
                oldCustomer.Card = customer.Card;
                _customerService.Update(oldCustomer);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to update a customer", e);
                return InternalServerError();
            }
        }

        // GET api/customer/id
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                return Get(id);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the customer by id={id}", e);
                return InternalServerError();
            }
        }
        private bool IsValidEmail(string email)
        {
            var pattern = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
            var r = new Regex(pattern, RegexOptions.IgnoreCase);
            var matched = r.Match(email).Success;
            return matched;
        }

        public override IService<Customer> GetService()
        {
            return _customerService;
        }
    }
}

