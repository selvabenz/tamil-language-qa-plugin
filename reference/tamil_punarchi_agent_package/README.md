# Tamil Punarchi / Sandhi QA — Coding Agent Package

**Version:** 1.0.0 (13 November 2026)  
**Audience:** coding agents implementing a conservative Tamil grammar-checking subsystem, especially Tamil IRV / USFM proofreading.  
**Scope:** classical Tamil புணர்ச்சி inventory, modern எழுத்துநடை / வல்லினம் usage, morphology, syntax, lexical exceptions, and a configurable editorial layer.

## Start here

1. Read `AGENT_PROMPT.md` and `ENGINE_SPEC.md` completely.
2. Read `rules.json` as the structured **rule inventory** (183 records), **not** as a ready-made replacement dictionary.
3. Read `RULE_REFERENCE.md` for a bilingual human-readable index of all 183 records.
4. Read `tests.jsonl` (99 regression scenarios), `FINDING_SCHEMA.json`, and `RULES_SCHEMA.json`.
5. Read `SOURCES.md` for Tamil grammar references and limits of evidence.
6. Deliver a **non-destructive** scanner and review workflow first; implement transformation or edit application only with explicit approval.

## Critical warning

**Tamil sandhi is not a character-adjacency replacement task.** A rule may depend on whether the construction is a compound, an overt case, an elided case, a participle, a relative participle, or a subject-predicate phrase. The same visible ending can therefore demand different behavior. A high-confidence suggestion requires a real grammatical analysis, often a dictionary and a reviewer.

For example:

| Input | Review result | Why |
|---|---|---|
| அறிக்கை செய்தான் | Leave unchanged | Lexical noun-final ஐ, not second-case ஐ |
| அறிக்கையை சமர்ப்பித்தான் | Suggest அறிக்கையைச் சமர்ப்பித்தான் | Overt accusative ஐ |
| வந்து சென்றான் | Leave unchanged | Menthodar participial construction |
| படித்து கொடுத்தான் | Suggest படித்துக் கொடுத்தான் | Vanthodar participial construction |
| வந்த பையன் | Leave unchanged | Positive relative participle |
| ஓடா குதிரை | Suggest ஓடாக் குதிரை | Clipped negative relative participle |

## Files

- `AGENT_PROMPT.md` — paste into a coding agent along with the package.
- `ENGINE_SPEC.md` — functional, architectural, data, Unicode, USFM, safety, and acceptance requirements.
- `RULE_REFERENCE.md` — all rule records grouped by topic and written for a human.
- `rules.json` — machine-readable, uniquely identified rule definitions and source mappings.
- `tests.jsonl` — positive, negative, compound, and USFM structural scenarios.
- `test_summary.json` — test counts.
- `FINDING_SCHEMA.json` — JSON Schema for review findings.
- `RULES_SCHEMA.json` — JSON Schema validating the machine-readable rule catalog.
- `validate_package.py` — checks schema, IDs, source links and fixture consistency; this does **not** run a full linguistic engine.
- `SOURCES.md` — grammar source links and evidence rules.
- `project_house_style.example.json` — editable per-project decisions. **Apply this only when explicitly configured.**

## Automation policy

**No direct Scripture modification.** Suggestion is not approval. Classes `LEXICON_REVIEW`, `TRANSFORM_COMPOUND`, `ALLOW_VARIANTS`, and `REVIEW` are review-only unless a lexicon entry and explicit contextual evidence exist. In an IRV project, even a high-confidence suggestion remains pending until the authorized human reviewer accepts it.

## Claim of coverage

The package is a **broad implementation inventory**, not a proof that every rare literary variation has been exhausted. The catalog includes explicit `REVIEW` and `LEXICON_REVIEW` records for areas that cannot be safely flattened into a generic rewrite rule. This makes the initial implementation appropriately conservative.

## Licence and attribution

Rules and example classifications are factual grammar descriptions, with original implementation metadata prepared for this package. External lesson pages are linked, not copied wholesale. Respect the licences and attribution requirements of any resources you subsequently embed. Do not ingest copyrighted Bible text without permission.
