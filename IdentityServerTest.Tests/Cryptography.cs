using System;
using Xunit;
using IdentityServerTest.Identity;

namespace IdentityServerTest.Tests
{
    public class Tests
    {
        [Fact]
        public void HashAndVerifyPassword()
        {
            var cryptography = new Cryptography();
            var pwd = "HelloWorld";
            var hashedPwd = cryptography.Hash(pwd);
            var result = cryptography.Verify(hashedPwd, pwd);

            Assert.NotEqual("HelloWorld", hashedPwd);
            Assert.True(result);
        }
    }
}
