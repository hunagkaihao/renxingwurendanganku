# Graph Report - E:\Tuta\project\renxingwurendanganku\RenXing_WCS_Web  (2026-08-24)

## Corpus Check
- Corpus is ~2,036 words - fits in a single context window. You may not need a graph.

## Summary
- 107 nodes · 108 edges · 15 communities (10 shown, 5 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_package json author 0|package json author 0]]
- [[_COMMUNITY_Wcs State Test 1|Wcs State Test 1]]
- [[_COMMUNITY_Order Page vue 2|Order Page vue 2]]
- [[_COMMUNITY_vue babel eslint 3|vue babel eslint 3]]
- [[_COMMUNITY_module jsconfig json 4|module jsconfig json 4]]
- [[_COMMUNITY_Wcs State Server 5|Wcs State Server 5]]
- [[_COMMUNITY_parser node eslint 6|parser node eslint 6]]
- [[_COMMUNITY_Order Page Data 7|Order Page Data 7]]
- [[_COMMUNITY_js axios instance 8|js axios instance 8]]
- [[_COMMUNITY_Tag Data get 9|Tag Data get 9]]
- [[_COMMUNITY_vue dependencies core 10|vue dependencies core 10]]
- [[_COMMUNITY_vue config js 11|vue config js 11]]

## God Nodes (most connected - your core abstractions)
1. `compilerOptions` - 7 edges
2. `eslintConfig` - 6 edges
3. `scripts` - 4 edges
4. `mounted()` - 3 edges
5. `paths` - 2 edges
6. `axios` - 2 edges
7. `env` - 2 edges
8. `parserOptions` - 2 edges
9. `getWcsState()` - 2 edges
10. `mounted()` - 2 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Import Cycles
- None detected.

## Communities (15 total, 5 thin omitted)

### Community 0 - "package json author 0"
Cohesion: 0.14
Nodes (13): author, browserslist, description, keywords, license, main, name, private (+5 more)

### Community 3 - "vue babel eslint 3"
Cohesion: 0.20
Nodes (10): devDependencies, @babel/core, @babel/eslint-parser, babel-plugin-component, eslint, eslint-plugin-vue, @vue/cli-plugin-babel, @vue/cli-plugin-eslint (+2 more)

### Community 4 - "module jsconfig json 4"
Cohesion: 0.22
Nodes (8): compilerOptions, baseUrl, lib, module, moduleResolution, paths, target, @/*

### Community 6 - "parser node eslint 6"
Cohesion: 0.25
Nodes (8): node, eslintConfig, env, extends, parserOptions, root, rules, parser

### Community 8 - "js axios instance 8"
Cohesion: 0.38
Nodes (4): axios, instance, instance, signalR

### Community 9 - "Tag Data get 9"
Cohesion: 0.38
Nodes (3): getMjjTagData(), getPlcTagData(), mounted()

### Community 10 - "vue dependencies core 10"
Cohesion: 0.33
Nodes (6): dependencies, core-js, element-ui, @microsoft/signalr, vue, vue-router

## Knowledge Gaps
- **39 isolated node(s):** `target`, `module`, `baseUrl`, `moduleResolution`, `@/*` (+34 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `vue dependencies core 10` to `package json author 0`, `js axios instance 8`?**
  _High betweenness centrality (0.336) - this node is a cross-community bridge._
- **Why does `axios` connect `js axios instance 8` to `vue dependencies core 10`?**
  _High betweenness centrality (0.307) - this node is a cross-community bridge._
- **Why does `devDependencies` connect `vue babel eslint 3` to `package json author 0`?**
  _High betweenness centrality (0.126) - this node is a cross-community bridge._
- **What connects `target`, `module`, `baseUrl` to the rest of the system?**
  _39 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `package json author 0` be split into smaller, more focused modules?**
  _Cohesion score 0.14285714285714285 - nodes in this community are weakly interconnected._