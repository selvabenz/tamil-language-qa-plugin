# Sandhi rules and usage review — 2026-11-14

Follow-up: the original findings below describe build 6. See [Scripture-context fixes](SCRIPTURE_CONTEXT_FIXES.md) for the build 7 changes and [issues #26–#30](https://github.com/selvabenz/tamil-language-qa-plugin/issues?q=is%3Aissue+26+27+28+29+30). The original probe evidence is retained as a historical reproduction.

Reviewed repository commit `8edb36ca9449664d16b9f067b6bff826e6cae83d`, engine source and the 183-entry reference catalog. Runtime probes used the existing `0.1.0-dev.6+g73fb595c` core assembly. This is a source and representative usage review, not certification of every catalog derivation or independent Scripture accuracy evaluation. No Scripture, installed plugin, or production rules were changed.

## Findings, in priority order

### SR-01 — High: an ambiguous lexical form is asserted to be accusative

`Engine.cs:12,48–49` treats every occurrence of `அவரை` followed by a listed verb as the accusative of `அவர்`, and supplies `lexical_ai_flag=false` without disambiguation. The runtime suggests `அவரைக் கொடுத்தான்` for `அவரை கொடுத்தான்`.

However, `அவரை` also names a bean crop, as documented by [Tamil Nadu Agricultural University](https://agritech.tnau.ac.in/ta/horticulture/horti_Vegetables_ta.html). When the intended object is that crop with an omitted accusative suffix, the [TVA rule for இரண்டாம் வேற்றுமைத் தொகை](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214662-16318) does not license insertion merely from the lexical ஐ ending. This is an ambiguous input, not proof that the pronoun interpretation is always wrong. The defect is asserting a unique grammatical analysis from the surface word.

Required resolution: recognize lexical homographs and abstain unless context/morphology resolves them. Keep the pronoun reading as a separate positive test, and add the crop reading and unresolved reading as negative/abstention tests. The Tamil explanation must describe limited lexical evidence rather than imply that syntax was established.

### SR-02 — High: wrong and unnecessary ஒற்று are not checked

The engine only constructs insertion patches (`Engine.cs:37–62`). It does not analyze a removable or replaceable final ஒற்று. Reproduced inputs with no finding:

| Input | Expected modern usage in the indicated construction |
|---|---|
| `இந்தக் புத்தகம்` | `இந்தப் புத்தகம்` — consonant must match ப |
| `எனக்குப் கொடு` | `எனக்குக் கொடு` — consonant must match க |
| `வந்தப் பையன்` | `வந்த பையன்` — ordinary relative participle |

The first two follow the [TVA addition rules](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214661-16317); the third follows the [relative-participle non-addition rule](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214662-16318).

Required resolution: separate insertion, replacement and removal analyses, with grammatical evidence and protected-name/house-style checks before offering each patch. Do not implement blanket stripping of final consonants.

### SR-03 — High: implemented usage coverage is much smaller than the catalog

Only four IDs can produce supported suggestions: `HARD-DEM`, `CASE-002-OVERT`, `CASE-004-OVERT`, `HARD-PART-HARDU`. Their built-in word lists are small. Catalog loading does not execute all 183 entries, which also include classifications, protections and engineering policies.

Reproduced limitations:

| Input | Expected correction under the supplied fixture interpretation | Current result |
|---|---|---|
| `மலரை பறித்தான்` | `மலரைப் பறித்தான்` | Review only |
| `கதவை தட்டு` | `கதவைத் தட்டு` | Review only |
| `ஊருக்கு போனான்` | `ஊருக்குப் போனான்` | Review only |
| `கற்று கொடுத்தான்` | `கற்றுக் கொடுத்தான்` | No finding |
| `ஓடி போனான்` | `ஓடிப் போனான்` | No finding |

These constructions are covered by the [TVA addition lesson](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214661-16317). Even the currently supported hard-participle family misses `கற்று` because it is absent from the lexical set. Glide, compound, consonant-change, numeral and சாரியை entries are not implemented correction families. Already joined words are not analyzed internally by this adjacent-word scanner.

Required resolution: publish an explicit capability matrix, distinguish unsupported boundaries from verified-correct ones in scan summaries, and extend coverage using validated morphology/lexical evidence. A clean scan currently means no supported finding was generated; it does not establish correct sandhi throughout the scope.

### SR-04 — Medium: B010 contradicts its own suggestion expectation

`reference/tamil_punarchi_agent_package/tests.jsonl:59` contains input and expected output both equal to `அப்படிச் சொல்`, while `expected_action` is `suggest`. The engine correctly returns no finding for this already-correct input. It should be an idempotence/protection scenario, or its input should be changed to `அப்படி சொல்` in a maintained derived fixture set. Preserve the imported package unchanged and document the correction outside it.

Re-running all 24 B scenarios produced **10 expected suggestions, 3 review-only results and 11 no-finding results**. One of those 11 is the already-correct B010. Consequently there are **23 actual correction-needed inputs: 10 suggested, 3 review-only, 10 missed**. The existing 10/24 figure describes the supplied B bucket, not correction recall or independent accuracy.

`tests/CoreTests.cs:124` checks the correctness of predictions that happen to be emitted, but allows every other B input to be missed. This is documented development coverage, but must not become a linguistic acceptance gate. Add explicit expectations for implemented rules and a separately tracked unsupported set.

### SR-05 — Medium: editorial protection and grammar provenance need separation

`NO-NAMES` is categorized as modern grammar and cites `TVA_CLASS`, but its own note says its Bible examples reflect configured house style. Proper-name status is not, by itself, a universal no-sandhi rule; inflected names still require case analysis. The catalog itself illustrates this distinction with `கர்த்தர் சொன்னார்` and the B024 dative `கர்த்தருக்குச் சொன்னான்`.

The runtime model does not load catalog `preconditions`, `tier`, examples or notes (`Model.cs:74–80`); it uses hardcoded licensing for the four families. `NoInsertBefore` is a global exact-right-word suppression, not a context-specific names policy. Current narrow supported families are modern rules, so lack of a classical-mode gate is principally a future expansion constraint.

Required resolution: label editorial protections as editorial policies with project provenance, define their precedence separately from universal grammar, and enforce rule tier/register before activating more catalog entries. In particular, do not interpret every `NO_INSERT` entry as a universal prohibition across all contexts.

## Representative grammar checks

The central distinctions checked against primary instructional sources are sound:

- Demonstratives, overt accusative/dative forms and hard-series verbal participles can license addition in their stated constructions: `இந்தப் புத்தகம்`, `கதையைப் படித்தான்`, `எனக்குக் கொடு`, `படித்துக் கொடுத்தான்`. See [TVA addition](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214661-16317).
- Omitted accusative, ordinary relative participles and soft/medial participles require different treatment: `நகை செய்தான்`, `வந்த பையன்`, `வந்து சென்றான்`, `செய்து தந்தான்`. See [TVA non-addition](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214662-16318).
- The catalog's basic glide distinction agrees with [TVA உடம்படுமெய்](https://www.tamilvu.org/courses/hg200/hg215/html/hg215ple.htm): இ/ஈ/ஐ take ய்; ஏ admits ய்/வ் alternatives. This does not justify joining arbitrary adjacent prose words.
- The reviewed மை derivation examples, including `வெறுமை + இலை → வெற்றிலை`, have [TVA instructional support](https://www.tamilvu.org/ta/courses-diploma-20052010-bkup-a021-a0211-html-a0211514-49276). They are lexical derivations, not general replacements to apply to arbitrary pairs.

No additional incorrect lexical derivation was established by these representative checks. That is not exhaustive validation: 57 catalog entries have no positive example and only 3 have negative examples. Each executable rule needs independent attestation, counterexamples, ambiguity tests and register/house-style expectations before activation.

## Evidence and disposition

- [Synthetic runtime probes](sandhi-rule-review-probes.json): 15 inputs, no host writes.
- [All B fixture outcomes](sandhi-rule-review-fixtures.json): 24 inputs, no external boundary annotations.
- Existing 403 core assertions largely cover engineering contracts; they do not measure accuracy on independently annotated Scripture.
- SR-01 through SR-05 are review findings requiring follow-up; they are not claimed fixed, installed or released. This review did not create GitHub issues or change board status.

Production acceptance should use an independently annotated, permission-cleared Tamil Scripture corpus, with a separate development and held-out evaluation set. Report insertion/replacement/removal precision and recall, abstention rate, ambiguity handling and rule-family coverage. Until then, 100% linguistic accuracy remains unproven.
