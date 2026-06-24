# RAR — Relatório de Análise de Riscos — Doc Organo v1

> Rascunho inicial para preenchimento. Use [plano-seguranca-informacao.md](../plano-seguranca-informacao.md) seção 5 como guia de investigação.

**Data:** _preencher_  
**Responsável:** _preencher_  
**Escopo:** MVP2 — clínica controlada, Docker SQL local, branch `feature/harness`

## 1. Contexto

- Produto: Doc Organo — gestão clínica (PHI: CPF, prontuário, atendimento, senhas convênio).
- Controles existentes: `security-phi.mdc`, SDD Verify, Paciente/Atendimento PHI-safe.
- Lacunas conhecidas: auth ausente (ADR-004), logs legados, backup SQL não formalizado.

## 2. Matriz de riscos (preencher após investigação)

| ID | Risco | Prob. (1-5) | Impacto (1-5) | Nível | Controle atual | Tratamento | Dono | Prazo | Status |
|----|-------|-------------|---------------|-------|----------------|------------|------|-------|--------|
| R01 | PHI em logs/prompts/commits | | | | Parcial | | | | Aberto |
| R02 | API sem auth | | | | ADR-004 | | | | Aberto |
| R03 | Stubs SQL / migração incompleta | | | | Sequência SDD | | | | Em progresso |
| R04 | Perda volume Docker | | | | Nenhum | PRD backup | | | Aberto |
| R05 | Engenharia social | | | | — | PSI treinamento | | | Aberto |
| R06 | Ransomware | | | | — | Backup offline | | | Aberto |
| R07 | API exposta publicamente | | | | Assumido dev | PSI | | | Aberto |
| R08 | Extração PDF frágil | | | | Parcial | Testes sintéticos | | | Aberto |

**Legenda nível:** Prob. × Impacto → pequeno / moderado / alto / crítico (matriz UNIASSELVI).

## 3. Evidências coletadas

| Risco | Evidência | Arquivo/comando |
|-------|-----------|-----------------|
| R01 | | `rg Console.WriteLine DocAPI` |
| R02 | | Swagger sem `[Authorize]` |
| R04 | | Último backup: _n/a_ |

## 4. Riscos aceitos (justificativa formal)

| ID | Justificativa | Aprovado por | Data |
|----|---------------|--------------|------|
| R02 | Ambiente controlado MVP2 — ADR-004 | | |

## 5. Plano de ação prioritário

1. _Preencher após investigação_
2.
3.

## 6. Próxima revisão

Data: _trimestral ou pós Prontuário Verify_
