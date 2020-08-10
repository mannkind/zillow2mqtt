using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwoMQTT.Core.Utils;
using Zillow.Liasons;
using Zillow.Models.Options;
using Zillow.Models.Shared;

namespace ZillowTest.Liasons
{
    [TestClass]
    public class MQTTLiasonTest
    {
        [TestMethod]
        public void MapDataTest()
        {
            var tests = new[] {
                new {
                    Q = new SlugMapping { ZPID = BasicZPID, Slug = BasicSlug },
                    Resource = new Resource { ZPID = BasicZPID, ZEstimate = BasicAmount },
                    Expected = new { ZPID = BasicZPID, ZEstimate = BasicAmount, Slug = BasicSlug, Found = true }
                },
                new {
                    Q = new SlugMapping { ZPID = BasicZPID, Slug = BasicSlug },
                    Resource = new Resource { ZPID = $"{BasicZPID}-fake" , ZEstimate = BasicAmount },
                    Expected = new { ZPID = string.Empty, ZEstimate = string.Empty, Slug = string.Empty, Found = false }
                },
            };

            foreach (var test in tests)
            {
                var logger = new Mock<ILogger<MQTTLiason>>();
                var generator = new Mock<IMQTTGenerator>();
                var sharedOpts = Options.Create(new SharedOpts
                {
                    Resources = new[] { test.Q }.ToList(),
                });

                generator.Setup(x => x.BuildDiscovery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Reflection.AssemblyName>(), false))
                    .Returns(new TwoMQTT.Core.Models.MQTTDiscovery());
                generator.Setup(x => x.StateTopic(test.Q.Slug, nameof(Resource.ZEstimate)))
                    .Returns($"totes/{test.Q.Slug}/topic/{nameof(Resource.ZEstimate)}");

                var mqttLiason = new MQTTLiason(logger.Object, generator.Object, sharedOpts);
                var results = mqttLiason.MapData(test.Resource);
                var actual = results.FirstOrDefault();

                Assert.AreEqual(test.Expected.Found, results.Any(), "The mapping should exist if found.");
                if (test.Expected.Found)
                {
                    Assert.IsTrue(actual.topic.Contains(test.Expected.Slug), "The topic should contain the expected ZPID.");
                    Assert.AreEqual(test.Expected.ZEstimate, actual.payload, "The payload be the expected ZEstimate.");
                }
            }
        }

        [TestMethod]
        public void DiscoveriesTest()
        {
            var tests = new[] {
                new {
                    Q = new SlugMapping { ZPID = BasicZPID, Slug = BasicSlug },
                    Resource = new Resource { ZPID = BasicZPID, ZEstimate = BasicAmount },
                    Expected = new { ZPID = BasicZPID, ZEstimate = BasicAmount, Slug = BasicSlug }
                },
            };

            foreach (var test in tests)
            {
                var logger = new Mock<ILogger<MQTTLiason>>();
                var generator = new Mock<IMQTTGenerator>();
                var sharedOpts = Options.Create(new SharedOpts
                {
                    Resources = new[] { test.Q }.ToList(),
                });

                generator.Setup(x => x.BuildDiscovery(test.Q.Slug, nameof(Resource.ZEstimate), It.IsAny<System.Reflection.AssemblyName>(), false))
                    .Returns(new TwoMQTT.Core.Models.MQTTDiscovery());

                var mqttLiason = new MQTTLiason(logger.Object, generator.Object, sharedOpts);
                var results = mqttLiason.Discoveries();
                var result = results.FirstOrDefault();

                Assert.IsNotNull(result, "A discovery should exist.");
            }
        }

        private static string BasicSlug = "totallyaslug";
        private static string BasicAmount = "10000.52";
        private static string BasicZPID = "15873525";
    }
}
