# ShoppingCart (Restful Api)

Building a Restful Api with ASP.NET MVC 5 and NHibernate 4.

Requirements:
Visual Studio 2015
MySql 5

Project deployment guide:
1. Visual studio -> View -> Team Explorer -> Clone -> Manage Connection -> Enter the URL: https://github.com/alexremnev/ShoppingCart.git
2.Enter the URL: https://{server}/products and the list of products will be displayed in json format.
{server} - name of server or localhost. You can sort products, add the filter, and сhoose appropriate paging .(for example if you enter https://{server}/products?filter=Car&sortby=Price&page=1&pageSize=10 you will get the second page of cars list sorted by  price)
3. Also you can add new products or delete the product by id.

