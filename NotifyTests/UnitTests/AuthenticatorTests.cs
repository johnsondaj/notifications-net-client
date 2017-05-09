﻿using Notify.Authentication;
using Notify.Exceptions;
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Notify.UnitTests
{
    [TestFixture()]
    public class AuthenticatorTests
    {
        private const String NOTIFY_SECRET_ID = "d2d425c1-d1d7-4af2-b727-fbd7b612aae1";
        private const String NOTIFY_SERVICE_ID = "9b8c5eef-0b4a-4d59-b8e4-0a7a4267e6c3";
        private const String INVALID_TOKEN = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjbGFpbTEiOjAsImNsYWltMiI6ImNsYWltMi12YWx1ZSJ9.8pwBI_HtXqI3UgQHQ_rDRnSQRxFL1SR8fbQoS-5kM5s";

        [Test, Category("Unit/AuthenticationTests")]
        public void CreateTokenWithInvalidSecretThrowsAuthException()
        {
			Assert.Throws<NotifyAuthException>(() => Authenticator.CreateToken("invalidsecret", NOTIFY_SERVICE_ID));
        }

		[Test, Category("Unit/AuthenticationTests")]
        public void CreateTokenWithInvalidServiceIdThrowsAuthException()
        {
			Assert.Throws<NotifyAuthException>(() => Authenticator.CreateToken(NOTIFY_SECRET_ID, "invalid service id"));
        }

		[Test, Category("Unit/AuthenticationTests")]
        public void CreateTokenWithCredentialsShouldGenerateValidJWT()
        {
            String token = Authenticator.CreateToken(NOTIFY_SECRET_ID, NOTIFY_SERVICE_ID);

            // Is correct string
            if(String.IsNullOrWhiteSpace(token) || token.Contains(" "))
            {
                Assert.Fail();
            }

            // Validate can decode and payload matches
            IDictionary<String, Object> jsonPayload = Authenticator.DecodeToken(token, NOTIFY_SECRET_ID);
            Assert.AreEqual(jsonPayload["iss"], NOTIFY_SERVICE_ID);

            // Validate token issed time is within reasonable time
            Double currentTimeAsSeconds = Authenticator.GetCurrentTimeAsSeconds();
            Double tokenIssuedAt = Convert.ToDouble(jsonPayload["iat"]);
            if(tokenIssuedAt < (currentTimeAsSeconds - 15) || 
                tokenIssuedAt > (currentTimeAsSeconds + 15))
            {
                Assert.Fail();
            }

        }

		[Test, Category("Unit/AuthenticationTests")]
        public void DecodeInvalidTokenWithNoDotsShouldThrowAuthException()
        {
					Assert.Throws<NotifyAuthException>(() => Authenticator.DecodeToken("tokenwithnodots", NOTIFY_SECRET_ID));
        }

		[Test, Category("Unit/AuthenticationTests")]
        public void DecodeInvalidTokenShouldThrowAuthException()
        {
					Assert.Throws<NotifyAuthException>(() => Authenticator.DecodeToken(INVALID_TOKEN, NOTIFY_SECRET_ID));
        }
    }
}