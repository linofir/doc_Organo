### Context Acquisition Governance — Lessons Learned from the First Multi-Tool Feature Implementation
Version: 3.0
Date: 2026-07-09
## Executive Summary
The first large-scale feature implementation executed after the Multi-Tool Governance migration exposed a significant difference in execution behavior between AI tools.
Although the implementation governance, architectural guidance, and execution workflow were correctly followed, the execution agent performed an extensive repository exploration before beginning implementation.
This behavior resulted in excessive context acquisition, high token consumption, exhaustion of the execution window, and zero completed implementation tasks.
The investigation concludes that this is not a model quality issue, nor an implementation failure.
Instead, it reveals a governance gap:
> The AI Harness currently governs what information is authoritative, but does not yet govern how execution context should be progressively acquired.
This finding represents an opportunity to strengthen the Harness methodology and improve its independence from tool-specific context management capabilities.
## Pilot Context
Feature:
> Prontuario SQL Stabilization
Execution Environment:
- Cline
- Kimi K2.7 Code
- Multi-Tool Harness
- Execute Phase
Previous experience:
The same repository had been successfully developed for months using Cursor without similar behavior.
This was also the largest feature implemented so far, making it the first realistic stress test of the new Multi-Tool governance.
## Observed Behavior
Before implementing the first task, the execution agent:
- validated Docker environment;
- executed build;
- executed migration checks;
- executed integration tests;
- explored a substantial portion of the repository.
Repository exploration included:
- Controllers
- Repositories
- DTOs
- Entities
- EF Configurations
- Blazor Frontend
- Mapping Profiles
- Technical documentation
- Product documentation
- ADRs
- Harness documentation
- Legacy implementation
No implementation task was started before the execution budget was exhausted.
## Evidence
### Repository Exploration
Initial exploration:
- 98 files
- 2 folders
- multiple repository searches
Additional exploration:
- 78 files
- 4 folders
Approximate total:
- 176 files loaded
### Documentation Loaded
Examples include:
- PM documentation
- State.md
- Migration guides
- ADRs
- Domain Overview
- Harness Architecture
- Reporting Strategy
- Verification Governance
- Knowledge Strategy
- Research documentation
- SDD templates
Most of these documents were not required to implement the first execution batch.
### Legacy Code
Legacy implementation was eventually loaded:
```
Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs

```
Approximately:
- 600 lines
This occurred before any implementation work had begun.
The repository had initially blocked access through .clineignore.
The restriction was intentionally removed because the Harness allows consultation of legacy implementations whenever behavioral comparison becomes necessary.
The experiment demonstrated that this flexibility requires additional governance defining when legacy artifacts should be consulted.
### Resource Consumption
Observed consequences:
- 5-hour execution window exhausted
- approximately 20% monthly execution quota consumed
- zero implementation tasks completed
## Root Cause Analysis
### Immediate Cause
The execution agent attempted to reconstruct architectural understanding of the repository before beginning implementation.
Instead of progressively acquiring context during execution, the model tried to understand nearly the entire system first.
### Contributing Factors
#### Execute workflow did not explicitly govern context acquisition.
The workflow assumed that execution agents would naturally perform incremental repository exploration.
This assumption proved valid for Cursor, but not for Cline.
#### Tool behavior differs significantly.
Cursor performs aggressive internal context optimization.
Typical behavior:
- open few files
- implement
- expand context only when required
Cline delegates context acquisition almost entirely to the language model.
As a consequence, repository exploration becomes highly dependent on model behavior.
#### Large execution scope
The feature contained numerous implementation tasks.
Without explicit execution batching, the model interpreted the request as requiring complete architectural understanding before implementation.
## Architectural Finding
The incident does not indicate that Cline or Kimi performed incorrectly.
Instead, it demonstrates that:
> Multi-tool governance cannot rely on tool-specific context optimization.
The methodology itself must govern context acquisition.
This finding is consistent with the architectural vision established during the Multi-Tool initiative:
- Harness Assets define methodology.
- Tool Assets implement methodology.
- Behavioral expectations must remain identical across tools.
Context acquisition therefore becomes a governance concern rather than a tool capability.
## Proposed Evolution
### Introduce Context Acquisition Governance
A new cross-cutting governance concept should be evaluated.
Rather than belonging exclusively to Execute, it should apply throughout the entire SDD workflow.
Each phase should explicitly define:
- minimum required context;
- authoritative sources;
- prohibited exploratory loading;
- conditions allowing context expansion;
- expected context budget;
- stopping criteria for further exploration.
### Progressive Context Expansion
The default execution strategy should become:
1. Start with minimum required context.
2. Execute current task.
3. Expand context only when blocked.
4. Record the reason for expansion.
5. Continue execution.
The objective is not to restrict access to repository knowledge.
The objective is to govern when additional knowledge should be acquired.
### Execution Batches
Large implementations should be divided into independent execution batches.
Each batch should:
- load only required artifacts;
- complete implementation;
- build;
- execute tests;
- report progress;
- continue with the next batch.
This reduces execution cost while improving reliability.
### Legacy Context Policy
Existing governance already establishes that legacy implementations are reference sources rather than primary implementation sources.
However, this experiment suggests that the operational workflow should make this behavior more explicit.
Legacy implementations should only become execution context after the current implementation task demonstrates that behavioral comparison is actually required.
This recommendation should be validated during governance review.
## Expected Benefits
If adopted, the proposed governance improvements are expected to produce:
- significantly lower token consumption;
- lower execution cost;
- higher implementation throughput;
- predictable execution behavior across AI tools;
- reduced unnecessary repository exploration;
- improved scalability for large features.
## Recommendations
### Immediate
Review the AI Harness workflow and introduce explicit execution guidance for progressive context acquisition.
Particular attention should be given to the Execute phase, including support for execution batches and controlled context expansion.
### Medium Term
Evaluate introducing Context Acquisition Governance as a transversal concern of the entire SDD workflow.
Rather than prescribing which files may never be read, the governance should define when additional context becomes justified during execution.
This preserves flexibility for legitimate architectural discoveries while preventing unnecessary repository-wide exploration.
## Conclusion
This pilot successfully validated the architectural independence introduced by the Multi-Tool initiative.
Rather than exposing a limitation of a specific AI tool, it revealed an implicit assumption in the current Harness methodology.
By making context acquisition an explicit governance responsibility, the Harness can become more predictable, more scalable, and genuinely tool-independent—allowing different AI tools to achieve consistent behavior without relying on proprietary context management capabilities.
