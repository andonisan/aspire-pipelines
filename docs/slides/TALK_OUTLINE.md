# Del aspire run al aspire deploy - Outline

## 📋 Información de la Charla

**Título**: Del aspire run al aspire deploy. Pipelines as Code  
**Duración sugerida**: 45-60 minutos  
**Nivel**: Intermedio-Avanzado  
**Audiencia**: Developers interesados en CI/CD, DevOps, y .NET Aspire

## 🎯 Objetivos de Aprendizaje

Al final de esta charla, los asistentes podrán:

1. Entender qué es Pipeline as Code y por qué es importante
2. Configurar CI/CD con Aspire Pipelines para aplicaciones polyglot
3. Ejecutar `aspire do ci` localmente y en pipelines de CI/CD
4. Desplegar aplicaciones a Azure Container Apps con `aspire deploy`
5. Gestionar secretos y configuración por entorno
6. Implementar builds reproducibles con lock files y CPM

## 📊 Estructura de la Charla

### Parte 1: Introducción (10 min)
- ¿Por qué Pipeline as Code?
- El problema de las aplicaciones polyglot
- El viaje del desarrollador: run → ci → deploy
- Aspire 13 y Pipelines (experimental)

### Parte 2: aspire run - Desarrollo Local (10 min)
- Dashboard integrado
- Orquestación de servicios
- Definición de recursos en AppHost
- Telemetría automática con OpenTelemetry

### Parte 3: aspire do ci - Pipeline CI (15 min)
- Estructura de CI Steps (Setup, Install, Lint, Test)
- Extensiones para .NET (restore, build, format, test)
- Extensiones para JavaScript (install, lint)
- Extensiones para Python (uv setup, sync, ruff, pytest)
- Integración completa en AppHost
- Integración con GitHub Actions y Azure DevOps

### Parte 4: Builds Reproducibles (5 min)
- global.json para versión del SDK
- Central Package Management (CPM)
- Lock files (.NET, npm, uv)
- Directory.Build.props

### Parte 5: aspire deploy - Deployment (15 min)
- Azure Container Apps overview
- Arquitectura de deployment
- Gestión de secretos con Key Vault y Managed Identity
- Configuración por entorno (dev/staging/prod)
- Promoción entre entornos
- Observabilidad en producción (Application Insights)
- Rollback y blue-green deployment
- Costos y auto-scaling

### Parte 6: Demo en Vivo (5 min)
- `aspire run` → Dashboard local
- `aspire do ci` → Ver steps ejecutarse
- `aspire deploy` → Desplegar a Azure
- Verificar en Azure Portal

### Parte 7: Conclusiones y Q&A (5 min)
- Beneficios demostrados
- Limitaciones actuales (experimental)
- Futuro de Aspire Pipelines
- Recursos y comunidad

## 🔑 Mensajes Clave

1. **Un comando para cada fase**: `aspire run`, `aspire do ci`, `aspire deploy`
2. **Polyglot support**: .NET, JavaScript, Python en un solo pipeline
3. **Pipeline as Code**: CI/CD definido en C#, no YAML disperso
4. **Local-first**: Mismo comportamiento local y en CI
5. **Azure simplificado**: Container Apps + Managed Services sin complejidad

## 🎬 Tips para la Demo

### Pre-demo Setup
- Tener el proyecto clonado y funcionando: `aspire run`
- Azure CLI autenticado: `az login`
- Variables de entorno configuradas: `AZURE_SUBSCRIPTION_ID`, `AZURE_LOCATION`
- Terminal con output limpio

### Demo Flow
1. **Mostrar código del AppHost** [src/aspire-pipelines.AppHost/AppHost.cs](../../src/aspire-pipelines.AppHost/AppHost.cs)
   - Definición de resources
   - CI steps añadidos con extensiones

2. **Ejecutar `aspire run`**
   - Abrir dashboard en navegador
   - Mostrar logs, traces, metrics

3. **Ejecutar `aspire do ci`**
   - Ver output de cada step
   - Mostrar cómo falla si hay errores de formato/tests

4. **Ejecutar `aspire deploy`**
   - Esperar provisioning (puede tardar 2-3 min)
   - Mientras espera, hablar de la infraestructura que se crea

5. **Verificar en Azure Portal**
   - Container Apps Environment
   - Logs en Application Insights
   - Probar URL pública

### Backup Plan
- Screenshots de cada paso en `docs/slides/images/`
- Video pregrabado de la demo completa
- Código de ejemplo con resultados comentados

## 📝 Notas Adicionales

### Prerequisitos Mencionados
- .NET 10 SDK (preview)
- Node.js 20+ para frontend
- Python 3.12+ (opcional para demos Python)
- Azure CLI
- Azure subscription activa

### Recursos del Repo
- [AppHost.cs](../../src/aspire-pipelines.AppHost/AppHost.cs) - Definición principal
- [DotNetCIStepsExtensions.cs](../../src/aspire-pipelines.AppHost/CISteps/DotNetCIStepsExtensions.cs) - Extensiones .NET
- [JavaScriptCIStepsExtensions.cs](../../src/aspire-pipelines.AppHost/CISteps/JavaScriptCIStepsExtensions.cs) - Extensiones JS
- [PythonCIStepsExtensions.cs](../../src/aspire-pipelines.AppHost/CISteps/PythonCIStepsExtensions.cs) - Extensiones Python
- [azure-pipelines.yml](../../.azdo/azure-pipelines.yml) - Azure DevOps
- [ci-cd.yml](../../.github/workflows/ci-cd.yml) - GitHub Actions

### Preguntas Frecuentes Anticipadas

**Q: ¿Funciona con otros clouds además de Azure?**  
A: Aspire está optimizado para Azure Container Apps, pero puede deployar a cualquier Kubernetes. El deploy será menos "mágico" en otros clouds.

**Q: ¿Cuándo sale de experimental?**  
A: Microsoft no ha dado fecha, pero esperamos estabilización en Aspire 14 o 15 (2026).

**Q: ¿Funciona con lenguajes no soportados (Go, Rust, Java)?**  
A: Sí, puedes crear extensiones personalizadas para cualquier lenguaje. Comunidad está creando extensiones.

**Q: ¿Qué pasa si mi pipeline es más complejo?**  
A: Puedes crear custom steps con cualquier lógica en C#. Aspire solo provee los WellKnownSteps como convención.

**Q: ¿Puedo usar esto en producción?**  
A: Es experimental, así que usa con precaución en producción. Ideal para proyectos nuevos o internos donde puedes iterar rápido.

## 🔗 Enlaces Útiles

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [GitHub: dotnet/aspire](https://github.com/dotnet/aspire)
- [Azure Container Apps Docs](https://learn.microsoft.com/azure/container-apps/)
- [Este proyecto](https://github.com/andonisan/aspire-pipelines)

---

**Última actualización**: Enero 2026  
**Versión de Aspire**: 13.1.0  
**Autor**: Andoni Santamaria
