using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

using Newtonsoft.Json;

namespace SampleClientService
{
    public class UserIdentityImposter: IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .DeclareResource("users", HttpMethod.Post)
                    .When(r => r.Content.Contains("abc@xyz.com"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }

    internal class DummyResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            var userCreatedResponse = new DummyUserCreatedResponse()
                                      {
                                          Message = "User created successfully.",
                                          UserId = new Random().Next(Int32.MaxValue)
                                      };

            return new ResponseBuilder()
                    .WithContent(userCreatedResponse, new JsonContentSerializer())
                    .WithStatusCode(HttpStatusCode.Created)
                    .Build();
        }
    }

    internal class DummyUserCreatedResponse
    {
        public int UserId { get; set; }
        public string Message { get; set; }
    }

    internal class JsonContentSerializer: IContentSerializer
    {
        public string Serialize(object content)
        {
            var jsonSerializer = new JsonSerializer();

            StringBuilder contentStringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(contentStringBuilder))
            {
                jsonSerializer.Serialize(stringWriter, content);
            }

            return contentStringBuilder.ToString();
        }
    }
}
