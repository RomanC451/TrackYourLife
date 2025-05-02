using FluentAssertions;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using TrackYourLife.Modules.Common.Presentation.Swagger;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Common.Presentation.UnitTests.Swagger;

public class IdResponseSchemaFilterTests
{
    private readonly IdResponseSchemaFilter _sut;

    public IdResponseSchemaFilterTests()
    {
        _sut = new IdResponseSchemaFilter();
    }

    [Fact]
    public void Process_WhenTypeIsIdResponse_ShouldSetIdPropertySchema()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(IdResponse).ToContextualType();

        var context = new SchemaProcessorContext(
            contextualType,
            schema,
            resolver,
            generator,
            settings
        );

        // Act
        _sut.Process(context);

        // Assert
        schema.Properties.Should().ContainKey("id");
        var idProperty = schema.Properties["id"];
        idProperty.Type.Should().Be(JsonObjectType.String);
        idProperty.Format.Should().Be("uuid");
    }

    [Fact]
    public void Process_WhenTypeIsNotIdResponse_ShouldNotModifySchema()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(string).ToContextualType();

        var context = new SchemaProcessorContext(
            contextualType,
            schema,
            resolver,
            generator,
            settings
        );

        // Act
        _sut.Process(context);

        // Assert
        schema.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Process_WhenSchemaIsNull_ShouldNotThrowException()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(IdResponse).ToContextualType();

        var context = new SchemaProcessorContext(
            contextualType,
            schema,
            resolver,
            generator,
            settings
        );

        // Act & Assert
        _sut.Invoking(x => x.Process(context)).Should().NotThrow();
    }
}
