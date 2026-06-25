namespace Neighborhood.Services.Domain.Chatbot
{
    public enum ChatbotRole
    {
        User,
        Assistant,
        // A tool/function result produced during an assistant turn. Persisted so a logged-in
        // user's later messages still "see" what tools returned (e.g. technician ids), and
        // replayed to the model as context. Hidden from the chat UI.
        Tool
    }
}
