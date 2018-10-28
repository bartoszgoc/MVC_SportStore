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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 3);
            Assert.AreEqual(prodArray[0].Name, "p4");
            Assert.AreEqual(prodArray[1].Name, "p5");
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
            Assert.AreEqual(result.ToString(), @"<a href=""Strona1"">1</a>" + @"<a class=""selected"" href=""Strona2"">2</a>" + @"<a href=""Strona3"">3</a>");
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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;


            //asert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1 , Name = "p1", Category = "cat1"},
                new Product {ProductID = 2 , Name = "p2", Category = "cat2"},
                new Product {ProductID = 3 , Name = "p3", Category = "cat2"},
                new Product {ProductID = 4 , Name = "p4", Category = "cat4"},
                new Product {ProductID = 5 , Name = "p5", Category = "cat5"},
                new Product {ProductID = 6 , Name = "p6", Category = "cat6"}
            }.AsQueryable());

            //preparing
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //working
            Product[] result = ((ProductsListViewModel)controller.List("cat2", 1).Model).Products.ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "p2" && result[0].Category == "cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1 , Name = "p1", Category = "jeden"},
                new Product {ProductID = 2 , Name = "p2", Category = "jeden"},
                new Product {ProductID = 3 , Name = "p3", Category = "dwa"},
                new Product {ProductID = 4 , Name = "p4", Category = "trzy"},
                new Product {ProductID = 5 , Name = "p5", Category = "cztery"},
                new Product {ProductID = 6 , Name = "p6", Category = "piec"}
            }.AsQueryable());

            //preparing
            NavController target = new NavController(mock.Object);

            //working
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //assert
            Assert.AreEqual(results.Length, 5);
            Assert.AreEqual(results[0], "jedne");

        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {

            // przygotowanie
            // - utworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "jabłka"},
                new Product {ProductID = 4, Name = "P2", Category = "pomarańcze"},
            }.AsQueryable());

            // przygotowanie - utworzenie kontrolera 
            NavController target = new NavController(mock.Object);

            // przygotowanie - definiowanie kategorii do wybrania
            //string categoryToSelect = "jabłka";

            // działanie
            // string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // asercje
            //Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {

            // przygotowanie
            // - utworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "cat1"},
                new Product {ProductID = 3, Name = "P3", Category = "cat2"},
                new Product {ProductID = 4, Name = "P4", Category = "cat3"}
            }.AsQueryable());

            // przygotowanie - utworzenie kontrolera 
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            // działanie
            int res1 = ((ProductsListViewModel)target.List("cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("cat3").Model).PagingInfo.TotalItems;
            int resAll =((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            // asercje
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 1);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 4);
        }

    }
}

