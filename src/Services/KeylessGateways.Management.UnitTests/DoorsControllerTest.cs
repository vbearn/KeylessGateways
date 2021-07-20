using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KeylessGateways.Common;
using KeylessGateways.Management.Controllers;
using KeylessGateways.Management.Data;
using KeylessGateways.Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KeylessGateways.Management.UnitTests
{
    public class DoorsControllerControllerTest
    {
        private readonly DbContextOptions<ManagementDbContext> _dbOptions;

        public DoorsControllerControllerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<ManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            using (var dbContext = new ManagementDbContext(_dbOptions))
            {
                dbContext.AddRange(GetFakeDoors());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_doors_success()
        {
            // TODO: Still Work in Progress"


            //Arrange
            var expectedTotalItems = 2;

            var managementDbContext = new ManagementDbContext(_dbOptions);

            var userDoorRepository = new Repository<UserDoor>(managementDbContext);
            var doorsRepository = new Repository<Door>(managementDbContext);
            var logger = new Mock<ILogger<DoorsController>>();
            var mapper = new Mock<IMapper>();


            //Act
            var orderController =
                new DoorsController(logger.Object, mapper.Object, userDoorRepository, doorsRepository);
            var actionResult = await orderController.GetAll(CancellationToken.None);

            //Assert
            Assert.IsType<ActionResult<List<DoorDto>>>(actionResult);
            var page = Assert.IsAssignableFrom<List<DoorDto>>(actionResult.Value);
            Assert.Equal(expectedTotalItems, page.Count);
        }

        private List<Door> GetFakeDoors()
        {
            return new List<Door>
            {
                new Door
                {
                    Id = new Guid(),
                    Name = "Tunnel",
                },
                new Door
                {
                    Id = new Guid(),
                    Name = "Office",
                }
            };
        }
    }
}