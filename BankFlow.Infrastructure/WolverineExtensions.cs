using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;

namespace BankFlow.Infrastructure;

public static class WolverineExtensions
{
    public static WolverineOptions Subscribe<TMessage>(this WolverineOptions options, string groupId = "BankFlow")
    {
        var topic = ToTopic<TMessage>();

        options.ConfigurePublisher<TMessage>();//Must have it for AutoProvision the topic
        options.ListenToKafkaTopic(topic)
            .DefaultIncomingMessage<TMessage>()
            .WithGroupId(groupId);
        return options;
    }
    public static WolverineOptions ConfigurePublisher<TMessage>(this WolverineOptions options)
    {
        var topic = ToTopic<TMessage>();
        options.PublishMessage<TMessage>()
            .ToKafkaTopic(topic);
        return options;
    }
    public static IAdditionalActions PublishToDeadLetterTopic(
        this IFailureActions then,
        string? topicName = null)
    {
        var additionalActions = then
            .Discard()
            .And(async (_, context, _) =>
            {
                var dlqTopic = topicName
                    ?? $"{context.Envelope.TopicName}.dlq";

                await context.BroadcastToTopicAsync(
                    dlqTopic,
                    context.Envelope.Message);
            });
        return additionalActions;
    }

    private static string ToTopic<TMessage>()
    {
        var type = typeof(TMessage);
        var topic = Regex
            .Replace(type.Name, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
        return topic;
    }
}