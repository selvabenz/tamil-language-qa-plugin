# Development implementation status

The first implementation is a development preview, not a production release or a demonstration of 100% linguistic accuracy.

## Implemented

- C# .NET Framework 4.8 core and native WinForms control embedded through Paratext's window plugin interface.
- Host menus, scope controls, verse-linked findings, Tamil explanations, context preview, status filters, and visible informational build version.
- Current verse, selected passage, chapter, book, selected books and whole-project scans. Multi-verse/chapter selections require exact raw-USFM mapping; unmatched or changed selection text is refused without widening the scope.
- Immutable scans, conservative USFM segments, UTF-8/grapheme boundary checks, stable findings and explicit abstention for nominated unresolved boundaries.
- Current source: 11 bounded modern addition families and 5 non-addition checks, with insertion/replacement/removal, homograph abstention and explicit register/tier gates. All 183 imported entries have an explicit [runtime capability](RULE_CAPABILITIES.md). The enumerated local lexical contexts are not a full syntax/meaning parser.
- Shared Apply / Modify & Apply service with source-hash validation under the host lock, span/permission checks, durable intent and save verification.
- Reject, scan-only Ignore, project-persistent occurrence Ignore, Leave open, saved-decision reset, and local review audit records.
- Guarded Revert using a full post-edit chapter hash; durable pending/uncertain transaction reconciliation before subsequent writes.
- Annotation provider and context commands sharing window findings/services. Ambiguous host verse mapping fails closed.
- Opt-in exact house lexemes, local journals/configuration, offline runtime and no third-party runtime dependencies.
- Pre-analysis verse-unit counts, per-verse progress/cancellation, a local review-history window and structured JSON findings export validated against the original supplied schema. Exported confidence is zero with an explicit uncalibrated-confidence blocker; no probability of correctness has been measured.
- Build/install/package scripts and Windows CI using the official compile-only SDK pinned to 2.0.100.

## Validation evidence so far

- Local build against Paratext 9.5.110.1's installed assemblies succeeds with warnings treated as errors.
- 708 current-source core assertions pass, consuming all 99 supplied scenarios plus 51 synthetic Scripture-style cases covering narrative, speech, poetry barriers, notes, names, ambiguity, wrong/unnecessary consonants and safe Apply/Revert. A 176-verse synthetic core scan took 9 ms in the clean candidate build; this is not an end-to-end host performance guarantee.
- The 24 imported B rows contain 22 unambiguous corrections, one ambiguous correction-needed input (B004), and one already-correct input (B010). All 22 expected corrections are mandatory and produced; B004 abstains and B010 remains unchanged. These are development fixtures, not independent accuracy evidence. The original import is unchanged; maintained expectation corrections are explicit.
- Host-contract tests load both interfaces, enumerate menus and construct/layout the native control against SDK 2.0.100. They are not in-host acceptance tests.
- Administrative installation succeeded after the initial Windows access denial. All four installed files were checksum-verified in `C:\Program Files\Paratext 9\plugins\TamilSandhiQA`; the installed identity is `0.1.0-dev.6+g73fb595c`.
- [Exact-host build 3 transaction tests](host-acceptance-build3.json) pass in Paratext 9.5.110.1: lock/apply/readback, stale refusal, exact Revert, Modify & Apply/Revert, and exact original disposable-chapter restoration. Paratext also displayed the native review window, build identity and scope selector. A current-chapter scope bug was discovered and fixed. [Build 4 host regression tests](host-acceptance-build4.json) pass, including the production window scope. The normal plugin then completed a native chapter scan of seven synthetic verses with zero failures and zero findings. The disposable fixture used for transaction tests separately yields three actionable findings. The consumed helper was removed. Full interactive host acceptance remains pending. Modal dialog automation has been unreliable; a guarded, one-shot disposable-project acceptance runner is documented in [HOST_ACCEPTANCE.md](HOST_ACCEPTANCE.md). No real Scripture edits were performed by this test session.
- GitHub Windows build and reference-validation workflows pass. [Development prerelease v0.1.0-dev.6](https://github.com/selvabenz/tamil-language-qa-plugin/releases/tag/v0.1.0-dev.6) contains the plugin ZIP.

## Review fixes in current source

Issues #16–#18 fix post-note checking, wrapped-boundary checking with exact whitespace-preserving Apply/Revert, and build/package provenance. Core regressions and isolated Git/package tests pass. The fixes are included in installed build 6. Exact-host post-note Apply/Revert and corrected export state pass. The SDK canonicalizes raw wrapping, so native wrapped-source acceptance is explicitly unexercised; core whitespace-preservation tests pass separately. See [developer handoff](DEVELOPER_HANDOFF.md) for reproduction, commands and next acceptance work.

## Remaining release gates

- Verify installation, discovery, navigation, annotation mapping/context menus and save/revert inside the exact host on a disposable project.
- Verify exact selected-passage mapping, verse counts/progress, export and review-history interaction inside the host, including chapter and book boundaries and custom versification.
- Expand morphology/syntax and licensed lexicons, independently review word classifications, and complete remaining suggestion and compound analyses.
- Evaluate precision, recall and abstention on held-out expert-reviewed text. No independent evaluation has occurred.
- Complete accessibility, appearance, long-scope performance, offline-host, concurrent-write and restart/recovery acceptance; establish measured budgets.

Issues stay open until their full acceptance criteria have evidence. A prerelease ZIP does not mean all 14 tasks are finished. The [product plan](PRODUCT_PLAN.md) remains the target behavior.

## Whole-app follow-up

[All seven review fixes and their regression evidence](WHOLE_APP_REVIEW_FIXES.md) are included in build 6. [Exact-host evidence](host-acceptance-build6.json) records successful transaction/post-note checks, exact disposable restoration, helper removal and a normal seven-verse scan. The developer-only fixture/recovery incident and the wrapped-source host limitation are documented in [the handoff](DEVELOPER_HANDOFF.md).

## Scripture-context follow-up

The user-provided BSI OV export has now been compared read-only over all 66 books, 1,189 chapters and 31,102 verse units. The frozen revised profile proposes 264 changes and abstains on 1,596 nominated findings; all source files remain unchanged. Controlled missing-ஒற்று recovery is 13,652/33,589, not independent accuracy. See [full aggregate evaluation and limitations](BSI_REFERENCE_EVALUATION.md). Build 8 contains these reference-driven context/coverage improvements.

The [five linguistic review fixes](SCRIPTURE_CONTEXT_FIXES.md) are tracked in #26–#30. Build 7 candidate `0.1.0-dev.7+g78186d91` was installed; build 6 remains the last published host-tested baseline until new host evidence and publication are recorded. Both candidate CI workflows passed (Windows run 43563472388; reference run 43563472350). The user selected OTCV, TAOVBSI and TaOVM+ for read-only comparison. The host refused resource text access, so zero chapters were evaluated; see [evidence](resource-benchmark-build7.json) and [remaining gate #31](https://github.com/selvabenz/tamil-language-qa-plugin/issues/31). No resource text was committed or published, and no independent accuracy score exists.
