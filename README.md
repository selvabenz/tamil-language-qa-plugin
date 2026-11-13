# Tamil Language QA Plugin

An offline Tamil புணர்ச்சி / sandhi checker planned for **Paratext 9.5.110.1 on Windows**.

**Status: project foundation.** This repository contains a rule inventory, regression fixtures, validation tooling, and an implementation roadmap. It does not yet contain a working checker, Paratext plugin DLL, or installer. Compatibility with the target Paratext build has not been tested.

## Goals

- Check Tamil sandhi locally, without cloud services, telemetry, or uploading project text.
- Explain findings in Tamil using rule IDs and grammatical evidence.
- Preserve USFM, Unicode, verse references, whitespace, and original text during scans.
- Present suggestions for human review; keep uncertain analyses open.
- Scan a selected Scripture scope verse by verse, with Apply, Reject, Ignore, and Modify & Apply actions in a Paratext-style window and, where supported, inline context menus.
- Keep project house style separate from general Tamil grammar.

## Accuracy

The ambition is 100% accuracy, but no such result has been demonstrated. Tamil sandhi depends on morphology, syntax, lexical meaning, and editorial context. The checker must abstain when evidence is insufficient. Abstention must be reported separately and must not be counted as a correct prediction.

Release reports must state the tested scope, dataset version, precision, recall, false positives, false negatives, and unresolved cases. Passing the supplied fixtures alone cannot establish accuracy on unseen Tamil text.

## Supplied reference package

The original package is preserved under [`reference/tamil_punarchi_agent_package`](reference/tamil_punarchi_agent_package/README.md):

- 183 rule records in `rules.json`.
- 99 regression scenarios in `tests.jsonl`.
- Engineering specification, diagnostic schemas, grammar source links, and example project configuration.

Documents such as `AGENT_PROMPT.md` are supplied reference material. Their embedded instructions are distinct from the repository owner's current requests and do not independently authorize implementation, text edits, or publication actions. The example house style is opt-in.

The package's generation scripts retain their original `/mnt/data/...` paths for provenance. Do not run them to regenerate files without first adapting and reviewing those paths.

## Validate the package

Python 3.12 is used for validation. These are development dependencies; the future plugin's runtime dependencies remain to be decided.

```powershell
python -m pip install -r requirements-dev.txt
python reference/tamil_punarchi_agent_package/validate_package.py
```

The validator checks catalog and fixture structure, including JSON Schema when `jsonschema` is installed. It does **not** execute a linguistic checker or test Paratext compatibility. GitHub Actions runs it with schema validation installed.

## Next steps

See the [product and safe-edit plan](docs/PRODUCT_PLAN.md), [implementation roadmap](docs/ROADMAP.md), and [Paratext compatibility plan](docs/PARATEXT_COMPATIBILITY.md). The development version baseline is [`0.1.0-dev`](VERSION); no plugin binary has been built yet.

Implementation is tracked by the [delivery epic](https://github.com/selvabenz/tamil-language-qa-plugin/issues/1), [project board](https://github.com/users/selvabenz/projects/4), and [issue index](docs/GITHUB_TRACKING.md).

## Attribution and licensing

The supplied package retains its original attribution and source notes in [SOURCES.md](reference/tamil_punarchi_agent_package/SOURCES.md). A repository-wide distribution license has not yet been selected. Public visibility alone does not grant an open-source license. Do not commit proprietary Paratext binaries, credentials, or restricted project text.
