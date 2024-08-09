// Source-generated logging messages for the OpaClient.
// Ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-library-authors#prefer-source-generated-logging
using Microsoft.Extensions.Logging;

namespace Styra.Opa;

internal static partial class LogMessages
{
    [LoggerMessage(
        Message = "Batch Query API unavailable, falling back to sequential OPA queries",
        Level = LogLevel.Warning)]
    internal static partial void LogBatchQueryFallback(
        this ILogger logger);

    [LoggerMessage(
        Message = "executing policy '{rule}' failed with exception: {message}",
        Level = LogLevel.Error)]
    internal static partial void LogQueryError(
        this ILogger logger,
        string rule,
        string message);

    [LoggerMessage(
        Message = "default policy failed with exception: {message}",
        Level = LogLevel.Error)]
    internal static partial void LogDefaultQueryError(
        this ILogger logger,
        string message);

    [LoggerMessage(
        Message = "executing policy at '{rule}' succeeded, but OPA did not reply with a result",
        Level = LogLevel.Warning)]
    internal static partial void LogQueryNullResult(
        this ILogger logger,
        string rule);

    [LoggerMessage(
        Message = "executing server default policy succeeded, but OPA did not reply with a result",
        Level = LogLevel.Warning)]
    internal static partial void LogDefaultQueryNullResult(
        this ILogger logger);
}