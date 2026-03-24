# Catálogo Itaú API - Documentação

## Visão Geral

A **Catálogo Itaú API** é uma API RESTful completa para gerenciamento de catálogo de produtos e pedidos de compra, construída com .NET 10, ASP.NET Core e PostgreSQL.

## Características

- ✅ **Arquitetura Limpa**: Separação clara entre camadas (Domain, Application, Infrastructure, API)
- ✅ **CQRS Pattern**: Implementação com MediatR para separar leitura e escrita
- ✅ **Validação Fluente**: FluentValidation para regras de validação robustas
- ✅ **Controle de Concorrência**: Otimista com timestamp de atualização
- ✅ **Gerenciamento de Transações**: UnitOfWork pattern para operações atômicas
- ✅ **Documentação OpenAPI**: Swagger/Swashbuckle com comentários XML-Doc completos
- ✅ **Tratamento de Exceções**: Middleware centralizado com mapeamento para HTTP status codes
- ✅ **Soft Delete**: Produtos podem ser inativados mantendo histórico
- ✅ **Paginação**: Suporte para paginação em GET endpoints
- ✅ **Depleção de Estoque**: Automática e transacional

## Stack Tecnológico

- **Runtime**: .NET 10
- **Web Framework**: ASP.NET Core 10.0
- **ORM**: Entity Framework Core 10.0.5
- **Database**: PostgreSQL
- **CQRS**: MediatR 14.1.0
- **Validação**: FluentValidation 11.11.0
- **Documentação**: Swashbuckle.AspNetCore 10.1.5
- **Serialização**: System.Text.Json

## Acesso à Documentação Interativa

A documentação OpenAPI (Swagger) está disponível em **desenvolvimento**:

```
http://localhost:5000
```

ou diretamente no endpoint:

```
http://localhost:5000/swagger/index.html
```

## Endpoints da API

### Produtos

#### Listar Produtos (Paginado)

```http
GET /api/produtos?page=1&pageSize=10
```

**Parâmetros:**

- `page`: Número da página (padrão: 1)
- `pageSize`: Itens por página (padrão: 10, máximo: 100)

**Response (200 OK):**

```json
{
  "items": [
    {
      "id": 1,
      "nome": "Produto A",
      "descricao": "Descrição do Produto A",
      "preco": 99.99,
      "estoque": 50,
      "ativo": true,
      "dataCriacao": "2026-03-23T10:30:00Z",
      "atualizadoEm": "2026-03-23T10:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalItems": 150,
  "totalPages": 15,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

#### Obter Produto por ID

```http
GET /api/produtos/{id}
```

**Response (200 OK):**

```json
{
  "id": 1,
  "nome": "Produto A",
  "descricao": "Descrição do Produto A",
  "preco": 99.99,
  "estoque": 50,
  "ativo": true
}
```

**Response (404 Not Found):** Produto não encontrado

#### Criar Produto

```http
POST /api/produtos
Content-Type: application/json

{
  "nome": "Novo Produto",
  "descricao": "Descrição opcional",
  "preco": 49.99,
  "estoque": 100
}
```

**Response (201 Created):**

```
Location: /api/produtos/42
Body: (vazio)
```

**Response (400 Bad Request):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": ["'Nome' não deve estar vazio."]
  }
}
```

#### Atualizar Produto

```http
PUT /api/produtos/{id}
Content-Type: application/json

{
  "nome": "Produto Atualizado",
  "descricao": "Nova descrição",
  "preco": 59.99,
  "estoque": 80
}
```

**Response (204 No Content):** Sucesso

**Response (404 Not Found):** Produto não encontrado

#### Deletar Produto

```http
DELETE /api/produtos/{id}
```

**Response (204 No Content):** Produto deletado (soft delete)

**Response (404 Not Found):** Produto não encontrado

### Pedidos

#### Listar Pedidos (Paginado)

```http
GET /api/pedidos?page=1&pageSize=10
```

**Response (200 OK):**

```json
{
  "items": [
    {
      "id": 1,
      "numeroPedido": "PED-2026-001",
      "clienteNome": "João Silva",
      "clienteEmail": "joao@example.com",
      "dataPedido": "2026-03-23T10:30:00Z",
      "status": "Pendente",
      "valorTotal": 299.97,
      "itens": [
        {
          "id": 1,
          "produtoId": 1,
          "quantidade": 2,
          "precoUnitario": 99.99,
          "subtotal": 199.98
        }
      ]
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalItems": 50,
  "totalPages": 5
}
```

#### Obter Pedido por ID

```http
GET /api/pedidos/{id}
```

**Response (200 OK):** Dados completos do pedido

#### Criar Pedido

```http
POST /api/pedidos
Content-Type: application/json

{
  "numeroPedido": "PED-2026-042",
  "clienteNome": "Maria Santos",
  "clienteEmail": "maria@example.com",
  "itens": [
    {
      "produtoId": 1,
      "quantidade": 2
    },
    {
      "produtoId": 3,
      "quantidade": 1
    }
  ]
}
```

**Response (201 Created):**

```json
{
  "id": 42,
  "numeroPedido": "PED-2026-042",
  "clienteNome": "Maria Santos",
  "clienteEmail": "maria@example.com",
  "dataPedido": "2026-03-23T11:00:00Z",
  "status": "Pendente",
  "valorTotal": 299.97,
  "itens": [...]
}
```

**Response (400 Bad Request):**

- Número de pedido duplicado
- Dados inválidos
- Estoque insuficiente

**Response (404 Not Found):** Produto não encontrado

**Response (409 Conflict):** Conflito de concorrência (estoque foi modificado simultaneamente)

#### Atualizar Status do Pedido

```http
PUT /api/pedidos/{id}/status
Content-Type: application/json

{
  "status": "Enviado"
}
```

**Valores de Status:**

- `Pendente`: Pedido recém-criado
- `Enviado`: Pedido foi despachado
- `Entregue`: Pedido chegou ao destino
- `Cancelado`: Pedido foi cancelado

**Response (204 No Content):** Status atualizado

#### Cancelar Pedido

```http
DELETE /api/pedidos/{id}
```

**Response (204 No Content):** Pedido cancelado

**Response (404 Not Found):** Pedido não encontrado

## Tratamento de Erros

### HTTP Status Codes

| Código | Significado  | Exemplo                   |
| ------ | ------------ | ------------------------- |
| 200    | OK           | GET bem-sucedido          |
| 201    | Created      | POST bem-sucedido         |
| 204    | No Content   | PUT/DELETE bem-sucedido   |
| 400    | Bad Request  | Validação falhou          |
| 404    | Not Found    | Recurso não encontrado    |
| 409    | Conflict     | Conflito de concorrência  |
| 500    | Server Error | Erro interno não esperado |

### Resposta de Erro (400)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": ["'Nome' deve ter comprimento máximo de 100 caracteres."],
    "Preco": ["'Preco' deve ser maior que 0."]
  }
}
```

### Resposta de Erro (409 Conflict)

Pode ocorrer quando múltiplas requisições tentam atualizar o mesmo pedido simultaneamente (conflito otimista de concorrência):

```json
{
  "message": "Conflito ao atualizar o pedido. Tente novamente.",
  "status": 409,
  "timestamp": "2026-03-23T11:00:00Z"
}
```

## Regras de Validação

### Produtos

| Campo     | Regra                              |
| --------- | ---------------------------------- |
| Nome      | Obrigatório, máximo 100 caracteres |
| Preço     | Obrigatório, deve ser > 0          |
| Estoque   | Obrigatório, deve ser >= 0         |
| Descrição | Opcional                           |

### Pedidos

| Campo        | Regra                                    |
| ------------ | ---------------------------------------- |
| NumeroPedido | Obrigatório, único, máximo 50 caracteres |
| ClienteNome  | Obrigatório, máximo 150 caracteres       |
| ClienteEmail | Obrigatório, formato de email válido     |
| Itens        | Obrigatório, mínimo 1 item               |

### Itens do Pedido

| Campo      | Regra                     |
| ---------- | ------------------------- |
| ProdutoId  | Obrigatório, deve existir |
| Quantidade | Obrigatório, >= 1         |

## Exemplo de Fluxo Completo

### 1. Criar um produto

```bash
curl -X POST http://localhost:5000/api/produtos \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Notebook Dell",
    "descricao": "Notebook Dell Inspiron 15",
    "preco": 3499.99,
    "estoque": 15
  }'
# Resposta: 201 Created, Location: /api/produtos/1
```

### 2. Listar produtos

```bash
curl http://localhost:5000/api/produtos?page=1&pageSize=10
```

### 3. Criar um pedido

```bash
curl -X POST http://localhost:5000/api/pedidos \
  -H "Content-Type: application/json" \
  -d '{
    "numeroPedido": "PED-2026-001",
    "clienteNome": "João Silva",
    "clienteEmail": "joao@example.com",
    "itens": [
      {
        "produtoId": 1,
        "quantidade": 2
      }
    ]
  }'
# Resposta: 201 Created com dados completos do pedido
```

### 4. Atualizar status do pedido

```bash
curl -X PUT http://localhost:5000/api/pedidos/1/status \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Enviado"
  }'
# Resposta: 204 No Content
```

## Características Avançadas

### Paginação

Todos os endpoints GET de listagem suportam paginação:

```
?page=2&pageSize=25
```

O resultado inclui metadados:

- `totalItems`: Total de itens
- `totalPages`: Número total de páginas
- `hasNextPage`: Se há próxima página
- `hasPreviousPage`: Se há página anterior

### Controle de Concorrência Otimista

O campo `AtualizadoEm` em Produtos é usado para rastrear mudanças. Se múltiplas requisições tentarem atualizar simultaneamente, a API retornará **409 Conflict**.

### Depleção Automática de Estoque

Quando um pedido é criado:

1. Cada item é validado
2. O estoque é bloqueado (SELECT FOR UPDATE)
3. O estoque é decrementado
4. A transação é confirmada

Se algum erro ocorrer, a transação é revertida.

### Soft Delete

Produtos deletados não são removidos do banco, apenas marcados como `Ativo = false`.

## Ambiente

### Development

```bash
cd src/CatalogoItau.Api
dotnet run
```

API disponível em: `http://localhost:5000`
Swagger disponível em: `http://localhost:5000`

### Build

```bash
dotnet build .\CatalogoItau.slnx
```

### Testes

```bash
dotnet test
```

## Configuração de Banco de Dados

### Connection String

Configurada em `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CatalogoItau;Username=admin;Password=postgres@Itau123"
  }
}
```

### Migrations

```bash
# Criar migração
dotnet ef migrations add NomeMigracao

# Aplicar migração
dotnet ef database update
```

## Licença

MIT

## Contato

Equipe de Desenvolvimento
Email: dev@itau.com
