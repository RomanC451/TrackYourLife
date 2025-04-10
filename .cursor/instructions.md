# AI Assistant Instructions

This file contains specific instructions for the AI coding assistant to better assist with the TrackYourLife project.

## Code Generation Guidelines

### Architecture & Patterns

- Follow Clean Architecture principles strictly
- Maintain proper separation between layers (Domain, Application, Infrastructure, API)
- Use CQRS pattern for commands and queries
- Implement proper validation using FluentValidation
- Use Result pattern for error handling

### Code Style

- Use C# 12 features where appropriate
- Follow Microsoft's C# coding conventions
- Use expression-bodied members when it improves readability
- Prefer immutable objects where possible
- Use record types for DTOs and value objects

### Naming Conventions

- Use PascalCase for public members
- Use camelCase for private fields
- Prefix interfaces with 'I'
- Use descriptive names that indicate purpose
- Suffix async methods with 'Async'

### Documentation

- Add XML documentation for all public APIs
- Include parameter descriptions
- Document exceptions that may be thrown
- Add examples for complex methods
- Keep comments focused on "why" not "what"

## Project-Specific Rules

### Domain Layer

- Keep domain entities rich with behavior
- Use value objects for immutable concepts
- Implement domain events for important state changes
- Use domain services for complex operations
- Maintain aggregate boundaries

### Application Layer

- Use MediatR for command/query handling
- Implement proper validation
- Use AutoMapper for object mapping
- Handle cross-cutting concerns via behaviors
- Keep services thin and focused

### Infrastructure Layer

- Implement interfaces from Application layer
- Use proper connection string management
- Implement proper logging
- Handle external service integration
- Use proper caching strategies

### API Layer

- Use proper HTTP status codes
- Implement proper API versioning
- Use proper authentication/authorization
- Implement rate limiting
- Use proper error responses

## Error Handling

### Domain Errors

- Use domain-specific exceptions
- Include proper error messages
- Maintain error hierarchy
- Use proper error codes
- Include context in errors

### Application Errors

- Use Result pattern
- Handle validation errors
- Log errors properly
- Return proper HTTP status codes
- Include error details in responses

## Security Guidelines

### Authentication

- Use proper token-based authentication
- Implement refresh tokens
- Handle token expiration
- Secure sensitive data
- Use proper password hashing

### Authorization

- Implement role-based access control
- Use proper claims
- Handle permissions properly
- Secure endpoints
- Validate user input

## Performance Guidelines

### Database

- Use proper indexing
- Optimize queries
- Use proper connection pooling
- Implement caching where appropriate
- Monitor query performance

### API

- Implement proper pagination
- Use compression
- Implement caching headers
- Optimize response size
- Monitor API performance

## Additional Instructions

### Code Review Focus

- Check for security vulnerabilities
- Verify proper error handling
- Ensure proper logging
- Validate performance implications
- Check for proper testing

### Documentation Requirements

- Update API documentation
- Document breaking changes
- Keep README up to date
- Document deployment procedures
- Maintain changelog

### Communication Style

- Be concise and clear
- Explain complex concepts
- Provide examples when needed
- Use proper technical terminology
- Be professional and helpful
