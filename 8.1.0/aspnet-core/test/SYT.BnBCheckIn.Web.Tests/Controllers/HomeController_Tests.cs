using System.Threading.Tasks;
using SYT.BnBCheckIn.Models.TokenAuth;
using SYT.BnBCheckIn.Web.Controllers;
using Shouldly;
using Xunit;

namespace SYT.BnBCheckIn.Web.Tests.Controllers
{
    public class HomeController_Tests: BnBCheckInWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}