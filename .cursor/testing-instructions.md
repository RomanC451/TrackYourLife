## Testing Guidelines

- When you create a new test or update an existing one run the tests and fix the failing ones.
- Don't include the usings that are part of the global usings like NSubtitute and FluentAssertions.

- use Faker whenever is possible for craeting entities and pass only the properties that are realy needed to the faker.

- Don't add common messages on validators, add them only when they are really needed.
- Don't check the actual error message but just the property.
- Use FluentValidation.TestHelper for testing fluent validators.

### Unit Tests

- Test each public method
- Use proper test naming (MethodName_Scenario_ShouldExpectedResult)
- Mock external dependencies with NSubtitute
- Use FluentAssertions
- Test edge cases
- Test error conditions

### Integration Tests

- Test critical paths
- Use test databases
- Clean up after tests
- Test external service integration
- Test authentication/authorization
