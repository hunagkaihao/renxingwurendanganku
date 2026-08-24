import json
from pathlib import Path
from collections import defaultdict

detect = json.loads(Path('graphify-out/.graphify_detect.json').read_text(encoding='utf-16'))

# Extract architectural concepts from file structure
nodes = []
edges = []
node_ids = set()

def normalize_id(path, entity):
    """Create node ID from path and entity name"""
    # Get relative path from scan root
    scan_root = Path(detect['scan_root'])
    try:
        rel = Path(path).relative_to(scan_root)
    except:
        rel = Path(path)
    
    # Build ID from path segments
    parts = []
    for part in rel.parts:
        # Normalize each part
        normalized = ''.join(c if c.isalnum() else '_' for c in part.lower())
        if normalized:
            parts.append(normalized)
    
    # Add entity name
    if entity:
        entity_norm = ''.join(c if c.isalnum() else '_' for c in entity.lower())
        parts.append(entity_norm)
    
    return '_'.join(parts)

def add_node(id, label, file_type, source_file):
    """Add a node if not already present"""
    if id not in node_ids:
        nodes.append({
            'id': id,
            'label': label,
            'file_type': file_type,
            'source_file': source_file,
            'source_location': None,
            'source_url': None,
            'captured_at': None,
            'author': None,
            'contributor': None
        })
        node_ids.add(id)
    return id

def add_edge(source, target, relation, confidence, confidence_score, source_file):
    """Add an edge"""
    edges.append({
        'source': source,
        'target': target,
        'relation': relation,
        'confidence': confidence,
        'confidence_score': confidence_score,
        'source_file': source_file,
        'source_location': None,
        'weight': 1.0
    })

# Extract from file structure
code_files = detect.get('files', {}).get('code', [])

# Group by module/service
modules = defaultdict(list)
services = defaultdict(list)

for f in code_files:
    p = Path(f)
    parts = p.relative_to(detect['scan_root']).parts
    
    if len(parts) >= 2:
        if parts[0] == 'modules':
            module_name = parts[1] if len(parts) > 1 else 'unknown'
            modules[module_name].append(f)
        elif parts[0] == 'services':
            service_name = parts[1] if len(parts) > 1 else 'unknown'
            services[service_name].append(f)

# Create module nodes
for module_name, files in modules.items():
    node_id = add_node(
        f'module_{module_name.lower()}',
        module_name,
        'code',
        files[0] if files else ''
    )
    
    # Find domain concepts in module
    for f in files:
        p = Path(f)
        if 'Domain' in str(p) and 'Aggregates' in str(p):
            # Domain aggregate
            entity_name = p.stem
            if entity_name and not entity_name.startswith('I'):
                entity_id = add_node(
                    normalize_id(f, entity_name),
                    entity_name,
                    'code',
                    f
                )
                add_edge(node_id, entity_id, 'contains', 'EXTRACTED', 1.0, f)

# Create service nodes
for service_name, files in services.items():
    node_id = add_node(
        f'service_{service_name.lower()}',
        service_name,
        'code',
        files[0] if files else ''
    )

# Add key architectural concepts
arch_concepts = [
    ('warehouse_management_system', 'Warehouse Management System'),
    ('identity_server', 'Identity Server'),
    ('sql_server', 'SQL Server'),
    ('abp_framework', 'ABP Framework'),
    ('ddd_architecture', 'DDD Architecture'),
]

for concept_id, concept_label in arch_concepts:
    add_node(concept_id, concept_label, 'concept', '')

# Add relationships between concepts
add_edge('warehouse_management_system', 'identity_server', 'depends_on', 'EXTRACTED', 1.0, '')
add_edge('warehouse_management_system', 'sql_server', 'depends_on', 'EXTRACTED', 1.0, '')
add_edge('warehouse_management_system', 'abp_framework', 'uses', 'EXTRACTED', 1.0, '')
add_edge('abp_framework', 'ddd_architecture', 'implements', 'EXTRACTED', 1.0, '')

# Add module to system relationship
for module_name in modules:
    module_id = f'module_{module_name.lower()}'
    if module_id in node_ids:
        add_edge('warehouse_management_system', module_id, 'contains', 'EXTRACTED', 1.0, '')

# Add service to system relationship
for service_name in services:
    service_id = f'service_{service_name.lower()}'
    if service_id in node_ids:
        add_edge('warehouse_management_system', service_id, 'contains', 'EXTRACTED', 1.0, '')

# Create the extraction JSON
extraction = {
    'nodes': nodes,
    'edges': edges,
    'hyperedges': [],
    'input_tokens': 0,
    'output_tokens': 0
}

# Write to file
Path('graphify-out/.graphify_semantic.json').write_text(
    json.dumps(extraction, indent=2, ensure_ascii=False),
    encoding='utf-8'
)

print(f'Extracted: {len(nodes)} nodes, {len(edges)} edges')
print(f'Modules: {list(modules.keys())}')
print(f'Services: {list(services.keys())}')
