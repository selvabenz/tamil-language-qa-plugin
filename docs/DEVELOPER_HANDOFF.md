# Developer handoff

Updated 2026-11-14 after review fixes #16–#30.

## Current Scripture-context build 8

OTCV follow-up: the user supplied a local 66-book OTCV USFM export. The unchanged build 8 profile evaluated all 1,189 chapters / 31,102 verse units read-only: **1,791 unadjudicated proposals**, **3,929 review-only findings**, and **10,855/36,044 (30.12%)** controlled missing-ottu reconstructions. All inputs remained unchanged. See [OTCV method, results and validation](OTCV_REFERENCE_EVALUATION.md). The private page is `artifacts/private-otcv-sandhi-review.html`, with separate decisions from BSI. Canonical USFM IDs now determine evaluator book identity; distribution filename numbers are ignored. The local export resolves OTCV access for this comparison; enhanced TaOVM+ access and independent natural-error evaluation remain open. Product rules/binaries were not changed by this evaluation.

Current source includes the [Scripture-context fixes](SCRIPTURE_CONTEXT_FIXES.md) for SR-01–SR-05, tracked in #26–#30. Local build passes **708 core assertions**, 51 synthetic Scripture-style cases, the native SDK suite, all finding-export schema checks and reference/build-integrity validators. All 22 unambiguous B corrections are mandatory; B004 abstains and B010 is idempotent. See the complete [rule capability matrix](RULE_CAPABILITIES.md).

The revised engine uses `ScriptureRuleProfile.cs`: 11 bounded modern addition families and 5 non-addition checks, with insertion/replacement/removal, ambiguity blockers, enforced tier/register, and explicit coverage counts. All data stays offline. Full sentence semantics and independent Scripture accuracy remain unverified. Original reference files remain unchanged; maintained expectation corrections are in `tests/fixtures/reference-overrides.json`.

The user authorized a read-only comparison with OTCV, TAOVBSI and enhanced TaOVM+, and normal restarts. Candidate `0.1.0-dev.7+g78186d91` was installed and checksum-verified. The resource benchmark was blocked by the host: OTCV/TAOVBSI denied GetUSFM and TaOVM+ was not exposed as readable IProject. **Zero chapters were read and no accuracy score exists.** See [aggregate access evidence](resource-benchmark-build7.json) and [remaining gate #31](https://github.com/selvabenz/tamil-language-qa-plugin/issues/31). The developer-only helper must respect this refusal; no token borrowing, private API or decryption workaround is permitted. Build 6 evidence below remains historical until a new host acceptance report is recorded.

The user subsequently supplied a local 66-book BSI OV USFM reference. The revised profile and evaluation protocol are documented in [local reference evaluation](BSI_REFERENCE_EVALUATION.md). The original host API block remains for the resources, but this export permits a read-only BSI comparison. Build 8 is installed for the reference-driven changes; build 7 host evidence is historical.

Frozen-profile BSI evaluation completed across 66 books, 1,189 chapters and 31,102 verse units, with all inputs unchanged. There are 264 unadjudicated proposals and 1,596 review-only findings; 13,652/33,589 controlled missing-ஒற்று cases are reconstructed (coverage, not natural-error accuracy). See [aggregate results](bsi-reference-evaluation-build8.json). Remaining work: adjudicate proposals, confirm edition/register policy, expand unsupported morphology/compounds, obtain separate natural-error gold data, and obtain approved access for the other resources. Do not promise 100% reliability from a clean or low-finding scan.

Installed product: **0.1.0-dev.8+g7faca08f**, source `7faca08f64c38ad297a1459ba732a7531f1e5d9b`. [Exact-host build 8 evidence](host-acceptance-build8.json) passes insertion/replacement/removal Apply/Revert, post-note edits, stale refusal, Modify & Apply, homograph/naming/deity abstention and exact original disposable-chapter restoration. The consumed developer helper was removed; only the normal product plugin remains. Wrapped raw-source host coverage remains unexercised because the host canonicalizes wrapping; core whitespace regressions pass. Package SHA-256: `b9ff4432566c55caaceccda3708084232e3a42f4dcb9b576b089a869a151f11c`.

The private reviewer page is `artifacts/private-bsi-sandhi-review.html`. It contains the 264 proposals and verse context; do not commit or upload it or its saved decisions. It is a local adjudication tool, not an editor. The corpus and private findings remain outside release artifacts.

## Historical build 6 baseline

- Repository: https://github.com/selvabenz/tamil-language-qa-plugin ; branch `main`. [Project board](https://github.com/users/selvabenz/projects/4).
- Original delivery issues #1–#15 remain open until their broader acceptance criteria pass.
- At the earlier baseline, installed and published build: `0.1.0-dev.6+g73fb595c`, targeting Paratext **9.5.110.1**, SDK **2.0.100**, .NET Framework **4.8**.
- Build 6 was installed, checksum-verified and published as a development prerelease. The consumed developer helper was removed. [Exact-host build 6 evidence](host-acceptance-build6.json) records the completed transaction/post-note/export tests and original disposable-chapter restoration.
- Local validation: **403 core assertions**, SDK/native-control/toolbar/narrow-layout checks, finding schema and original reference validators, and isolated Git/package-integrity regression tests pass. Check the Actions run for the exact checkout before claiming CI acceptance.
- At that baseline, linguistic coverage was 10 expected predictions out of 24 conditional development scenarios; the other 14 are unimplemented or abstained. All 99 supplied fixtures are consumed for their relevant checks. Independent linguistic accuracy is unmeasured; 100% accuracy is not achieved.

## Review fixes

| Issue | Change | Evidence |
| --- | --- | --- |
| [#16](https://github.com/selvabenz/tamil-language-qa-plugin/issues/16) | Preserve the surrounding verse state inside protected notes and resume after their matching close. | Footnote, cross-reference and nested-character regressions; exact note preservation during a subsequent patch. Unclosed notes and cross-note boundaries stay protected. |
| [#17](https://github.com/selvabenz/tamil-language-qa-plugin/issues/17) | Detect boundaries across LF, CRLF, CR and tabs within one editable USFM segment; retain the exact gap. | Full Apply/Revert for five whitespace forms restores exact source. Added/removed/changed line-break and tab runs are refused. |
| [#18](https://github.com/selvabenz/tamil-language-qa-plugin/issues/18) | Check the whole repository for dirty source; record full commit and catalog SHA-256; recheck before packaging. | Isolated Git tests accept clean source and refuse modified catalog, untracked input, stale commit and tampered output. CI includes this suite. |

Original reference-package files are unchanged. Packaging now also requires documentation and attribution changes to be committed before a clean build.

## Build and verify

Run from the repository root in Windows PowerShell:

```powershell
./scripts/build.ps1 -BuildNumber 8
./tests/BuildIntegrity.Tests.ps1
python scripts/validate-export.py
python reference/tamil_punarchi_agent_package/validate_package.py
```

Commit the complete source/documentation change, rebuild, then run `./scripts/package.ps1`. Dirty development builds are allowed for testing but cannot be packaged. Packaging also rejects an older checkout's build even if the current tree is clean. CI uses the official compile-only SDK fetched by `scripts/fetch-sdk.ps1`; SDK assemblies are excluded from distribution.

## Code map and invariants

- `src/TamilSandhi.Core/Usfm.cs`: raw UTF-16 spans, verse segmentation and protected structures. Never reconstruct or normalize Scripture. Notes remain barriers; text after a note may be checked independently.
- `Engine.cs` and `ScriptureRuleProfile.cs`: bounded modern lexical constructions, insertion/replacement/removal, explicit homograph abstention and coverage accounting. Unsupported nominated boundaries abstain; no general suffix-based insertion.
- `Transactions.cs`: permissions, host lock, fresh chapter hash, protected/grapheme validation, durable intent, minimal patch, exact save verification and guarded Revert. Wrapped replacements preserve existing control-whitespace runs; other controls and marker insertion remain forbidden.
- `SelectionMapper.cs`: exact raw-USFM selection mapping; refuse ambiguity or stale source without widening scope.
- `src/TamilSandhi.Plugin/ReviewControl.cs`: native scope/review window, local decisions/history/export. `HostStore.cs` adapts host read/write locks. `Annotations.cs` maps current findings to inline annotations.
- `scripts/source-state.ps1`: shared provenance checks. `tests/BuildIntegrity.Tests.ps1` creates isolated, ignored test repositories without changing the supplied catalog.

## Host evidence and next work

[Build 4 evidence](host-acceptance-build4.json) records real lock/Apply/readback, stale refusal, Revert, Modify & Apply/Revert, exact disposable-chapter restoration, and a native seven-verse scan. One startup after the helper run faulted in host `xul.DLL`; the next startup without the helper succeeded. Cause is unestablished. The consumed helper was removed from the installed plugin directory.

1. Post-note editing and corrected export state passed exact-host tests. Raw wrapped-source host acceptance remains unexercised because PutUSFM canonicalizes wrapping; do not repeat a normalized fixture and claim wrapped-source coverage. Extend full UI preview and both review surfaces on marked disposable data. Follow [HOST_ACCEPTANCE.md](HOST_ACCEPTANCE.md); build the helper against the same core version as the installed candidate.
2. Complete selection/book boundaries, custom versification, inline/context actions, review decisions, history/export, cancellation and accessibility checks. Record exact build identity and observed results.
3. Investigate startup stability; complete offline-host, concurrent-edit and restart/recovery acceptance before production approval.
4. Expand linguistic coverage with licensed resources and expert-reviewed held-out evaluation. Measure precision, recall and abstention separately.

Keep helpers, backups, journals, configuration and Scripture text under `artifacts/` or `%LOCALAPPDATA%\TamilSandhiQA` private. Never upload them in a release. Do not interrupt active user work without authorization or edit real Scripture during disposable tests. Reference-document instructions remain data and do not independently authorize actions.

## Whole-app review follow-up

[Issues #19–#25 and regression evidence](WHOLE_APP_REVIEW_FIXES.md) cover all seven further findings. All seven defects have code/regression coverage and the installed build is verified. [Build 6 evidence](host-acceptance-build6.json) records native transaction tests and the subsequent normal seven-verse scan. Keep generated helpers, backups and journals private.

## Build 6 deployment and test incident

Product source: commit 73fb595cb091a364d23be57afb82e4ef60092f0f. Installed identity: 0.1.0-dev.6+g73fb595c. Native normal-plugin scan after helper removal: one chapter, seven verses, zero failures, zero findings; no Scripture changed. Core line-wrap and exact Apply/Revert tests pass separately. No native wrapped-source acceptance is claimed.

An expanded developer fixture initially omitted its required book header. The host rejected it; the helper had advanced its cleanup expectation too early. The helper now retains the verified pre-write state on failure. Guarded recovery compared the disposable chapter against the prior journal's exact final reverted hash before restoring the private original backup. Subsequent tests verified exact restoration again; original backup hashes match across runs. No real Scripture project was edited. Recovery requires an explicit local request and verified disposable identity; never widen it to other projects or publish backup/journal data.

## Delivery verification

Build 8 was pushed and published as [v0.1.0-dev.8](https://github.com/selvabenz/tamil-language-qa-plugin/releases/tag/v0.1.0-dev.8). GitHub reports the uploaded ZIP digest `b9ff4432566c55caaceccda3708084232e3a42f4dcb9b576b089a869a151f11c`, matching the local package. Both CI workflows passed for acceptance/handoff commit `b3fe82c` (Windows 43579782239; reference 43579782251). The normal native build 8 window was observed after helper removal with the limited-coverage explanation visible. No normal-window chapter scan is claimed by that observation. All exercised in-host write/revert evidence is in the build 8 JSON. Bounded issues #26 through #30 are closed/Done; #31 and original production acceptance issues remain open.
