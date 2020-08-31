using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zillow.DataAccess;
using Zillow.Liasons;
using Zillow.Models.Options;
using Zillow.Models.Shared;

namespace ZillowTest.Liasons
{
    [TestClass]
    public class SourceLiasonTest
    {
        [TestMethod]
        public async Task FetchAllAsyncTest()
        {
            var tests = new[] {
                new {
                    Q = new SlugMapping { ZPID = BasicZPID, Slug = BasicSlug },
                    Expected = new { ZPID = BasicZPID, ZEstimate = BasicAmount }
                },
            };

            foreach (var test in tests)
            {
                var logger = new Mock<ILogger<SourceLiason>>();
                var sourceDAO = new Mock<ISourceDAO>();
                var opts = Options.Create(new SourceOpts());
                var sharedOpts = Options.Create(new SharedOpts
                {
                    Resources = new[] { test.Q }.ToList(),
                });

                sourceDAO.Setup(x => x.FetchOneAsync(test.Q, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Zillow.Models.Source.Response
                     {
                         ZPID = test.Expected.ZPID,
                         Amount = test.Expected.ZEstimate,
                     });

                var sourceLiason = new SourceLiason(logger.Object, sourceDAO.Object, opts, sharedOpts);
                await foreach (var result in sourceLiason.FetchAllAsync())
                {
                    Assert.AreEqual(test.Expected.ZPID, result.ZPID);
                    Assert.AreEqual(test.Expected.ZEstimate, result.ZEstimate);
                }
            }
        }

        private static string BasicSlug = "totallyaslug";
        private static string BasicAmount = "10000.52";
        private static string BasicZPID = "15873525";
    }
}
