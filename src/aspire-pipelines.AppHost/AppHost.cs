using AppHost.CISteps;
#pragma warning disable ASPIREPIPELINES001

var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.aspire_pipelines_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithRestoreStep()
    .WithBuildStep()
    .WithFormatCheckStep()
    .WithDotNetTestStep();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithNpm(install: true)
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.WithCiSteps();

builder.Build().Run();
