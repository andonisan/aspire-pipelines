using Aspire.Hosting;
using Aspire.Hosting.Pipelines;

#pragma warning disable ASPIREPIPELINES001

namespace AppHost.CISteps;

public static class DotNetCIStepsExtensions
{
    extension(IResourceBuilder<ProjectResource> builder)
    {
        public IResourceBuilder<ProjectResource> WithRestoreStep() => builder.WithRestoreStep([]);

        public IResourceBuilder<ProjectResource> WithRestoreStep(string[] args)
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;
                // For .NET projects, use the current directory (AppHost location)
                // dotnet CLI will find the project based on the solution or project references
                var workingDir = ".";

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Install}-dotnet-restore-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", string.Join(" ", ["restore", ..args]), workingDir, ctx.Logger),
                    RequiredBySteps = [WellKnownCIStepNames.Install]
                };
            });
        }

        public IResourceBuilder<ProjectResource> WithBuildStep(string configuration = "Release")
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;
                var workingDir = ".";

                return new PipelineStep
                {
                    Name = $"dotnet-build-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", $"build --no-restore --configuration {configuration}", workingDir, ctx.Logger),
                    DependsOnSteps = [$"{WellKnownCIStepNames.Install}-dotnet-restore-{resource.Name}"]
                };
            });
        }



        public IResourceBuilder<ProjectResource> WithFormatCheckStep()
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;
                var workingDir = ".";

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Lint}-dotnet-format-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", "format --verify-no-changes --no-restore", workingDir, ctx.Logger),
                    RequiredBySteps = [WellKnownCIStepNames.Lint],
                    DependsOnSteps = [$"dotnet-build-{resource.Name}"]
                };
            });
        }

        public IResourceBuilder<ProjectResource> WithDotNetTestStep(string configuration = "Release")
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;
                var workingDir = ".";

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Test}-dotnet-test-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", $"test --no-build --configuration {configuration}", workingDir, ctx.Logger),
                    RequiredBySteps = [WellKnownCIStepNames.Test],
                    DependsOnSteps = [$"dotnet-build-{resource.Name}"]
                };
            });
        }

        // Note: Overloads with string[] args temporarily removed to avoid duplicate step names
        // Will be added back once we find a solution for the annotation issue
        /*
        public IResourceBuilder<ProjectResource> WithDotNetTestStep(string[] args)
        {
            if (builder.Resource.TryGetLastAnnotation<DotNetTestAnnotation>(out _))
            {
                return builder;
            }

            return builder
                .WithAnnotation(new DotNetTestAnnotation())
                .WithPipelineStepFactory(factoryContext =>
                {
                    var resource = factoryContext.Resource;
                    var workingDir = ".";

                    return new PipelineStep
                    {
                        Name = $"{WellKnownCIStepNames.Test}-dotnet-test-{resource.Name}",
                        Action = ctx => CLIHelper.RunProcess("dotnet", string.Join(" ", ["test", "--no-build", ..args]), workingDir, ctx.Logger),
                        RequiredBySteps = [WellKnownCIStepNames.Test],
                        DependsOnSteps = [$"build-{resource.Name}"]
                    };
                });
        }
        */
    }
}

// Annotations to track which steps have been added
internal class DotNetRestoreAnnotation : IResourceAnnotation { }
internal class DotNetBuildAnnotation : IResourceAnnotation { }
internal class DotNetFormatAnnotation : IResourceAnnotation { }
internal class DotNetTestAnnotation : IResourceAnnotation { }
