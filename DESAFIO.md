 
# Desafio Técnico - Implementação de API REST em .NET
Você deve criar uma API para **gerenciar produtos de um catálogo**. A API deve permitir operações básicas de CRUD com validações e boas práticas de programação.

### 1. Modelo de Dados
#### Produto
Criar uma entidade `Produto` com os seguintes atributos:
- **Id** (int): Identificador único (gerado automaticamente)
- **Nome** (string): Nome do produto (obrigatório, máximo 100 caracteres)
- **Descricao** (string): Descrição (máximo 500 caracteres)
- **Preco** (decimal): Preço do produto (obrigatório, maior que 0)
- **Estoque** (int): Quantidade em estoque (padrão: 0, mínimo: 0)
- **DataCriacao** (DateTime): Data de criação (preenchida automaticamente)
- **Ativo** (bool): Indicador de status (padrão: true)
#### Pedido
Criar uma entidade `Pedido` com os seguintes atributos:
- **Id** (int): Identificador único (gerado automaticamente)
- **NumeroPedido** (string): Número do pedido (único, obrigatório)
- **ClienteNome** (string): Nome do cliente (obrigatório, máximo 150 caracteres)
- **ClienteEmail** (string): Email do cliente (obrigatório, deve ser válido)
- **DataPedido** (DateTime): Data de criação (preenchida automaticamente)
- **Status** (string): Status do pedido (padrão: "Pendente" - valores possíveis: "Pendente", "Processando", "Enviado", "Entregue", "Cancelado")
- **ValorTotal** (decimal): Valor total do pedido (calculado automaticamente)
- **Itens** (List<ItemPedido>): Lista de itens do pedido
#### ItemPedido
Criar uma entidade `ItemPedido` com os seguintes atributos:
- **Id** (int): Identificador único (gerado automaticamente)
- **ProdutoId** (int): ID do produto (obrigatório, deve existir em Produtos)
- **Quantidade** (int): Quantidade do produto (obrigatório, mínimo: 1)
- **PrecoUnitario** (decimal): Preço unitário no momento do pedido (preenchido automaticamente)
- **Subtotal** (decimal): Quantidade × PrecoUnitario (calculado automaticamente)
### 2. Endpoints da API
#### GET `/api/produtos`
- Retorna lista de todos os produtos ativos
- **Response 200:**
  ```json
  [
    {
      "id": 1,
      "nome": "Notebook",
      "descricao": "Notebook i7",
      "preco": 3500.00,
      "estoque": 5,
      "dataCriacao": "2026-03-18T10:00:00",
      "ativo": true
    }
  ]
  ```
#### GET `/api/produtos/{id}`
- Retorna um produto específico pelo Id
- **Response 200:** Dados do produto
- **Response 404:** Produto não encontrado
#### POST `/api/produtos`
- Cria um novo produto
- **Body:**
  ```json
  {
    "nome": "Mouse",
    "descricao": "Mouse wireless",
    "preco": 50.00,
    "estoque": 20
  }
  ```
- **Response 201:** Produto criado com status URI
- **Response 400:** Validação falhou
#### PUT `/api/produtos/{id}`
- Atualiza um produto existente
- **Body:** Mesma estrutura do POST
- **Response 204:** Sucesso
- **Response 404:** Produto não encontrado
- **Response 400:** Validação falhou
#### DELETE `/api/produtos/{id}`
- Remove um produto (marcar como inativo, não deletar fisicamente)
- **Response 204:** Sucesso
- **Response 404:** Produto não encontrado
### 3. Endpoints de Pedidos
#### GET `/api/pedidos`
- Retorna lista de todos os pedidos
- **Response 200:**
  ```json
  [
    {
      "id": 1,
      "numeroPedido": "PED-001",
      "clienteNome": "João Silva",
      "clienteEmail": "joao@example.com",
      "dataPedido": "2026-03-18T10:00:00Z",
      "status": "Pendente",
      "valorTotal": 550.00,
      "itens": [
        {
          "id": 1,
          "produtoId": 1,
          "quantidade": 2,
          "precoUnitario": 250.00,
          "subtotal": 500.00
        }
      ]
    }
  ]
  ```
#### GET `/api/pedidos/{id}`
- Retorna um pedido específico pelo Id
- **Response 200:** Dados do pedido
- **Response 404:** Pedido não encontrado
#### POST `/api/pedidos`
- Cria um novo pedido com seus itens
- **Body:**
  ```json
  {
    "numeroPedido": "PED-001",
    "clienteNome": "João Silva",
    "clienteEmail": "joao@example.com",
    "itens": [
      {
        "produtoId": 1,
        "quantidade": 2
      },
      {
        "produtoId": 2,
        "quantidade": 1
      }
    ]
  }
  ```
- **Validações:**
  - Todos os `produtoId` devem existir
  - Quantidade deve ser >= 1
  - NumeroPedido deve ser único
  - Email deve ser válido
- **Response 201:** Pedido criado com ValorTotal calculado
- **Response 400:** Validação falhou
#### PUT `/api/pedidos/{id}/status`
- Atualiza o status de um pedido
- **Body:**
  ```json
  {
    "novoStatus": "Processando"
  }
  ```
- **Response 204:** Sucesso
- **Response 404:** Pedido não encontrado
- **Response 400:** Status inválido
#### DELETE `/api/pedidos/{id}`
- Cancela um pedido (muda status para "Cancelado")
- **Response 204:** Sucesso
- **Response 404:** Pedido não encontrado
---
 