# Development implementation status

The first implementation is a development preview, not a production release or a demonstration of 100% linguistic accuracy.

## Implemented

- C# .NET Framework 4.8 core and native WinForms control embedded through Paratext's window plugin interface.
- Host menus, scope controls, verse-linked findings, Tamil explanations, context preview, status filters, and visible informational build version.
- Current verse, selected passage, chapter, book, selected books and whole-project scans. Multi-verse/chapter selections require exact raw-USFM mapping; unmatched or changed selection text is refused without widening the scope.
- Immutable scans, conservative USFM segments, UTF-8/grapheme boundary checks, stable findings and explicit abstention for nominated unresolved boundaries.
- Four limited suggestion families: demonstrative + known noun, known overt accusative + known verb, known overt dative + known verb, and known vanthodar participle + known verb. The small built-in lexical sets still require linguistic review.
- Shared Apply / Modify & Apply service with source-hash validation under the host lock, span/permission checks, durable intent and save verification.
- Reject, scan-only Ignore, project-persistent occurrence Ignore, Leave open, saved-decision reset, and local review audit records.
- Guarded Revert using a full post-edit chapter hash; durable pending/uncertain transaction reconciliation before subsequent writes.
- Annotation provider and context commands sharing window findings/services. Ambiguous host verse mapping fails closed.
- Opt-in exact house lexemes, local journals/configuration, offline runtime and no third-party runtime dependencies.
- Pre-analysis verse-unit counts, per-verse progress/cancellation, a local review-history window and structured JSON findings export validated against the original supplied schema. Exported confidence is zero with an explicit uncalibrated-confidence blocker; no probability of correctness has been measured.
- Build/install/package scripts and Windows CI using the official compile-only SDK pinned to 2.0.100.

## Validation evidence so far

- Local build against Paratext 9.5.110.1's installed assemblies succeeds with warnings treated as errors.
- 331 core assertions pass, consuming all 99 supplied scenarios for preservation, protected examples, finding contracts and limited prediction coverage. A 176-verse synthetic core scan took 11 ms locally; this is a baseline observation, not an end-to-end host performance guarantee.
- Of 24 conditional suggestion scenarios, 10 receive the expected suggestion without external annotations; 14 are unimplemented or abstained. This is development-fixture coverage, not independent accuracy evidence.
- Host-contract tests load both interfaces, enumerate menus and construct/layout the native control against SDK 2.0.100. They are not in-host acceptance tests.
- Administrative installation succeeded after the initial Windows access denial. All four installed files were checksum-verified in `C:\Program Files\Paratext 9\plugins\TamilSandhiQA`; the installed identity is `0.1.0-dev.3+gc9d89992`.
- [Exact-host build 3 transaction tests](host-acceptance-build3.json) pass in Paratext 9.5.110.1: lock/apply/readback, stale refusal, exact Revert, Modify & Apply/Revert, and exact original disposable-chapter restoration. Paratext also displayed the native review window, build identity and scope selector. A current-chapter scope bug was discovered during interactive checking and is corrected in source; its host retest is pending. Full interactive host acceptance remains pending. Modal dialog automation has been unreliable; a guarded, one-shot disposable-project acceptance runner is documented in [HOST_ACCEPTANCE.md](HOST_ACCEPTANCE.md). No real Scripture edits were performed by this test session.
- GitHub Windows build and reference-validation workflows pass. [Development prerelease v0.1.0-dev.3](https://github.com/selvabenz/tamil-language-qa-plugin/releases/tag/v0.1.0-dev.3) contains the plugin ZIP.

## Remaining release gates

- Verify installation, discovery, navigation, annotation mapping/context menus and save/revert inside the exact host on a disposable project.
- Verify exact selected-passage mapping, verse counts/progress, export and review-history interaction inside the host, including chapter and book boundaries and custom versification.
- Expand morphology/syntax and licensed lexicons, independently review word classifications, and complete remaining suggestion and compound analyses.
- Evaluate precision, recall and abstention on held-out expert-reviewed text. No independent evaluation has occurred.
- Complete accessibility, appearance, long-scope performance, offline-host, concurrent-write and restart/recovery acceptance; establish measured budgets.

Issues stay open until their full acceptance criteria have evidence. A prerelease ZIP does not mean all 14 tasks are finished. The [product plan](PRODUCT_PLAN.md) remains the target behavior.
