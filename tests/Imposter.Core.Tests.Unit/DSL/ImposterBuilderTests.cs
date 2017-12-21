using System;
using System.Linq;

using FluentAssertions;

using FluentImposter.Core.DSL;

using Xunit;

namespace Imposter.Core.Tests.Unit.DSL
{
    public class ImposterBuilderTests
    {
        private readonly Uri _testUri = new Uri("http://localhost:5000");

        [Fact]
        public void HasAnImposter_WhenImposterNameIsBlank_AssignsARandomGuidAsName()
        {
            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter("");

            var imposter = imposterHostBuilder.Imposters.First();

            Action action =()=> Guid.Parse(imposter.Name);

            action.Should().NotThrow<ArgumentNullException>();
            action.Should().NotThrow<FormatException>();
        }

        [Fact]
        public void HasAnImposter_WhenImposterNameIsNull_AssignsARandomGuidAsName()
        {
            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter(null);

            var imposter = imposterHostBuilder.Imposters.First();

            Action action = () => Guid.Parse(imposter.Name);

            action.Should().NotThrow<ArgumentNullException>();
            action.Should().NotThrow<FormatException>();
        }

        [Fact]
        public void HasAnImposter_WhenImposterNameIsProvided_CreatesAnImposterWithThatName()
        {
            string expectedImposterName = "test_imposter";

            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter(expectedImposterName);

            var imposter = imposterHostBuilder.Imposters.First();

            imposter.Name.Should().Be(expectedImposterName);
        }
    }
}
