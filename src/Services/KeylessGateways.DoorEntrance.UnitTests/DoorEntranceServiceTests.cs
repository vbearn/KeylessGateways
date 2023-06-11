using AutoFixture;
using FluentAssertions;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using KeylessGateways.DoorEntrance.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KeylessGateways.DoorEntrance.UnitTests
{
    public sealed class DoorEntranceServiceTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<DoorEntranceService>> _loggerStub;
        private readonly OpenDoorExtendedModel _openDoorModel;
        private readonly CancellationToken _cancellationToken;

        public DoorEntranceServiceTests()
        {
            _fixture = new Fixture();
          
            _cancellationToken = _fixture.Build<CancellationToken>().Create();
            _openDoorModel = _fixture.Build<OpenDoorExtendedModel>().Create();

            _loggerStub = new Mock<ILogger<DoorEntranceService>>();
        }


        [Fact]
        public async Task OpenDoor_WithAuthorizedToOpenDoorTrue_ShouldReturnTrue()
        {

            // Arrange

            var doorEntranceRepositoryStub = new Mock<IRepository<DoorEntranceHistory>>();

            var doorEntranceAccessServiceStub = new Mock<IUserDoorAccessService>();
            doorEntranceAccessServiceStub
                .Setup(m => m.IsAuthorizedToOpenDoorAsync(It.IsAny<OpenDoorExtendedModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));


            var _sut = new DoorEntranceService(
                _loggerStub.Object,
                doorEntranceRepositoryStub.Object,
                doorEntranceAccessServiceStub.Object);


            // Act

            var result = await _sut.OpenDoor(_openDoorModel, _cancellationToken);

            // Assert

            result.Should().Be(true);
        }



        [Fact]
        public async Task OpenDoor_WithAuthorizedToOpenDoorFalse_ShouldReturnFalse()
        {
            // Arrange

            var doorEntranceRepositoryStub = new Mock<IRepository<DoorEntranceHistory>>();
            var doorEntranceAccessServiceStub = new Mock<IUserDoorAccessService>();

            doorEntranceAccessServiceStub
                .Setup(m => m.IsAuthorizedToOpenDoorAsync(It.IsAny<OpenDoorExtendedModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var _sut = new DoorEntranceService(
                _loggerStub.Object,
                doorEntranceRepositoryStub.Object,
                doorEntranceAccessServiceStub.Object);


            // Act

            var result = await _sut.OpenDoor(_openDoorModel, _cancellationToken);

            // Assert
            result.Should().Be(false);
           
        }

    }


}
