using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BookLibrary.Tests
{
    [CollectionDefinition("TestCollection")]
    public class TestCollection : ICollectionFixture<WebApplicationFactory<Startup>>
    {
    }
}
