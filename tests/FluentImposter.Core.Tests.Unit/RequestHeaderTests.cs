using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class RequestHeaderTests
    {
        [Fact]
        public void Add_CanAddRequestHeaderKeyValuePair()
        {
            RequestHeader header = new RequestHeader();

            header.Add("test",
                       new[]
                       {
                           "test_value"
                       });

            header.HeadersInternal.Keys.Should().Contain("test");
            header.HeadersInternal.Values.Should().BeEquivalentTo(new[]
                                                          {
                                                              new[]{"test_value"}
                                                          });
        }

        [Fact]
        public void Contains_WhenHeadersContainAMatchingKeyValuePair_ReturnsTrue()
        {
            RequestHeader header = new RequestHeader();

            header.Add("test",
                       new[]
                       {
                           "test_value"
                       });


            header.Contains("test")
                  .Should().BeTrue();
        }

        [Fact]
        public void Contains_WhenHeadersDoesNotContainAMatchingKeyValuePair_ReturnsFalse()
        {
            RequestHeader header = new RequestHeader();

            header.Add("test",
                       new[]
                       {
                           "test_value"
                       });


            header.Contains("I_do_not_exist_as_a_key")
                  .Should().BeFalse();
        }

        [Fact]
        public void ContainsKeyAndValues_WhenHeadersContainsKeyAndValues_ReturnsTrue()
        {
            RequestHeader header = new RequestHeader();

            header.Add("test",
                       new[]
                       {
                           "test_value"
                       });


            header.ContainsKeyAndValues("test",
                                        new[]
                                        {
                                            "test_value"
                                        })
                  .Should().BeTrue();
        }


        [Fact]
        public void ContainsKeyAndValues_WhenHeadersContainsKeyButNotMatchingValues_ReturnsFalse()
        {
            RequestHeader header = new RequestHeader();

            header.Add("test",
                       new[]
                       {
                           "test_value"
                       });


            header.ContainsKeyAndValues("test",
                                        new[]
                                        {
                                            "this_value_doesnt_exist"
                                        })
                  .Should().BeFalse();
        }

        [Fact]
        public void ContainsKeyAndValues_WhenHeadersDoesNotContainKey_ReturnsFalse()
        {
            RequestHeader header = new RequestHeader();

            header.Add("this_key_does_not_exist",
                       new[]
                       {
                           "test_value"
                       });


            header.ContainsKeyAndValues("test",
                                        new[]
                                        {
                                            "test_value"
                                        })
                  .Should().BeFalse();
        }
    }
}
