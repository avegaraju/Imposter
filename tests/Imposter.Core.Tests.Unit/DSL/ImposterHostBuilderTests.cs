using System;

using FluentAssertions;

using FluentImposter.Core.DSL;

using Xunit;

namespace FluentImposter.Core.Tests.Unit.DSL
{
    /// ////////////////////////////////////////////////////////////////
    //End goal of the DSL
    //var imposterHost = new ImposterHostBuilder()
    //        .HostedOn("http://localhost:5000")
    //        .HasAnImposter("",
    //                       i =>
    //                       {
    //                           i.IsOfType("Rest")
    //                            .StubbingResource("products/productId")
    //                            .WhenRequestBodyContains("")
    //                            .RespondsWithStatus("OK")
    //                            .And.WithResponseBody("");

    //                       })
    //        .HasAnImposter("",
    //                       imposter =>
    //                       {

    //                       })
    //        .Create();

    //imposterHost.Start();
    /////////////////////////////////////////////////////////////////////
    public class ImposterHostBuilderTests
    {
        private readonly Uri _testUri = new Uri("http://localhost:5000");

        [Fact]
        public void Create_ReturnsIImposterHost()
        {

            var imposterHost = new ImposterHostBuilder().Create();

            imposterHost.Should().BeAssignableTo<ImposterHost>();
        }

        [Fact]
        public void HostedOn_Returns_IImposterBuilder()
        {
            var imposterBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri);

            imposterBuilder
                    .Should().BeAssignableTo<IImposterBuilder>();
        }

        [Fact]
        public void HasAnImposter_ReturnsImposterHostBuilder()
        {
            var imposterHostBuilder = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter("", i => { });

            imposterHostBuilder
                    .Should().BeOfType<ImposterHostBuilder>();
        }

        [Fact]
        public void HostedOn_AddsCorectHostInformationToTheHostBuilder()
        {
            var imposterHost = new ImposterHostBuilder()
                    .HostedOn(_testUri)
                    .HasAnImposter("", i => { })
                    .Create();

            imposterHost
                    .Host.BaseUri
                    .Should().Be(_testUri);
        }
    }
}
