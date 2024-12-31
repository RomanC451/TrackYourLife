namespace TrackYourLife.SharedLib.Domain.Errors;

public static class DomainErrors
{
    public static class General
    {
        public static Error UnProcessableRequest =>
            new("General.UnProcessableRequest", "The server could not process the request.");

        public static Error ServerError =>
            new("General.ServerError", "The server encountered an unrecoverable error.");
    }

    public static class ArgumentError
    {
        public static readonly Func<string, string, Error> Empty = (entityName, argumentName) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.Empty",
                $"The {argumentName} is empty for entity {entityName}."
            );

        public static readonly Func<string, string, Error> Null = (entityName, argumentName) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.Null",
                $"The {argumentName} is null for entity {entityName}."
            );

        public static readonly Func<string, string, Error> Invalid = (entityName, argumentName) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.Invalid",
                $"The {argumentName} is invalid for entity {entityName}."
            );

        public static readonly Func<string, string, Error> Negative = (entityName, argumentName) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.NotPositive",
                $"The {argumentName} can't be negative for entity {entityName}."
            );

        public static readonly Func<string, string, Error> NotPositive = (
            entityName,
            argumentName
        ) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.NotPositive",
                $"The {argumentName} must be greater than zero for entity {entityName}."
            );

        public static readonly Func<string, string, Error> OutOfIndex = (
            entityName,
            argumentName
        ) =>
            new Error(
                $"{entityName}.{argumentName.ToCapitalCase()}.OutOfIndex",
                $"The {argumentName.ToCapitalCase()} is out of index."
            );

        public static readonly Func<string, string, string, Error> Custom = (
            entityName,
            argumentName,
            message
        ) => new Error($"{entityName}.{argumentName.ToCapitalCase()}.Custom", message);
    }

    public static string ToCapitalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpper(value[0]) + value[1..];
    }
}
