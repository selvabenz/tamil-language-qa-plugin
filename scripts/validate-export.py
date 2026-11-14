"""Check actual C# finding output against the unchanged supplied schema."""
import json
from pathlib import Path
from jsonschema import Draft202012Validator

root = Path(__file__).resolve().parent.parent
schema = json.loads((root / 'reference/tamil_punarchi_agent_package/FINDING_SCHEMA.json').read_text(encoding='utf-8'))
sample = json.loads((root / 'artifacts/build/finding-schema-sample.json').read_text(encoding='utf-8-sig'))
Draft202012Validator(schema).validate(sample)
context_samples = json.loads((root / 'artifacts/build/scripture-finding-schema-samples.json').read_text(encoding='utf-8-sig'))
for context_sample in context_samples:
    Draft202012Validator(schema).validate(context_sample)
print('C# finding export: original JSON Schema validation passed')
