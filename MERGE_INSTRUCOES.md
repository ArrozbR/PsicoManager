# Como mesclar estes arquivos no repositório ArrozbR/PsicoManager

O repositório é um projeto MVC único (namespace `PsicoManager`, tudo na raiz).
Estes arquivos já estão com o namespace ajustado para `PsicoManager` e o
domínio (entidades/enums) embutido na pasta `Domain/`. É só copiar para a raiz
do seu clone e commitar.

## Estrutura entregue

```
Controllers/      -> 5 controllers MVC (Home, Agenda, Prontuario, Financeiro, Pacientes)
Models/           -> ViewModels.cs (todos os ViewModels das telas)
Data/             -> MockDataStore.cs (dados mockados em memória, sem banco)
Domain/           -> Entities, Enums, Common, Exceptions (regras de negócio)
Views/            -> telas .cshtml + _Layout, _ViewImports, _ViewStart
wwwroot/css/      -> site.css
Program.cs        -> configuração (cultura pt-BR + injeção do MockDataStore)
```

## Passo a passo

No seu clone local (que já existe):

```bash
# 1. crie uma branch para a mesclagem
git checkout -b feature/mvc-mockado

# 2. copie os arquivos desta pasta para a RAIZ do clone
#    (no Windows, use o Explorer; no terminal, algo como:)
cp -r /caminho/para/merge-psicomanager/* /caminho/do/seu/clone/

# 3. veja o que mudou
git status
```

## Sobre conflitos (arquivos que JÁ existem no repo)

Como você quer **manter o que já existe**, atenção a estes que provavelmente
já estão lá do template "Criando Projeto MVC":

- **Program.cs** — o seu atual será substituído pelo desta entrega. O novo
  registra o `MockDataStore` e a cultura pt-BR. Se você tiver customizações no
  Program.cs atual, abra os dois lado a lado e junte manualmente (o trecho
  essencial é `builder.Services.AddSingleton<MockDataStore>();`).

- **Controllers/HomeController.cs** e **Views/Home/Index.cshtml** — o template
  padrão tem versões "vazias" desses. As desta entrega substituem por Dashboard.
  Se quiser manter o Home antigo, renomeie o desta entrega (ex.: DashboardController).

- **Views/Shared/_Layout.cshtml** — esta entrega traz um layout próprio (menu
  lateral). Se o repo já tem um _Layout que você quer manter, NÃO copie este e
  ajuste o menu manualmente.

- **wwwroot/css/site.css** — idem; se o repo já tem site.css, renomeie este
  para `psico.css` e referencie no _Layout.

Os demais (Agenda/Prontuario/Financeiro/Pacientes Controllers e Views, Models,
Data e Domain) são novos — não devem conflitar.

## Finalizando

```bash
git add .
git commit -m "Adiciona telas MVC com dados mockados (Dashboard, Agenda, Prontuario, Financeiro, Pacientes)"
git push -u origin feature/mvc-mockado
```

Depois é só abrir o Pull Request no GitHub e mesclar na main.

## Conferência rápida antes do commit

```bash
dotnet build      # tem que compilar sem erros
dotnet run        # abre em http://localhost:5xxx -> cai no Dashboard
```
