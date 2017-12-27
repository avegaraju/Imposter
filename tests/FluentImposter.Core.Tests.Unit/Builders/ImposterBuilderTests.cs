using System;
using System.Linq;

using FluentAssertions;

using FluentImposter.Core.Builders;

using Xunit;

namespace FluentImposter.Core.Tests.Unit.Builders
{
    public class ImposterBuilderTests
    {
        private readonly Uri _testUri = new Uri("http://localhost:5000");

        [Fact]
        public void HasAnImposter_WhenImposterNameIsBlank_AssignsARandomGuidAsName()
        {
            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter("", i => { });

            var imposter = imposterHostBuilder.Imposters.First();

            Action action = () => Guid.Parse(imposter.Name);

            action.Should().NotThrow<ArgumentNullException>();
            action.Should().NotThrow<FormatException>();
        }

        [Fact]
        public void HasAnImposter_WhenImposterNameIsNull_AssignsARandomGuidAsName()
        {
            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter(null, i => { });

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
                    .HasAnImposter(expectedImposterName, i => { });

            var imposter = imposterHostBuilder.Imposters.First();

            imposter.Name.Should().Be(expectedImposterName);
        }
    }
}
