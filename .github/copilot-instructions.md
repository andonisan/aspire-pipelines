# Copilot Instructions - Aspire Pipelines

## 🎯 Contexto del Proyecto

Este es un **proyecto de demostración para una charla** sobre cómo usar **.NET Aspire Pipelines** para crear pipelines de CI/CD como código en aplicaciones multi-servicio.

## 🏗️ Arquitectura

- **AppHost** (`src/aspire-pipelines.AppHost/`): Orquestador Aspire con definición de pipelines
- **Server** (`src/aspire-pipelines.Server/`): API ASP.NET Core
- **Frontend** (`src/frontend/`): Aplicación Vite con JavaScript
- **Pipelines**: `.azdo/azure-pipelines.yml` y `.github/workflows/ci-cd.yml`
- **Slides**: `docs/slides/` contiene presentación Slidev para la charla

## 🔧 Stack Tecnológico

- **.NET 10 SDK** con preview features
- **Aspire 13.1.0** (Aspire.Hosting.Pipelines - experimental)
- **Aspire.Hosting.JavaScript 13.1.0**
- **Aspire.Hosting.Python 13.1.0**
- **Vite + TypeScript** para frontend
- **#pragma warning disable ASPIREPIPELINES001** requerido

## 📦 Implementación de CI Steps

### Extensiones Disponibles

#### .NET (`DotNetCIStepsExtensions.cs`)
- `WithRestoreStep()` / `WithRestoreStep(string[] args)` → `dotnet restore`
- `WithBuildStep(string configuration = "Release")` → `dotnet build --no-restore`
- `WithFormatCheckStep()` → `dotnet format --verify-no-changes`
- `WithDotNetTestStep(string configuration = "Release")` → `dotnet test --no-build`

#### JavaScript (`JavaScriptCIStepsExtensions.cs`)
- `WithInstallationStep()` → npm/yarn/pnpm install
- `WithLintingStep()` → npm run lint

#### Python (`PythonCIStepsExtensions.cs`)
- `AddUvPythonSetup()` → Instala uv globalmente
- `WithUvInstallationStep()` → uv sync
- `WithUvLintingSteps()` → uv run ruff check
- `WithUvTestingStep()` → uv run pytest

### Patrón de Implementación

```csharp
var server = builder.AddProject<Projects.aspire_pipelines_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithRestoreStep()
    .WithBuildStep()
    .WithFormatCheckStep()
    .WithDotNetTestStep();

builder.WithCISteps();

builder.Pipeline.AddStep(new PipelineStep
{
    Name = "ci",
    Description = "CI Pipeline",
    DependsOnSteps = [WellKnownCIStepNames.Setup, WellKnownCIStepNames.Install, 
                      WellKnownCIStepNames.Lint, WellKnownCIStepNames.Test]
});
```

## ⚠️ Consideraciones Importantes

### Naming de Steps - CRÍTICO
- **NO usar** `"build-{resource.Name}"` → Causa conflicto con el step automático de Aspire para container builds
- **SÍ usar** `"dotnet-build-{resource.Name}"` o similar con prefijo específico del lenguaje
- Aspire automáticamente crea steps `"build-{resourceName}"` para resources con container images

### Working Directory
- Para **ProjectResource**: Usar `"."` (current directory)
- El dotnet CLI encuentra proyectos automáticamente desde la solution
- `resource.WorkingDirectory` no funciona para ProjectResource (no tiene ExecutableAnnotation)

### Dependencies entre Steps
- **Restore** → RequiredBySteps = [Install]
- **Build** → DependsOnSteps = [Install-dotnet-restore-{name}]
- **Format/Test** → DependsOnSteps = [dotnet-build-{name}]

### CLIHelper
- Usar `CLIHelper.RunProcess()` para ejecutar comandos CLI
- Working directory configurado con `WorkingDirectory = "."`
- Parámetros: `(ctx, command, args, WorkingDirectory, Env)`

## 🚀 Comandos Disponibles

```bash
cd src/aspire-pipelines.AppHost

# Comandos individuales
aspire do setup    # Prepara entorno
aspire do install  # Instala dependencias (dotnet restore, npm install, uv sync)
aspire do lint     # Linting y formato (dotnet format, npm run lint)
aspire do test     # Tests (dotnet test, npm test, pytest)

# Comando completo
aspire do ci       # Ejecuta: setup → install → lint → test

# Desarrollo
aspire run         # Ejecuta aplicación
```

## 📋 CI/CD Pipelines

### Azure DevOps (`.azdo/azure-pipelines.yml`)
- **CI Stage**: `aspire do ci`
- **CD Stage**: `aspire deploy` (solo en main)
- Variables: `AZURE_SUBSCRIPTION_ID`, `AZURE_LOCATION`

### GitHub Actions (`.github/workflows/ci-cd.yml`)
- **CI Job**: `aspire do ci`
- **CD Job**: `aspire deploy` (solo en main)
- Secrets: `AZURE_CREDENTIALS`, `AZURE_SUBSCRIPTION_ID`, `AZURE_LOCATION`

## 🎤 Slides (Slidev)

La presentación está en `docs/slides/`:
- Archivo principal: `slides.md`
- Configuración: `package.json`, `netlify.toml`, `vercel.json`
- Diagramas: `diagrams/*.excalidraw`
- Tema custom: `theme/rider.ts`, `styles/rider.css`

## 🎨 Estilo de Código

- Usar **file links** con markdown: `[file.cs](path/file.cs)` o `[método](file.cs#L10)`
- **NO usar backticks** para nombres de archivos
- Usar backticks para: símbolos (`MyClass`), métodos (`DoSomething()`), variables
- Brevedad en respuestas cuando sea apropiado

## 📚 Referencias Útiles

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Pipelines](https://github.com/dotnet/aspire) - Característica experimental
- ADR: `docs/adr/` contiene decisiones arquitecturales

---

**Objetivo del proyecto**: Demostrar cómo Aspire Pipelines simplifica CI/CD en aplicaciones polyglot, permitiendo `aspire do ci` en lugar de conocer comandos específicos de cada lenguaje.
