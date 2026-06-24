# Plano de Segurança da Informação — Doc Organo

> Plano operacional adaptado ao projeto Doc Organo, com base nos conceitos de *Segurança em Tecnologia da Informação* (UNIASSELVI, 2020) e ao estado atual documentado em [State.md](../State.md) e [PM_DocOrgano.md](../Product/PM_DocOrgano.md).

## 1. Propósito e escopo

Este documento define **como aplicar** segurança da informação no Doc Organo durante o **MVP2** (migração SQL clínica), integrando-se ao workstream **WS04 — Segurança, PHI e Identity/Auth** e aos artefatos já existentes (SDD, Verifier, `security-phi-review`).

**Escopo MVP2:**

- Plataforma de gestão clínica (Paciente, Prontuário, Agendamento, Atendimento).
- Stack: ASP.NET Core 7, Blazor Server, SQL Server (Docker local), PDF/extração de documentos.
- Ambiente: uso real em clínica controlada, repositório privado, **sem auth/RBAC completo ainda** (ADR-004).

**Fora do escopo MVP2** (registrar como MVP3/futuro): RBAC avançado, cloud production hardening, certificação ISO 27001, DPO formal em produção.

## 2. Nomenclatura (correções do material acadêmico)

| Sigla | Nome completo | Papel no Doc Organo |
|-------|---------------|---------------------|
| **PSI** | Política de Segurança da Informação | Documento-mestre de regras (humano, físico, lógico, criptográfico) |
| **PCN** | Plano de Continuidade do Negócio | Manter atendimento clínico/administrativo após incidente |
| **PCO** | Plano de Contingência Operacional | Procedimentos alternativos (ex.: API fora, SQL indisponível) |
| **PRD** | Plano de Recuperação de Desastres | Restore de banco, retomada em prazo tolerável |
| **PGC** | Plano de Gerenciamento de Crises | Comunicação, decisão e escalonamento (vazamento PHI, ransomware) |
| **RAR** | Relatório de Análise de Riscos | Matriz de risco + tratamentos |
| **RIPD** | Relatório de Impacto à Proteção de Dados | Obrigação LGPD quando tratamento de alto risco |

**Nota:** no material da faculdade, “PHI” no sentido clínico (Protected Health Information) **não** substitui **PSI** (Política de Segurança da Informação). No Doc Organo, PHI = dados clínicos sensíveis; PSI = política organizacional.

## 3. Integração com a governança existente

O Doc Organo **já possui** mecanismos de segurança parciais. Este plano **não duplica** State, PM ou Verifier — complementa com PSI/PCN e relatórios formais.

| Artefato existente | Papel na segurança | Relação com este plano |
|--------------------|--------------------|-------------------------|
| `.cursor/rules/security-phi.mdc` | Regra operacional PHI | Base da PSI — seção “Desenvolvimento e IA” |
| `security-phi-review.md` | Sensor de revisão em Verify | Evidência para relatório de auditoria interna |
| `verification.md` (por SDD) | Gate de conclusão + risco residual | Entrada do RAR e do relatório de feature |
| `sdd-pilot-report-v*.md` | Calibração de processo | Não substitui RAR/RIPD; complementa governança |
| ADR-004 | Auth como capacidade de suporte | Decisão registrada na PSI — “Ambiente controlado” |
| ADR-001 | Soft delete clínico | Integridade/disponibilidade de histórico |
| `runbook.md` / `migration-sql.md` | Operação local | Base do PCO/PRD técnico |

## 4. Propriedades e ativos críticos

### 4.1 Propriedades CIA (Unidade 1)

| Propriedade | Ativo Doc Organo | Risco atual MVP2 |
|-------------|------------------|------------------|
| **Confidencialidade** | CPF, prontuário, senhas de convênio, mensagens ao médico | API sem auth; logs em controllers legados |
| **Integridade** | Prontuário versionado, jornada de Atendimento, FK Paciente | Stubs Prontuário/Agendamento; drift front/API |
| **Disponibilidade** | SQL Docker, API, Blazor | Volume Docker único; sem backup documentado |

### 4.2 Inventário de ativos (investigar e manter atualizado)

| Ativo | Local | Dono sugerido |
|-------|-------|---------------|
| `DocDb` (SQL Server) | Docker `docorgano-sql` | Desenvolvedor / clínica |
| `.env` / `SA_PASSWORD` | Repo root (gitignored) | Desenvolvedor |
| Google Sheets (legado) | MVP1 / `main` | Referência apenas |
| PDFs clínicos / Dropbox paths | Dev artifacts | Remover paths hardcoded |
| Repositório GitHub privado | `linofir/doc_Organo` | Desenvolvedor |
| Estações médico/secretária | Ambiente clínico | Clínica |

## 5. Fase 1 — Análise de riscos (Matriz de Risco)

### 5.1 Riscos já identificados no PM (ponto de partida)

| ID | Risco | Prob. | Impacto | Controle atual | Tratamento |
|----|-------|-------|---------|----------------|------------|
| R01 | PHI em logs, prompts, commits | Média | Alto | Regra + review Paciente/Atendimento OK | Remover `Console.WriteLine` em Prontuário/Agendamento; Verify em cada SDD |
| R02 | API sem auth com dados reais | Média | Alto | ADR-004 ambiente controlado | Documentar restrições na PSI; auth mínima MVP2 |
| R03 | Migração SQL incompleta / stubs | Alta | Alto | Sequência aprovada + Verify | Prontuário → Agendamento → Workflow |
| R04 | Perda do volume Docker SQL | Média | Catastrófico | Nenhum backup formal | PRD: backup + teste de restore |
| R05 | Engenharia social / phishing | Média | Alto | Treinamento clínica (externo) | PSI: uso de e-mail e dispositivos |
| R06 | Ransomware / malware | Baixa | Catastrófico | Antivírus SO (assumido) | Backup offline; PCO |
| R07 | Vazamento via Swagger exposto | Média | Alto | Dev only (assumido) | PSI: não expor API publicamente |
| R08 | Extração PDF/portais frágil | Média | Médio | Serviços legados | Logs PHI-safe; arquivos de teste sintéticos |

### 5.2 Perguntas da investigação (por risco)

Para **cada** risco acima, responder:

1. **Ameaça:** o que provoca o incidente? (humana, técnica, ambiental)
2. **Vulnerabilidade:** qual falha permite? (código, processo, configuração)
3. **Evidência:** onde verificar? (grep logs, git history, Swagger, Docker, `.env`)
4. **Probabilidade × impacto:** posição na matriz (insignificante → catastrófico)
5. **Controle:** preventivo / detectivo / reativo — existe? funciona?
6. **Dono e prazo:** quem corrige e quando?

### 5.3 Como investigar (comandos e artefatos)

```powershell
# PHI em código
rg -i "Console\.WriteLine|cpf|prontuario" DocAPI --glob "*.cs"

# Secrets expostos
rg -i "password|connectionstring|serviceaccount" . --glob "!**/bin/**" --glob "!**/obj/**"

# Estado da migração
# Ler Documentation/State.md e verification.md de cada SDD

# SQL e backup
docker volume ls
docker inspect docorgano-sql
# Verificar se existe rotina de BACKUP DATABASE documentada
```

**Entregável:** `Documentation/Security/reports/rar-doc-organo-v1.md` (template na seção 8).

**Critério satisfatório:** riscos R01–R04 com dono, ação e prazo; riscos “alto/crítico” não aceitos sem justificativa escrita.

## 6. Fase 2 — PSI (Política de Segurança da Informação)

### 6.1 Estrutura proposta (`Documentation/Security/PSI-doc-organo.md`)

| Seção PSI | Conteúdo específico Doc Organo |
|-----------|--------------------------------|
| Sumário executivo | Clínica real; MVP2; compromisso médico/desenvolvedor |
| Objetivos | Proteger PHI; garantir continuidade clínica; conformidade LGPD progressiva |
| Papéis | Desenvolvedor (CSO técnico); médico; secretária; futuro DPO |
| Ambiente controlado | ADR-004: rede local, sem exposição pública, usuários conhecidos |
| Dados clínicos | CPF, prontuário, atendimento — classificação “sigiloso” |
| Desenvolvimento | `security-phi.mdc`; SDD + Verify; dados sintéticos em testes |
| Logs e IA | Proibido PHI em commits, prompts, exemplos, `Console.WriteLine` |
| Credenciais | `.env` only; nunca JSON Google SA no repo |
| API | Swagger em Development; auth mínima quando definida |
| Soft delete | ADR-001 — não apagar histórico clínico |
| Mesa/tela limpa | Estações na clínica |
| Backup | Referência ao PRD |
| Sanções / violação | Procedimento interno clínica + registro de incidente |
| Revisão | A cada SDD major ou semestral |

### 6.2 Perguntas para redigir a PSI

| Área | Perguntas |
|------|-----------|
| Acesso físico | Quem acessa o consultório/servidor? Horários? Visitantes? |
| Acesso lógico | Quem roda API/Blazor? Máquinas compartilhadas? VPN? |
| Dados | Quais campos são PHI? O que pode ir para WhatsApp/e-mail? |
| LGPD | Quem é controlador (clínica)? Base legal? Consentimento? |
| Terceiros | GitHub, Google (legado), Dropbox — contratos e dados? |
| IA/Cursor | O que nunca entra no chat? Como revisar diffs? |

### 6.3 Critério satisfatório

- PSI escrita, aprovada pelo responsável clínico + desenvolvedor.
- Termo de ciência assinado (médico, secretária).
- Referenciada no `Readme.md` e WS04 do PM.

## 7. Fase 3 — PCN e planos de contingência

### 7.1 Cenários prioritários Doc Organo

| Cenário | Subplano | Pergunta central |
|---------|----------|------------------|
| Container SQL parado/corrompido | PRD | Restore em quanto tempo? RPO/RTO? |
| `docker compose down -v` acidental | PRD | Backup existe fora do volume? |
| API/Blazor indisponível | PCO | Atendimento manual temporário? |
| Vazamento PHI (log, commit, prompt) | PGC | Notificar titulares/ANPD? |
| Notebook roubado com `.env` | PGC + PSI | Rotacionar SA_PASSWORD? |
| Ransomware | PRD + PGC | Backup imutável? |

### 7.2 Metodologia (6 passos — Ghoddosi)

| Passo | Doc Organo — o que fazer |
|-------|--------------------------|
| 1. Avaliação do projeto | Escopo: clínica única, MVP2, Docker local |
| 2. Análise de risco | Reutilizar seção 5 deste plano |
| 3. BIA | Quantificar: horas sem sistema = impacto no consultório |
| 4. Elaborar PCN | `Documentation/Security/PCN-doc-organo.md` + checklists |
| 5. Treinar e testar | Simular restore SQL; simular API down |
| 6. Manter | Revisar após cada vertical SQL verified |

### 7.3 PCO técnico mínimo (checklist)

**Quando SQL/API cair:**

1. Confirmar sintoma (Docker, logs API, porta 1433/7004).
2. Comunicar médico/secretária (PGC).
3. Tentar `docker compose restart` (runbook).
4. Se falhar → acionar PRD (restore backup).
5. Registrar incidente (relatório seção 8).
6. Pós-mortem: atualizar runbook/PCN.

### 7.4 PRD técnico mínimo

| Item | Investigar |
|------|------------|
| Frequência de backup | Diário? Antes de migration? |
| Local | Fora do volume Docker? Criptografado? |
| Teste de restore | Última data bem-sucedida? |
| RTO | Tempo máximo aceitável offline |
| RPO | Perda máxima de dados aceitável |

**Comando investigação:**

```powershell
# Documentar procedimento após definir pasta de backup
# sqlcmd / docker exec — BACKUP DATABASE DocDb TO DISK = ...
```

**Entregável:** `Documentation/Security/PCN-doc-organo.md` com PCO, PRD e PGC resumidos.

**Critério satisfatório:** um restore testado documentado; checklist PCO impresso ou acessível na clínica.

## 8. Relatórios — o que produzir e quando

### 8.1 Mapa de relatórios

| Relatório | Quando | Onde salvar | Perguntas que deve responder |
|-----------|--------|-------------|------------------------------|
| **RAR** | Início MVP2 segurança + revisão trimestral | `Documentation/Security/reports/rar-*.md` | Quais riscos? Matriz? Ações? |
| **RIPD** | Tratamento PHI em escala / auth multi-usuário | `Documentation/Security/reports/ripd-*.md` | Finalidade, necessidade, riscos ao titular, medidas |
| **Relatório de incidente** | Qualquer suspeita de vazamento ou perda | `Documentation/Security/reports/incidente-YYYY-MM-DD.md` | O quê, quando, causa, impacto, correção, LGPD |
| **Relatório de auditoria interna** | Após 2+ SDDs verified ou semestral | `Documentation/Security/reports/auditoria-*.md` | PSI cumprida? Gaps? Plano de ação |
| **Relatório de teste PCN** | Após simulação restore/failover | `Documentation/Security/reports/teste-pcn-*.md` | Cenário, tempo, falhas, lições |
| **Feature report / verification** | Já existente por SDD | `Documentation/SDD/*/verification.md` | Risco residual aceito — **entrada** do RAR |

### 8.2 Template — Relatório de incidente (investigação até resultado)

```markdown
# Incidente — [DATA] — [TÍTULO CURTO]

## 1. Detecção
- Como foi detectado?
- Quem reportou?

## 2. Descrição (sem PHI no relatório versionado)
- Tipo: confidencialidade / integridade / disponibilidade
- Sistemas afetados: DocAPI / SQL / Front / Git / IA

## 3. Linha do tempo

## 4. Cadeia causal
- Ameaça → Vulnerabilidade → Ataque/incidente → Impacto

## 5. Contenção e erradicação

## 6. Recuperação

## 7. LGPD (se aplicável)
- Notificação ANPD/titulares necessária?

## 8. Ações corretivas (PSI/PCN/código)

## 9. Evidências (links internos, sem dados reais)
```

### 8.3 Loop de investigação (qualquer achado)

```text
PERGUNTAR → entrevista / checklist PSI
OBSERVAR  → código, Docker, clínica, git log
EVIDENCIAR → verification.md, grep, testes, backup log
COMPARAR  → prática vs PSI vs ADR-004 vs security-phi-review
CLASSIFICAR → matriz de risco
DECIDIR   → corrigir / aceitar risco / adiar (com prazo)
DOCUMENTAR → relatório adequado (seção 8.1)
TESTAR    → dotnet test, restore SQL, smoke API
REVISAR   → auditoria interna → repetir se gap crítico
```

**Parar quando:** gap crítico fechado ou risco residual explicitamente aceito no `verification.md` / RAR.

## 9. Plano alinhado ao MVP2 (cronograma sugerido)

| Ordem | Ação | Workstream | Depende de |
|-------|------|------------|------------|
| 1 | Completar remoção PHI logs (Prontuário, Agendamento) | WS04 | Prontuário SDD Execute |
| 2 | Publicar RAR v1 | Security | Inventário seção 5 |
| 3 | Redigir PSI v1 + termos | Security | RAR |
| 4 | Definir auth mínima MVP2 (ADR-004 update) | WS04 | PSI |
| 5 | PRD: backup SQL + 1 restore testado | Security | runbook |
| 6 | PCN v1 (PCO + PGC resumidos) | Security | PRD testado |
| 7 | Auditoria interna pós Prontuário Verify | Security | verification.md |
| 8 | RIPD v1 (se multi-usuário ou novos fluxos PHI) | Security | Auth definida |

**Não bloquear** a sequência SQL clínica (Paciente → Atendimento → Prontuário → …) — segurança corre **em paralelo** via Verify + WS04.

## 10. Perguntas por fatia SDD (aplicar em cada Verify)

Antes de marcar uma vertical como verified:

1. Controllers/repositories logam PHI?
2. DTOs/tests usam apenas dados sintéticos?
3. `security-phi-review` foi aplicado?
4. Stubs restantes estão explicitamente indisponíveis (não “falso positivo”)?
5. Soft delete preserva histórico clínico (ADR-001)?
6. Risco residual está escrito no `verification.md`?
7. Incidente hipotético (perda SQL) — PCN cobre?

## 11. Referências internas

- [State.md](../State.md)
- [PM_DocOrgano.md](../Product/PM_DocOrgano.md) — WS04
- [ADR-004](../Architecture/ADR/ADR-004-identity-auth-supporting-capability.md)
- [runbook.md](../Technical/runbook.md)
- [migration-sql.md](../Technical/migration-sql.md)
- [security-phi-review.md](../AI-Harness/review-prompts/security-phi-review.md)
- [reporting-strategy.md](../AI-Harness/Harness-Design/reporting-strategy.md)

## 12. Próximos artefatos a criar (backlog documental)

| Arquivo | Status |
|---------|--------|
| `Documentation/Security/PSI-doc-organo.md` | Pendente |
| `Documentation/Security/PCN-doc-organo.md` | Pendente |
| `Documentation/Security/reports/rar-doc-organo-v1.md` | Pendente |
| `Documentation/Security/reports/` (pasta) | Criada com este plano |

---

*Última atualização: 2026-06-19 — plano inicial derivado do material UNIASSELVI e do estado do repositório.*
