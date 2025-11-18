# Sistema de Gerenciamento para Oficina Mecânica

Sistema integrado de gestão para oficinas mecânicas, desenvolvido com arquitetura em camadas e Domain-Driven Design (DDD), implementando os principais processos de atendimento, orçamentação, execução de serviços e controle de estoque.

## Índice
- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Licença](#licença)
- [Equipe](#equipe)

## Sobre o Projeto
Este sistema foi desenvolvido como solução para modernizar e digitalizar os processos de uma oficina mecânica, substituindo gestão manual por uma plataforma integrada, robusta e escalável. O MVP (Minimum Viable Product) foca nos processos centrais:
- Gestão de clientes e veículos
- Criação e acompanhamento de ordens de serviço
- Controle de estoque de peças
- Criação de Orçamento automático
- Fluxo completo de aprovação e execução
- Métricas operacionais

## Arquitetura
O sistema implementa Arquitetura em Camadas com conceitos de Domain-Driven Design (DDD):

~~~
┌─────────────────────────────────────┐
│     API Layer (Presentation)        │  ← Controllers RESTful
├─────────────────────────────────────┤
│    Application Layer (Use Cases)    │  ← Application Services, DTOs
├─────────────────────────────────────┤
│    Domain Layer (Business Logic)    │  ← Entities, Value Objects, Domain Services
├─────────────────────────────────────┤
│  Infrastructure Layer (Data/I/O)    │  ← Repositories, Database, External Services
└─────────────────────────────────────┘
~~~

### Conceitos DDD Implementados
- Entidades Ricas: `OrdemDeServico`, `Cliente`, `Veiculo`, `Orcamento`
- Value Objects: `CpfCnpj`, `Placa`, `OrdemServicoPeca`, `OrdemServicoItemServico`
- Agregados: `OrdemDeServico` como aggregate root gerenciando peças e serviços
- Repositórios: Abstrações para persistência
- Serviços de Domínio: `OrdemDeServicoDomainService` para cálculos
- Bounded Contexts: Separação entre contextos de Atendimento e Estoque

### Princípios Aplicados
- ✅ SOLID: Todos os cinco princípios implementados consistentemente
- ✅ Clean Code: Nomenclatura expressiva, métodos pequenos, ausência de duplicação
- ✅ Separation of Concerns: Responsabilidades claramente divididas entre camadas
- ✅ Dependency Inversion: Dependências sempre para abstrações
- ✅ Single Responsibility: Cada classe possui uma única responsabilidade

## Tecnologias Utilizadas

### Backend
- .NET 9
- C# 13
- ASP.NET Core (Web API)
- Entity Framework Core
- Npgsql (PostgreSQL)

### Banco de Dados
- PostgreSQL 16
- Conformidade ACID completa
- Tipos de dados nativos (UUID, NUMERIC, JSONB)
- Performance e escalabilidade
- Licença open-source permissiva

### Segurança
- JWT (JSON Web Tokens) — Autenticação stateless
- ASP.NET Core Identity — Gerenciamento de usuários
- Role-Based Access Control (RBAC) — Autorização baseada em papéis

### Testes
- xUnit
- Moq
- Cobertura de 80%+ nos domínios críticos
- Teste Unitários e de Integração

### DevOps
- Docker
- Docker Compose
- Git

### Documentação
- Swagger/OpenAPI: http://localhost:5207/swagger

## Funcionalidades

### 1. Gestão de Clientes
- ✅ Cadastro com validação de CPF/CNPJ
- ✅ Verificação de unicidade de documentos e e-mails
- ✅ Listagem paginada
- ✅ Atualização e remoção com cascade delete em veículos associados

### 2. Gestão de Veículos
- ✅ Cadastro vinculado a clientes
- ✅ Validação de placas (Mercosul e formato antigo)
- ✅ Consulta por cliente
- ✅ Histórico de serviços

### 3. Ordem de Serviço — Fluxo Completo
Estados da Ordem: Recebida → Em Diagnóstico → Aguardando Aprovação → Em Execução → Finalizada → Entregue

Funcionalidades:
- ✅ Validação de cliente e veículo
- ✅ Seleção de serviços do catálogo
- ✅ Adição de peças com verificação de estoque
- ✅ Reserva automática de estoque
- ✅ Cálculo automático de orçamento
- ✅ Envio de e-mail assíncrono ao cliente
- ✅ Aprovação de orçamento
- ✅ Transições de estado controladas
- ✅ Registro de timestamps em cada etapa
- ✅ Cálculo automático de tempo de execução

### 4. Controle de Estoque
- ✅ Integração entre contextos de Atendimento e Estoque
- ✅ Verificação de disponibilidade
- ✅ Reserva automática ao criar ordem
- ✅ Prevenção de ordens sem estoque

### 5. Métricas e Relatórios
- ✅ Tempo médio de execução de ordens
- ✅ Histórico completo com timestamps
- ✅ Base para expansão de analytics

### 6. Catálogo de Serviços
- ✅ Serviços pré-cadastrados
- ✅ CRUD completo de Serviços

### 7. Catálogo de Peças
- ✅ Peças pré-cadastradas
- ✅ CRUD completo de Peças

## Pré-requisitos

### Para Execução com Docker (Recomendado)
- Docker Desktop 20.10+
- Docker Compose 2.0+

### Para Execução Local (Desenvolvimento)
- .NET 9 SDK
- PostgreSQL 16+
- Visual Studio 2022 ou VS Code
- Git

## Instalação e Execução

### Docker Compose (Recomendado)

1. Clone o repositório.
2. Configure variáveis de ambiente: crie o arquivo `.env` na raiz do projeto com as chaves necessárias.
   ~~~
   # Database
   POSTGRES_USER=
   POSTGRES_PASSWORD=
   POSTGRES_DB=

   # Application
   ConnectionStrings__DefaultConnection=

   # JWT
   JWT__KEY=
   JWT__ISSUER=
   JWT__AUDIENCE=

   # Senha User Seed
   USERSEED__PASSWORD=
   ~~~
3. Inicie os containers (app/database):
   ~~~
   docker-compose up
   ~~~
4. Acesse a aplicação:
   - API: http://localhost:5001
   - Swagger: http://localhost:5001/swagger

### Execução Local (.NET)

1. Na raiz do projeto:
   ~~~
   dotnet restore
   dotnet build
   cd src/Oficina.Api
   dotnet run
   # ou durante desenvolvimento
   dotnet watch run
   ~~~

### Migrations (EF Core)

- Atendimento Context
  ~~~
  dotnet ef migrations add NomeDaMigration --context AtendimentoDbContext --project src/atendimento/Oficina.Atendimento.Infrastructure --startup-project src/Oficina.Api
  ~~~
- Estoque Context
  ~~~
  dotnet ef migrations add NomeDaMigration --context EstoqueDbContext --project src/estoque/Oficina.Estoque.Infrastructure --startup-project src/Oficina.Api
  ~~~
- Common Context
  ~~~
  dotnet ef migrations add NomeDaMigration --context CommonDbContext --project src/common/Oficina.Common.Infrastructure --startup-project src/Oficina.Api
  ~~~
- Aplicar Migrations
  ~~~
  dotnet ef database update --context EstoqueDbContext --project src/estoque/Oficina.Estoque.Infrastructure --startup-project src/Oficina.Api
  dotnet ef database update --context AtendimentoDbContext --project src/atendimento/Oficina.Atendimento.Infrastructure --startup-project src/Oficina.Api
  dotnet ef database update --context CommonDbContext --project src/common/Oficina.Common.Infrastructure --startup-project src/Oficina.Api
  ~~~

### Testes com relatório de cobertura
Na raiz do projeto:
- xUnit + Moq
- Script de cobertura:
~~~
.\test-coverage.ps1
~~~
O relatório será gerado na pasta `TestResults` e está configurado para uma cobertura de 80% usando Coverlet.

### Parar containers
~~~
docker-compose down
~~~

## Estrutura do Projeto
~~~
oficinaOps/
├── src/
│   ├── Oficina.Api/                           # API RESTful
│   │   ├── Controllers/                       # Endpoints HTTP
│   │   └── Program.cs                         # Configuração da aplicação
│   │
│   ├── atendimento/
│   │   ├── Oficina.Atendimento.Domain/        # Lógica de negócio
│   │   │   ├── Entities/                      # Entidades (Cliente, Veiculo, OrdemDeServico, Orcamento)
│   │   │   ├── ValueObjects/                  # Value Objects (CpfCnpj, Placa, etc)
│   │   │   ├── Enum/                          # Enumerações (OrdemStatus, OrcamentoStatus)
│   │   │   ├── IRepository/                   # Interfaces de repositórios
│   │   │   └── Services/                      # Serviços de domínio
│   │   │
│   │   ├── Oficina.Atendimento.Application/   # Casos de uso
│   │   │   ├── Services/                      # Application Services
│   │   │   ├── Dto/                           # Data Transfer Objects
│   │   │   └── IServices/                     # Interfaces de serviços
│   │   │
│   │   └── Oficina.Atendimento.Infrastructure/# Persistência
│   │       ├── Data/                          # DbContext
│   │       ├── Repositories/                  # Implementação de repositórios
│   │       └── Migrations/                    # Migrations EF Core
│   │
│   ├── estoque/
│   │   ├── Oficina.Estoque.Domain/            # Domínio de Estoque
│   │   │   ├── Entities/                      # Entidades (Estoque,Peça)
│   │   │   ├── Enum/                          # Enumerações (TipoMovimentoEstoque, TipoPeca)
│   │   │   ├── IRepository/                   # Interfaces de repositórios
│   │   │   └── Services/                      # Serviços de domínio
│   │   │
│   │   ├── Oficina.Estoque.Application/       # Aplicação de Estoque
│   │   │   ├── Services/                      # Application Services
│   │   │   ├── Dto/                           # Data Transfer Objects
│   │   │   └── IServices/                     # Interfaces de serviços
│   │   │
│   │   └── Oficina.Estoque.Infrastructure/    # Infraestrutura de Estoque
│   │       ├── Data/                          # DbContext
│   │       ├── Repositories/                  # Implementação de repositórios
│   │       └── Migrations/                    # Migrations EF Core
│   │
│   └── common/
│       ├── Oficina.Common.Domain/             # Domínio Compartilhado
│       │   ├── Entities/                      # Entidades (Base, EmailMessage, User)
│       │   ├── Enum/                          # Enumerações (UserRole)
│       │   ├── IHelper/                       # Interfaces de Helpers
│       │   ├── IRepository/                   # Interfaces de repositórios
│       │   └── ISecurity/                     # Interfaces de Segurança
│       │
│       ├── Oficina.Common.Application/        # Serviço de Aplicação Compartilhados
│       │   ├── Services/                      # Application Services
│       │   ├── Helper/                        # Application Helpers
│       │   └── IServices/                     # Interfaces de serviços
│       │
│       └── Oficina.Common.Infrastructure/     # Infraestrutura Compartilhada
│           ├── Data/                          # DbContext
│           ├── Helper/                        # Infra Helpers
│           ├── Repositories/                  # Implementação de repositórios
│           ├── Security/                      # Autenticação e Autorização
│           └── Migrations/                    # Migrations EF Core
│
├── tests/
│   ├── Oficina.Tests/                        # Testes Unitários
│   └── Oficina.Tests.Integration             # Testes de Integração
│
├── docker-compose.yml                         # Orquestração Docker
├── Dockerfile                                 # Build da aplicação
├── test-coverage.ps1                          # Script de cobertura de testes
├── .gitignore
└── README.md
~~~

## Licença
Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## Equipe
Desenvolvido como Tech Challenge da Pós-Graduação FIAP em Arquitetura de Software.
