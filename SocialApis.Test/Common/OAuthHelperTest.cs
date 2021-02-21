using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using System.Text;
using SocialApis.Utils;
using SocialApis.Core;

namespace SocialApis.Test.Common
{
    public class OAuthHelperTest
    {
        [Fact]
        public void UrlEncode_Empty()
        {
            Assert.Equal("", OAuthHelper.UrlEncode(""));
        }

        [Fact]
        public void UrlEncode_FullChars()
        {
            Assert.Equal("%E3%81%82%E3%81%84%E3%81%86%E3%81%88%E3%81%8A", OAuthHelper.UrlEncode("あいうえお"));
        }

        [Fact]
        public void UrlEncode_TwitterSample()
        {
            Assert.Equal("Ladies%20%2B%20Gentlemen", OAuthHelper.UrlEncode("Ladies + Gentlemen"));
            Assert.Equal("An%20encoded%20string%21", OAuthHelper.UrlEncode("An encoded string!"));
            Assert.Equal("Dogs%2C%20Cats%20%26%20Mice", OAuthHelper.UrlEncode("Dogs, Cats & Mice"));
            Assert.Equal("%E2%98%83", OAuthHelper.UrlEncode("☃"));
        }

        [Fact]
        public void GenearteSignatureBase()
        {
            var method = HttpMethod.Post;
            var endpoint = new Uri("https://api.twitter.com/1.1/statuses/update.json");
            var parameters = new Dictionary<string, object>
            {
                ["include_entities"] = true,
                ["oauth_consumer_key"] = "xvz1evFS4wEEPTGEFPHBog",
                ["oauth_nonce"] = "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg",
                ["oauth_signature_method"] = "HMAC-SHA1",
                ["oauth_timestamp"] = 1318622958,
                ["oauth_token"] = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb",
                ["oauth_version"] = "1.0",
                ["status"] = "Hello Ladies + Gentlemen, a signed OAuth request!",
            };
            var queryParameter = OAuthHelper.UrlEncode(QueryParameterFactory.ToStringNameValuePairs(parameters));

            var expected = "POST&https%3A%2F%2Fapi.twitter.com%2F1.1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521";
            var actual = OAuthHelper.GenerateSignatureBase(method, endpoint, queryParameter);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GenerateSignature()
        {
            var signatureBase = "POST&https%3A%2F%2Fapi.twitter.com%2F1.1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521";

            var consumerSecret = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
            var tokenSecret = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";

            var expected = "hCtSmYh+iHYCEqBWrE7C7hYmtUk=";
            var actual = OAuthHelper.GenerateSignature(consumerSecret, tokenSecret, signatureBase);

            Assert.Equal(expected, actual);
        }
    }
}
