# Exemplos de Uso - Catálogo Itaú API

## Usando cURL

Defina a URL base primeiro (desenvolvimento local):

```bash
BASE_URL="http://localhost:5112"
```

### 1. Listar Produtos

```bash
curl -X GET "$BASE_URL/api/produtos?page=1&pageSize=10" \
  -H "accept: application/json"
```

### 2. Obter Produto Específico

```bash
curl -X GET "$BASE_URL/api/produtos/1" \
  -H "accept: application/json"
```

### 3. Criar Novo Produto

```bash
curl -X POST "$BASE_URL/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Mouse Gamer RGB",
    "descricao": "Mouse Gamer com RGB e 16000 DPI",
    "preco": 189.99,
    "estoque": 50
  }'
```

### 4. Atualizar Produto

```bash
curl -X PUT "$BASE_URL/api/produtos/1" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Mouse Gamer RGB Pro",
    "descricao": "Mouse Gamer atualizado com RGB e 16000 DPI",
    "preco": 199.99,
    "estoque": 45
  }'
```

### 5. Deletar Produto

```bash
curl -X DELETE "$BASE_URL/api/produtos/1" \
  -H "accept: */*"
```

### 6. Listar Pedidos

```bash
curl -X GET "$BASE_URL/api/pedidos?page=1&pageSize=10" \
  -H "accept: application/json"
```

### 7. Obter Pedido Específico

```bash
curl -X GET "$BASE_URL/api/pedidos/1" \
  -H "accept: application/json"
```

### 8. Criar Novo Pedido

```bash
curl -X POST "$BASE_URL/api/pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "numeroPedido": "PED-2026-001",
    "clienteNome": "João Silva Santos",
    "clienteEmail": "joao.silva@example.com",
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
  }'
```

### 9. Atualizar Status do Pedido

```bash
curl -X PUT "$BASE_URL/api/pedidos/1/status" \
  -H "Content-Type: application/json" \
  -d '{
    "novoStatus": "Enviado"
  }'
```

Status válidos:

- `Pendente`
- `Processando`
- `Enviado`
- `Entregue`
- `Cancelado`

### 10. Cancelar Pedido

```bash
curl -X DELETE "$BASE_URL/api/pedidos/1" \
  -H "accept: */*"
```

## Usando PowerShell

### Criar Produto

```powershell
$body = @{
    nome = "Teclado Mecânico"
    descricao = "Teclado Mecânico com Switches RGB"
    preco = 349.99
    estoque = 30
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5112/api/produtos" `
  -Method POST `
  -Headers @{"Content-Type"="application/json"} `
  -Body $body
```

### Criar Pedido com Múltiplos Itens

```powershell
$body = @{
    numeroPedido = "PED-2026-002"
    clienteNome = "Maria Santos"
    clienteEmail = "maria@example.com"
    itens = @(
        @{
            produtoId = 1
            quantidade = 1
        },
        @{
            produtoId = 2
            quantidade = 3
        }
    )
} | ConvertTo-Json -Depth 2

Invoke-RestMethod -Uri "http://localhost:5112/api/pedidos" `
  -Method POST `
  -Headers @{"Content-Type"="application/json"} `
  -Body $body
```

## Usando C# HttpClient

```csharp
using HttpClient client = new HttpClient();

// Criar Produto
var produto = new
{
    nome = "Monitor UltraWide",
    descricao = "Monitor UltraWide 34 polegadas",
    preco = 1299.99,
    estoque = 10
};

var jsonContent = JsonContent.Create(produto);
var response = await client.PostAsync(
  "http://localhost:5112/api/produtos",
    jsonContent
);

if (response.IsSuccessStatusCode)
{
    var createdLocation = response.Headers.Location;
    Console.WriteLine($"Produto criado: {createdLocation}");
}

// Criar Pedido
var pedido = new
{
    numeroPedido = "PED-2026-003",
    clienteNome = "Ana Costa",
    clienteEmail = "ana.costa@example.com",
    itens = new[]
    {
        new { produtoId = 1, quantidade = 1 },
        new { produtoId = 2, quantidade = 2 }
    }
};

var pedidoContent = JsonContent.Create(pedido);
var pedidoResponse = await client.PostAsync(
  "http://localhost:5112/api/pedidos",
    pedidoContent
);

var pedidoResult = await pedidoResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
Console.WriteLine($"Pedido criado com ID: {pedidoResult?["id"]}");
```

## Cenários de Teste

### Cenário 1: Fluxo Completo de Vendas

1. **Criar 3 Produtos**

```bash
# Produto 1: Notebook
curl -X POST "$BASE_URL/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Notebook","descricao":"Notebook 16GB RAM","preco":3499.99,"estoque":5}'

# Produto 2: Mouse
curl -X POST "$BASE_URL/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Mouse","descricao":"Mouse Wireless","preco":89.99,"estoque":20}'

# Produto 3: Teclado
curl -X POST "$BASE_URL/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Teclado","descricao":"Teclado Mecânico","preco":299.99,"estoque":15}'
```

2. **Criar Pedido com os 3 Produtos**

```bash
curl -X POST "$BASE_URL/api/pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "numeroPedido":"PED-2026-COMPLETO",
    "clienteNome":"Cliente Premium",
    "clienteEmail":"premium@example.com",
    "itens":[
      {"produtoId":1,"quantidade":1},
      {"produtoId":2,"quantidade":2},
      {"produtoId":3,"quantidade":1}
    ]
  }'
```

3. **Atualizar Status do Pedido (Pendente → Enviado → Entregue)**

```bash
# Enviado
curl -X PUT "$BASE_URL/api/pedidos/1/status" \
  -H "Content-Type: application/json" \
  -d '{"novoStatus":"Enviado"}'

# Entregue
curl -X PUT "$BASE_URL/api/pedidos/1/status" \
  -H "Content-Type: application/json" \
  -d '{"novoStatus":"Entregue"}'
```

### Cenário 2: Teste de Validações

**Tentar criar produto sem nome (deve falhar com 400)**

```bash
curl -X POST "$BASE_URL/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{"preco":100,"estoque":10}'
```

**Tentar criar pedido com número duplicado (deve falhar com 400)**

```bash
# Primeira tentativa (sucesso)
curl -X POST "$BASE_URL/api/pedidos" \
  -H "Content-Type: application/json" \
  -d '{"numeroPedido":"PED-DUP","clienteNome":"Cliente","clienteEmail":"cli@test.com","itens":[{"produtoId":1,"quantidade":1}]}'

# Segunda tentativa com mesmo número (deve falhar)
curl -X POST "$BASE_URL/api/pedidos" \
  -H "Content-Type: application/json" \
  -d '{"numeroPedido":"PED-DUP","clienteNome":"Outro Cliente","clienteEmail":"outro@test.com","itens":[{"produtoId":1,"quantidade":1}]}'
```

### Cenário 3: Teste de Paginação

```bash
# Página 1, 10 itens por página
curl "$BASE_URL/api/produtos?page=1&pageSize=10"

# Página 2, 20 itens por página
curl "$BASE_URL/api/produtos?page=2&pageSize=20"

# Página 3, 100 itens por página (máximo)
curl "$BASE_URL/api/produtos?page=3&pageSize=100"
```

### Cenário 4: Teste de Concorrência

Simular múltiplas requisições simultâneas tentando atualizar o mesmo pedido:

```bash
# Terminal 1
curl -X PUT "$BASE_URL/api/pedidos/1/status" \
  -H "Content-Type: application/json" \
  -d '{"novoStatus":"Enviado"}' &

# Terminal 2 (simultaneamente)
curl -X PUT "$BASE_URL/api/pedidos/1/status" \
  -H "Content-Type: application/json" \
  -d '{"novoStatus":"Entregue"}' &

# Uma das requisições deve retornar 409 Conflict
```

## Dicas de Teste

1. **Use o Swagger**: Abra `http://localhost:5112/swagger/index.html` no navegador para testar interativamente

2. **Use Postman**: Importe os endpoints no Postman para melhor organização

3. **Use Visual Studio Code**: Instale a extensão "REST Client" e crie arquivos `.http`

4. **Verifique Estoque**: Após criar um pedido, o estoque dos produtos deve ser decrementado

5. **Monitore Logs**: Verifique os logs do servidor para ver o fluxo de execução

## Codes de Erro Comuns

| Erro | Causa                    | Solução                                  |
| ---- | ------------------------ | ---------------------------------------- |
| 400  | Validação falhou         | Verifique o formato do JSON e os valores |
| 404  | Recurso não encontrado   | Verifique se o ID existe                 |
| 409  | Conflito de concorrência | Recarregue e tente novamente             |
| 500  | Erro servidor            | Verifique os logs do servidor            |
