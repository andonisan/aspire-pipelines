# ADR 001: Estrategia de Deployment

**Fecha**: 2026-01-11  
**Estado**: Aceptado  
**Decisores**: Equipo de arquitectura

## Contexto

Aspire Pipelines soporta múltiples opciones de deployment. Necesitamos documentar las opciones disponibles, sus trade-offs, y cuándo usar cada una para guiar a los equipos en la toma de decisiones.

## Opciones Consideradas

### Opción 1: Azure Container Apps (Recomendada por defecto)

**Descripción**: PaaS managed de Azure diseñado específicamente para aplicaciones containerizadas con integración nativa con Aspire.

**Pros**:
- ✅ Integración perfecta con `aspire deploy` - cero configuración
- ✅ Auto-scaling de 0 a N instancias según demanda
- ✅ HTTPS automático con certificados managed
- ✅ Integración nativa con servicios Azure (SQL, Redis, Service Bus)
- ✅ Application Insights incluido para observabilidad
- ✅ Managed Identity para seguridad sin credenciales
- ✅ Rollback y blue-green deployment nativos
- ✅ Consumption plan: $0 cuando no hay tráfico
- ✅ DDoS protection y WAF disponibles

**Contras**:
- ⚠️ Vendor lock-in con Azure
- ⚠️ Costos pueden crecer con alto tráfico constante
- ⚠️ Menos control granular sobre infraestructura
- ⚠️ Limitaciones en configuración de red avanzada

**Cuándo usar**:
- Proyectos nuevos sin infraestructura existente
- Startups que necesitan MVP rápido
- Aplicaciones con tráfico variable (benefit de scale-to-zero)
- Equipos pequeños sin expertise DevOps dedicado
- Cuando Application Insights es requirement para observabilidad

### Opción 2: Docker SSH Deploy

**Descripción**: Deploy a servidores remotos (VPS, on-premise) via SSH usando Docker Compose.

**Pros**:
- ✅ Sin vendor lock-in - portable a cualquier proveedor
- ✅ Control total sobre infraestructura y configuración
- ✅ Costos predecibles (VPS desde $5-50/mes)
- ✅ Ideal para aprovechar infraestructura existente
- ✅ Funciona on-premise o en cualquier cloud
- ✅ Simple para equipos familiarizados con Docker Compose

**Contras**:
- ⚠️ Requiere configurar y mantener servidor manualmente
- ⚠️ Setup inicial más complejo (Docker, SSH, firewall, SSL)
- ⚠️ Sin auto-scaling automático (requiere implementación manual)
- ⚠️ Observabilidad requiere configuración separada
- ⚠️ Rollback manual (docker-compose down/up)
- ⚠️ Responsabilidad de seguridad, patches, backups

**Cuándo usar**:
- Infraestructura existente (VPS, on-premise datacenter)
- Requisitos de compliance que prohíben cloud público
- Presupuesto muy limitado
- Control total es requirement crítico
- Migración gradual de legacy a Aspire

**Configuración**:
```csharp
// AppHost.cs
builder.AddDockerComposeEnvironment("prod")
    .WithSshDeploySupport();
```

```json
// appsettings.json
{
  "Deploy": {
    "SshHost": "your-server.com",
    "SshUser": "deploy-user",
    "SshKeyPath": "~/.ssh/id_rsa_deploy",
    "RemotePath": "/opt/myapp",
    "Registry": "registry.example.com"
  }
}
```

**Recursos**:
- [Aspire Docker SSH Deploy](https://github.com/davidfowl/aspire-ssh-deploy)
- [Package](https://f.feedz.io/davidfowl/aspire/packages/Aspire.Hosting.Docker.SshDeploy/latest)

### Opción 3: Kubernetes (AKS, EKS, GKE)

**Descripción**: Deploy a clusters Kubernetes managed o self-hosted.

**Pros**:
- ✅ Portable multi-cloud (evita lock-in)
- ✅ Ecosistema rico (Helm, operators, service mesh)
- ✅ Ideal para aplicaciones muy grandes y complejas
- ✅ Fine-grained control sobre scheduling, networking
- ✅ Community y tooling maduro

**Contras**:
- ⚠️ Complejidad operacional alta
- ⚠️ Curva de aprendizaje empinada
- ⚠️ Overhead de mantenimiento del cluster
- ⚠️ Costos de infraestructura (control plane + nodes)
- ⚠️ Integración con Aspire menos seamless

**Cuándo usar**:
- Organizaciones grandes con expertise Kubernetes
- Requisitos de multi-cloud o cloud portability
- Aplicaciones con necesidades complejas de networking/storage
- Ya existe infraestructura K8s establecida

## Decisión

**Para este proyecto de demostración**, usamos **Azure Container Apps** como target principal porque:

1. **Objetivo de la charla**: Demostrar la simplicidad de `aspire deploy`
2. **Mejor developer experience**: Cero configuración de infraestructura
3. **Audiencia**: Desarrolladores que quieren ver el flujo completo rápidamente

Sin embargo, **documentamos todas las opciones** (especialmente SSH Deploy) para que los usuarios puedan elegir según su contexto.

## Consecuencias

### Positivas
- ✅ La demo muestra el happy path más simple
- ✅ Usuarios pueden replicar fácilmente en sus cuentas Azure
- ✅ Documentación SSH Deploy ayuda a equipos con infraestructura existente

### Negativas
- ⚠️ Puede dar impresión de que Aspire solo funciona con Azure
- ⚠️ Usuarios sin Azure necesitan buscar alternativas

### Mitigación
- 📖 Slides dedicadas a alternativas de deployment (SSH, K8s)
- 📖 README con comparación detallada de opciones
- 📖 Links a davidfowl/aspire-ssh-deploy
- 📖 Mencionar que Aspire es multi-cloud aunque Azure tiene mejor integración

## Notas Adicionales

### Comparación de Costos (Estimados)

**Aplicación pequeña (2 containers, low traffic)**:
- Azure Container Apps: $0-10/mes (consumption plan, scale-to-zero)
- VPS (DigitalOcean, Hetzner): $5-12/mes
- AKS/EKS/GKE: $70-100/mes (cluster + nodes)

**Aplicación mediana (5 containers, moderate traffic)**:
- Azure Container Apps: $50-200/mes
- VPS (múltiples): $40-100/mes
- AKS/EKS/GKE: $200-500/mes

**Aplicación grande (10+ containers, high traffic)**:
- Azure Container Apps: $500-2000/mes
- Self-hosted cluster: $300-800/mes (hardware + staff)
- Managed K8s: $1000-5000/mes

### Matriz de Decisión

| Criterio | Container Apps | SSH Deploy | Kubernetes |
|----------|----------------|------------|------------|
| Setup inicial | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| Mantenimiento | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| Costo (small) | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐ |
| Costo (large) | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| Portabilidad | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Auto-scaling | ⭐⭐⭐⭐⭐ | ⭐ | ⭐⭐⭐⭐ |
| Observabilidad | ⭐⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |
| Control | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

## Referencias

- [Azure Container Apps Pricing](https://azure.microsoft.com/pricing/details/container-apps/)
- [Aspire Docker SSH Deploy](https://github.com/davidfowl/aspire-ssh-deploy)
- [Aspire Deployment Docs](https://learn.microsoft.com/dotnet/aspire/deployment/overview)
- [DORA Metrics](https://dora.dev/guides/)
