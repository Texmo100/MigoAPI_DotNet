using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigoAPI.Controllers;
using MigoAPI.Data;
using MigoAPI.Models;
using MigoAPI.Repository;
using MigoAPI.Repository.IRepository;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.Tests
{
    public class UsersControllerTest
    {
        private User _fakeUser = new User()
        {
            Id = 1,
            UserName = "TexmoTest",
            Password = "passTest",
            FirstName = "isaacTest",
            LastName = "iniguezTest",
            Age = 24
        };

        [Test]
        public void ShouldCreateUser()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<User>>();
            repositoryMock.Setup(x => x.CreateMemberAsync(It.IsAny<User>())).ReturnsAsync(true);
            repositoryMock.Setup(x => x.MemberExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var usersController = new UsersController(repositoryMock.Object);
            var response = usersController.CreateUserAsync(_fakeUser);
            var objResult = response.Result as ObjectResult;
            var statusCode = (int)objResult.StatusCode;

            // Assert
            Assert.AreEqual(statusCode, 201);
        }

        [Test]
        [TestCase(true, true, true,  ExpectedResult = 400)]
        [TestCase(false, true, true,  ExpectedResult = 404)]
        [TestCase(false, false, false, ExpectedResult = 500)]
        public async Task<int> ShouldFailCreatingUser(bool useNullUser, bool memberExistReturn, bool creationReturn)
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<User>>();
            repositoryMock.Setup(x => x.CreateMemberAsync(It.IsAny<User>())).ReturnsAsync(creationReturn);
            repositoryMock.Setup(x => x.MemberExistsAsync(It.IsAny<string>())).ReturnsAsync(memberExistReturn);

            // Act
            var usersController = new UsersController(repositoryMock.Object);
            var response = await usersController.CreateUserAsync(useNullUser ? null : _fakeUser);
            var objResult = response as ObjectResult;
            var statusCode = (int)objResult.StatusCode;

            // Assert
            return statusCode;
        }

        [Test]
        public void ShouldCallTheUserCreationMethod()
        {
            // Arrange
            // MockBehavior enum will indicate the way the mock will behave
            // Strict => all the involve method calls need to be perfom
            // Loose => the method calls can be flexible
            // Default => Its the default mock behavior
            var repositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.CreateMemberAsync(It.IsAny<User>())).ReturnsAsync(true);
            repositoryMock.Setup(x => x.MemberExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            // This way we can preconditionate the throws (handling exceptions)
            //repositoryMock.Setup(x => x.CreateMemberAsync(It.IsAny<User>())).ThrowsAsync<InvalidOperationException>();
            //repositoryMock.Setup(x => x.CreateMemberAsync(It.IsAny<User>())).
            //    ThrowsAsync(new InValidOperationException("Test Exception"));

            // Act
            var usersController = new UsersController(repositoryMock.Object);
            var response = usersController.CreateUserAsync(_fakeUser);
            var objResult = response.Result as ObjectResult;
            var statusCode = (int)objResult.StatusCode;

            // Assert
            repositoryMock.Verify(x => x.CreateMemberAsync(It.IsAny<User>()), Times.Once);
            repositoryMock.Verify(x => x.MemberExistsAsync(It.IsAny<string>()), Times.Once);
            repositoryMock.VerifyNoOtherCalls(); // This verify no other method calls was made during the test

            // To veriry if certain properties were called properly, in this case verify the set and get operations
            //repositoryMock.VerifyGet(x => x.SomeProperty, Times.Once);
            //repositoryMock.VerifySet(x => x.SomeProperty = It.IsAny<string>(), Times.Once);
        }
    }
}