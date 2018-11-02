using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //preparing
            Product p1 = new Product { ProductID = 1, Name = "jeden" };
            Product p2 = new Product { ProductID = 2, Name = "dwa" };

            //new cart
            Cart target = new Cart();

            //working
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Exisiting_Lines()
        {
            //preparing
            Product p1 = new Product { ProductID = 1, Name = "jeden" };
            Product p2 = new Product { ProductID = 2, Name = "dwa" };

            //new cart
            Cart target = new Cart();

            //working
            target.AddItem(p1, 1);
            target.AddItem(p2, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[1].Quantity, 10);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //preparing
            Product p1 = new Product { ProductID = 1, Name = "jeden" };
            Product p2 = new Product { ProductID = 2, Name = "dwa" };

            //new cart
            Cart target = new Cart();

            //working
            target.AddItem(p1, 1);
            target.AddItem(p2, 10);
            target.RemoveLine(p1);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assert
            Assert.AreEqual(target.Lines.Where( p => p.Product == p1).Count(), 0 );
        }

        [TestMethod]
        public void Can_Clear_Cart()
        {
            //preparing
            Product p1 = new Product { ProductID = 1, Name = "jeden" };
            Product p2 = new Product { ProductID = 2, Name = "dwa" };

            //new cart
            Cart target = new Cart();

            //working
            target.AddItem(p1, 1);
            target.AddItem(p2, 10);

            //cleaning cart
            target.Clear();
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        { 
        
            //preparing - create the repo
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID = 1, Name = "P1", Category = "Apples"},
            }.AsQueryable());

            //preparing - create the cart
            Cart cart = new Cart();

            //preparing - create the controller
            CartController target = new CartController(mock.Object);

            //working - add product to the cart
            target.AddToCart(cart, 1 ,null);

            //asseration
            Assert.AreEqual(cart.Lines.Count(),1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID,1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { new Product { ProductID = 1, Name = "P1", Category = "Apples" }}.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object);

            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            Cart cart = new Cart();

            CartController target = new CartController(null);

            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual(result.Cart , cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

    }
}
