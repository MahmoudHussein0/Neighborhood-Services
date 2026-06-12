using MediatR;
using Neighborhood.Services.Application.AI.Interfaces;

namespace Neighborhood.Services.Application.Knowledge.Commands.ReindexKnowledge
{
    public class ReindexKnowledgeCommandHandler : IRequestHandler<ReindexKnowledgeCommand, bool>
    {
        private readonly IKnowledgeIndexer _indexer;

        public ReindexKnowledgeCommandHandler(IKnowledgeIndexer indexer) => _indexer = indexer;

        public async Task<bool> Handle(ReindexKnowledgeCommand request, CancellationToken cancellationToken)
        {
            await _indexer.ReindexAllAsync();
            return true;
        }
    }
}
