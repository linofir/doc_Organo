# Título: [Dependência Técnica] Conflito de Design: Soft Delete vs. Required Relationships no EF Core

1. Descrição do Problema (O Alerta):
Ao subir a migration, o EF Core emitiu alertas sobre Global Query Filters e Optional dependent using table sharing.

Causa: O uso de filtros globais para Soft Delete (IsDeleted == false) em entidades que possuem relacionamentos obrigatórios (Required).

2. Dependência Técnica Identificada:
O banco de dados físico exige o relacionamento (FK), mas o filtro lógico do EF Core pode "esconder" o pai, fazendo com que o filho pareça órfão no nível de aplicação.

3. Impacto no Negócio / Funcionalidade:

Risco de Integridade: Consultas que esperam dados obrigatórios retornarão nulo, causando NullReferenceException.

Complexidade de Manutenção: Desenvolvedores precisarão lembrar de usar .IgnoreQueryFilters() em auditorias ou relatórios, aumentando a chance de bugs financeiros/médicos.

4. Hipótese Técnica:
O design atual de Soft Delete é incompatível com relacionamentos 1:1 estritos. Precisamos decidir se tornamos o relacionamento opcional (permitindo nulos) ou se abandonamos o Soft Delete físico em favor de uma tabela de histórico.

5. Ações Sugeridas:

Curto Prazo: Criar um teste de integração para validar o comportamento da query filter com o relacionamento atual.

Médio Prazo: Realizar um Spike de 4 horas para avaliar o uso de Shadow Properties para auditoria em vez de filtros globais.
