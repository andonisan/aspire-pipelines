using Aspire.Hosting.Pipelines;
using Microsoft.Extensions.Logging;

#pragma warning disable ASPIREPIPELINES001

namespace AppHost.CISteps;

internal class SetupAnnotation : IResourceAnnotation
{
}

internal class InstallationAnnotation : IResourceAnnotation
{
}

internal class LintingAnnotation : IResourceAnnotation
{
}

internal class TestingAnnotation : IResourceAnnotation
{
}

public static class CIStepExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IDistributedApplicationBuilder WithSetup()
        {
            builder.Pipeline.AddStep(WellKnownCIStepNames.Setup, context =>
            {
                context.Logger.LogInformation("Setup step completed successfully.");
                return Task.CompletedTask;
            });

            return builder;
        }

        public IDistributedApplicationBuilder WithInstallation()
        {
            builder.Pipeline.AddStep(WellKnownCIStepNames.Install, context =>
            {
                context.Logger.LogInformation("Installation step completed successfully.");
                return Task.CompletedTask;
            });

            return builder;
        }

        public IDistributedApplicationBuilder WithLinting()
        {
            builder.Pipeline.AddStep(WellKnownCIStepNames.Lint, context =>
            {
                context.Logger.LogInformation("Linting finished successfully.");
                return Task.CompletedTask;
            });
            return builder;
        }

        public IDistributedApplicationBuilder WithTesting()
        {
            builder.Pipeline.AddStep(WellKnownCIStepNames.Test, context =>
            {
                context.Logger.LogInformation("Testing finished successfully.");
                return Task.CompletedTask;
            });
            return builder;
        }

        public IDistributedApplicationBuilder WithCiSteps()
        {
            builder.Pipeline.AddStep(new PipelineStep
            {
                Name = "ci",
                Description = "CI Pipeline",
                Action = ctx =>
                {
                    ctx.Logger.LogInformation("CI finished successfully");
                    return Task.CompletedTask;
                },
                DependsOnSteps = [WellKnownCIStepNames.Setup, WellKnownCIStepNames.Install, WellKnownCIStepNames.Lint, WellKnownCIStepNames.Test]
            });

            return builder
                .WithSetup()
                .WithInstallation()
                .WithLinting()
                .WithTesting();
        }
    }

    extension<T>(IResourceBuilder<T> builder) where T : IResource
    {
        // To allow specific installation commands like winget / apt get install specific dependencies for the app
        // Can be copied to the docker file!
        internal IResourceBuilder<T> WithSetupStep()
        {
            if (builder.Resource.HasAnnotationOfType<SetupAnnotation>())
            {
                return builder;
            }

            return builder.WithAnnotation<SetupAnnotation>()
                .WithPipelineStepFactory(factoryContext =>
                {
                    var resource = factoryContext.Resource;

                    return new PipelineStep
                    {
                        Name = builder.SetupStepName,
                        Action = ctx =>
                        {
                            ctx.Logger.LogInformation("Setup for {resourceName} completed successfully.", resource.Name);
                            return Task.CompletedTask;
                        },
                        RequiredBySteps = [WellKnownCIStepNames.Setup]
                    };
                });
        }

        internal string SetupStepName => $"{WellKnownCIStepNames.Setup}-{builder.Resource.Name}";

        internal IResourceBuilder<T> WithInstallationStep()
        {
            if (builder.Resource.HasAnnotationOfType<InstallationAnnotation>())
            {
                return builder;
            }

            return builder.WithAnnotation<InstallationAnnotation>()
                .WithPipelineStepFactory(factoryContext =>
                {
                    var resource = factoryContext.Resource;

                    return new PipelineStep
                    {
                        Name = builder.InstallStepName,
                        Action = ctx =>
                        {
                            ctx.Logger.LogInformation("Installation for {resourceName} completed successfully.", resource.Name);
                            return Task.CompletedTask;
                        },
                        RequiredBySteps = [WellKnownCIStepNames.Install]
                    };
                });
        }

        internal string InstallStepName => $"{WellKnownCIStepNames.Install}-{builder.Resource.Name}";

        internal IResourceBuilder<T> WithLintingStep()
        {
            if (builder.Resource.HasAnnotationOfType<LintingAnnotation>())
            {
                return builder;
            }

            return builder.WithAnnotation<LintingAnnotation>()
                .WithPipelineStepFactory(factoryContext =>
                {
                    var resource = factoryContext.Resource;

                    return new PipelineStep
                    {
                        Name = $"{WellKnownCIStepNames.Lint}-{resource.Name}",
                        Action = ctx =>
                        {
                            ctx.Logger.LogInformation("Linting for {resourceName} completed successfully.", resource.Name);
                            return Task.CompletedTask;
                        },
                        RequiredBySteps = [WellKnownCIStepNames.Lint]
                    };
                });
        }

        internal string LintingStepName => $"{WellKnownCIStepNames.Lint}-{builder.Resource.Name}";


        internal IResourceBuilder<T> WithTestingStep()
        {
            if (builder.Resource.HasAnnotationOfType<TestingAnnotation>())
            {
                return builder;
            }

            return builder.WithAnnotation<TestingAnnotation>()
                .WithPipelineStepFactory(factoryContext =>
                {
                    var resource = factoryContext.Resource;

                    return new PipelineStep
                    {
                        Name = builder.TestingStepName,
                        Action = ctx =>
                        {
                            ctx.Logger.LogInformation("Testing for {resourceName} completed successfully.", resource.Name);
                            return Task.CompletedTask;
                        },
                        RequiredBySteps = [WellKnownCIStepNames.Test]
                    };
                });
        }

        internal string TestingStepName => $"{WellKnownCIStepNames.Test}-{builder.Resource.Name}";
    }
}