using MEI.Core.Commands.Decorators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class PostCommitRegistratorTests
    {
        private PostCommitRegistrator _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new PostCommitRegistrator();
        }

        [TestMethod]
        public void ExecuteActions_CommittedEventIsCalled()
        {
            var wasCalled = false;
            _target.Committed += () => wasCalled = true;

            _target.ExecuteActions();

            Assert.IsTrue(wasCalled);
        }
    }
}
