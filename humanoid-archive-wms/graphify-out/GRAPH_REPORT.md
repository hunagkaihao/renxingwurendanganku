# Graph Report - .  (2026-08-21)

## Corpus Check
- Large corpus: 1099 files · ~568,511 words. Semantic extraction will be expensive (many Claude tokens). Consider running on a subfolder.

## Summary
- 33 nodes · 32 edges · 2 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Domain Model - Business Entities
- System Architecture - Infrastructure

## God Nodes (most connected - your core abstractions)
1. `WarehouseManagement` - 25 edges
2. `ArchiveBox` - 1 edges
3. `ArchiveBoxDetail` - 1 edges
4. `Archive` - 1 edges
5. `CheckDetailHis` - 1 edges
6. `CheckHis` - 1 edges
7. `Check` - 1 edges
8. `CheckDetail` - 1 edges
9. `Face` - 1 edges
10. `Vein` - 1 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Import Cycles
- None detected.

## Communities (2 total, 0 thin omitted)

### Community 0 - "Domain Model - Business Entities"
Cohesion: 0.08
Nodes (25): WarehouseManagement, ArchiveBox, ArchiveBoxDetail, Archive, CheckDetailHis, CheckHis, Check, CheckDetail (+17 more)

### Community 1 - "System Architecture - Infrastructure"
Cohesion: 0.25
Nodes (8): ABP Framework, DDD Architecture, Identity Server, host, src, test, SQL Server, Warehouse Management System

## Knowledge Gaps
- **27 isolated node(s):** `ArchiveBox`, `ArchiveBoxDetail`, `Archive`, `CheckDetailHis`, `CheckHis` (+22 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `WarehouseManagement` connect `Domain Model - Business Entities` to `System Architecture - Infrastructure`?**
  _High betweenness centrality (0.944) - this node is a cross-community bridge._
- **What connects `ArchiveBox`, `ArchiveBoxDetail`, `Archive` to the rest of the system?**
  _27 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Domain Model - Business Entities` be split into smaller, more focused modules?**
  _Cohesion score 0.08 - nodes in this community are weakly interconnected._