# Local OTCV reference evaluation

The user supplied `D:\Claude Lab\OTCV Tamil` for read-only use. It contains all 66 canonical USFM books and an accompanying copyright notice identifying Biblica's Open Indian Tamil Contemporary Version, copyright 2005, 2020, 2022, under CC BY-SA 4.0. The notice reports source files dated 21 May 2025 and HTML generated 30 October 2026. Exact revision equivalence with the installed Paratext OTCV resource remains unverified. The enhanced BSI TaOVM+ resource is still pending.

## Method

The existing installed build 8 core (`0.1.0-dev.8+g7faca08f`, profile `scripture-modern-2`) and catalog were used without changing linguistic rules. Both OTCV partitions were evaluated after the profile was frozen; neither was used for tuning. The legacy `development` partition is Genesis, Psalms, Matthew and John; `holdout` contains the other 62 books. These names describe the evaluator's book partitions, not an independently adjudicated natural-error gold set.

OTCV filenames use distribution numbering (for example Genesis starts with `02`, Acts with `74`). The evaluator now identifies books by canonical USFM `\id` markers, requires all 66 unique identities, and refuses missing, duplicate or unknown identities. It reads strict UTF-8, preserves USFM segment boundaries and compares each evaluated file's bytes before and after scanning.

```powershell
./scripts/evaluate-reference.ps1 -ReferenceDir '<local OTCV directory>' -ReferenceId otcv -Stage development
./scripts/evaluate-reference.ps1 -ReferenceDir '<local OTCV directory>' -ReferenceId otcv -Stage holdout
python tests-corpus/test_reference_identity.py
```

## Results

See [text-free aggregate evidence](otcv-reference-evaluation-build8.json).

| Measurement | First 4 books | Other 62 books | Total |
|---|---:|---:|---:|
| Chapters | 249 | 940 | 1,189 |
| Verse units | 5,943 | 25,159 | 31,102 |
| Examined word pairs | 58,632 | 304,549 | 363,181 |
| Supported word pairs | 2,578 | 10,082 | 12,660 |
| Proposed changes | 297 | 1,494 | 1,791 |
| Review-only findings | 602 | 3,327 | 3,929 |
| Controlled missing-ottu candidates | 6,384 | 29,660 | 36,044 |
| Reconstructed controlled cases | 2,278 | 8,577 | 10,855 |

All 66 input files remained byte-for-byte unchanged. Controlled reconstruction is **30.12%**, a measure of coverage on isolated word-pair mutations, not precision or natural-error recall. The 1,791 proposals need linguistic adjudication and edition/register review; they are not confirmed errors in OTCV. No independent natural-error accuracy score or 100% claim is established.

## Private review and remaining work

The private offline page is `artifacts/private-otcv-sandhi-review.html`. Its local decision namespace and downloaded decision filename are separate from the BSI review. It shows proposal context, accepts local human decisions and cannot edit Scripture. Raw Scripture, per-verse findings, manifests, generated review pages and decisions remain ignored and outside releases.

Run outputs: `artifacts/otcv-reference-development-20261114-095421/` and `artifacts/otcv-reference-holdout-20261114-095432/`. Each contains its private findings, input manifest and summary. These outputs and the accompanying licence notice remain local.

Next: adjudicate proposals, confirm edition policy and revision, evaluate independent natural errors, and obtain an approved source for enhanced BSI TaOVM+. The local OTCV export resolves the text-access need for this comparison; it does not change Paratext resource API permissions.

Evaluator validation passed synthetic misleading-filename partition checks, duplicate/missing/unknown identity refusal, source preservation, and a BSI development regression with unchanged results (22 proposals, 239 review-only, 2,828/5,856 controlled reconstruction). Generated OTCV review JavaScript passes `node --check`. Product binaries and installed build are unchanged; only evaluation tools and documentation changed.
