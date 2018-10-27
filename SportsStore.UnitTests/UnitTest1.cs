using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using Moq;
using System.Linq;
using SportsStore.WebUI.Models;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using SportsStore.WebUI.HtmlHelpers;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //preparing
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1 , Name = "p1"},
                new Product {ProductID = 2 , Name = "p2"},
                new Product {ProductID = 3 , Name = "p3"},
                new Product {ProductID = 4 , Name = "p4"},
                new Product {ProductID = 5 , Name = "p5"},
                new Product {ProductID = 6 , Name = "p6"}
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //working
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //assert

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Link()
        {
            //preparing
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            //working
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //asert
            Assert.AreEqual(result.ToString(), @"< href=""Strona1"">1</a>"
                + @"< href=""Strona2"">2</a>"
                + @"< href=""Strona3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagionation_View_Model()
        {
            //preparing
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1 , Name = "p1"},
                new Product {ProductID = 2 , Name = "p2"},
                new Product {ProductID = 3 , Name = "p3"},
                new Product {ProductID = 4 , Name = "p4"},
                new Product {ProductID = 5 , Name = "p5"},
                new Product {ProductID = 6 , Name = "p6"}
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //working
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;


            //asert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.CurrentPage, 3);
            Assert.AreEqual(pageInfo.CurrentPage, 4);
            Assert.AreEqual(pageInfo.CurrentPage, 6);
            Assert.AreEqual(pageInfo.CurrentPage, 5);
        }

    }
}

