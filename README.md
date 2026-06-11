# Imgdup — Verificador de Imagens Duplicadas (Windows)

Desktop app para Windows que encontra imagens **duplicadas (byte-idênticas)** e **similares (perceptual)** em uma ou mais pastas, exibe os grupos numa galeria, e permite enviar as selecionadas para a **Lixeira**.

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Runtime | .NET 10 (LTS) / C# 14 |
| GUI | WPF + CommunityToolkit.Mvvm (MVVM) |
| Galeria virtualizada | VirtualizingWrapPanel |
| Decode/thumbnail | SkiaSharp |
| Hash perceptual | CoenM.ImageSharp.ImageHash (dHash) |
| Hash exato | System.IO.Hashing (xxHash3) |
| Índice near-dup | BK-tree + union-find (Hamming) |
| Cache de hashes | SQLite (Microsoft.Data.Sqlite) |
| Lixeira | Microsoft.VisualBasic.FileIO.FileSystem |

## Estrutura

```
Imgdup.slnx
Directory.Build.props        # LangVersion, Nullable, warnings-as-errors, analyzers
Directory.Packages.props     # Central Package Management (versões centralizadas)
src/
  Imgdup.Core/               # Engine, sem dependência de UI — net10.0
    Models/                  # PerceptualHash, ImageEntry, DuplicateGroup, ScanOptions, ScanProgress
    Hashing/                 # IImageHasher + ImageHasher (xxHash3 + dHash)
    Dedup/                   # BkTree, UnionFind, DuplicateFinder
    Scanning/                # FileScanner, ImageScanEngine (pipeline paralelo)
    Caching/                 # IHashCache + SqliteHashCache (re-scan incremental)
  Imgdup.App/                # WPF — net10.0-windows
    Services/                # FolderPicker, RecycleBin, Thumbnail (SkiaSharp), UserDialog
    ViewModels/              # MainViewModel, DuplicateGroupViewModel, ImageItemViewModel
    Views/ (MainWindow)      # Toolbar + galeria agrupada/virtualizada
tests/
  Imgdup.Core.Tests/         # xUnit — PerceptualHash, BkTree, DuplicateFinder
```

## Como o motor funciona

1. Enumera arquivos (paralelo, filtra por extensão e tamanho mínimo).
2. Agrupa por tamanho → só calcula **hash exato (xxHash3)** em arquivos com tamanho colidente (byte-idênticos têm o mesmo tamanho).
3. Calcula **hash perceptual (dHash)** decodificando cada imagem (SkiaSharp/ImageSharp).
4. Agrupa: exatos por hash; similares via **BK-tree** (raio de Hamming = tolerância) + **union-find**.
5. Cache em SQLite (chave: caminho + data de modificação + tamanho) → re-scans não redecodificam.

Complexidade: pré-filtro O(n); near-dup ≈ O(n·log n) em vez de O(n²) ingênuo.

## Build e execução (Windows)

Pré-requisitos: **.NET 10 SDK** + Windows 10/11.

```powershell
dotnet restore
dotnet build -c Release
dotnet run -c Release --project src/Imgdup.App
```

Testes do motor (multiplataforma):

```powershell
dotnet test tests/Imgdup.Core.Tests
```

> **Nota:** o projeto WPF (`Imgdup.App`) **só compila/executa no Windows**. Em Linux/WSL é possível
> apenas compilar com `-p:EnableWindowsTargeting=true` (validação), mas não executar. O `Imgdup.Core`
> e os testes rodam em qualquer plataforma.

## Funcionalidades

- Selecionar **uma ou mais pastas** (diálogo nativo multi-seleção), com/sem subpastas.
- Detecção de duplicatas **exatas** e **similares** (tolerância ajustável 0–16).
- Galeria agrupada por cluster, **virtualizada** (escala para muitos resultados).
- Modos de visualização: **Pequeno / Médio / Grande**.
- Ordenação: **data, tamanho, nome** (asc/desc).
- Sugestão de "manter" (maior resolução → mais antigo) destacada por grupo.
- Selecionar 1+ (ou todos os "extras") e **enviar para a Lixeira** (reversível).

## Segurança / robustez

- Exclusão sempre vai para a **Lixeira** (`RecycleOption.SendToRecycleBin`) — nunca permanente.
- Arquivos >2 GB, em rede ou em uso **não** vão para a Lixeira (limitação do Windows) → reportados, nunca falham em silêncio.
- `SixLabors.ImageSharp` fixado em 3.1.12 (corrige CVEs GHSA-2cmq-823j-5qj8 / GHSA-rxmq-m78w-7wmc).
- `warnings-as-errors` + analyzers + NuGet audit habilitados.

## Limitações conhecidas / próximos passos

- Cache de thumbnails em memória sem teto — para bibliotecas enormes, adicionar evicção LRU.
- Suporte a RAW/HEIC requer adicionar Magick.NET como decoder de fallback.
- Empacotamento (single-file/MSIX) não configurado ainda.
