using Xunit;

namespace BrailleToolkit.Tests
{
    /// <summary>
    /// Defines a test collection named "Singleton-Sensitive Tests" and associates it
    /// with the GlobalSetupFixture. All test classes that are part of this collection
    /// will share this fixture, ensuring the setup code in the fixture runs only once
    /// before any test in the collection starts.
    /// </summary>
    [CollectionDefinition("Singleton-Sensitive Tests")]
    public class TestCollectionDefinition : ICollectionFixture<GlobalSetupFixture>
    {
        // This class has no code, its purpose is solely to be decorated with
        // the [CollectionDefinition] attribute and to associate the ICollectionFixture<T>.
    }
}
