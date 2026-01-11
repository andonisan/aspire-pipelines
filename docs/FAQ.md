# FAQ - Aspire Pipelines

Preguntas frecuentes sobre el uso de Aspire Pipelines para CI/CD.

## General

### ¿Qué es Aspire Pipelines?

Aspire Pipelines es una característica experimental de .NET Aspire 13.1 que permite definir pipelines de CI/CD como código C# en el AppHost. En lugar de tener múltiples scripts YAML dispersos, defines todo en un solo lugar con type-safety y ejecutas con comandos simples como `aspire do ci`.

### ¿Es Aspire Pipelines production-ready?

**No, es experimental** (Aspire 13.1). La API puede cambiar en futuras versiones. Sin embargo, es estable para proyectos internos, demos, y early adopters dispuestos a actualizar código si hay breaking changes.

### ¿Funciona solo con Azure?

No. Aunque Azure Container Apps tiene la mejor integración (`aspire deploy`), puedes:
- Usar Docker SSH Deploy para VPS/on-premise
- Generar manifests para Kubernetes
- Usar cualquier CI/CD que ejecute .NET CLI

La parte de *pipelines CI* (`aspire do ci`) funciona en cualquier entorno que tenga .NET SDK.

### ¿Cuándo sale de experimental?

Microsoft no ha anunciado fecha oficial, pero se espera estabilización en Aspire 14 o 15 (estimado 2026).

## Configuración

### ¿Cómo agrego CI steps a mi proyecto existente?

```csharp
// En AppHost.cs
#pragma warning disable ASPIREPIPELINES001

var server = builder.AddProject<Projects.Server>("server")
    .WithRestoreStep()
    .WithBuildStep()
    .WithTestStep();

builder.WithCISteps();
```

Luego ejecuta: `aspire do ci`

### ¿Puedo personalizar los comandos de CI?

Sí, puedes crear custom steps:

```csharp
builder.Pipeline.AddStep(new PipelineStep
{
    Name = "custom-security-scan",
    Description = "Security vulnerability scan",
    RequiredBySteps = [WellKnownCIStepNames.Test],
    Implementation = async () =>
    {
        await CLIHelper.RunProcess(ctx, "trivy", ["scan", "."]);
    }
});
```

### ¿Cómo añado support para otros lenguajes (Go, Rust, Java)?

Crea extensiones siguiendo el patrón de [DotNetCIStepsExtensions.cs](../src/aspire-pipelines.AppHost/CISteps/DotNetCIStepsExtensions.cs):

```csharp
public static class GoCIStepsExtensions
{
    public static IResourceBuilder<T> WithGoTestStep<T>(
        this IResourceBuilder<T> builder) 
        where T : IResourceWithExecutable
    {
        return builder.WithCIStep(ctx =>
        {
            ctx.Step = new PipelineStep
            {
                Name = $"go-test-{ctx.Resource.Name}",
                RequiredBySteps = [WellKnownCIStepNames.Test],
                Implementation = async () =>
                {
                    await CLIHelper.RunProcess(ctx, "go", 
                        ["test", "./..."],
                        WorkingDirectory: ctx.Resource.WorkingDirectory);
                }
            };
        });
    }
}
```

## CI/CD

### ¿Cómo integro con GitHub Actions?

```yaml
name: CI/CD
on: [push, pull_request]

jobs:
  ci:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Run CI
        working-directory: src/aspire-pipelines.AppHost
        run: aspire do ci
```

### ¿Cómo integro con Azure DevOps?

```yaml
trigger: [main, develop]

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '10.0.x'
  - script: |
      cd src/aspire-pipelines.AppHost
      aspire do ci
    displayName: 'Run Aspire CI'
```

### ¿Puedo ejecutar solo un step específico?

Sí:

```bash
aspire do setup    # Solo setup
aspire do install  # Solo install
aspire do lint     # Solo lint
aspire do test     # Solo test
```

### ¿Cómo cacheo dependencies en CI?

**GitHub Actions**:
```yaml
- uses: actions/cache@v4
  with:
    path: |
      ~/.nuget/packages
      ~/.npm
      ~/.cache/uv
    key: ${{ runner.os }}-deps-${{ hashFiles('**/*.csproj', '**/package-lock.json', '**/uv.lock') }}
```

**Azure DevOps**:
```yaml
- task: Cache@2
  inputs:
    key: 'nuget | "$(Agent.OS)" | **/packages.lock.json'
    path: $(NUGET_PACKAGES)
```

## Deployment

### ¿Cómo despliego a Azure Container Apps?

```bash
cd src/aspire-pipelines.AppHost
aspire deploy
```

Primera vez te pedirá:
- Azure subscription ID
- Location (región)
- Resource group name

Aspire crea automáticamente toda la infraestructura necesaria (Bicep).

### ¿Puedo desplegar a mi propio servidor?

Sí, usa Docker SSH Deploy:

```bash
# 1. Instalar package
dotnet nuget add source https://f.feedz.io/davidfowl/aspire/nuget/index.json --name davidfowl-aspire
aspire add docker-sshdeploy

# 2. Configurar
builder.AddDockerComposeEnvironment("prod")
    .WithSshDeploySupport();

# 3. Deploy
aspire deploy
```

Ver [ADR-001](./docs/adr/001-deployment-strategy.md) para comparación completa.

### ¿Cómo gestiono secretos?

**Nunca** commitear secretos. Opciones:

1. **Azure Key Vault** (para Azure):
   ```csharp
   var keyVault = builder.AddAzureKeyVault("keyvault");
   var secret = keyVault.GetSecret("MySecret");
   ```

2. **Variables de entorno en CI/CD**:
   - GitHub: Settings → Secrets and variables
   - Azure DevOps: Pipelines → Variables (marcar como secret)

3. **User Secrets local**:
   ```bash
   dotnet user-secrets set "ConnectionStrings:Default" "value"
   ```

### ¿Cómo hago rollback?

**Azure Container Apps**:
```bash
az containerapp revision list --name myapp --resource-group myapp-rg
az containerapp revision activate --name myapp --revision myapp--abc123
```

**SSH Deploy**: Manual
```bash
ssh user@server
cd /opt/myapp
docker-compose down
git checkout previous-version
docker-compose up -d
```

## Troubleshooting

### "Step failed with no clear error message"

```bash
# Ver logs detallados
aspire do <step> --verbose

# O ejecutar comando directamente para ver output
cd src/aspire-pipelines.AppHost
dotnet restore
dotnet build
```

### "Build works locally but fails in CI"

1. ✅ Verificar version del SDK (global.json)
2. ✅ Verificar lock files committed (packages.lock.json, package-lock.json, uv.lock)
3. ✅ Limpiar: `dotnet clean`, `npm ci` (no `npm install`)
4. ✅ Revisar paths (working directory diferente en CI)

### "Deploy to Azure fails"

```bash
# Ver logs
az containerapp logs show --name myapp --resource-group myapp-rg

# Ver estado
az containerapp show --name myapp --resource-group myapp-rg

# Revisar health check
curl https://myapp.azurecontainerapps.io/health
```

Común:
- Health check endpoint no accesible → Verificar ruta y puerto
- Timeout en startup → Aumentar timeout en AppHost
- Environment variables faltantes → Configurar en Azure Portal

### "ASPIREPIPELINES001 warning"

Es esperado. Pipelines es experimental, así que necesitas:

```csharp
#pragma warning disable ASPIREPIPELINES001
```

Al inicio del AppHost.cs.

## Performance

### ¿Cómo optimizo CI speed?

1. **Cache dependencies** (ver arriba)
2. **Paralelizar steps** independientes (Aspire hace esto automáticamente)
3. **Incremental builds**: `dotnet build --no-restore`
4. **Skip steps innecesarios**: Si solo cambias docs, skip tests

### ¿Cómo optimizo deployment speed?

1. **Multi-stage Docker builds** para reducir tamaño de imágenes
2. **Registry en misma región** que deployment target
3. **Layer caching**: Dependencies primero, código después

## Best Practices

### ¿Debo tener un step de build explícito?

Para .NET, **no es necesario** si tienes `WithBuildStep()` en tu resource. El step de test automáticamente depende del build.

Para lenguajes interpretados (JS, Python), no hay build step separado.

### ¿Cuándo debo crear un step custom?

- Cuando necesitas ejecutar una herramienta específica (security scan, code coverage)
- Cuando el comportamiento por defecto no es suficiente
- Cuando necesitas integrar con sistemas externos

### ¿Debo versionar lock files?

**Sí, siempre**. Lock files (packages.lock.json, package-lock.json, uv.lock) garantizan builds reproducibles. Commítelos al repo.

## Recursos

- [Aspire Docs](https://learn.microsoft.com/dotnet/aspire/)
- [GitHub dotnet/aspire](https://github.com/dotnet/aspire)
- [Aspire Docker SSH Deploy](https://github.com/davidfowl/aspire-ssh-deploy)
- [Este proyecto](https://github.com/andonisan/aspire-pipelines)

## ¿No encuentras tu pregunta?

Abre un issue en [GitHub](https://github.com/andonisan/aspire-pipelines/issues) o pregunta en la comunidad Aspire (Discord, Twitter #dotnetaspire).
