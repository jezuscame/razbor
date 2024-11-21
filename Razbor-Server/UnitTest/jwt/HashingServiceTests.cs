using NUnit.Framework.Legacy;
using Services.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.jwt
{
    [TestFixture]
    public class HashingServiceTests
    {
        private HashingService _hashingService;

        [SetUp]
        public void Setup()
        {
            _hashingService = new HashingService();
        }

        [Test]
        public void Hash_ReturnsValidHash_WhenPasswordIsProvided()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hash = _hashingService.Hash(password);

            // Assert
            ClassicAssert.IsNotNull(hash, "Hash should not be null.");
            ClassicAssert.IsNotEmpty(hash, "Hash should not be empty.");
        }

        [Test]
        public void Hash_ReturnsDifferentHashes_ForDifferentPasswords()
        {
            // Arrange
            var password1 = "PasswordOne";
            var password2 = "PasswordTwo";

            // Act
            var hash1 = _hashingService.Hash(password1);
            var hash2 = _hashingService.Hash(password2);

            // Assert
            ClassicAssert.AreNotEqual(hash1, hash2, "Hashes for different passwords should not be equal.");
        }

        [Test]
        public void Verify_ReturnsTrue_WhenPasswordMatchesHash()
        {
            // Arrange
            var password = "MatchingPassword";
            var hash = _hashingService.Hash(password);

            // Act
            var result = _hashingService.Verify(hash, password);

            // Assert
            ClassicAssert.IsTrue(result, "Verify should return true for a matching password.");
        }

        [Test]
        public void Verify_ReturnsFalse_WhenPasswordDoesNotMatchHash()
        {
            // Arrange
            var hash = _hashingService.Hash("CorrectPassword");
            var incorrectPassword = "IncorrectPassword";

            // Act
            var result = _hashingService.Verify(hash, incorrectPassword);

            // Assert
            ClassicAssert.IsFalse(result, "Verify should return false for an incorrect password.");
        }       
    }
}
