# Del aspire run al aspire deploy - Slides

Presentación en [Slidev](https://github.com/slidevjs/slidev) sobre **Pipeline as Code con .NET Aspire 13**.

## 🚀 Iniciar la presentación

```bash
# Instalar dependencias
npm install
# o
pnpm install

# Modo desarrollo (con hot reload)
npm run dev
# o
pnpm dev

# Visitar http://localhost:3030
```

## 📦 Build para producción

```bash
# Generar SPA estático
npm run build

# Exportar a PDF
npm run export
```

Los archivos generados estarán en `dist/`.

## 📝 Estructura

- **[slides.md](./slides.md)** - Contenido principal de las slides
- **[TALK_OUTLINE.md](./TALK_OUTLINE.md)** - Outline y notas del speaker
- **diagrams/** - Diagramas Excalidraw
- **images/** - Imágenes y screenshots
- **styles/** - Estilos custom (Rider theme)
- **theme/** - Tema personalizado

## 🎨 Tema

Usa `the-unnamed` theme con customizaciones en [styles/rider.css](./styles/rider.css).

## 🎯 Contenido de la Charla

1. **Introducción a Pipeline as Code**
2. **Fase 1: aspire run** - Desarrollo local
3. **Fase 2: aspire do ci** - Integración continua
4. **Builds reproducibles** - Lock files, CPM
5. **Fase 3: aspire deploy** - Deployment a Azure
6. **Demo en vivo**
7. **Conclusiones y futuro**

## 🔗 Enlaces

- **Documentación de Slidev**: <https://sli.dev/>
- **Proyecto completo**: [aspire-pipelines](../../)
- **Aspire Docs**: <https://learn.microsoft.com/dotnet/aspire/>

---

**Versión**: Aspire 13.1.0  
**Fecha**: Enero 2026  
**Autor**: Andoni Santamaria
