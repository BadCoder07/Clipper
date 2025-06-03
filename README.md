# 🎬 YouTube Clipper

Uma aplicação Windows Forms (C# + .NET 8) para baixar vídeos do YouTube e gerar automaticamente clipes personalizados com base em intervalos de tempo definidos pelo usuário.

---

## ✨ Funcionalidades

- Baixa vídeos diretamente do YouTube via [`yt-dlp`](https://github.com/yt-dlp/yt-dlp)
- Gera múltiplos clipes com nomes e tempos personalizados via [`ffmpeg`](https://ffmpeg.org/)
- Interface limpa e leve
- Verificação e instalação automática das dependências
- Suporte a múltiplas qualidades e formatos de vídeo
- Modo silencioso e com execução como administrador

---

## 🖥️ Requisitos

- **Windows 10 ou superior**
- **.NET 8 Runtime Desktop** (caso use a versão compacta)
- Acesso à internet (para baixar dependências se necessário)

> Ou use a versão **self-contained**, que não exige nada instalado.

---

## 📦 Instalação

### 🔹 Via Executável Publicado

1. Baixe a versão mais recente da aba [Releases](https://github.com/seu-usuario/clipper/releases)
2. Extraia os arquivos `.zip`
3. Execute `Clipper.exe` como administrador

> O app pedirá permissões de admin automaticamente se necessário.

---

### 🔹 Ou compile manualmente

1. Clone este repositório:
   ```bash
   git clone https://github.com/seu-usuario/clipper.git
