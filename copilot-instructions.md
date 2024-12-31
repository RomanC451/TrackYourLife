
"Implement the IDisposable interface in your test class and clear the mocks in the Dispose method and use this template for implementing the Dispose method: 
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodDiaryRepository.ClearSubstitute();
        servingSizeRepository.ClearSubstitute();
        userIdentifierProvider.ClearSubstitute();
        foodRepository.ClearSubstitute();
    }



    {
      "text": "Use NSubstitute to create mocks for services.",
    },
    {
      "text": "Make only one private readonly object of the class that is under test for all tests and call it sut.",
    },

    {
      "text": "Use the Arrange, Act, Assert pattern in your tests.",
    },
    {
      "text": "Use the FluentAssertions library to write your assertions"
    },
    {
      "text" : "Follow this pattern for test names: [MethodName]_When[Scenario]_[ExpectedBehavior]"
    }