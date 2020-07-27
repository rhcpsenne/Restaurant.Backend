using Backend.Controllers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Priority;
namespace BackendUnitTest
{
    
    public class BasicTests
    {
        #region Arrange
        OrdersController _controller = new OrdersController(new RestaurantDBContext()); //Mocked DBContext
        //list of input scenarios
        List<string> listInput = new List<string>()
            {
                "morning, 1, 2, 3",
                "morning, 2, 1, 3",
                "morning, 1, 2, 3, 4",
                "morning, 1, 2, 3, 3, 3",
                "night, 1, 2, 3, 4",
                "night, 1, 2, 2, 4",
                "night, 1, 2, 3, 5",
                "night, 1, 1, 2, 3, 5"
            };
        //list of expected output
        List<string> listExpected = new List<string>()
            {
                "eggs, toast, coffee",
                "eggs, toast, coffee",
                "eggs, toast, coffee, error",
                "eggs, toast, coffee(x3)",
                "steak, potato, wine, cake",
                "steak, potato(x2), cake",
                "steak, potato, wine, error",
                "steak, error"
            };
        #endregion

        [Fact, Priority(-10)] // First test to run
        public void TestPostOrder()
        {
            for(int i = 0; i < listInput.Count;i++)
            {
                var input = listInput[i];
                var expected = listExpected[i];
                //Act
                var result = _controller.PostOrder(new OrderViewModel { OrderDescription = input });
                var actual = ((OrderViewModel)((result.Result as CreatedAtActionResult).Value)).OrderDescription;
                //Assert
                Assert.Equal(expected, actual);
            }
        }

        [Fact, Priority(0)] // Second test to run
        public void TestGetOrder()
        {
            //Act
            var result = _controller.GetOrder(53); //specif order

            //Assert
            Assert.IsType<ActionResult<OrderViewModel>>(result);
        }

        [Fact, Priority(10)] // Third test to run
        public void TestGetOrders()
        {
            List<string> actual = new List<string>();

            //Act
            var result = _controller.GetOrders();
            var listViewModel = result.Value;
            foreach (var element in listViewModel)
            {
                actual.Add(element.OrderDescription);
            }

            //Assert
            Assert.Equal(listExpected, actual);
        }
    }
}
