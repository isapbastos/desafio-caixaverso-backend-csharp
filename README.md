# 📘 InvestimentosJwtApi

## **Descrição**
Este projeto é uma API desenvolvida em **C# (.NET)** que implementa autenticação via **JWT (JSON Web Token)** e está preparada para execução em contêineres utilizando **Docker Compose**. 
API para simulação de investimentos com autenticação JWT, gestão de produtos e telemetria.

Tecnologias utilizadas:

- .NET 8  
- SQLite  
- Docker & Docker Compose  
- Swagger para documentação da API  

---

## **Tecnologias**
- **.NET 8**
- **Docker & Docker Compose**
- **JWT**
- **Banco de Dados** (Sqlite)

## 🐳 Pré-requisitos
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (apenas para desenvolvimento local)  

> Docker Compose já vem integrado no Docker Desktop.

## **Instalação e Execução**

### **Local**
```bash
git clone https://github.com/isapbastos/Desafio-backend-net.git
cd Desafio-backend-net
dotnet restore
dotnet run
```

### **Com Docker Compose**
```bash
docker-compose up --build
```

#### **Para acessar o Swagger, abra o navegador e digite:**
```
http://localhost:{port}/swagger/index.html
```

## ⚙️ Estrutura do projeto
```
/InvestimentosJwtApi.sln
/docker-compose.yml
/InvestimentosJwtApi/ <-- Projeto Web API
/InvestimentosJwt.Application/ <-- Lógica de aplicação e serviços
/InvestimentosJwt.Domain/ <-- Entidades e modelos do domínio
/InvestimentosJwt.Infra.Data/ <-- Repositórios e acesso a dados
/InvestimentosJwt.Tests/ <-- Repositórios com testes unitários e de integração
```

## **Testes**
Execute:
```bash
dotnet test
```

## **Exemplo de Requisição**
```bash
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"email":"email","password":"pass"}'
```

## **Autenticação JWT**
- **Login**: `POST /api/Auth/login`
- **Header**:
```http
Authorization: Bearer <token>
```
