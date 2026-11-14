# Local BSI OV reference evaluation

The user supplied a local 66-book BSI OV USFM reference and authorized its use. Reference text, per-verse findings and file manifests stay in ignored `artifacts/bsi-reference-*`; they are not included in GitHub or the plugin ZIP. The evaluator reads strict UTF-8, splits chapters, preserves verse/USFM boundaries, and verifies every input file is byte-for-byte unchanged.

## Protocol

- Development books selected before review: Genesis, Psalms, Matthew and John (4 books; 249 chapters; 5,944 verse units).
- The remaining 62 books are a separate reference split, to be scanned only after freezing the revised profile. They are not a gold set of naturally occurring errors.
- Baseline proposals against reviewed text are **disagreements for adjudication**, not automatically errors in the Bible or false positives by the checker.
- Controlled missing-ஒற்று recovery removes a written consonant at a source boundary and tests whether the checker reconstructs it. All eligible source boundaries are counted, including unsupported ones. The controlled mutation uses an isolated word pair; its recovery rate is coverage, not full-verse natural-error recall or precision.
- No independent natural-error precision/recall or 100% accuracy claim is made.

Run locally:

```powershell
./scripts/evaluate-reference.ps1 -ReferenceDir '<local BSI OV USFM directory>' -Stage development
# Freeze the profile and commit before inspecting the other split.
./scripts/evaluate-reference.ps1 -ReferenceDir '<local BSI OV USFM directory>' -Stage holdout
```

## Development findings and changes

The initial build 7 profile supported only 46 of 51,415 examined word pairs and reconstructed 43 of 5,856 controlled missing-ஒற்று boundaries. It also nominated a modern addition in a deity-name phrase the reference leaves unchanged. This exposed overly restrictive following-verb lists and editorial uncertainty.

The revised `scripture-modern-2` profile expands explicitly known accusative/dative forms and participles, and does not require an immediately following finite verb where the cited modern rule does not require one. It retains unknown-case suffix abstention. It uses full local verse-segment lookahead to recognize an explicit naming clause (`என்று பேரி…/பெயரி…/நாமமி…`) and leaves the boundary for human review. Deity-name/தேவ/தெய்வ and the supplied editorial-sensitive lexeme are review policies, not universal no-sandhi laws. Correctly matched written consonants without a supported analysis are counted as unassessed rather than filling the review list with no-change items.

The last development run before freezing supported 2,840 pairs and reconstructed 2,828 of 5,856 controlled boundaries, with 22 proposed changes and 239 review-only findings. Inputs remained unchanged. Some proposals concern mismatched written consonants; others concern edition/register choices. An expert-reviewed decision set is still required before labeling all disagreements or measuring precision.

## Limits

The user-supplied BSI export enables local reference comparison; it does not remove Paratext's API restriction on OTCV/TAOVBSI or make the enhanced resource readable. Those original restrictions remain documented in [resource-access evidence](resource-benchmark-build7.json). The export's exact edition/version and independent editorial judgments should be confirmed before production acceptance. Joined compounds, arbitrary morphology, unknown names, wider classical usage and natural-error recall remain outside the supported guarantee.
