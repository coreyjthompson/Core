using MEI.Core.Commands;

// keep .Admin on namespace as it is used for testing
// ReSharper disable once CheckNamespace
namespace MEI.Core.Tests.Infrastructure.Mocks
{
    public class MockAdminCommand
        : ICommand<MockResult>
    {}
}
