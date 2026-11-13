# Task for coding agent: implement Tamil Sandhi QA without damaging authoritative text

You are a senior engineer building a production-quality Tamil புணர்ச்சி / சந்தி QA engine, initially for Tamil Bible USFM, extensible to arbitrary Tamil prose.

**Read all files in this package first.** `ENGINE_SPEC.md` is normative; `rules.json` is the rule inventory; `tests.jsonl` is the regression fixture set; `project_house_style.example.json` is an optional project-specific configuration; `FINDING_SCHEMA.json` defines the required diagnostic object.

## Objective

Deliver an auditable, pluggable engine that detects plausible sandhi omissions, wrong additions, lexical/inflection confusion, and compound transformations. It must understand syntactic and morphological categories when possible, mark unanalysed cases as uncertain, and never overwrite authoritative text automatically.

## Non-negotiable requirements

- NEVER implement a global regex like “if previous word ends with ஐ and next begins with ச, insert ச்.” Lexical ஐ ≠ accusative ஐ.
- NEVER apply classical compounds mechanically to independently written sentence words.
- NEVER equate every `உ`-ending word with குற்றியலுகரம்.
- NEVER assume all participles behave alike: `வந்து சென்றான்` is protected; `படித்துக் கொடுத்தான்` is a positive example.
- NEVER insert an extra letter before தேவன்-family terms when the IRV house-style override is enabled. Example: `அவனை தேவசாயலாக` must be preserved.
- Preserve configured canonical spelling `யெகோவா`; do not rename divine names automatically.
- Protect USFM structure and exact raw text; operate on a parsed text layer and emit offsets and suggested patches rather than rewriting input.
- Do not use Bible websites as grammar evidence. Use the cited Tamil language grammar references and local editorial rules.
- Do not introduce any AI reviewer identity/credit or claim that AI approved an edit. All approvals must be explicitly human.

## Deliverables

1. Clear design identifying tokenizer, Unicode grapheme handling, USFM extraction and offset map, morphology/syntax adapter, rule evaluator, override manager, diagnostic reporter, reviewer workflow, and test runner.
2. An implementation in the project's existing language/tooling. If no repository is supplied, create a small language-neutral CLI/API reference implementation with no invented dependencies or external API requirements.
3. An internal enum for `decision`, `tier`, `layer`, `rule_id`, evidence confidence, and blockers.
4. A validated `rules.json` loader and a `tests.jsonl` regression runner.
5. Typed findings conforming to `FINDING_SCHEMA.json`.
6. Tests for all positive and negative fixtures, USFM round trip, Unicode normalization, project exceptions, ambiguous parses, duplicate diagnostics, and repeated scans.
7. A reviewer UI/API that shows complete sentence context, proposed suggestion, source rule ID, parsed grammatical justification, confidence, source link, and decision choices: approve / reject / leave open.
8. Audit logs and exportable review results; application of any accepted change requires separate human confirmation and preserves original content.
9. Documentation describing **which subset** is implemented, which rules require a parser/lexicon, how unknowns are reported, and any differences from the catalog.

## Implementation phases

- **Phase A:** validate catalog and fixtures, USFM-safe Tamil tokenization, annotation interfaces, protect/no-change checks, read-only finding output.
- **Phase B:** implement syntactic and morphological classification (including lexical-root-final ஐ vs overt ஐ); surface contextual suggestions for well-defined வல்லினம் cases.
- **Phase C:** curated lexical compound dictionary, noun declensions, குற்றியலுகரம் classes and classical variants. Add only after evidence and regression coverage.
- **Phase D:** editorial approval workflow and integration with existing Language QA → Flags → Record check process, if available in the repository.

## Acceptance gates

- All 99 catalog fixture scenarios are consumed and correctly typed; morphological derivations may be checked against an approved lexical dictionary rather than generated automatically.
- All `A`-series protected tests must never produce an insertion suggestion.
- All `B`-series examples must suggest the expected correction **only if** the required grammatical annotations are established. Missing annotations must downgrade to review/open, not presume a fix.
- All `C`-series pairs are lexicon-restricted compound examples, not arbitrary free-text rewrite instructions.
- All `D`-series fixtures must preserve raw USFM/text bytes on scan; do not edit even one marker, whitespace token, or verse number.
- 100% of findings identify an actual `rules.json` rule ID and include a human-readable `காரணம்:` explanation in Tamil.
- No unapproved modification of authoritative Bible text; edits are always reversible and auditable.

Begin with an architecture summary and gap analysis, then implement code/tests, run them, and report passing/failing tests honestly. Do not silently weaken these tests to make them pass.
