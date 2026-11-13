"""Static QA of catalog and fixtures; does not execute a Tamil QA engine."""
from pathlib import Path
import json
from collections import Counter

P = Path(__file__).resolve().parent
catalog = json.loads((P / 'rules.json').read_text(encoding='utf-8'))
rows = [json.loads(line) for line in (P / 'tests.jsonl').read_text(encoding='utf-8').splitlines()]
ids = [rule['id'] for rule in catalog['rules']]
assert len(ids) == catalog['rule_count'] == len(set(ids)), 'Rule IDs/count mismatch'
assert all(set(r['source_ids']) <= set(catalog['sources']) for r in catalog['rules']), 'Unknown source ID'
assert len(rows) == len({r['id'] for r in rows}), 'Duplicate case ID'
assert all(set(r.get('expected_rule_ids', [])) <= set(ids) for r in rows), 'Test references missing rule'
assert all(r.get('must_not_change_automatically') is True for r in rows), 'Unexpected auto edit test'
assert all(r['expected_text_if_approved'] == r['input_text'] for r in rows if r['id'].startswith('A')), 'Protected case changed'
try:
    import jsonschema
except ImportError:
    print('jsonschema not installed; using built-in structural validation')
else:
    for f in ['FINDING_SCHEMA.json', 'RULES_SCHEMA.json']:
        jsonschema.Draft202012Validator.check_schema(json.loads((P / f).read_text(encoding='utf-8')))
    schema = json.loads((P / 'RULES_SCHEMA.json').read_text(encoding='utf-8'))
    jsonschema.Draft202012Validator(schema).validate(catalog)
    print('JSON Schema: OK')
print(f'Catalog static validation: OK ({len(ids)} entries)')
print(f'Test fixture validation: OK ({len(rows)} examples)')
print('Breakdown:', dict(Counter(r['category'] for r in catalog['rules'])))
print('NOTE: this validates package structure, not linguistic prediction accuracy or engine functionality.')
