# Complete bilingual rule reference / தமிழ்ப் புணர்ச்சி சரிபார்ப்பு விதிகள்

Contains **183 catalog entries**. Each entry has an ID, grammatical condition, expected decision, sample correct forms where available, evidence sources and implementation tier. This is a conservative rule inventory, *not* a statement that each entry can be reduced to a regex.

**Key:** `candidate` = suggest when parser proves the conditions; `review` = morphological/lexicon/human review; `block` = protection against incorrect insertion. None permit automatic authoritative-text edits.

## 01. Fundamentals / புணர்ச்சி வகைகள்

### `BASE-001` — இயல்புப் புணர்ச்சி / No sound change
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Both words form a genuine grammatical sandhi construction and no sound change is licensed

**Sources:** `TVA_CLASS`

### `BASE-002` — விகாரப் புணர்ச்சி / Sound changes
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Actual combination undergoes addition / transformation / deletion

**Sources:** `TVA_CLASS`

### `BASE-003` — வேற்றுமைப் புணர்ச்சி / Case relation sandhi
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Constituents are connected by a case relation

**Required analysis:** `case_relation`

**Sources:** `TVA_CLASS`

### `BASE-004` — அல்வழிப் புணர்ச்சி / Non-case sandhi
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** No case relation; identify compound or phrase category

**Required analysis:** `syntactic_relation`

**Sources:** `TVA_CLASS`

### `BASE-005` — அகச்சந்தி / Within-word sandhi
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Stem plus inflection/derivational suffix

**Sources:** `TVA_CASE`

### `BASE-006` — புறச்சந்தி / Across-word sandhi
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Between words in syntactic/compound context

**Sources:** `TVA_BOOK`

### `BASE-007` — தோன்றல் / Insertion
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** New phoneme/letter appears

**Sources:** `TVA_CLASS`

### `BASE-008` — திரிதல் / Substitution
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Boundary letter transforms

**Sources:** `TVA_CLASS`

### `BASE-009` — கெடுதல் / Elision
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Boundary letter drops

**Sources:** `TVA_CLASS`

### `BASE-010` — வேற்றுமை விரி / Overt case marker
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Case suffix explicitly present

**Required analysis:** `case_marker`

**Sources:** `TVA_ADD`

### `BASE-011` — வேற்றுமைத்தொகை / Elliptical case marker
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Case relation present, marker omitted

**Required analysis:** `case_relation`

**Sources:** `TVA_NO`

### `BASE-012` — உருபும் பயனும் உடன்தொக்க தொகை / Marker and associated element omitted
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Case marker and semantic intervening element implicit

**Required analysis:** `compound_semantics`

**Sources:** `TVA_ADD`

### `BASE-013` — தொகைநிலைத் தொடர் / Compound
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Nominal/other compound with grammatical contraction

**Required analysis:** `compound_type`

**Sources:** `TVA_CLASS`

### `BASE-014` — தொகாநிலைத் தொடர் / Free phrase
**Decision:** `CLASSIFY` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Independent syntactic phrase

**Required analysis:** `syntactic_relation`

**Sources:** `TVA_NO`

## 02. Vowel-endings / உயிரீற்றுப் புணர்ச்சி

### `GLIDE-001` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel இ; next element begins with vowel; insert ய்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Correct examples:** மணி + ஓசை → மணியோசை

**Sources:** `TVA_GLIDE`

### `GLIDE-002` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஈ; next element begins with vowel; insert ய்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Correct examples:** தீ + எரி → தீயெரி

**Sources:** `TVA_GLIDE`

### `GLIDE-003` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஐ; next element begins with vowel; insert ய்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Correct examples:** வாழை + இலை → வாழையிலை

**Sources:** `TVA_GLIDE`

### `GLIDE-004` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஆ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Correct examples:** நிலா + அழகு → நிலாவழகு

**Sources:** `TVA_GLIDE`

### `GLIDE-005` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஊ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Correct examples:** பூ + இதழ் → பூவிதழ்

**Sources:** `TVA_GLIDE`

### `GLIDE-006` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel உ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Sources:** `TVA_GLIDE`

### `GLIDE-007` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel எ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Sources:** `TVA_GLIDE`

### `GLIDE-008` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஒ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Sources:** `TVA_GLIDE`

### `GLIDE-009` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஓ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Sources:** `TVA_GLIDE`

### `GLIDE-010` — உடம்படுமெய் / Vowel-to-vowel glide
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Actual compound/suffix boundary; stem ends with vowel ஔ; next element begins with vowel; insert வ்

**Required analysis:** `boundary_is_compound_or_suffix`, `terminal_vowel`, `following_vowel`

**Sources:** `TVA_GLIDE`

### `GLIDE-011` — ஏகார உடம்படுமெய் / E-ending glide alternation
**Decision:** `ALLOW_VARIANTS` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Final ஏ can admit ய் and வ் depending on lexical usage

**Correct examples:** சே + அழகு → சேயழகு; சே + அழகு → சேவழகு

**Note:** Never choose a glide by simple suffix-only regex.

**Sources:** `TVA_GLIDE`

### `GLIDE-012` — சொல் எல்லை / Do not join independent words
**Decision:** `PROTECT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Adjacent independent words are not morphological compound/suffix; vowel adjacency not enough to insert a glide

**Required analysis:** `token_boundary_kind`

**Correct examples:** அவன் அங்கே சென்றான்

**Sources:** `TVA_CLASS`

## 03. குற்றியலுகரம், முற்றியலுகரம் and related rules

### `UCLASS-001` — குற்றியலுகரம் — நெடில்தொடர் / நெடில்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in நெடில்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** பாகு; காது; ஆறு

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `UCLASS-002` — குற்றியலுகரம் — ஆய்தத்தொடர் / ஆய்தத்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in ஆய்தத்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** அஃது; எஃகு

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `UCLASS-003` — குற்றியலுகரம் — உயிர்த்தொடர் / உயிர்த்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in உயிர்த்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** வரகு; முரடு; கிணறு

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `UCLASS-004` — குற்றியலுகரம் — வன்தொடர் / வன்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in வன்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** பாக்கு; பட்டு; பத்து

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `UCLASS-005` — குற்றியலுகரம் — மென்தொடர் / மென்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in மென்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** குரங்கு; பஞ்சு; வண்டு

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `UCLASS-006` — குற்றியலுகரம் — இடைத்தொடர் / இடைத்தொடர் shortened-U classification
**Decision:** `CLASSIFY` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Identify word final shortened உ with preceding sound in இடைத்தொடர்

**Required analysis:** `stem_morphology`, `phonology`

**Correct examples:** மார்பு; சார்பு

**Note:** Examples are words illustrating class; not isolated correction triggers.

**Sources:** `TVA_U`

### `U-007` — குற்றியலுகரம் உயிர்முன் கெடுதல் / Short-u elision before vowels
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Verified final குற்றியலுகரம் followed by vowel-initial suffix or joined constituent

**Required analysis:** `short_u_class`, `boundary_is_compound_or_suffix`

**Correct examples:** வண்டு + இனம் → வண்டினம்

**Sources:** `TVA_U`

### `U-008` — இறுதி எழுத்து மாற்றங்கள் / Some suffixes cause additional alternation
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Suffix-specific stem alternation requires lexicon and morphological analysis

**Required analysis:** `lexical_paradigm`

**Correct examples:** ஆடு + இன் → ஆட்டின்; வயிறு + ஐ → வயிற்றை

**Note:** Do not generalize consonant doubling to every short-u stem.

**Sources:** `TVA_U`

### `U-009` — முற்றியலுகரம் / Full-u stems
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Full உ does not behave identically to shortened உ; phonological and lexical class must be known

**Required analysis:** `stem_morphology`

**Correct examples:** பசு; மது; கதவு; நிலவு

**Note:** Do not automatically elide all full உ endings.

**Sources:** `TVA_U`

### `U-010` — குற்றியலிகரம் / Shortened-i
**Decision:** `REVIEW` · **Layer:** `classical` · **Tier:** `review`

**Condition:** Classical shortening of இ in applicable environments, including certain combinations before ய்

**Required analysis:** `morphological_environment`

**Sources:** `TVA_U`

### `SPECIAL-MUTTRU-ODU` — ஒடு உருபு / Instrumental/comitative ஒடு suffix
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Comitative suffix ஒடு followed by predicate

**Required analysis:** `case_marker`

**Correct examples:** இராமனொடு சென்றான்

**Sources:** `TVA_UKAR`

## 04. General consonant endings / மெய்யீறு

### `C-001` — மெய்யுடன் உயிர் சேர்தல் / Consonant-vowel attachment
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Final pure consonant + vowel initial next element, actual compound/suffix

**Required analysis:** `boundary_is_compound_or_suffix`

**Correct examples:** வேல் + எறிந்தான் → வேலெறிந்தான்

**Sources:** `TVA_BOOK`

### `C-002` — தனிக்குறில் முன் ஒற்று இரட்டல் / Short monosyllable coda gemination
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Single short vowel before final consonant + attached vowel

**Required analysis:** `short_monovocalic_stem`, `boundary_is_compound_or_suffix`

**Correct examples:** கல் + அழகு → கல்லழகு; பொன் + ஆடை → பொன்னாடை

**Sources:** `TVA_BOOK`

## 05. மகர ஈற்றுப் புணர்ச்சி

### `M-001` — மகர ஈறு கெட்டு வல்லினம் மிகுதல் / M-loss + hard consonant increase
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Specific case or adjective/simile compound; ம் final and hard initial

**Required analysis:** `compound_type`

**Correct examples:** மரம் + கிளை → மரக்கிளை; மரம் + கலம் → மரக்கலம்

**Sources:** `TVA_M`

### `M-002` — மகர ஈறு இனமெல்லெழுத்தாதல் / M assimilates to matching nasal
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Applicable case or classical compound before a hard consonant

**Required analysis:** `compound_type`

**Correct examples:** காலம் + கடந்து → காலங்கடந்து; மரம் + கண்டார் → மரங்கண்டார்

**Sources:** `TVA_M`

### `M-003` — மகர ஈறு கெட்டு இடையினம் வருதல் / M-loss with medial onset
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Licensed compound whose ம் drops before medial initial

**Required analysis:** `compound_type`

**Correct examples:** மரம் + வேர் → மரவேர்

**Sources:** `TVA_M`

### `M-004` — மகர ஈறு மாறாமல் நிற்றல் / M-retention
**Decision:** `PROTECT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Independent subject phrase / relative participle; ம் remains

**Required analysis:** `syntactic_relation`

**Correct examples:** மரம் வளர்ந்தது; கொல்லும் யானை

**Sources:** `TVA_M`

### `M-005` — மகர ஈற்று விகற்பம் / M-final accepted alternatives
**Decision:** `ALLOW_VARIANTS` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classical variant formation of compounds with ம்

**Correct examples:** குளம் + கரை → குளக்கரை; குளம் + கரை → குளங்கரை

**Sources:** `TVA_M`

### `M-006` — அகம் சிறப்புப் புணர்ச்சி / Akam special compounds
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Stem அகம் with கை or செவி

**Correct examples:** அகம் + கை → அங்கை; அகம் + செவி → அஞ்செவி

**Sources:** `TVA_M`

### `M-007` — நும் தம் எம் நம் / Pronoun stem alternations
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Special declensional stems and nasals

**Required analysis:** `pronoun_paradigm`

**Correct examples:** நாம் + ஐ → நம்மை; தாம் + ஐ → தம்மை

**Sources:** `TVA_M`

## 06. லகர / ளகர ஈறு

### `LR-001` — லகரம் றகரமாகத் திரிதல் / L → R-retroflex class
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Specific case/special-name compounds, final ல் before hard initial

**Required analysis:** `compound_type`

**Correct examples:** பால் + குடம் → பாற்குடம்; வேல் + படை → வேற்படை

**Sources:** `TVA_L`

### `LR-002` — ளகரம் டகரமாகத் திரிதல் / LL → retroflex T
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Specific compounds final ள் before hard initial

**Required analysis:** `compound_type`

**Correct examples:** வாள் + படை → வாட்படை; முள் + செடி → முட்செடி

**Sources:** `TVA_L`

### `LR-003` — லகர + நகர மாற்றம் / L+N assimilation
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Genuine compound with final ல் plus ந initial

**Required analysis:** `compound_type`

**Correct examples:** நல் + நூல் → நன்னூல்

**Sources:** `TVA_L`

### `LR-004` — லகர ளகர இயல்பு / No universal L/LL conversion
**Decision:** `PROTECT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Ordinary free phrase or coordinative compound may preserve ல்/ள்

**Required analysis:** `syntactic_relation`

**Correct examples:** கால் கை; தோள் வலிமை

**Sources:** `TVA_L`

## 07. னகர / ணகர ஈறு

### `NN-001` — னகர மாற்றம் / N → R in established compounds
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Specific compounds final ன் followed by க initial

**Required analysis:** `lexical_paradigm`

**Correct examples:** பொன் + கலம் → பொற்கலம்

**Sources:** `TVA_BOOK`

### `NN-002` — ணகர மாற்றம் / Retroflex N → retroflex T
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Specific compounds final ண் followed by க initial

**Required analysis:** `lexical_paradigm`

**Correct examples:** மண் + குடம் → மட்குடம்

**Sources:** `TVA_BOOK`

### `NN-003` — மெய் இரட்டல் / Nasals double in licensed vowel boundary
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Short stem final ன்/ண் before vowel attached

**Required analysis:** `boundary_is_compound_or_suffix`

**Correct examples:** பொன் + ஆடை → பொன்னாடை

**Sources:** `TVA_BOOK`

## 08. யகர / ரகர / ழகர ஈறு

### `YRZ-001` — யகர ரகர ழகர ஈறு / Y/R/Z + vallinam
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Hard consonant joining compounds with final ய்/ர்/ழ்

**Required analysis:** `compound_type`

**Correct examples:** மெய் + கீர்த்தி → மெய்க்கீர்த்தி; கார் + பருவம் → கார்ப்பருவம்; யாழ் + கருவி → யாழ்க்கருவி

**Sources:** `TVA_HOUSE`

### `YRZ-002` — யகர ரகர ழகர இயல்பு / Y/R/Z may remain before hard consonants
**Decision:** `PROTECT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Free phrases/other constructions do not universally gain hard consonants

**Required analysis:** `syntactic_relation`

**Correct examples:** வேர் சிறிது; யாழ் பெரிது

**Sources:** `TVA_HOUSE`

## 09. வகர ஈறு

### `V-001` — அவ்/இவ் சொற்களின் புணர்ச்சி / V-final demonstrative alternations
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Lexically specified demonstrative prefix compounds

**Required analysis:** `lexical_paradigm`

**Correct examples:** அவ் + யானை → அவ்வியானை

**Note:** Do not infer a blanket வ்→ற் rule from this example; verify individual lexical derivatives.

**Sources:** `TVA_BOOK`

## 10. Case relations / வேற்றுமை

### `CASE-002-OVERT` — இரண்டாம் வேற்றுமை விரி / Explicit accusative ஐ
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `candidate`

**Condition:** Following word begins க/ச/த/ப and previous noun is explicitly accusative (not lexical ஐ)

**Required analysis:** `case_marker`, `lexical_ai_flag`

**Correct examples:** கதையைப் படித்தான்; மலரைப் பறித்தான்; கதவைத் தட்டு; ஒலியைக் குறை

**Examples needing review/correction:** கதையை படித்தான்; மலரை பறித்தான்

**Sources:** `TVA_ADD`

### `CASE-002-ELLIP` — இரண்டாம் வேற்றுமைத்தொகை / Implied accusative
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Accusative meaning but ஐ unexpressed

**Required analysis:** `case_relation`, `case_marker`

**Correct examples:** நகை செய்தான்; கனி தின்றான்; தமிழ் படித்தான்

**Examples needing review/correction:** நகைச் செய்தான்

**Sources:** `TVA_NO`

### `CASE-002-COMPOUND` — இரண்டாம் உருபும் பயனும் உடன்தொக்க தொகை / Accusative compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Noun compound with implicit object relation and implicit predicate

**Required analysis:** `compound_semantics`

**Correct examples:** நகைக்கடை; எலிப்பொறி; தயிர்க்குடம்; நெய்க்குடம்

**Sources:** `TVA_ADD`

### `CASE-003-COMPOUND` — மூன்றாம் உருபும் பயனும் உடன்தொக்க தொகை / Instrumental/material compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Compound specifying material/instrument

**Required analysis:** `compound_semantics`

**Correct examples:** வெள்ளிக் கிண்ணம்; இரும்புப் பெட்டி; தேங்காய்ச் சட்னி

**Sources:** `TVA_ADD`

### `CASE-004-OVERT` — நான்காம் வேற்றுமை விரி / Explicit dative கு
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `candidate`

**Condition:** Dative suffix கு/க்கு explicit and following க/ச/த/ப

**Required analysis:** `case_marker`

**Correct examples:** எனக்குக் கொடு; ஊருக்குப் போனான்; வீட்டுக்குத் தலைவி

**Examples needing review/correction:** எனக்கு கொடு

**Sources:** `TVA_ADD`

### `CASE-004-ELLIP-HUMAN` — நான்காம் வேற்றுமைத்தொகை உயர்திணை / Implied dative human
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Human-animate noun in omitted dative relation

**Required analysis:** `noun_animacy`, `case_relation`

**Correct examples:** பொன்னி கணவன்

**Sources:** `TVA_NO`

### `CASE-004-ELLIP-NONHUMAN` — நான்காம் வேற்றுமைத்தொகை அஃறிணை / Implied dative nonhuman
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Nonhuman noun in omitted dative compound

**Required analysis:** `noun_animacy`, `compound_semantics`

**Correct examples:** குறிஞ்சித் தலைவன்; படைத்தளபதி; கூலிப்படை

**Sources:** `TVA_ADD`

### `CASE-004-COMPOUND` — நான்காம் உருபும் பயனும் உடன்தொக்க தொகை / Dative compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Dative semantic compound with elided material (most, not all)

**Required analysis:** `compound_semantics`

**Correct examples:** தண்ணீர்க்குடம்

**Note:** Most (பெரும்பாலும்), not unconditional.

**Sources:** `TVA_ADD`

### `CASE-005-OVERT` — ஐந்தாம் வேற்றுமை விரி / Ablative expression
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Explicit இருந்து/நின்று removal expression

**Correct examples:** வீட்டிலிருந்து சென்றான்; கூட்டினின்று பறந்தது

**Sources:** `TVA_NO`

### `CASE-006-ELLIP-HUMAN` — ஆறாம் வேற்றுமைத்தொகை உயர்திணை / Human genitive compound
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Human noun possessive relation

**Required analysis:** `noun_animacy`, `compound_semantics`

**Correct examples:** ஆசிரியர் கழகம்; தம்பி துணி; மகளிர் கல்லூரி

**Sources:** `TVA_NO`

### `CASE-006-ELLIP-NONHUMAN` — ஆறாம் வேற்றுமைத்தொகை அஃறிணை / Nonhuman possessive compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Nonhuman noun possessive compound

**Required analysis:** `noun_animacy`, `compound_semantics`

**Correct examples:** கிளிக்கூண்டு; புலிக்குட்டி

**Sources:** `TVA_ADD`

### `CASE-006-OVERT` — உடைமைக் குறி / Explicit possessive
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Possessive -அது/உடைய in grammatical phrase

**Correct examples:** எனது புத்தகம்; என்னுடைய புத்தகம்

**Sources:** `TVA_NO`

### `CASE-007-COMPOUND` — ஏழாம் உருபும் பயனும் உடன்தொக்க தொகை / Locative compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Locative compound with implicit location word

**Required analysis:** `compound_semantics`

**Correct examples:** மலைப்பாம்பு; சென்னைக் கல்லூரி; மதுரைக்கோயில்

**Sources:** `TVA_ADD`

### `CASE-ALL-SCOPE` — வேற்றுமை விதிகள் விதிவிலக்கு / Case scope
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Six non-nominative/non-vocative cases have different overt, elliptical, and compound forms

**Required analysis:** `case_relation`

**Sources:** `TVA_CASE`, `TVA_ADD`, `TVA_NO`

## 11. வல்லினம் மிகும் இடங்கள்

### `HARD-DEM` — Demonstrative adjective / Demonstrative adjective
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `candidate`

**Condition:** Confirmed Demonstrative adjective grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** அந்தப் பையன்; இந்தக் குழந்தை; எந்தப் பொருள்

**Sources:** `TVA_ADD`

### `HARD-LOCADV` — Locative/interrogative adverb / Locative/interrogative adverb
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Locative/interrogative adverb grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** அங்குக் கண்டான்; எங்குப் போனான்

**Sources:** `TVA_ADD`

### `HARD-MANNER` — Manner adverb / Manner adverb
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `candidate`

**Condition:** Confirmed Manner adverb grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** அப்படிச் சொல்; இப்படிப் பேசு; எப்படிச் சொல்வான்

**Sources:** `TVA_ADD`

### `HARD-PREFIX` — Single-letter demonstrative prefix / Single-letter demonstrative prefix
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Single-letter demonstrative prefix grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** அக்காலம்; எத்திசை

**Sources:** `TVA_ADD`

### `HARD-ONELETTER` — One-letter lexical stems / One-letter lexical stems
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed One-letter lexical stems grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** கைக்குழந்தை; தீப்பெட்டி; பூப்பல்லக்கு; மைப்பேனா

**Sources:** `TVA_ADD`

### `HARD-INFINITIVE` — A-final infinitive / A-final infinitive
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `candidate`

**Condition:** Confirmed A-final infinitive grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** வரச் சொன்னான்; உண்ணப் போனான்; உட்காரப் பார்த்தான்

**Sources:** `TVA_ADD`

### `HARD-PART-I` — I-final verbal participle / I-final verbal participle
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed I-final verbal participle grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** ஓடிப் போனான்; தேடிப் பார்த்தான்; கூறிச் சென்றான்

**Sources:** `TVA_ADD`

### `HARD-PART-Y` — Y-final verbal participle / Y-final verbal participle
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Y-final verbal participle grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** போய்ப் பார்த்தான்

**Sources:** `TVA_ADD`

### `HARD-PART-HARDU` — Vanthodar short-u participle / Vanthodar short-u participle
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Vanthodar short-u participle grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** கற்றுக் கொடுத்தான்; படித்துக் கொடுத்தான்; வைத்துப் போனான்

**Sources:** `TVA_ADD`

### `HARD-PART-AG` — ஆக/ஆய்/என adverbial participles / ஆக/ஆய்/என adverbial participles
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed ஆக/ஆய்/என adverbial participles grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** நன்றாகச் சொன்னான்; வருவதாய்க் கூறினார்; வா எனக் கூறினார்

**Sources:** `TVA_ADD`

### `HARD-PART-QUALITY` — Qualitative adverbs / Qualitative adverbs
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Qualitative adverbs grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** மெல்லப் பேசினார்; வேகமாகக் கூறினார்; நிறையப் பேசுவான்

**Sources:** `TVA_ADD`

### `HARD-NEG-ELLIP` — Clipped negative relative participles / Clipped negative relative participles
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Clipped negative relative participles grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** செல்லாக் காசு; ஓடாக் குதிரை; தீராச் சிக்கல்; காணாப் பொருள்

**Sources:** `TVA_ADD`

### `HARD-QUALITY` — Quality compounds / Quality compounds
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Quality compounds grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** சிவப்புத்துணி; புதுமைப்பெண்; வெள்ளைத்தாள்; மெய்ப்பொருள்

**Sources:** `TVA_ADD`

### `HARD-SPEC-GEN` — Specific+generic noun compound / Specific+generic noun compound
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Specific+generic noun compound grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** மல்லிகைப்பூ

**Sources:** `TVA_ADD`

### `HARD-SIMILE` — Simile/metaphor compounds / Simile/metaphor compounds
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Simile/metaphor compounds grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** தாமரைப்பாதம்

**Sources:** `TVA_ADD`

### `HARD-HARDU-NOUN` — Vanthodar short-u noun compounds / Vanthodar short-u noun compounds
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Vanthodar short-u noun compounds grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** பாக்குத்தோப்பு; எட்டுத்தொகை; பத்துப்பாட்டு

**Sources:** `TVA_ADD`

### `HARD-SOFTU-NOUN` — Selected menthodar short-u noun compounds / Selected menthodar short-u noun compounds
**Decision:** `INSERT_HARD` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Confirmed Selected menthodar short-u noun compounds grammatical category; second constituent begins with க/ச/த/ப

**Required analysis:** `syntactic_relation`, `compound_type`

**Correct examples:** குரங்குக் குட்டி; மருந்துச் சீட்டு; பாம்புத்தோல்

**Sources:** `TVA_ADD`

## 12. வல்லினம் மிகா இடங்கள்

### `NO-PRON` — வல்லினம் மிகா இடம் / Pronouns
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Pronouns; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அது சிறிது; இது பெரியது; எது பெரியது; அவை போயின; இவை சிறந்தவை

**Sources:** `TVA_NO`

### `NO-DEMONSTRATIVE-QUANT` — வல்லினம் மிகா இடம் / Quantifiers
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Quantifiers; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அத்தனை செடிகள்; இத்தனை பொருட்கள்; எத்தனை பழங்கள்; அவ்வளவு பேர்; எவ்வாறு பேசினான்

**Sources:** `TVA_NO`

### `NO-TIME-ADV` — வல்லினம் மிகா இடம் / Time and reporting adverbs
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Time and reporting adverbs; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அன்று பார்த்தான்; இன்று சென்றான்; என்று சொன்னான்

**Sources:** `TVA_NO`

### `NO-NUM` — வல்லினம் மிகா இடம் / Ordinary cardinal numeral
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Ordinary cardinal numeral; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** ஒன்று போதும்; இரண்டு தடவை; மூன்று குழந்தைகள்; நான்கு கால்கள்; ஐந்து சிறுவர்கள்; நூறு பழங்கள்

**Sources:** `TVA_NO`

### `NO-NUM-ADJ` — வல்லினம் மிகா இடம் / ஒரு/இரு/அறு/எழு adjectival numbers
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: ஒரு/இரு/அறு/எழு adjectival numbers; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** ஒரு புத்தகம்; இரு தடவை; அறுபடைவீடு; எழுகடல்

**Sources:** `TVA_NO`

### `NO-SUBJECT` — வல்லினம் மிகா இடம் / Subject-predicate
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Subject-predicate; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** கண்ணகி பேசினாள்; கோழி கூவியது; யெகோவா காப்பார்; கர்த்தர் சொன்னார்

**Sources:** `TVA_NO`

### `NO-VOCATIVE` — வல்லினம் மிகா இடம் / Vocative
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Vocative; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** மகனே கேள்; அண்ணா பாருங்கள்; அரசே பார்

**Sources:** `TVA_NO`

### `NO-IMPERATIVE` — வல்லினம் மிகா இடம் / Imperative/optative boundary
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Imperative/optative boundary; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** வா கண்ணா; படி பாடத்தை; வாழ்க தலைவா

**Sources:** `TVA_NO`

### `NO-REL-POS` — வல்லினம் மிகா இடம் / Positive relative participle
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Positive relative participle; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** வந்த பையன்; கேட்ட கேள்வி; ஓடிய குதிரை

**Sources:** `TVA_NO`

### `NO-ADJECTIVE` — வல்லினம் மிகா இடம் / Qualifying adjective
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Qualifying adjective; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** நல்ல பையன்; பெரிய தெரு; புதிய சிந்தனை

**Sources:** `TVA_NO`

### `NO-REL-NEG` — வல்லினம் மிகா இடம் / Unclipped negative relative participle
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Unclipped negative relative participle; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** செல்லாத காசு; காணாத கண்கள்; ஓடாத குதிரை

**Sources:** `TVA_NO`

### `NO-PART-SOFTU` — வல்லினம் மிகா இடம் / Menthodar verbal participle
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Menthodar verbal participle; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** கண்டு பேசினார்; கொண்டு போனான்; வந்து சென்றான்; தின்று பார்த்தான்

**Sources:** `TVA_NO`

### `NO-PART-MIDU` — வல்லினம் மிகா இடம் / Idaithodar verbal participle
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Idaithodar verbal participle; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** செய்து தந்தான்; கொய்து கொடுத்தாள்

**Sources:** `TVA_NO`

### `NO-PART-QUALU` — வல்லினம் மிகா இடம் / U-final qualitative adverb
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: U-final qualitative adverb; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** நன்கு பேசினான்; நன்று பேசினாய்

**Sources:** `TVA_NO`

### `NO-PART-PADI` — வல்லினம் மிகா இடம் / படி/ஆறு adverbs
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: படி/ஆறு adverbs; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** சொன்னபடி செய்தான்; கண்டவாறு சொன்னான்; வருமாறு கூறினான்

**Sources:** `TVA_NO`

### `NO-VERBCOMP` — வல்லினம் மிகா இடம் / வினைத்தொகை
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: வினைத்தொகை; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** சுடுகாடு; நடுகல்; சுடுசோறு; விடுகதை

**Sources:** `TVA_NO`

### `NO-UMMAI` — வல்லினம் மிகா இடம் / உம்மைத்தொகை
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: உம்மைத்தொகை; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** இரவு பகல்; யானை குதிரை; நன்மை தீமை

**Sources:** `TVA_NO`

### `NO-QUESTION` — வல்லினம் மிகா இடம் / Interrogative ஆ/ஓ
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Interrogative ஆ/ஓ; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அவனா தந்தான்; அவளோ கொடுத்தாள்

**Sources:** `TVA_NO`

### `NO-PALA-SILA` — வல்லினம் மிகா இடம் / பல/சில
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: பல/சில; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** பல பொருள்; சில பூக்கள்

**Sources:** `TVA_NO`

### `NO-GEN` — வல்லினம் மிகா இடம் / Possessive explicit
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Possessive explicit; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** எனது புத்தகம்; என்னுடைய புத்தகம்

**Sources:** `TVA_NO`

### `NO-SANSKRIT` — வல்லினம் மிகா இடம் / Selected learned loan compounds
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Selected learned loan compounds; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** ஆதிபகவன்; தேச பக்தி

**Sources:** `TVA_NO`

### `NO-SUFFIX-PL` — வல்லினம் மிகா இடம் / Plural கள் suffix to certain short-u stems
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Plural கள் suffix to certain short-u stems; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** எழுத்துகள்; வாக்குகள்; வழக்குகள்

**Sources:** `TVA_NO`

### `NO-SUFFIX-NOM` — வல்லினம் மிகா இடம் / Nominalizing தல் suffix
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Nominalizing தல் suffix; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** ஆக்குதல்; வாழ்த்துதல்; கூப்புதல்

**Sources:** `TVA_NO`

### `NO-LEXICAL-AI` — வல்லினம் மிகா இடம் / Lexical ஐ is not accusative
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Lexical ஐ is not accusative; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அறிக்கை செய்தான்; கொலை செய்தான்

**Sources:** `TVA_CLASS`

### `NO-PUNCT` — வல்லினம் மிகா இடம் / Punctuation/quote boundaries
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Punctuation/quote boundaries; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** அவன் சொன்னான்: “பார்.”

**Sources:** `TVA_CLASS`

### `NO-NAMES` — வல்லினம் மிகா இடம் / Proper names and names of deity
**Decision:** `NO_INSERT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Protected context: Proper names and names of deity; must classify grammatical relation, not suffix alone

**Required analysis:** `syntactic_relation`

**Correct examples:** யெகோவா காப்பார்; கர்த்தர் சொன்னார்

**Note:** Bible examples reflect configured house-style testing, not quotations of source text.

**Sources:** `TVA_CLASS`

## 13. Special lexical compounds / சிறப்புப் புணர்ச்சிகள்

### `LEX-QUAL-LOSS` — சிறப்புப் புணர்ச்சி — ஈறுபோதல் / மை drop
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஈறுபோதல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** நன்மை + நூல் → நன்னூல்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-LENGTH` — சிறப்புப் புணர்ச்சி — ஆதிநீடல் / Initial vowel lengthening
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஆதிநீடல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** பசுமை + இலை → பாசிலை

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-VOW` — சிறப்புப் புணர்ச்சி — அடியகரம் ஐயாதல் / Initial a to ai
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for அடியகரம் ஐயாதல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** பசுமை + தமிழ் → பைந்தமிழ்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-DOUBLE` — சிறப்புப் புணர்ச்சி — தன்னொற்று இரட்டல் / Consonant doubling in quality compound
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for தன்னொற்று இரட்டல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** வெறுமை + இலை → வெற்றிலை

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-UY` — சிறப்புப் புணர்ச்சி — இடைஉகரம் இய்யாதல் / Medial உ to இ
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for இடைஉகரம் இய்யாதல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-MEI` — சிறப்புப் புணர்ச்சி — முன்னின்ற மெய் திரிதல் / Previous consonant transformation
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for முன்னின்ற மெய் திரிதல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-NASAL` — சிறப்புப் புணர்ச்சி — இனமிகல் / Nasal increment
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for இனமிகல்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-QUAL-FLEX` — சிறப்புப் புணர்ச்சி — இணையவும்பெறும் / Alternative derivations
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for இணையவும்பெறும்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-DIR-NORTH` — சிறப்புப் புணர்ச்சி — வடக்கு / North direction compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for வடக்கு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** வடக்கு + கிழக்கு → வடகிழக்கு; வடக்கு + நாடு → வடநாடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `LEX-DIR-SOUTH` — சிறப்புப் புணர்ச்சி — தெற்கு / South direction compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for தெற்கு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** தெற்கு + மேற்கு → தென்மேற்கு; தெற்கு + நாடு → தென்னாடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `LEX-DIR-WEST` — சிறப்புப் புணர்ச்சி — மேற்கு / West direction compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for மேற்கு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** மேற்கு + நாடு → மேல்நாடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `LEX-DIR-EAST` — சிறப்புப் புணர்ச்சி — கிழக்கு / East direction compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for கிழக்கு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** கிழக்கு + நாடு → கீழ்நாடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `LEX-DIR-ALT` — சிறப்புப் புணர்ச்சி — திசைப்பெயர் விகற்பம் / Untransformed directional alternatives
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for திசைப்பெயர் விகற்பம்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** வடக்கு + திசை → வடக்குத் திசை; மேற்கு + திசை → மேற்குத் திசை

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `LEX-NUM-1` — சிறப்புப் புணர்ச்சி — ஒன்று / One compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஒன்று; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஒன்று + மாடு → ஒரு மாடு; ஒன்று + ஆடு → ஓராடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM1`

### `LEX-NUM-2` — சிறப்புப் புணர்ச்சி — இரண்டு / Two compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for இரண்டு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** இரண்டு + கால் → இருகால்; இரண்டு + ஆயிரம் → ஈராயிரம்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM2`

### `LEX-NUM-3` — சிறப்புப் புணர்ச்சி — மூன்று / Three compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for மூன்று; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** மூன்று + தமிழ் → முத்தமிழ்; மூன்று + உலகு → மூவுலகு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM2`

### `LEX-NUM-4` — சிறப்புப் புணர்ச்சி — நான்கு / Four compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for நான்கு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** நான்கு + வகை → நால்வகை; நான்கு + படை → நாற்படை; நான்கு + மணி → நான்மணி

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM2`

### `LEX-NUM-5` — சிறப்புப் புணர்ச்சி — ஐந்து / Five compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஐந்து; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஐந்து + பொறி → ஐம்பொறி

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM1`

### `LEX-NUM-6` — சிறப்புப் புணர்ச்சி — ஆறு / Six compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஆறு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஆறு + படைவீடு → அறுபடைவீடு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM1`

### `LEX-NUM-7` — சிறப்புப் புணர்ச்சி — ஏழு / Seven compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஏழு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஏழு + பிறப்பு → எழுபிறப்பு

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM1`

### `LEX-NUM-8` — சிறப்புப் புணர்ச்சி — எட்டு / Eight compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for எட்டு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** எட்டு + குணம் → எண்குணம்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM1`

### `LEX-NUM-9` — சிறப்புப் புணர்ச்சி — ஒன்பது / Nine compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for ஒன்பது; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஒன்பது + ஆயிரம் → ஒன்பதினாயிரம்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM3`

### `LEX-NUM-10` — சிறப்புப் புணர்ச்சி — பத்து / Ten compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for பத்து; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** பத்து + ஒன்று → பதினொன்று; பத்து + இரண்டு → பன்னிரண்டு; பத்து + பத்து → பதிற்றுப்பத்து

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM3`

### `LEX-NUM-TENS` — சிறப்புப் புணர்ச்சி — பத்துகளின் புணர்ச்சி / Tens
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for பத்துகளின் புணர்ச்சி; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** இரண்டு + பத்து → இருபது; மூன்று + பத்து → முப்பது; ஐந்து + பத்து → ஐம்பது

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM3`

### `LEX-NUM-REDUP` — சிறப்புப் புணர்ச்சி — எண்ணுப்பெயர் இரட்டிப்பு / Numeric reduplication
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for எண்ணுப்பெயர் இரட்டிப்பு; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** ஒன்று + ஒன்று → ஒவ்வொன்று

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_NUM3`

### `LEX-TREE-MA` — சிறப்புப் புணர்ச்சி — மரப்பெயர் மா / Mango compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for மரப்பெயர் மா; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** மா + காய் → மாங்காய்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-TREE-PALAA` — சிறப்புப் புணர்ச்சி — மரப்பெயர் பலா / Jackfruit compounds
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for மரப்பெயர் பலா; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** பலா + காய் → பலாக்காய்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_BOOK`

### `LEX-TREE-THENGU` — சிறப்புப் புணர்ச்சி — தெங்கு + காய் / Coconut compound
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Established lexical derivation for தெங்கு + காய்; never generate unknown forms

**Required analysis:** `lemma_id`, `approved_dictionary`

**Correct examples:** தெங்கு + காய் → தேங்காய்

**Note:** Listed as an inventory item, not an open-ended suffix replacement.

**Sources:** `TVA_DIR`

### `SPECIAL-MA-QUALITY` — மா உரிச்சொல் / Maa as intensifier differs from tree noun
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical` · **Tier:** `review`

**Condition:** மா is an uriccol (intensifier), not tree noun; takes distinctive ய் derivation

**Required analysis:** `lexical_sense`

**Correct examples:** மா + இரு → மாயிரு

**Note:** Contrast noun மா + இலை → மாவிலை.

**Sources:** `TVA_PURAN`

### `SPECIAL-MA-NOUN` — மா பெயர்ச்சொல் / Maa noun takes ordinary வ் glide
**Decision:** `LEXICON_REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** மா means mango tree or animal; vowel onset

**Required analysis:** `lexical_sense`

**Correct examples:** மா + இலை → மாவிலை

**Sources:** `TVA_PURAN`

### `SPECIAL-PALA-SILA` — பல/சில இரட்டிப்பு / Several accepted reduplication forms
**Decision:** `ALLOW_VARIANTS` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Reduplicated பல+பல or சில+சில

**Required analysis:** `lexical_paradigm`

**Correct examples:** பலபல; பலப்பல; சிலசில; சிலச்சில

**Sources:** `TVA_AKAR`

## 14. சாரியைகள் and special declensions

### `SARI-AN` — சாரியை — அன் / Connecting morph அன்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அன்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-AAN` — சாரியை — ஆன் / Connecting morph ஆன்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element ஆன்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-IN` — சாரியை — இன் / Connecting morph இன்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element இன்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-AL` — சாரியை — அல் / Connecting morph அல்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அல்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-ATTRU` — சாரியை — அற்று / Connecting morph அற்று
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அற்று; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-ITTRU` — சாரியை — இற்று / Connecting morph இற்று
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element இற்று; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-ATTHU` — சாரியை — அத்து / Connecting morph அத்து
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அத்து; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-AM` — சாரியை — அம் / Connecting morph அம்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அம்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-THAM` — சாரியை — தம் / Connecting morph தம்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element தம்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-NAM` — சாரியை — நம் / Connecting morph நம்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element நம்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-NUM` — சாரியை — நும் / Connecting morph நும்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element நும்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-E` — சாரியை — ஏ / Connecting morph ஏ
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element ஏ; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-A` — சாரியை — அ / Connecting morph அ
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element அ; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-U` — சாரியை — உ / Connecting morph உ
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element உ; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-AI` — சாரியை — ஐ / Connecting morph ஐ
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element ஐ; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-KU` — சாரியை — கு / Connecting morph கு
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element கு; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-N` — சாரியை — ன் / Connecting morph ன்
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Classify linking element ன்; recognize only when morphology supports

**Required analysis:** `stem_morphology`, `suffix`, `grammar_model`

**Sources:** `TVA_SARI`

### `SARI-ATTHU-EX` — அத்துச் சாரியை / Aththu in case inflection
**Decision:** `TRANSFORM_COMPOUND` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Lexeme மரம் with accusative marker ஐ

**Required analysis:** `stem_morphology`, `case_marker`

**Correct examples:** மரம் + ஐ → மரத்தை

**Sources:** `TVA_SARI`

### `SARI-IN-EX` — இன் சாரியை / In increment
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Recognized paradigm insertion before case suffixes

**Required analysis:** `stem_morphology`

**Correct examples:** பூவிற்கு; பூவுக்கு

**Note:** Do not force one of optional accepted forms.

**Sources:** `TVA_SARI`

### `SPECIAL-SARI-PROTECT` — சாரியைகள் இடைச்சொற்கள் / Sariyai is not the same as a lexical final ன்
**Decision:** `PROTECT` · **Layer:** `modern` · **Tier:** `block`

**Condition:** Do not apply lexical final ன் → ற் change to a grammatical சாரியை

**Required analysis:** `morphological_segmentation`

**Correct examples:** ஒன்றன் கூட்டம்; வண்டின்கால்

**Sources:** `TVA_PURAN`

### `SPECIAL-ALLAM` — எல்லாம் எல்லாரும் எல்லீரும் / Special quantifier case forms
**Decision:** `REVIEW` · **Layer:** `classical_and_modern` · **Tier:** `review`

**Condition:** Quantifier/pronominal paradigms use special சாரியை placement

**Required analysis:** `pronoun_paradigm`

**Sources:** `TVA_URUPU2`

### `SPECIAL-PRON-CASE` — தான் தாம் நாம் / Pronoun case inflection
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Pronouns change stems and linkers before case suffixes

**Required analysis:** `pronoun_paradigm`

**Correct examples:** தாம் + ஐ → தம்மை; நாம் + ஐ → நம்மை

**Sources:** `TVA_URUPU2`

### `SPECIAL-AH` — அஃது சுட்டுப்பெயர் / Aytham-containing pronoun case forms
**Decision:** `REVIEW` · **Layer:** `classical` · **Tier:** `review`

**Condition:** Special case inflection of அஃது and related demonstratives

**Required analysis:** `pronoun_paradigm`

**Sources:** `TVA_URUPU2`

### `SPECIAL-IRU` — ஆ மா கோ ஒரெழுத்துப் பெயர்கள் / One-letter stems in case inflection
**Decision:** `REVIEW` · **Layer:** `modern` · **Tier:** `review`

**Condition:** Case endings of special single-letter nouns

**Required analysis:** `stem_morphology`

**Sources:** `TVA_URUPU2`

## 15. Engineering safeguards and project-specific policy

### `SAFE-001` — பாதுகாப்பு விதி / Unicode NFC
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Normalize input for analysis; preserve original bytes for output

**Sources:** `TVA_CLASS`

### `SAFE-002` — பாதுகாப்பு விதி / Not a whitespace-only task
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Never infer grammar from last/first letters alone

**Sources:** `TVA_CLASS`

### `SAFE-003` — பாதுகாப்பு விதி / Preserve Scripture
**Decision:** `PROTECT` · **Layer:** `house` · **Tier:** `block`

**Condition:** No silent changes to USFM Bible text

**Sources:** `TVA_CLASS`

### `SAFE-004` — பாதுகாப்பு விதி / Protect USFM markers
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Treat USFM verse/book/chapter markers, footnotes, xrefs, quotes and punctuation as protected boundaries

**Sources:** `TVA_CLASS`

### `SAFE-005` — பாதுகாப்பு விதி / Lexical ஐ
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Distinguish root-final ஐ from case marker ஐ

**Correct examples:** அறிக்கை செய்தான்; கொலை செய்தான்

**Sources:** `TVA_CLASS`

### `SAFE-006` — பாதுகாப்பு விதி / Project exception
**Decision:** `PROTECT` · **Layer:** `house` · **Tier:** `block`

**Condition:** No added த் before தேவன்-family terms in Bible IRV house style

**Correct examples:** அவனை தேவசாயலாக

**Sources:** `TVA_CLASS`

### `SAFE-007` — பாதுகாப்பு விதி / Proper divine name spelling
**Decision:** `PROTECT` · **Layer:** `house` · **Tier:** `block`

**Condition:** Canonical configured name spelling யெகோவா; not யெஹோவா

**Correct examples:** யெகோவா

**Sources:** `TVA_CLASS`

### `SAFE-008` — பாதுகாப்பு விதி / No forced emendation
**Decision:** `PROTECT` · **Layer:** `house` · **Tier:** `block`

**Condition:** Human must approve edits to authoritative target

**Sources:** `TVA_CLASS`

### `SAFE-009` — பாதுகாப்பு விதி / Rule conflict
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Specific protection > explicit house override > verified grammatical rule > optional lexical rule

**Sources:** `TVA_CLASS`

### `SAFE-010` — பாதுகாப்பு விதி / Open findings
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Unknown syntax or low confidence creates review-only finding

**Sources:** `TVA_CLASS`

### `SAFE-011` — பாதுகாப்பு விதி / No forced word join
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Do not rewrite independent word spacing from classical compounds

**Sources:** `TVA_CLASS`

### `SAFE-012` — பாதுகாப்பு விதி / Morphology tokens
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Do not treat postpositions/suffixes as lexical independent words without analysis

**Sources:** `TVA_CLASS`

### `SAFE-013` — பாதுகாப்பு விதி / Source separation
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** House rules are separate from grammatical facts

**Sources:** `TVA_CLASS`

### `SAFE-014` — பாதுகாப்பு விதி / Idempotence
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Repeated check on unchanged text emits same findings; applying approved fix is idempotent

**Sources:** `TVA_CLASS`

### `SAFE-015` — பாதுகாப்பு விதி / Audit trail
**Decision:** `PROTECT` · **Layer:** `engineering` · **Tier:** `block`

**Condition:** Log context, exact text spans, rule id, evidence, reviewer and reason, not just corrected form

**Sources:** `TVA_CLASS`

