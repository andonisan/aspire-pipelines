# Aspire Pipelines - CI/CD con Código

Este proyecto demuestra cómo usar **.NET Aspire Pipelines** para crear pipelines de CI/CD como código en aplicaciones multi-servicio con diferentes lenguajes (JavaScript, Python, .NET).

## 🎯 ¿Qué es Aspire Pipelines?

Aspire Pipelines permite definir comandos estándar (setup, install, lint, test, build) para todos los servicios de tu aplicación, independientemente del lenguaje que usen. Esto facilita:

- ✅ Onboarding de nuevos desarrolladores
- ✅ Integración con CI/CD (Azure DevOps, GitHub Actions)
- ✅ Ejecución local consistente
- ✅ Soporte para aplicaciones polyglot

## 🏗️ Estructura del Proyecto

```
src/
├── aspire-pipelines.AppHost/     # Orquestador Aspire con definición de pipelines
│   └── CISteps/
│       ├── DotNetCIStepsExtensions.cs      # ✨ CI steps para .NET
│       ├── JavaScriptCIStepsExtensions.cs  # CI steps para JavaScript
│       └── PythonCIStepsExtensions.cs      # CI steps para Python
├── aspire-pipelines.Server/      # Proyecto ASP.NET Core (API)
└── frontend/                     # Aplicación Vite (JavaScript)
```

## 🚀 Comandos Disponibles

### 1. Setup
Instala herramientas y prepara el entorno de desarrollo:

```bash
cd src/aspire-pipelines.AppHost
aspire do setup
```

**Qué hace:**
- Instala `uv` para proyectos Python (si los hubiera)
- Prepara el entorno para cada servicio

### 2. Install
Restaura/instala dependencias de todos los servicios:

```bash
aspire do install
```

**Qué hace:**
- **JavaScript**: `npm install` (o yarn/pnpm según el proyecto)
- **Python**: `uv sync`
- **.NET**: `dotnet restore`

### 3. Lint
Ejecuta linters y validación de formato en todos los servicios:

```bash
aspire do lint
```

**Qué hace:**
- **JavaScript**: `npm run lint`
- **Python**: `uv run ruff check` y otros linters configurados  
- **.NET**: `dotnet build` (si es necesario) y luego `dotnet format --verify-no-changes`

### 4. Test
Ejecuta tests de todos los servicios:

```bash
aspire do test
```

**Qué hace:**
- **JavaScript**: `npm run test` (si está configurado)
- **Python**: `uv run pytest`
- **.NET**: `dotnet build` (si es necesario) y luego `dotnet test --no-build`

### 5. CI Complete
Ejecuta todo el pipeline CI (setup → install → lint → test):

```bash
aspire do ci
```

### 6. Run (Desarrollo)
Ejecuta la aplicación en modo desarrollo:

```bash
aspire run
```

## 🔧 Configuración de CI Steps para .NET

En [AppHost.cs](src/aspire-pipelines.AppHost/AppHost.cs), se configuran los CI steps para cada servicio:

```csharp
var server = builder.AddProject<Projects.aspire_pipelines_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithRestoreStep()                // dotnet restore
    .WithBuildStep()                  // dotnet build --no-restore --configuration Release
    .WithFormatCheckStep()            // dotnet format --verify-no-changes
    .WithDotNetTestStep();            // dotnet test --no-build --configuration Release
```

## 🎨 Extensiones Disponibles

### .NET (`DotNetCIStepsExtensions`)
```csharp
.WithRestoreStep()                           // dotnet restore (argumentos por defecto)
.WithRestoreStep(["--force"])                // Con argumentos personalizados

.WithBuildStep()                             // dotnet build --configuration Release
.WithBuildStep("Debug")                      // Con configuración personalizada

.WithFormatCheckStep()                       // dotnet format --verify-no-changes
                                             // (se ejecuta después del build)

.WithDotNetTestStep()                        // dotnet test --configuration Release
.WithDotNetTestStep("Debug")                 // Con configuración personalizada
```

**Nota**: Los steps de .NET se ejecutan en orden: Restore → Build → Format/Test. El step de Build se ejecuta automáticamente cuando se llama a `aspire do lint` o `aspire do test`.

### JavaScript (`JavaScriptCIStepsExtensions`)
```csharp
.WithInstallationStep()                      // npm/yarn/pnpm install
.WithInstallationStep(["--frozen-lockfile"]) 
.WithLintingStep()                           // npm run lint
```

### Python (`PythonCIStepsExtensions`)
```csharp
builder.AddUvPythonSetup()                   // Instala uv globalmente

.WithUvInstallationStep()                    // uv sync
.WithUvLintingSteps([...])                   // uv run <linter>
.WithUvTestingStep([...])                    // uv run pytest
```

## 📋 Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (para el frontend)

## 🔄 Integración con CI/CD

### Azure DevOps

El proyecto incluye [.azdo/azure-pipelines.yml](.azdo/azure-pipelines.yml) con dos stages:

**CI Stage**: Ejecuta el pipeline completo
```yaml
- script: |
    cd src/aspire-pipelines.AppHost
    aspire do ci
  displayName: 'Run Aspire CI Pipeline'
```

**CD Stage**: Despliega a Azure (solo en rama `main`)
```yaml
- script: |
    cd src/aspire-pipelines.AppHost
    aspire deploy --subscription-id $(AZURE_SUBSCRIPTION_ID) --location $(AZURE_LOCATION)
  displayName: 'Deploy with Aspire'
```

**Configurar variables en Azure DevOps**:
- `AZURE_SUBSCRIPTION_ID`: ID de tu suscripción de Azure
- `AZURE_LOCATION`: Región de Azure (ej: `eastus`)

### GitHub Actions

El proyecto incluye [.github/workflows/ci-cd.yml](.github/workflows/ci-cd.yml) con dos jobs:

**CI Job**: Ejecuta el pipeline completo
```yaml
- name: Run Aspire CI Pipeline
  working-directory: src/aspire-pipelines.AppHost
  run: aspire do ci
```

**CD Job**: Despliega a Azure (solo en rama `main`)
```yaml
- name: Deploy with Aspire
  working-directory: src/aspire-pipelines.AppHost
  run: |
    aspire deploy --subscription-id ${{ secrets.AZURE_SUBSCRIPTION_ID }} --location ${{ secrets.AZURE_LOCATION }}
```

**Configurar secrets en GitHub**:
1. Ve a Settings → Secrets and variables → Actions
2. Añade los siguientes secrets:
   - `AZURE_CREDENTIALS`: JSON con las credenciales de Service Principal
   - `AZURE_SUBSCRIPTION_ID`: ID de tu suscripción de Azure
   - `AZURE_LOCATION`: Región de Azure (ej: `eastus`)

## 🎯 Ventajas de Aspire Pipelines

✅ **Un solo comando**: `aspire do ci` ejecuta setup, install, lint y test para todos los servicios  
✅ **Consistencia**: Misma experiencia local y en CI/CD  
✅ **Polyglot**: Soporta JavaScript, Python, .NET y más en un solo pipeline  
✅ **Simplicidad**: No necesitas conocer los comandos específicos de cada lenguaje  
✅ **Mantenibilidad**: Los pipelines CI/CD son más simples y fáciles de mantener

## 📚 Recursos

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Pipelines (Experimental)](https://github.com/dotnet/aspire)
- [Documentación de la Charla](docs/slides/slides.md)

---

**Nota**: Aspire Pipelines es una característica experimental y puede cambiar en futuras versiones.
