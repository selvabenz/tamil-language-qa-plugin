# Implementation roadmap

The detailed [product plan](PRODUCT_PLAN.md) records the verse-by-verse workflow, native-style interface, review actions, safe writes, and version requirements. Its phased delivery plan supersedes the earlier diagnostic-only final release goal.

## 1. Establish the baseline

- Preserve the supplied package and validate its 183 records and 99 fixtures.
- Review grammar evidence and identify conflicting or incomplete examples.
- Define the first supported modern Tamil constructions and an expert-reviewed evaluation dataset independent of development fixtures.

## 2. Build the offline core

- Use immutable source text, USFM text extraction, Tamil grapheme segmentation, and exact original byte-span mapping.
- Define morphology and syntax annotation interfaces and an offline lexicon with documented licensing.
- Distinguish lexical ஐ from accusative ஐ and compound boundaries from free phrases.
- Evaluate protective rules and scoped house style before producing suggestions.
- Emit evidence, Tamil explanations, source IDs, and explicit uncertainty. Do not use suffix-only global replacement rules.

## 3. Integrate with Paratext

- Confirm the SDK, framework, architecture, and deployment requirements against the installed 9.5.110.1 host.
- Build a minimal plugin that opens a review window before connecting the checker.
- Read only authorized text through supported host interfaces, scan locally, and display verse-linked findings.
- Deliver a read-only preview first, then add explicit per-occurrence Apply and Modify & Apply through a validated, locked host transaction. Reject and Ignore never change Scripture.

## 4. Validate a release

- Execute linguistic tests rather than only validating fixture structure.
- Test ambiguous parses, protected constructions, opt-in overrides, normalization variants, stable offsets, repeated scans, and byte-preserving USFM handling.
- Evaluate unseen expert-labeled text; publish precision, recall, abstention rate, dataset size, and limitations by rule category.
- Test loading, scanning, cancellation, review navigation, and error recovery on Paratext 9.5.110.1 with networking disabled.
- Document the exact host build, SDK version, installation process, supported scope, and remaining gaps.

The safe-application design is now specified in the product plan: human-triggered writes, stale-source detection under the host write lock, an audit journal, and guarded reversion. Implementation and host verification remain pending.
