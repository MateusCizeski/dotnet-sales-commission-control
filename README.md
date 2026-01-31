# 📦 Portal de Comissões – .NET 8

Projeto desenvolvido em **.NET 8**, composto por:
- **API** (ASP.NET Core Web API)
- **Front-end** (Razor Pages)
- **Banco de dados** SQL Server
- Suporte completo para **execução local** e **Docker**

O objetivo do projeto é permitir operações **CRUD** relacionadas ao controle de comissões, com integração entre Front e API.

---

## 🧱 Arquitetura

Estrutura do projeto:

- `/Api` → Camada de apresentação da API (ASP.NET Core Web API)
- `/Front` → Front-end da aplicação (Razor Pages)
- `/Application` → Camada de aplicação (casos de uso, DTOs, interfaces)
- `/Domain` → Domínio da aplicação (entidades, enums, regras de domínio)
- `/Infra` → Infraestrutura (Entity Framework Core, DbContext, repositórios, migrations)
- `/Tests` → Testes unitários
- `/docker-compose.yml` → Orquestração dos containers (API, Front-end e SQL Server)
- `Portal.slnx` → Solution do projeto

Principais características:
- Comunicação entre Front e API via HTTP  
- Entity Framework Core com **migrations**  
- Banco de dados **SQL Server**

---

## ⚙️ Requisitos

### Para rodar localmente
- .NET SDK 8+
- SQL Server (ex: SQLEXPRESS)
- Visual Studio ou terminal

### Para rodar com Docker
- Docker
- Docker Compose

---
## ▶️ Executando o projeto localmente

### 1️⃣ Configurar connection string local

Arquivo `Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ComissoesDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## 2️⃣ Rodar a API
Na pasta raiz do projeto:
- dotnet run --project Api/Api.csproj
- API disponível em: http://localhost:5001
- Swagger disponível em: http://localhost:5001/swagger

---
## 3️⃣ Rodar o Front-end
Em outro terminal:
- dotnet run --project Front/Front.csproj
- Front disponível em: http://localhost:7000

---
## ℹ️ Observação
- As migrations são aplicadas automaticamente ao iniciar a API, garantindo que o banco de dados seja criado e atualizado sem necessidade de passos manuais.

---
🧪 Executando os testes unitários
O projeto possui uma camada de testes localizada em:
- /Tests
Para executar os testes:
- cd Tests
- dotnet test

---
## 🐳 Executando o projeto com Docker
## 1️⃣ Subir todos os serviços
Na pasta raiz do projeto:
- docker compose up --build
Isso irá subir:
- SQL Server
- API
- Front-end

## 2️⃣ URLs disponíveis
- Front-end	http://localhost:7000
- API	http://localhost:5001
- Swagger	http://localhost:5001/swagger
