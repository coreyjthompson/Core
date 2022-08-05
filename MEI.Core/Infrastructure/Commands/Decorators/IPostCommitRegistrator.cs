using System;

namespace MEI.Core.Commands.Decorators
{
    public interface IPostCommitRegistrator
    {
        event Action Committed;

        void ExecuteActions();

        void Reset();
    }

    public sealed class PostCommitRegistrator
        : IPostCommitRegistrator
    {
        public event Action Committed = () => { };

        public void ExecuteActions()
        {
            Committed();
        }

        public void Reset()
        {
            Committed = () => { };
        }
    }
}