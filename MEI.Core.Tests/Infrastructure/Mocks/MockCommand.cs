using MEI.Core.Commands;

namespace MEI.Core.Tests.Infrastructure.Mocks
{
    public class MockCommand : ICommand<MockResult>
    {
        public override string ToString()
        {
            return "[MockCommand]";
        }
    }
}
