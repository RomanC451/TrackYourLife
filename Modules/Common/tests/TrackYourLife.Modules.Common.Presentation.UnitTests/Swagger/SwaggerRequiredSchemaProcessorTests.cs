using FluentAssertions;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using TrackYourLife.Modules.Common.Presentation.Swagger;

namespace TrackYourLife.Modules.Common.Presentation.UnitTests.Swagger;

public class SwaggerRequiredSchemaProcessorTests
{
    private readonly SwaggerRequiredSchemaProcessor _sut;

    public SwaggerRequiredSchemaProcessorTests()
    {
        _sut = new SwaggerRequiredSchemaProcessor();
    }

    [Fact]
    public void Process_WhenPropertiesAreNotNullable_ShouldAddToRequiredProperties()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(TestClass).ToContextualType();

        // Generate the schema properties
        generator.Generate(schema, contextualType, resolver);

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
        schema.RequiredProperties.Should().Contain("RequiredProperty");
    }

    [Fact]
    public void Process_WhenPropertiesAreNullable_ShouldNotAddToRequiredProperties()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(TestClass).ToContextualType();

        // Generate the schema properties
        generator.Generate(schema, contextualType, resolver);

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
        schema.RequiredProperties.Should().NotContain("NullableProperty");
        schema.RequiredProperties.Should().NotContain("RequiredNullableProperty");
    }

    [Fact]
    public void Process_WhenSchemaPropertiesIsNull_ShouldNotThrowException()
    {
        // Arrange
        var schema = new JsonSchema();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        };
        var generator = new JsonSchemaGenerator(settings);
        var resolver = new JsonSchemaResolver(schema, settings);
        var contextualType = typeof(TestClass).ToContextualType();

        // Generate the schema properties
        generator.Generate(schema, contextualType, resolver);

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

    private class TestClass
    {
        public string RequiredProperty { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private members should be removed",
            Justification = "Used in tests"
        )]
        public string? NullableProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private members should be removed",
            Justification = "Used in tests"
        )]
        public string? RequiredNullableProperty { get; set; }
    }
}
