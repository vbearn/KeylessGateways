using AutoFixture;
using FluentAssertions;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using KeylessGateways.DoorEntrance.Services;
using KeylessGateways.DoorEntrance.UnitTests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KeylessGateways.DoorEntrance.UnitTests
{
    public sealed class UserDoorAccessServiceTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IRepository<UserDoor>> _userDoorAccessRepositoryStub;
        private readonly UserDoor _allowedUserDoor;
        private readonly OpenDoorExtendedModel _openDoorModel;
        private readonly CancellationToken _cancellationToken;

        public UserDoorAccessServiceTests()
        {
            _fixture = new Fixture();

            _cancellationToken = _fixture.Build<CancellationToken>().Create();
            _openDoorModel = _fixture.Build<OpenDoorExtendedModel>().Create();

            _allowedUserDoor = _fixture.Build<UserDoor>()
                .With(p => p.DoorId, _openDoorModel.DoorId)
                .With(p => p.UserId, _openDoorModel.UserId)
                .Create();

            _userDoorAccessRepositoryStub = new Mock<IRepository<UserDoor>>();
            _userDoorAccessRepositoryStub
                .Setup(m => m.TableNoTracking)
                .Returns(new List<UserDoor> { _allowedUserDoor }.AsAsyncQueryable());
        }

        [Fact]
        public async Task IsAuthorizedToOpenDoor_WithPolicyThatAllowsUserAccess_ShouldReturnTrue()
        {

            // Arrange

            var policyThatAllowsUserAccessStub = new Mock<IDoorAccessPolicy>();
            policyThatAllowsUserAccessStub
                .Setup(m => m.ConfigureAllowedAccessCriteria(It.IsAny<OpenDoorExtendedModel>(), It.IsAny<List<UserDoor>>()))
                .Returns(Task.FromResult(new List<UserDoor> { _allowedUserDoor }));

            var validDoorAccessPolicies = new List<IDoorAccessPolicy> { policyThatAllowsUserAccessStub.Object };

            var doorAccessPolicyFactoryServiceStub = new Mock<IDoorAccessPolicyFactoryService>();
            doorAccessPolicyFactoryServiceStub
                .Setup(m => m.GetSortedDoorAccessPolicies())
                .Returns(new List<IDoorAccessPolicy> { policyThatAllowsUserAccessStub.Object });


            var _sut = new UserDoorAccessService(
                _userDoorAccessRepositoryStub.Object,
                doorAccessPolicyFactoryServiceStub.Object);


            // Act

            var result = await _sut.IsAuthorizedToOpenDoorAsync(_openDoorModel, _cancellationToken);

            // Assert

            result.Should().Be(true);
        }


        [Fact]
        public async Task IsAuthorizedToOpenDoor_WithPolicyThatDoesNotAllowUserAccess_ShouldReturnFalse()
        {

            // Arrange

            var policyThatDoesNotAllowUserAccessStub = new Mock<IDoorAccessPolicy>();
            policyThatDoesNotAllowUserAccessStub
                .Setup(m => m.ConfigureAllowedAccessCriteria(It.IsAny<OpenDoorExtendedModel>(), It.IsAny<List<UserDoor>>()))
                .Returns(Task.FromResult(new List<UserDoor> { }));

            var doorAccessPolicyFactoryServiceStub = new Mock<IDoorAccessPolicyFactoryService>();
            doorAccessPolicyFactoryServiceStub
                .Setup(m => m.GetSortedDoorAccessPolicies())
                .Returns(new List<IDoorAccessPolicy> { policyThatDoesNotAllowUserAccessStub.Object });


            var _sut = new UserDoorAccessService(
                _userDoorAccessRepositoryStub.Object,
                doorAccessPolicyFactoryServiceStub.Object);


            // Act

            var result = await _sut.IsAuthorizedToOpenDoorAsync(_openDoorModel, _cancellationToken);

            // Assert

            result.Should().Be(false);
        }

        [Fact]
        public async Task IsAuthorizedToOpenDoor_WithNoPolicy_ShouldReturnTrue()
        {

            // Arrange

            var filteredDoorsByPolicies = new List<IDoorAccessPolicy> { };


            // No valid policies => empty list
            var validDoorAccessPolicies = new List<IDoorAccessPolicy> { };

            var doorAccessPolicyFactoryServiceStub = new Mock<IDoorAccessPolicyFactoryService>();
            doorAccessPolicyFactoryServiceStub
                .Setup(m => m.GetSortedDoorAccessPolicies())
                .Returns(validDoorAccessPolicies);


            var _sut = new UserDoorAccessService(
                _userDoorAccessRepositoryStub.Object,
                doorAccessPolicyFactoryServiceStub.Object);


            // Act

            var result = await _sut.IsAuthorizedToOpenDoorAsync(_openDoorModel, _cancellationToken);

            // Assert

            result.Should().Be(true);
        }


    }


}
