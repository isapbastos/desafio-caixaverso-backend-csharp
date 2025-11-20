# 📘 InvestimentosJwtApi

API para simulação de investimentos com autenticação JWT, gestão de produtos e telemetria.

Tecnologias utilizadas:

- .NET 8  
- SQLite  
- Docker & Docker Compose  
- Swagger para documentação da API  

---

## 🐳 Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop)  
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (apenas para desenvolvimento local)  

> Docker Compose já vem integrado no Docker Desktop.

No terminal, dentro da pasta do projeto, execute:

```bash
docker-compose up --build

Para acessar o Swagger, abra o navegador e digite:

http://localhost:8080/swagger/index.html

## ⚙️ Estrutura do projeto

/InvestimentosJwtApi.sln
/docker-compose.yml
/InvestimentosJwtApi/ <-- Projeto Web API
/InvestimentosJwt.Application/ <-- Lógica de aplicação e serviços
/InvestimentosJwt.Domain/ <-- Entidades e modelos do domínio
/InvestimentosJwt.Infra.Data/ <-- Repositórios e acesso a dados