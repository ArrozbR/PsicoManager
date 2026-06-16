# PsicoManager — Backend (.NET 9 / Clean Architecture)

Plataforma de gestão clínica para psicólogos (agenda, prontuário eletrônico,
financeiro, teleconsulta, notificações e diário de emoções do paciente).
Implementação do backend conforme a **Entrega Parte III** (SRS IEEE 830, EOBD,
contratos de operação CO-01, diagramas C4 e matriz de rastreabilidade).

## Arquitetura

Quatro camadas com dependências unidirecionais (Clean Architecture):

```
Web  ──►  Application  ──►  Domain  ◄──  Infrastructure
 │                                          ▲
 └──────────────────────────────────────────┘
```

- **PsicoManager.Domain** — entidades, regras de negócio (RN1–RN6), enums,
  exceções e interfaces (repositórios + serviços). Sem dependências externas.
- **PsicoManager.Application** — casos de uso (orquestração), DTOs e contratos.
  Implementa CO-01 `registrarEvolucao()` e `lancarCobranca()` fiéis aos C4.
- **PsicoManager.Infrastructure** — EF Core (PostgreSQL), repositórios, AES-256,
  auditoria, gateway Pix, storage, salas de teleconsulta e adaptadores.
- **PsicoManager.Web** — API REST, autenticação JWT, Swagger, middleware de
  exceções e background jobs (trava RNF03 e lembretes UC05).

## Requisitos não funcionais atendidos

| RNF   | Descrição                              | Onde |
|-------|----------------------------------------|------|
| RNF01 | Sigilo do prontuário (vínculo)         | `AcessoSigiloService` |
| RNF02 | Criptografia AES-256 em repouso        | `CriptografiaService` |
| RNF03 | Trava de edição (24h)                  | `EvolucaoClinica` + `TravaEdicaoJob` |
| RNF04 | Log de auditoria imutável              | `LogAuditoria` + trigger SQL |
| RNF05 | Retenção mínima de 5 anos              | `ProntuarioClinico.GarantirRetencaoCumprida` |

## Pré-requisitos

- .NET SDK 9.0+
- PostgreSQL 14+

## Configuração

Edite `src/PsicoManager.Web/appsettings.json`:

- `ConnectionStrings:ClinicoDb` — conexão PostgreSQL.
- `Crypto:Key` — chave AES-256 (32 bytes em Base64). **Troque em produção**
  e armazene em cofre de segredos (KMS / Key Vault), nunca no arquivo.
- `Jwt:Key` / `Jwt:Issuer` / `Jwt:Audience` — parâmetros do token.
- `Gateway:Disponivel` — alterne para `false` para exercitar o fallback do UC04a.

## Banco de dados

Duas opções:

1. **Aplicar o schema SQL** diretamente (já inclui a trigger RNF04):
   ```bash
   createdb psicomanager
   psql -U postgres -d psicomanager -f db/schema.sql
   ```

2. **Gerar a migration via EF Core** a partir do modelo:
   ```bash
   dotnet tool install --global dotnet-ef
   cd src/PsicoManager.Infrastructure
   dotnet ef migrations add Inicial -s ../PsicoManager.Web
   dotnet ef database update -s ../PsicoManager.Web
   ```
   > Lembre de aplicar a trigger de imutabilidade do log (RNF04) — o SQL está
   > em `PsicoManager.Infrastructure/Persistence/MigrationSql.cs` e em `db/schema.sql`.

## Executar

```bash
dotnet restore
dotnet build
dotnet run --project src/PsicoManager.Web
```

Swagger em `http://localhost:5080/swagger`.

## Principais endpoints

| Módulo       | Método/rota                                   | Caso de uso |
|--------------|-----------------------------------------------|-------------|
| Agenda       | `POST /api/agenda/sessoes`                     | UC01a |
| Prontuário   | `POST /api/prontuarios/evolucoes`              | CO-01 |
| Financeiro   | `POST /api/financeiro/cobrancas`               | UC04a |
| Financeiro   | `POST /api/financeiro/cobrancas/{id}/confirmar`| UC04b |
| Financeiro   | `GET  /api/financeiro/relatorios/financeiro`   | CSU08 |
| Teleconsulta | `POST /api/teleconsultas/{sessaoId}/iniciar`   | UC03 |
| Paciente     | `POST /api/pacientes`                          | cadastro |
| Paciente     | `POST /api/pacientes/diario/emocoes`           | UC09 |
| Paciente     | `GET  /api/pacientes/relatorios/presenca`      | CSU09 |

Todos exigem JWT (`Authorization: Bearer <token>`). A emissão do token fica a
cargo do serviço de autenticação (fora do escopo deste backend clínico).

## Estrutura de pastas

```
PsicoManager/
├─ PsicoManager.sln
├─ db/schema.sql
├─ src/
│  ├─ PsicoManager.Domain/
│  ├─ PsicoManager.Application/
│  ├─ PsicoManager.Infrastructure/
│  └─ PsicoManager.Web/
└─ README.md
```
