# 📋 Checklist: Assets Pendientes para Slides

## 🎨 Diagramas a Crear (Excalidraw)

### 1. Pipeline Flow Diagram
**Archivo**: `diagrams/pipeline-flow.excalidraw`  
**Contenido**: 
```
Setup → Install → Lint → Test → Build → Deploy
  ↓       ↓        ↓      ↓       ↓        ↓
 uv    restore   format  tests  docker  Azure
     npm install eslint  pytest   
     uv sync    ruff
```

**Usado en**: Slide "Estructura de CI Steps"

### 2. Azure Container Apps Architecture
**Archivo**: `diagrams/azure-architecture.excalidraw`  
**Contenido**:
```
┌─────────────────────────────────────────┐
│   Azure Container Apps Environment      │
│                                          │
│  ┌──────────┐       ┌───────────────┐  │
│  │  Server  │──────▶│ Webfrontend   │  │
│  │(Backend) │       │  (Frontend)   │  │
│  └────┬─────┘       └───────────────┘  │
│       │                                  │
│       ▼                                  │
│  ┌─────────┐                            │
│  │Azure SQL│                            │
│  └─────────┘                            │
│                                          │
│  ┌──────────────┐                       │
│  │  Key Vault   │                       │
│  └──────────────┘                       │
└─────────────────────────────────────────┘
```

**Usado en**: Slide "Arquitectura de Deployment"

### 3. Multi-Environment Promotion Flow
**Archivo**: `diagrams/environment-promotion.excalidraw`  
**Contenido**:
```
┌──────────┐     ┌──────────┐     ┌────────────┐
│   Dev    │────▶│ Staging  │────▶│ Production │
│(Auto CI) │     │(Auto CD) │     │(Manual CD) │
└──────────┘     └──────────┘     └────────────┘
     ↓                ↓                  ↓
  commit          PR merge         Approval
```

**Usado en**: Slide "Promoción entre Entornos"

### 4. Traditional CI/CD vs Aspire Pipelines
**Archivo**: `diagrams/traditional-vs-aspire.excalidraw`  
**Contenido**:
```
Traditional:
┌────────────────────────────────────────┐
│  .gitlab-ci.yml                        │
│  azure-pipelines.yml                   │
│  github-actions.yml                    │
│  Jenkinsfile                           │
│  CircleCI config.yml                   │
└────────────────────────────────────────┘
        ↓ Duplicación, diferentes sintaxis

Aspire Pipelines:
┌────────────────────────────────────────┐
│  AppHost.cs (C# Type-safe)             │
│    .WithRestoreStep()                  │
│    .WithBuildStep()                    │
│    .WithTestStep()                     │
└────────────────────────────────────────┘
        ↓ Un lugar, ejecutable localmente
```

**Usado en**: Slide "¿Por qué Pipeline as Code?"

### 5. Polyglot Application Stack
**Archivo**: `diagrams/polyglot-stack.excalidraw`  
**Contenido**:
```
┌─────────────────────────────────────┐
│         Frontend (Vite)             │
│  TypeScript + React/Vue             │
│  npm install | npm run lint         │
└───────────┬─────────────────────────┘
            │
            ▼
┌─────────────────────────────────────┐
│       Backend (ASP.NET)             │
│         C# + .NET 10                │
│  dotnet restore | dotnet test       │
└───────────┬─────────────────────────┘
            │
            ▼
┌─────────────────────────────────────┐
│      Worker (Python)                │
│       Python 3.12 + uv              │
│   uv sync | pytest | ruff           │
└─────────────────────────────────────┘

Aspire Pipelines: aspire do ci
      ↓
Ejecuta TODOS los pasos de TODAS las tecnologías
```

**Usado en**: Slides de introducción y demo

### 6. **NUEVO** Docker SSH Deploy Architecture
**Archivo**: `diagrams/ssh-deploy-architecture.excalidraw`  
**Contenido**:
```
┌─────────────────────┐        SSH         ┌──────────────────────┐
│   Dev Machine / CI  │───────────────────▶│   Remote Server      │
│   aspire deploy     │                     │   docker compose up  │
└─────────────────────┘                     └──────────────────────┘
         │                                            │
         │ Build & Push                               │ Pull & Run
         ▼                                            ▼
┌─────────────────────┐                     ┌──────────────────────┐
│  Container Registry │────────────────────▶│   Running Containers │
│  (Docker Hub, ACR)  │                     │   (server, frontend) │
└─────────────────────┘                     └──────────────────────┘
```

**Usado en**: Slide "Docker SSH Deploy - Overview"

### 7. **NUEVO** Deployment Options Comparison
**Archivo**: `diagrams/deployment-comparison.excalidraw`  
**Contenido**:
```
┌─────────────────────────────────────────────────────┐
│  Azure Container Apps                               │
│  ✅ Auto-scaling                                    │
│  ✅ Managed services                                │
│  ✅ $0 start (consumption)                          │
│  ⚠️ Azure lock-in                                  │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│  Docker SSH Deploy                                  │
│  ✅ No vendor lock-in                               │
│  ✅ Control total                                   │
│  ✅ Infraestructura existente                       │
│  ⚠️ Setup manual                                   │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│  Kubernetes (AKS/EKS/GKE)                           │
│  ✅ Portable multi-cloud                            │
│  ✅ Ecosistema rico                                 │
│  ⚠️ Complejidad alta                               │
│  ⚠️ Overhead operacional                           │
└─────────────────────────────────────────────────────┘
```

**Usado en**: Slide "Comparación: Container Apps vs SSH Deploy"

## 📸 Screenshots/Imágenes a Capturar

### 1. Aspire Dashboard Local
**Archivo**: `images/aspire-dashboard-local.png`  
**Contenido**: Screenshot del dashboard con:
- Lista de recursos (server, webfrontend)
- Logs en tiempo real
- Traces distribuidos
- Métricas (CPU, memoria)

**Usado en**: Slide "Aspire Dashboard"

### 2. Terminal: aspire do ci Output
**Archivo**: `images/aspire-do-ci-output.png`  
**Contenido**: Terminal mostrando:
```
$ aspire do ci
[setup] Install uv globally... ✓
[install] dotnet restore server... ✓
[install] npm install webfrontend... ✓
[lint] dotnet format server... ✓
[lint] npm run lint webfrontend... ✓
[test] dotnet test server... ✓
✓ CI Pipeline completed successfully
```

**Usado en**: Slides de CI steps

### 3. Azure Portal: Container Apps
**Archivo**: `images/azure-portal-container-apps.png`  
**Contenido**: Azure Portal mostrando:
- Container Apps Environment
- Lista de apps (server, webfrontend)
- Estado: Running
- Ingress URL pública

**Usado en**: Slide "Azure Container Apps"

### 4. Application Insights Logs
**Archivo**: `images/application-insights-logs.png`  
**Contenido**: Application Insights mostrando:
- Query en Logs
- Traces distribuidos
- Performance metrics

**Usado en**: Slide "Observabilidad en Producción"

### 5. GitHub Actions Pipeline
**Archivo**: `images/github-actions-pipeline.png`  
**Contenido**: GitHub Actions UI mostrando:
- Workflow ejecutándose
- Step: "Run CI" con `aspire do ci`
- Logs expandidos

**Usado en**: Slide "GitHub Actions"

### 6. Azure DevOps Pipeline
**Archivo**: `images/azure-devops-pipeline.png`  
**Contenido**: Azure DevOps UI mostrando:
- Pipeline ejecutándose
- Stage: CI con `aspire do ci`
- Logs expandidos

**Usado en**: Slide "Azure DevOps Pipeline"

### 7. VS Code con AppHost.cs
**Archivo**: `images/vscode-apphost.png`  
**Contenido**: VS Code mostrando:
- AppHost.cs abierto
- Código con extensiones `.WithRestoreStep()`, etc.
- IntelliSense visible

**Usado en**: Slides de código

## 🎥 Videos Opcionales (Backup para Demo)

### 1. Full Demo Video
**Archivo**: `videos/full-demo.mp4`  
**Duración**: 3-5 minutos  
**Contenido**:
1. `aspire run` → Dashboard
2. `aspire do ci` → Output completo
3. `aspire deploy` → Azure provisioning
4. Azure Portal → Verificación

**Usado como**: Backup si la demo en vivo falla

### 2. Quick CI Execution
**Archivo**: `videos/ci-execution.gif`  
**Duración**: 30 segundos  
**Contenido**: Terminal mostrando `aspire do ci` ejecutándose rápidamente

**Usado en**: Slides de CI steps (animación)

## ✅ Assets ya Disponibles

- ✅ `dotnet-intro.png` - Slide de apertura/cierre
- ✅ `grafana.png` - Dashboard de Grafana (puede reutilizarse)
- ✅ Diagramas de modular monolith (se mantienen para contexto si se mencionan)

## 📝 Prioridad de Creación

### Alta Prioridad (Críticos para la charla)
1. ✅ Pipeline Flow Diagram
2. ✅ Azure Container Apps Architecture
3. ✅ Aspire Dashboard screenshot
4. ✅ `aspire do ci` terminal output
5. ✅ Azure Portal Container Apps screenshot

### Media Prioridad (Mejoran la presentación)
6. ⬜ Traditional vs Aspire Pipelines diagram
7. ⬜ Polyglot Application Stack diagram
8. ⬜ Environment Promotion Flow diagram
9. ⬜ Application Insights screenshot

### Baja Prioridad (Nice to have)
10. ⬜ GitHub Actions screenshot
11. ⬜ Azure DevOps screenshot
12. ⬜ VS Code screenshot
13. ⬜ Demo video backup

## 🛠️ Herramientas Necesarias

- **Excalidraw**: Para crear diagramas (ya instalado con `slidev-addon-excalidraw`)
- **Terminal**: Para capturar output de comandos
- **Azure Portal**: Para screenshots de Container Apps
- **Snipping Tool / Greenshot**: Para capturas de pantalla
- **OBS Studio** (opcional): Para grabar videos de demo

## 📅 Timeline Sugerido

1. **Día 1**: Crear diagramas en Excalidraw (2-3 horas)
2. **Día 2**: Capturar screenshots de Azure Portal y Dashboard (1 hora)
3. **Día 3**: Grabar terminal outputs y CI execution (30 min)
4. **Día 4**: (Opcional) Grabar video de demo completo (1 hora)
5. **Día 5**: Review y ajustes finales (30 min)

---

**Estado actual**: Slides completas, assets pendientes  
**Próximo paso**: Crear diagramas críticos (prioridad alta)
