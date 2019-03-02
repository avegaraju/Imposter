# Imposter [![Build status](https://ci.appveyor.com/api/projects/status/j52diltc7mksc8fc?svg=true)](https://ci.appveyor.com/project/avegaraju/imposter)

Project Imposter provides simple test doubles over the wire. It is easy to setup and configure. Project imposter supports stubbing as well as mocking of REST resources.


Imposter  package can be referenced from Nuget. Configure Nuget as your package source and install the package.
```
Install-Package Appify.FluentImposter.AspnetCore
```

Currently, the package supports AspnetCore projects only.

As a first step, Create a .Net Core console application and build the WebHost.

```
static void Main(string[] args)
{
    var host =  WebHost.CreateDefaultBuilder(args)
           .UseUrls("http://localhost:5000")
           .UseKestrel()
           .UseStartup<Startup>()
           .Build();

    host.Start();

    Console.ReadLine();
}
```

This will create a base Url for the imposters.

* Create your first Imposter

Creating an Imposter is easy. Just create a class and implement ```IImposter``` interface.

For example, below imposter would stub the REST resource which creates a user.

```
public class CustomerImposter: IImposter
{
        public Imposter Build()
        {
            return new ImposterDefinition("CustomersStub")
                    .DeclareResource("/api/Customers", HttpMethod.Post)
                    .When(r => r.Content.Contains("Name:Jack"))
                    .Then(new FailedToCreateCustomerResponseCreator())
                    .Build();
        }
}
```

The Fluent Api helps to build an imposter easily. In this case, an imposter with name ```CreateUserStub``` is being built, which stubs a REST resource ```http://localhost:5000/users``` and accepts ```Post``` requests.

You can define conditions when this imposter should be invoked. In this case, when the request body contains an email address ```abc@xyz.com``` then this imposter will be invoked and will respond with a fake response.

* Creating fake response.

Fake responses are easy to create. Just Create a class and implement ```IResponseCreator``` interface as shown below.

```
public class FailedToCreateCustomerResponseCreator : IResponseCreator
{
        public Response CreateResponse()
        {
            var orderCreatedResponse = new CreateCustomerResponse()
                                       {
                                           Message = "Customer creation failed.",
                                       };

            return new ResponseBuilder().WithContent(orderCreatedResponse, new JsonContentSerializer())
                                        .WithStatusCode(HttpStatusCode.InternalServerError)
                                        .Build();
        }
}
```

The `ResponseBuilder` will help you build the `Response`.

* Use the Imposter


```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ImpostersAsStubConfiguration stubConfiguration =
                    new ImpostersAsStubConfiguration(new StubImpostersBuilder()
                                                             .CreateStubImposters());

            app.UseStubImposters(stubConfiguration);

            /*Uncomment below code when DynamoDB is available at http://localhost:8000*/

            //ImpostersAsMockConfiguration mockConfiguration =
            //        new ImpostersAsMockConfiguration(new MockImpostersBuilder()
            //                                                 .CreateMockImposters(),
            //                                         new MocksDataStore()
            //                                                 .Create());

            //app.UseMockImposters(mockConfiguration);
}

```
As simple as that. ```ImpostersAsStubConfiguration ``` accepts an array of Imposters and ```UseStubImposters``` extension method of ```IApplicationBuilder``` accpets the ```ImpostersAsStubConfiguration ``` to host the imposters as REST resources.

Apart from the above, you need to call ```AddRouting``` on the ```IServiceCollection``` instance as shown below. The reason, you'd need to do that is because the imposter middleware creates and add ```Routes``` for each imposter REST resource. Without adding routing, imposters will not be able to receive requests from the client.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddRouting();
}
```


