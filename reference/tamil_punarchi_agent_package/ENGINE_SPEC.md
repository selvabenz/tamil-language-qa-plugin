# Tamil Sandhi Quality Assurance — Normative Engineering Specification

## 1. Purpose and limits

Tamil புணர்ச்சி (Sandhi) covers how meaningful linguistic units combine, including natural joining (இயல்பு) and altered joining (விகாரம்: தோன்றல், திரிதல், கெடுதல்). It cannot be checked with character bigrams alone. A sound-based transformation is not automatically licensed across an independently written word boundary. Read the grammatical relationship first.

This project serves:
- **General written Tamil** spelling/compound consistency.
- **Modern Tamil Bible IRV** read-only proofreading and editorial flags.
- **Classical Tamil** analysis/lexical validation, as an opt-in module rather than a default rewrite engine.

The first release is **diagnostic-only**; generating a suggestion is not applying a change.

## 2. The three separate rule layers

1. `classical`: தொல்காப்பியம் / நன்னூல் traditional descriptions and attested variants. Many are lexically conditioned and not the default for contemporary prose.
2. `modern`: contemporary spelling and contextual வல்லினம் மிகும் / மிகா norms. The main proofreading layer.
3. `house`: project-specific, versioned overrides. Example: IRV does **not** insert `த்` before configured தேவன்-family terms in specified contexts, even where a generic rule might propose it. The configured preferred divine-name spelling is `யெகோவா`. House rules **must not** be misrepresented as universal Tamil grammar.

The catalog also uses `classical_and_modern` for shared principles and `engineering` for protective guarantees.

## 3. Coverage inventory

### 3.1 Foundational distinctions

- நிலைமொழி and வருமொழி; compound vs free sentence phrase.
- இயல்புப் புணர்ச்சி / விகாரப் புணர்ச்சி.
- தோன்றல், திரிதல், கெடுதல்.
- வேற்றுமைப் புணர்ச்சி vs அல்வழிப் புணர்ச்சி.
- அகச்சந்தி (within lexical/inflectional word) vs புறச்சந்தி (across words).
- வேற்றுமை விரி, வேற்றுமைத்தொகை, உருபும் பயனும் உடன்தொக்க தொகை.
- தொகைநிலைத் தொடர் vs தொகாநிலைத் தொடர்.

### 3.2 Vowel and consonant sound changes

- உடம்படுமெய்: இ, ஈ, ஐ → ய்; other vowels commonly → வ்; ஏ may take ய் or வ். These apply to genuinely joined structures, not every adjacent vowel across prose words.
- குற்றியலுகரம்: six classifications (நெடில், ஆய்த, உயிர், வன், மென், இடை); elision with vowel-initial adjoining elements in licensed environments, with paradigm-specific alternations.
- முற்றியலுகரம்: not interchangeable with குற்றியலுகரம், and not simply “all two-letter words”.
- குற்றியலிகரம்: distinguish the phonological category, do not unconditionally replace syllables.
- மெய்யீறு and attached vowel, consonant doubling after standalone short-vowel stems.
- மகர (ம்) loss, doubling, nasal assimilation, retention, and accepted variants; e.g., `மரம் வளர்ந்தது` must remain unchanged.
- ல்/ள், ன்/ண், ய்/ர்/ழ் and lexically specified வ்-ending compounds; preserve non-compound phrases.

### 3.3 Case and compound classes

Do **not** collapse these into a single “before க/ச/த/ப” rule:

| Construction | Typical action | Grounded illustration |
|---|---|---|
| Overt accusative (ஐ) | Add matching hard-consonant marker | கதையைப் படித்தான் |
| Elliptical accusative (ஐ absent) | Do not add merely for the case relation | நகை செய்தான் |
| Accusative case+implicit purpose compound | Add in established compound | நகைக்கடை |
| Material/instrument compound | Add | இரும்புப் பெட்டி |
| Overt dative (கு/க்கு) | Add | எனக்குக் கொடு |
| Elliptical dative human noun | Do not add | பொன்னி கணவன் |
| Elliptical dative nonhuman compound | Contextual addition | படைத்தளபதி |
| Ablative with இருந்து/நின்று | No addition | வீட்டிலிருந்து சென்றான் |
| Possessive explicit உடைய | No addition | என்னுடைய புத்தகம் |
| Possessive compound, nonhuman | Contextual addition | புலிக்குட்டி |
| Locative compound | Contextual addition | மலைப்பாம்பு |

**Accusative ambiguity:** `அறிக்கை` and `கொலை` end with a lexical ஐ sound; `அறிக்கையை` and `கொலையை` contain an overt case marker. Parsing the written suffix **alone** is inadequate when lemmas are unknown.

### 3.4 Vallinam: additions

- Demonstrative/interrogative adjectives: அந்த, இந்த, எந்த (contrast stand-alone pronouns அது, இது, எது).
- Selected demonstrative/location/manner adverbs: அங்கு, இங்கு, எங்கு, அப்படி, இப்படி, எப்படி.
- Explicit second- and fourth-case suffixes (subject to genuine grammatical parsing).
- Case+purpose/possession/location compounds where rule licenses it.
- A-final infinitive constructions, I-final and Y-final verbal participles, **vanthodar** short-u verbal participles, ஆக/ஆய்/என adverbials and relevant qualitative adverbs.
- Simile, metaphor, quality compounds, specific+generic noun compounds.
- Clipped negative relative participles: ஓடாக் குதிரை, காணாப் பொருள் (unclipped ஓடாத குதிரை, காணாத பொருள் are different).
- Appropriate one-letter stems and many lexical compounds, but not every isolated appearance of those words.

### 3.5 Vallinam: protections

- Standalone pronouns and quantifiers; ordinary cardinal numbers and ஒரு / இரு / அறு / எழு.
- Subject + predicate (எழுவாய்த் தொடர்), vocative (விளித்தொடர்), independent command / optative constructions.
- Positive and normal negative relative participles; descriptive adjectives.
- Menthodar (`கண்டு`, `வந்து`) and idaithodar (`செய்து`) verbal participles vs vanthodar (`படித்து`) participles.
- வினைத்தொகை, உம்மைத்தொகை, second-case elliptical constructions.
- Explicit possessives and ablatives; question particles ஆ/ஓ; பல/சில; selected learned loan compounds.
- Stem+plural கள் and stem+verbal-noun தல் **may differ** from ordinary sandhi and must not be given false doubled consonants.
- Proper names, names of deity, citations, quotations, USFM structural boundaries, and explicitly recorded editorial overrides.

### 3.6 Lexical special classes

- பண்புப்பெயர் from மை: ஈறுபோதல், ஆதிநீடல், இடைஉகரம் இய்யாதல், அடியகரம் ஐயாதல், தன்னொற்று இரட்டல், முன்னின்ற மெய் திரிதல், இனமிகல், and variation. These are **lexicon-driven**, not general suffix rewrites.
- Directions: வடக்கு, தெற்கு, கிழக்கு, மேற்கு and alternatives.
- Numbers: ஒன்று through பத்து, including tens, duplication, numeric combinations, and irregular declensions.
- Tree names: மா, பலா, தெங்கு (and other approved lexemes).
- Other special paradigms: அகம், pronouns நும்/தம்/எம்/நம், சில/பல repetition, proper meaning-based exception of மா.
- சாரியைகள்: அன், ஆன், இன், அல், அற்று, இற்று, அத்து, அம், தம், நம், நும், ஏ, அ, உ, ஐ, கு, ன். Identify linker category and the compatible morphological context rather than inserting any linker mechanically.

## 4. Rule record schema

`rules.json` top-level: `schema_version`, `catalog_version`, `language`, `sources`, `rule_count`, `rules[]`.

Every `rules[]` object has:

```
{
  "id": "CASE-002-OVERT",
  "category": "case",
  "name_ta": "இரண்டாம் வேற்றுமை விரி",
  "name_en": "Explicit accusative ஐ",
  "decision": "INSERT_HARD",
  "preconditions": "...grammatical condition...",
  "requires_annotations": ["case_marker", "lexical_ai_flag"],
  "examples_correct": ["கதையைப் படித்தான்"],
  "examples_incorrect": ["கதையை படித்தான்"],
  "acceptable_alternatives": [],
  "layer": "modern",
  "tier": "candidate",
  "source_ids": ["TVA_ADD"],
  "notes": "..."
}
```

Decisions: `CLASSIFY`, `INSERT_HARD`, `NO_INSERT`, `ALLOW_VARIANTS`, `TRANSFORM_COMPOUND`, `LEXICON_REVIEW`, `REVIEW`, `PROTECT`.

Tiers: `block` (must prevent a wrong suggestion), `candidate` (can surface a suggestion if grammar supports it), `review` (human/lexicon-only). **No decision implies an automatic edit.**

## 5. Processing pipeline

```
Input UTF-8 Scripture/other Tamil text (immutable)
   |
   v
USFM-aware parser / plain text parser
   |
   v
Tamil Unicode grapheme segmenter + offset mapping
   |
   v
Word/compound segmentation + sentence/verse context
   |
   v
Lexicon + morphological analyser + syntactic relation classifier
   |
   v
Candidate generator (vowel, short-u, consonant, case, vallinam)
   |
   v
Protected-context checks + house-style overrides
   |
   v
Rule conflict and uncertainty resolver
   |
   v
Evidence-backed findings with "காரணம்:" and raw byte offsets
   |
   v
Human decision / audit trail / explicit patch application
```

1. **Parse** whole USFM into immutable raw tokens and typed text segments. Chapter/verse markers, paragraph headings, footnotes, cross-references, milestones, `\\w` attributes, quotes, and punctuation are not accidental word boundaries.
2. **Normalize for comparison only** (prefer NFC) while preserving byte-exact original and a mapping from canonical grapheme positions to original byte offsets. Tamil orthographic clusters may contain multiple Unicode code points. NEVER index corrections with Python string indices as if they were byte offsets.
3. **Lexical layer** returns lemma, morphological stem, segmentation, source-of-final-vowel (lexical vs suffix), case, participle subtype, animate/human class, named-entity status, and approved lexical compounds. Represent uncertainty honestly.
4. **Syntactic layer** returns constituent boundaries: subject/predicate, noun+verb, adjective+noun, participle+verb, relative participle+noun, overt case, elliptical case, named compound, vocative, ummai compound, etc.
5. **Candidate stage** proposes a boundary diagnosis **only** when `requires_annotations` are satisfied. For missing analysis, emit `needs_review` without claiming incorrectness.
6. **Protection stage** tests `NO_INSERT` and `PROTECT` before proposing insertions. Project-specific overrides are scoped by project, lexeme, and syntax—not broadly applied to unrelated words.
7. **Resolution** detects alternatives and competing rules; if two incompatible plausible parses exist, mark as ambiguous and request human review. Do not pick based on arbitrary precedence or a numeric confidence alone.
8. **Output** matches `FINDING_SCHEMA.json`, including `காரணம்:` text, offsets, rule ID, parsed evidence, source, confidence, and safe suggested patch.

## 6. Unicode and formatting requirements

- Read/write UTF-8 without lossy transliteration.
- Support Tamil உயிர், மெய், உயிர்மெய், ஆய்தம், vowel signs and pulli; treat Tamil logical grapheme clusters, not JavaScript UTF-16 code units, as user-visible characters.
- Keep original normalization form, zero-width marks and unusual signs visible to QA rather than silently destroying or stripping them.
- Preserve all whitespace, tabs, newlines, punctuation, quoted phrases, USFM markers, `\\f` footnotes, `\\x` cross-references, and `\\w` words with attributes; do not normalize output formatting.
- A scan MUST be byte-preserving. Even in a suggested patch, the replacement must be constrained to a known original text-span; never allow spans crossing USFM structural boundaries.
- Scope analysis to one syntactic context at a time; verse boundaries are normally a hard stop (except where USFM explicitly represents a continuation, which must be inspected by the parser).
- Ensure repeated scanning is deterministic and diagnosis IDs stable across unchanged texts.

## 7. Project-specific overrides

Read a separate, versioned project configuration (`project_house_style.example.json`), not embedded Tamil-wide rules. Defaults for non-Bible prose must NOT include custom divine-name exceptions.

Possible settings:

- `preferred_divine_name`: `யெகோவா` when applicable.
- `no_insert_before_lexemes`: configured தேவன் family, with `த்` as the banned inserted boundary marker, including `அவனை தேவசாயலாக`.
- `scripture_edit_policy`: `human_approval_required`.
- `lexical_whitelist`: accepted compound spellings; no automatic force joining.
- `review_policy`: flag-only and explanations in simple, natural Tamil.

## 8. Explanation format for each finding

Output should contain a short explanation in Tamil, beginning `காரணம்:` and including:

1. The relevant word identities, and whether any final ஐ is lexical or an explicit accusative ஐ.
2. The syntactic relation (எழுவாய் + பயனிலை, பெயர் + வினை, வினையெச்சம் + வினை, explicit case, compound, etc.).
3. Whether and why an additional க்/ச்/த்/ப் is licensed or forbidden, or why a morphological transformation is possible.
4. Any applicable lexicon evidence, alternate accepted form, or active house-style exception.
5. Final decision: `மாற்றம் தேவையில்லை`, `திருத்தம் பரிந்துரை`, or `மனித ஆய்வு தேவை`.

Example unchanged:

> காரணம்: ‘அறிக்கை’ என்ற சொல்லின் இறுதி ‘ஐ’ சொல்லின் இயல்பான எழுத்து; அது இரண்டாம் வேற்றுமை உருபு அல்ல. இது பெயர் + வினை அமைப்பு. ஆகவே ‘ச்’ சேர்க்க வேண்டியதில்லை. முடிவு: மாற்றம் தேவையில்லை.

Example suggestion:

> காரணம்: ‘அறிக்கையை’ என்ற சொல்லில் ‘ஐ’ இரண்டாம் வேற்றுமை உருபாக வந்துள்ளது. அடுத்த சொல் ‘சமர்ப்பித்தான்’ சகரத்தில் தொடங்குகிறது. எனவே ‘ச்’ சேர்த்து ‘அறிக்கையைச் சமர்ப்பித்தான்’ என்று எழுதுவது பொருத்தம். முடிவு: திருத்தம் பரிந்துரை; மனித ஒப்புதல் தேவை.

## 9. Confidence and human decision model

- `protected`: grammar/house context positively forbids a proposed insertion.
- `verified_suggestion`: morphological and syntactic evidence supports the suggestion, with no active conflicting protection; **human approval still required**.
- `needs_review`: morphology uncertain, lexical sense ambiguous, competing rules, classical variant, or insufficient source evidence.
- `accepted`, `rejected`, `open`: human reviewer decisions. Store reviewer identity only if genuinely supplied by the app, not fabricated.

Recommended priority:

1. Protected structure and human-approved authoritative text.
2. Explicit scoped house policy (for the configured project only).
3. Correct grammatical parse (including explicit vs elided case and compound/free-phrase distinction).
4. Attested lexical alternatives.
5. Fallback unknown => open, not correction.

## 10. No-change and change-suggestion examples

### Unchanged

`அறிக்கை செய்தான்`; `கொலை செய்தான்`; `கர்த்தர் சொன்னார்`; `யெகோவா காப்பார்`; `வந்து சென்றான்`; `கண்டு பேசினார்`; `செய்து தந்தான்`; `வந்த பையன்`; `ஓடாத குதிரை`; `அவனை தேவசாயலாக` (when house policy enabled).

### Suggest with explicit grammatical justification

- `அறிக்கையை சமர்ப்பித்தான்` → `அறிக்கையைச் சமர்ப்பித்தான்`
- `அவரை பார்த்தான்` → `அவரைப் பார்த்தான்`
- `கர்த்தருக்கு சொன்னான்` → `கர்த்தருக்குச் சொன்னான்`
- `படித்து கொடுத்தான்` → `படித்துக் கொடுத்தான்`
- `ஓடா குதிரை` → `ஓடாக் குதிரை`

### Ambiguous / lexical review

`குளக்கரை` and `குளங்கரை` can both be attested variants; `மா` as a tree noun and `மா` as an intensifying particle behave differently; case-like endings in a proper name may not be case markers. Do not resolve from suffix regex alone.

## 11. API guidance

`analyze(document, projectConfig, annotations?) -> { documentHash, findings[], summary }` (read-only)

`reviewFinding(findingId, reviewerDecision, rationale) -> reviewRecord` (does not apply edit)

`applyApprovedPatches(document, reviewRecords) -> { newDocument, patchLog }` (separate explicit human-authorized operation; validate source hash and span; no structural edits).

Each finding should contain: `rule_ids[]`, context, exact source text, target span, candidate, grammatical parse, `reason_ta`, confidence/status, source URLs and an audit-safe patch object. `FINDING_SCHEMA.json` gives the shape.

## 12. Validation and release gates

- Validate all 183 `rules.json` entries: unique IDs, valid decisions/tier/layers, valid source references, nonempty preconditions.
- Load and execute or fixture-validate 99 `tests.jsonl` scenarios. Test A: protected; B: conditional suggestion; C: lexical compound; D: raw text/USFM roundtrip.
- Code must distinguish lexical ஐ from overt accusative ஐ and first classify the syntactic structure.
- Never suggest adding க்/ச்/த்/ப் after subject+predicate, ordinary positive/negative relative participles, and specified non-doubling participles.
- Unknown grammatical tags => remain open. Do not silently treat parser absence as evidence that the grammatical rule matched.
- Confirm no byte mutation on analysis of original SFM/USFM (hash before/after).
- Test words with Tamil combining signs and user-provided house lexemes.
- Demonstrate rejection and re-approval cycles, conflicting version hashes, repeated scans, and reversible patch application.
- Report coverage by `category`, tier and source rather than declaring every rule production-implemented.

## 13. Deferred research areas

The inventory identifies but does not fully formalize every rare classical alternation, especially all derivational பண்புப்பெயர் forms, rare வகர ஈறு constructions, precise குற்றியலிகரம் triggers, archaic inflectional forms, and semantics-dependent compound spellings. Production implementation requires a reliable Tamil morphological dictionary, attested forms, and human-verified counterexamples before auto-suggesting these. Do not invent unattested transformations.
