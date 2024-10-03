# Atividade 1 - Checkpoint 2
Implemente um sistema de inventário de produtos que utiliza Redis como cache e MySQL como banco de dados. O objetivo é otimizar a busca por produtos e manter o cache sincronizado após adições ou atualizações.


## Banco de Dados MySQL:

- Crie uma tabela produtos com os campos id, nome, preco, quantidade_estoque e data_criacao.

## Endpoints:

- **GET /produtos**: Deve buscar todos os produtos do banco de dados e armazená-los no cache Redis. Se os dados já estiverem no cache, eles devem ser retornados diretamente de lá.

- **POST /produtos**: Deve adicionar um novo produto ao banco de dados. Após a inserção, o cache de produtos deve ser removido (invalidado) para que seja atualizado na próxima requisição GET.

- **PUT /produtos/{id}**: Atualiza um produto existente. Após a atualização, o cache também deve ser invalidado.

## Cache Redis:

- O cache deve ser configurado para armazenar os produtos por 10 minutos (TTL). Após esse tempo, o cache deve ser removido automaticamente.

## TTL do Cache:

Configure um TTL de 10 minutos para o cache de produtos.

Desafio:Implemente a função DELETE /produtos/{id} para remover um produto do banco de dados e invalidar o cache, garantindo que a remoção seja refletida na próxima requisição GET.

# Integrantes

- João Pedro Moura Tuneli **RM:93530**

- Enzo Obayashi **RM:95634**

# Comandos para subir as Imagens:

## MySql:
```Docker
docker run --name database-mysql -e MYSQL_ROOT_PASSWORD=123 -p 3306:3306 -d
mysql
```

## Redis:
```Docker
docker run --name redis -p 6379:6379 -d redis
```

## Comando para criar o banco de dados:
```SQL
CREATE TABLE produtos (
    id INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(255) NOT NULL,
    preco DECIMAL(10, 2) NOT NULL,
    quantidade_estoque INT NOT NULL,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id)
);
```