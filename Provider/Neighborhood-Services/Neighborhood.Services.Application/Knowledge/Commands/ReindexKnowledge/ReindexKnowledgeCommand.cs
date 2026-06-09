using MediatR;

namespace Neighborhood.Services.Application.Knowledge.Commands.ReindexKnowledge
{
    // Rebuilds the entire Qdrant knowledge index from the DB. Admin/ops action.
    public record ReindexKnowledgeCommand : IRequest<bool>;
}
