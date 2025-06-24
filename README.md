# 🎬 Bida Clipper 2.0

Bida Clipper é uma aplicação de desktop para Windows, desenvolvida em C# com Windows Forms, que permite aos usuários criar clipes de vídeo de forma rápida e eficiente. A ferramenta possibilita carregar vídeos a partir de URLs do YouTube ou de arquivos locais, especificar múltiplos segmentos para corte e exportá-los no formato desejado.

## ✨ Funcionalidades Principais

-   **Carregamento de Múltiplas Fontes:**
    -   **YouTube:** Cole a URL de um vídeo do YouTube, selecione a qualidade desejada (de 360p a 2160p) e carregue as informações do vídeo.
    -   **Arquivo Local:** Navegue e selecione um arquivo de vídeo diretamente do seu computador.

-   **Gerenciamento de Clipes:**
    -   Adicione múltiplos clipes a uma lista de processamento.
    -   Para cada clipe, especifique o **tempo de início** (`HH:MM:SS`), **tempo de fim** (`HH:MM:SS`) e um **nome de arquivo** personalizado.
    -   Remova clipes da lista facilmente.

-   **Configurações de Saída Flexíveis:**
    -   Escolha a pasta de destino onde os clipes gerados serão salvos.
    -   Selecione o formato de vídeo de saída, incluindo `mp4`, `mkv`, `mov`, e `avi`.

-   **Processamento e Feedback em Tempo Real:**
    -   Um botão central "Gerar Clipes" inicia o processo de corte.
    -   Acompanhe o progresso geral através de uma barra de progresso.
    -   Visualize logs detalhados do processo de codificação em uma caixa de texto dedicada, ideal para depuração e acompanhamento.

-   **Interface Moderna e Intuitiva:**
    -   Design limpo com seções bem definidas para entrada, saída, lista de clipes e ações.
    -   Uso de controles personalizados (`RoundedButton`, `RoundedPanel`) para um visual aprimorado.
    -   Paleta de cores e tipografia consistentes para uma melhor experiência de usuário.

## 🚀 Como Usar

1.  **Carregar um Vídeo:**
    -   **Opção 1 (YouTube):** Selecione a aba "URL do YouTube", cole o link do vídeo, escolha a qualidade e clique em **Carregar**. A duração do vídeo será exibida.
    -   **Opção 2 (Arquivo Local):** Selecione a aba "Arquivo Local", clique em **Procurar** para selecionar um vídeo e, em seguida, clique em **Carregar**.

2.  **Definir as Configurações de Saída:**
    -   Na seção "Configurações de Saída", clique em **Procurar** para escolher a pasta onde os clipes serão salvos.
    -   Selecione o formato de vídeo desejado (ex: `mp4`).

3.  **Adicionar Clipes à Lista:**
    -   Na seção "Lista de Clipes", a grade estará pronta para receber as informações.
    -   Adicione uma nova linha (geralmente clicando na última linha vazia) ou use o botão **Adicionar**.
    -   Preencha as colunas:
        -   `Início (HH:MM:SS)`: Tempo de início do corte.
        -   `Fim (HH:MM:SS)`: Tempo de final do corte.
        -   `Nome do Clipe`: O nome do arquivo final (sem a extensão).
    -   Repita o processo para todos os clipes que deseja criar. Para remover um clipe, selecione a linha e clique em **Remover**.

4.  **Gerar os Clipes:**
    -   Quando todas as configurações estiverem prontas, clique no botão **Gerar Clipes** na parte inferior da tela.
    -   A barra de progresso será ativada e os logs aparecerão na caixa de texto abaixo, mostrando o status da operação.

5.  **Concluído!**
    -   Após o término do processo, seus clipes estarão na pasta de destino que você selecionou.

## 🛠️ Tecnologias Utilizadas

-   **Linguagem:** C#
-   **Framework:** .NET / Windows Forms
-   **Estrutura da UI:** A interface é construída programaticamente usando `TableLayoutPanel` para garantir um layout flexível e organizado.
-   **Controles:** Controles padrão do Windows Forms (`TextBox`, `ComboBox`, `DataGridView`, etc.) e controles personalizados (`RoundedButton`, `RoundedPanel`) para um estilo moderno.
-   **Dependências (inferidas):**
    -   Provavelmente uma biblioteca como `YoutubeExplode` para extrair informações e streams do YouTube.
    -   Um wrapper de `FFmpeg` (como `FFMpegCore` ou `Xabe.FFmpeg`) para realizar o corte e a conversão dos vídeos.

