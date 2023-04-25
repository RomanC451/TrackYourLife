using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Domain.UnitTests.ValueObjects;

public class LastNameTests
{
[Fact]
        public void Create_ValidLastName_ReturnsSuccessResult()
        {
            // Arrange
            var lastName = "Doe";

            // Act
            var result = Name.Create(lastName);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_NullOrEmptyLastName_ReturnsFailureResultWithEmptyError(string name)
        {

            // Act
            var result = Name.Create(name);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(DomainErrors.Name.Empty, result.Error);
        }

        [Fact]
        public void Create_LongLastName_ReturnsFailureResultWithTooLongError()
        {
            // Arrange
            var lastName = new string('a', Name.MaxLength + 1);

            // Act
            var result = Name.Create(lastName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(DomainErrors.Name.TooLong, result.Error);
        }

        [Fact]
        public void GetAtomicValues_ReturnsSingleValue()
        {
            // Arrange
            var lastName = "Doe";
            var lastNameObject = Name.Create(lastName).Value;

            // Act
            var atomicValues = lastNameObject.GetAtomicValues();

            // Assert
            Assert.Single(atomicValues);
            Assert.Equal(lastName, atomicValues.First());
        }


}
