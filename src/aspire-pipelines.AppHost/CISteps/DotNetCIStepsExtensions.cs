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

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Install}-dotnet-restore-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", string.Join(" ", ["restore", .. args]),
                        WorkingDirectory(resource), ctx.Logger),
                    RequiredBySteps = [WellKnownCIStepNames.Install]
                };
            });
        }

        public IResourceBuilder<ProjectResource> WithBuildStep(string configuration = "Release")
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;

                return new PipelineStep
                {
                    Name = $"dotnet-build-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet",
                        $"build --no-restore --configuration {configuration}", WorkingDirectory(resource), ctx.Logger),
                    DependsOnSteps = [$"{WellKnownCIStepNames.Install}-dotnet-restore-{resource.Name}"]
                };
            });
        }

        public IResourceBuilder<ProjectResource> WithFormatCheckStep()
        {
            return builder.WithPipelineStepFactory(factoryContext =>
            {
                var resource = factoryContext.Resource;

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Lint}-dotnet-format-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", "format --verify-no-changes --no-restore",
                        WorkingDirectory(resource), ctx.Logger),
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

                return new PipelineStep
                {
                    Name = $"{WellKnownCIStepNames.Test}-dotnet-test-{resource.Name}",
                    Action = ctx => CLIHelper.RunProcess("dotnet", $"test --configuration {configuration}",
                        WorkingDirectory(resource), ctx.Logger),
                    RequiredBySteps = [WellKnownCIStepNames.Test],
                    DependsOnSteps = [$"dotnet-build-{resource.Name}"]
                };
            });
        }
    }

    public static string WorkingDirectory(IResource resource)
    {
        var initial = resource.WorkingDirectory;
        var currentDir = initial;

        // For ProjectResource, WorkingDirectory returns "." so we just return it
        if (initial == ".")
        {
            return initial;
        }

        while (currentDir != null)
        {
            var slnxFiles = Directory.GetFiles(currentDir, "*.slnx");
            if (slnxFiles.Length > 0)
            {
                return currentDir;
            }

            currentDir = Path.GetDirectoryName(currentDir);
        }
        return initial;
    }
}

// Annotations to track which steps have been added
internal class DotNetRestoreAnnotation : IResourceAnnotation { }
internal class DotNetBuildAnnotation : IResourceAnnotation { }
internal class DotNetFormatAnnotation : IResourceAnnotation { }
internal class DotNetTestAnnotation : IResourceAnnotation { }
