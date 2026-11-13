import json, pathlib
from collections import Counter
p=pathlib.Path('/mnt/data/tamil_punarchi_agent_package')
sources={
'TVA_CLASS':'https://www.tamilvu.org/courses/degree/c021/c0213/html/c0213112.htm',
'TVA_GLIDE':'https://www.tamilvu.org/courses/hg200/hg215/html/hg215ple.htm',
'TVA_U':'https://www.tamilvu.org/ta/courses-degree-c021-c0211-html-c0211405-15613',
'TVA_M':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214221-16222',
'TVA_L':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214331-16247',
'TVA_ADD':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214661-16317',
'TVA_NO':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214662-16318',
'TVA_CASE':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214441-16269',
'TVA_SARI':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214442-16270',
'TVA_NUM1':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213661-16132',
'TVA_NUM2':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213662-16133',
'TVA_NUM3':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213663-16134',
'TVA_DIR':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213557-16105',
'TVA_YEAR':'https://www.tamilvu.org/en/node/8057',
'TVA_HOUSE':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c02142nd-16245',
'TVA_BOOK':'https://www.tamilvu.org/courses/hg300/hg314/html/hg314ple.htm',
'TVA_PURAN':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214333-16249',
'TVA_AKAR':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213441-16065',
'TVA_UKAR':'https://www.tamilvu.org/ta/courses-degree-c021-c0213-html-c0213444-16068',
'TVA_URUPU2':'https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c02145nd-16315',
}
rules=[]
def add(id,category,tamil,title,decision,condition,correct=None,incorrect=None,needs=None,source='TVA_ADD',layer='modern',tier='review',note='',alternatives=None):
    if not isinstance(source,list):source=[source]
    if correct is None: correct=[]
    if isinstance(correct,str):correct=[correct]
    if incorrect is None:incorrect=[]
    if isinstance(incorrect,str):incorrect=[incorrect]
    if needs is None:needs=[]
    rules.append(dict(id=id,category=category,name_ta=tamil,name_en=title,decision=decision,
         preconditions=condition,requires_annotations=needs,examples_correct=correct,
         examples_incorrect=incorrect,acceptable_alternatives=alternatives or [],
         layer=layer,tier=tier,source_ids=source,notes=note))
# Definitions and broad grammar, not independent auto edits
add('BASE-001','classification','இயல்புப் புணர்ச்சி','No sound change','CLASSIFY','Both words form a genuine grammatical sandhi construction and no sound change is licensed',source='TVA_CLASS')
add('BASE-002','classification','விகாரப் புணர்ச்சி','Sound changes','CLASSIFY','Actual combination undergoes addition / transformation / deletion',source='TVA_CLASS')
add('BASE-003','classification','வேற்றுமைப் புணர்ச்சி','Case relation sandhi','CLASSIFY','Constituents are connected by a case relation',needs=['case_relation'],source='TVA_CLASS')
add('BASE-004','classification','அல்வழிப் புணர்ச்சி','Non-case sandhi','CLASSIFY','No case relation; identify compound or phrase category',needs=['syntactic_relation'],source='TVA_CLASS')
add('BASE-005','classification','அகச்சந்தி','Within-word sandhi','CLASSIFY','Stem plus inflection/derivational suffix',source='TVA_CASE')
add('BASE-006','classification','புறச்சந்தி','Across-word sandhi','CLASSIFY','Between words in syntactic/compound context',source='TVA_BOOK')
add('BASE-007','classification','தோன்றல்','Insertion','CLASSIFY','New phoneme/letter appears',source='TVA_CLASS')
add('BASE-008','classification','திரிதல்','Substitution','CLASSIFY','Boundary letter transforms',source='TVA_CLASS')
add('BASE-009','classification','கெடுதல்','Elision','CLASSIFY','Boundary letter drops',source='TVA_CLASS')
add('BASE-010','classification','வேற்றுமை விரி','Overt case marker','CLASSIFY','Case suffix explicitly present',source='TVA_ADD',needs=['case_marker'])
add('BASE-011','classification','வேற்றுமைத்தொகை','Elliptical case marker','CLASSIFY','Case relation present, marker omitted',source='TVA_NO',needs=['case_relation'])
add('BASE-012','classification','உருபும் பயனும் உடன்தொக்க தொகை','Marker and associated element omitted','CLASSIFY','Case marker and semantic intervening element implicit',source='TVA_ADD',needs=['compound_semantics'])
add('BASE-013','classification','தொகைநிலைத் தொடர்','Compound','CLASSIFY','Nominal/other compound with grammatical contraction',source='TVA_CLASS',needs=['compound_type'])
add('BASE-014','classification','தொகாநிலைத் தொடர்','Free phrase','CLASSIFY','Independent syntactic phrase',source='TVA_NO',needs=['syntactic_relation'])
# glide
for id,ending,glide,ex in [('GLIDE-001','இ','ய்','மணி + ஓசை → மணியோசை'),('GLIDE-002','ஈ','ய்','தீ + எரி → தீயெரி'),('GLIDE-003','ஐ','ய்','வாழை + இலை → வாழையிலை'),('GLIDE-004','ஆ','வ்','நிலா + அழகு → நிலாவழகு'),('GLIDE-005','ஊ','வ்','பூ + இதழ் → பூவிதழ்'),('GLIDE-006','உ','வ்',None),('GLIDE-007','எ','வ்',None),('GLIDE-008','ஒ','வ்',None),('GLIDE-009','ஓ','வ்',None),('GLIDE-010','ஔ','வ்',None)]:
 add(id,'vowel','உடம்படுமெய்','Vowel-to-vowel glide','TRANSFORM_COMPOUND',f'Actual compound/suffix boundary; stem ends with vowel {ending}; next element begins with vowel; insert {glide}',[ex] if ex else None,needs=['boundary_is_compound_or_suffix','terminal_vowel','following_vowel'],source='TVA_GLIDE',layer='classical_and_modern',tier='review')
add('GLIDE-011','vowel','ஏகார உடம்படுமெய்','E-ending glide alternation','ALLOW_VARIANTS','Final ஏ can admit ய் and வ் depending on lexical usage',['சே + அழகு → சேயழகு','சே + அழகு → சேவழகு'],source='TVA_GLIDE',layer='classical_and_modern',note='Never choose a glide by simple suffix-only regex.')
add('GLIDE-012','vowel','சொல் எல்லை','Do not join independent words','PROTECT','Adjacent independent words are not morphological compound/suffix; vowel adjacency not enough to insert a glide',['அவன் அங்கே சென்றான்'],needs=['token_boundary_kind'],source='TVA_CLASS',tier='block')
# short u class 6
for i,(name,ex) in enumerate([('நெடில்தொடர்',['பாகு','காது','ஆறு']),('ஆய்தத்தொடர்',['அஃது','எஃகு']),('உயிர்த்தொடர்',['வரகு','முரடு','கிணறு']),('வன்தொடர்',['பாக்கு','பட்டு','பத்து']),('மென்தொடர்',['குரங்கு','பஞ்சு','வண்டு']),('இடைத்தொடர்',['மார்பு','சார்பு'])],1):
 add(f'UCLASS-{i:03}','short_u','குற்றியலுகரம் — '+name,name+' shortened-U classification','CLASSIFY',f'Identify word final shortened உ with preceding sound in {name}',ex,needs=['stem_morphology','phonology'],source='TVA_U',layer='classical_and_modern',tier='review',note='Examples are words illustrating class; not isolated correction triggers.')
add('U-007','short_u','குற்றியலுகரம் உயிர்முன் கெடுதல்','Short-u elision before vowels','TRANSFORM_COMPOUND','Verified final குற்றியலுகரம் followed by vowel-initial suffix or joined constituent',['வண்டு + இனம் → வண்டினம்'],needs=['short_u_class','boundary_is_compound_or_suffix'],source='TVA_U')
add('U-008','short_u','இறுதி எழுத்து மாற்றங்கள்','Some suffixes cause additional alternation','REVIEW','Suffix-specific stem alternation requires lexicon and morphological analysis',['ஆடு + இன் → ஆட்டின்','வயிறு + ஐ → வயிற்றை'],source='TVA_U',needs=['lexical_paradigm'],note='Do not generalize consonant doubling to every short-u stem.')
add('U-009','short_u','முற்றியலுகரம்','Full-u stems','REVIEW','Full உ does not behave identically to shortened உ; phonological and lexical class must be known',['பசு','மது','கதவு','நிலவு'],needs=['stem_morphology'],source='TVA_U',note='Do not automatically elide all full உ endings.')
add('U-010','short_u','குற்றியலிகரம்','Shortened-i','REVIEW','Classical shortening of இ in applicable environments, including certain combinations before ய்',needs=['morphological_environment'],source='TVA_U',layer='classical',tier='review')
# consonant finals
add('C-001','consonant','மெய்யுடன் உயிர் சேர்தல்','Consonant-vowel attachment','TRANSFORM_COMPOUND','Final pure consonant + vowel initial next element, actual compound/suffix',['வேல் + எறிந்தான் → வேலெறிந்தான்'],needs=['boundary_is_compound_or_suffix'],source='TVA_BOOK')
add('C-002','consonant','தனிக்குறில் முன் ஒற்று இரட்டல்','Short monosyllable coda gemination','TRANSFORM_COMPOUND','Single short vowel before final consonant + attached vowel',['கல் + அழகு → கல்லழகு','பொன் + ஆடை → பொன்னாடை'],needs=['short_monovocalic_stem','boundary_is_compound_or_suffix'],source='TVA_BOOK')
add('M-001','consonant_m','மகர ஈறு கெட்டு வல்லினம் மிகுதல்','M-loss + hard consonant increase','TRANSFORM_COMPOUND','Specific case or adjective/simile compound; ம் final and hard initial',['மரம் + கிளை → மரக்கிளை','மரம் + கலம் → மரக்கலம்'],needs=['compound_type'],source='TVA_M')
add('M-002','consonant_m','மகர ஈறு இனமெல்லெழுத்தாதல்','M assimilates to matching nasal','TRANSFORM_COMPOUND','Applicable case or classical compound before a hard consonant',['காலம் + கடந்து → காலங்கடந்து','மரம் + கண்டார் → மரங்கண்டார்'],needs=['compound_type'],source='TVA_M',layer='classical_and_modern',tier='review')
add('M-003','consonant_m','மகர ஈறு கெட்டு இடையினம் வருதல்','M-loss with medial onset','TRANSFORM_COMPOUND','Licensed compound whose ம் drops before medial initial',['மரம் + வேர் → மரவேர்'],needs=['compound_type'],source='TVA_M')
add('M-004','consonant_m','மகர ஈறு மாறாமல் நிற்றல்','M-retention','PROTECT','Independent subject phrase / relative participle; ம் remains',['மரம் வளர்ந்தது','கொல்லும் யானை'],source='TVA_M',needs=['syntactic_relation'],tier='block')
add('M-005','consonant_m','மகர ஈற்று விகற்பம்','M-final accepted alternatives','ALLOW_VARIANTS','Classical variant formation of compounds with ம்',['குளம் + கரை → குளக்கரை','குளம் + கரை → குளங்கரை'],source='TVA_M',layer='classical_and_modern')
add('M-006','consonant_m','அகம் சிறப்புப் புணர்ச்சி','Akam special compounds','TRANSFORM_COMPOUND','Stem அகம் with கை or செவி',['அகம் + கை → அங்கை','அகம் + செவி → அஞ்செவி'],source='TVA_M',layer='classical_and_modern')
add('M-007','consonant_m','நும் தம் எம் நம்','Pronoun stem alternations','REVIEW','Special declensional stems and nasals',['நாம் + ஐ → நம்மை','தாம் + ஐ → தம்மை'],source='TVA_M',layer='classical_and_modern',needs=['pronoun_paradigm'])
add('LR-001','consonant_l','லகரம் றகரமாகத் திரிதல்','L → R-retroflex class','TRANSFORM_COMPOUND','Specific case/special-name compounds, final ல் before hard initial',['பால் + குடம் → பாற்குடம்','வேல் + படை → வேற்படை'],source='TVA_L',needs=['compound_type'])
add('LR-002','consonant_l','ளகரம் டகரமாகத் திரிதல்','LL → retroflex T','TRANSFORM_COMPOUND','Specific compounds final ள் before hard initial',['வாள் + படை → வாட்படை','முள் + செடி → முட்செடி'],source='TVA_L',needs=['compound_type'])
add('LR-003','consonant_l','லகர + நகர மாற்றம்','L+N assimilation','TRANSFORM_COMPOUND','Genuine compound with final ல் plus ந initial',['நல் + நூல் → நன்னூல்'],source='TVA_L',needs=['compound_type'])
add('LR-004','consonant_l','லகர ளகர இயல்பு','No universal L/LL conversion','PROTECT','Ordinary free phrase or coordinative compound may preserve ல்/ள்',['கால் கை','தோள் வலிமை'],source='TVA_L',needs=['syntactic_relation'],tier='block')
add('NN-001','consonant_n','னகர மாற்றம்','N → R in established compounds','TRANSFORM_COMPOUND','Specific compounds final ன் followed by க initial',['பொன் + கலம் → பொற்கலம்'],source='TVA_BOOK',needs=['lexical_paradigm'])
add('NN-002','consonant_n','ணகர மாற்றம்','Retroflex N → retroflex T','TRANSFORM_COMPOUND','Specific compounds final ண் followed by க initial',['மண் + குடம் → மட்குடம்'],source='TVA_BOOK',needs=['lexical_paradigm'])
add('NN-003','consonant_n','மெய் இரட்டல்','Nasals double in licensed vowel boundary','TRANSFORM_COMPOUND','Short stem final ன்/ண் before vowel attached',['பொன் + ஆடை → பொன்னாடை'],source='TVA_BOOK',needs=['boundary_is_compound_or_suffix'])
add('YRZ-001','consonant_yrz','யகர ரகர ழகர ஈறு','Y/R/Z + vallinam','REVIEW','Hard consonant joining compounds with final ய்/ர்/ழ்',['மெய் + கீர்த்தி → மெய்க்கீர்த்தி','கார் + பருவம் → கார்ப்பருவம்','யாழ் + கருவி → யாழ்க்கருவி'],needs=['compound_type'],source='TVA_HOUSE')
add('YRZ-002','consonant_yrz','யகர ரகர ழகர இயல்பு','Y/R/Z may remain before hard consonants','PROTECT','Free phrases/other constructions do not universally gain hard consonants',['வேர் சிறிது','யாழ் பெரிது'],source='TVA_HOUSE',needs=['syntactic_relation'],tier='block')
add('V-001','consonant_v','அவ்/இவ் சொற்களின் புணர்ச்சி','V-final demonstrative alternations','REVIEW','Lexically specified demonstrative prefix compounds',['அவ் + யானை → அவ்வியானை'],source='TVA_BOOK',needs=['lexical_paradigm'],note='Do not infer a blanket வ்→ற் rule from this example; verify individual lexical derivatives.')
# overt case and compound groups
add('CASE-002-OVERT','case','இரண்டாம் வேற்றுமை விரி','Explicit accusative ஐ','INSERT_HARD','Following word begins க/ச/த/ப and previous noun is explicitly accusative (not lexical ஐ)',['கதையைப் படித்தான்','மலரைப் பறித்தான்','கதவைத் தட்டு','ஒலியைக் குறை'],['கதையை படித்தான்','மலரை பறித்தான்'],['case_marker','lexical_ai_flag'],source='TVA_ADD',tier='candidate')
add('CASE-002-ELLIP','case','இரண்டாம் வேற்றுமைத்தொகை','Implied accusative','NO_INSERT','Accusative meaning but ஐ unexpressed',['நகை செய்தான்','கனி தின்றான்','தமிழ் படித்தான்'],['நகைச் செய்தான்'],['case_relation','case_marker'],source='TVA_NO',tier='block')
add('CASE-002-COMPOUND','case','இரண்டாம் உருபும் பயனும் உடன்தொக்க தொகை','Accusative compound','INSERT_HARD','Noun compound with implicit object relation and implicit predicate',['நகைக்கடை','எலிப்பொறி','தயிர்க்குடம்','நெய்க்குடம்'],needs=['compound_semantics'],source='TVA_ADD')
add('CASE-003-COMPOUND','case','மூன்றாம் உருபும் பயனும் உடன்தொக்க தொகை','Instrumental/material compound','INSERT_HARD','Compound specifying material/instrument',['வெள்ளிக் கிண்ணம்','இரும்புப் பெட்டி','தேங்காய்ச் சட்னி'],needs=['compound_semantics'],source='TVA_ADD')
add('CASE-004-OVERT','case','நான்காம் வேற்றுமை விரி','Explicit dative கு','INSERT_HARD','Dative suffix கு/க்கு explicit and following க/ச/த/ப',['எனக்குக் கொடு','ஊருக்குப் போனான்','வீட்டுக்குத் தலைவி'],['எனக்கு கொடு'],['case_marker'],source='TVA_ADD',tier='candidate')
add('CASE-004-ELLIP-HUMAN','case','நான்காம் வேற்றுமைத்தொகை உயர்திணை','Implied dative human','NO_INSERT','Human-animate noun in omitted dative relation',['பொன்னி கணவன்'],needs=['noun_animacy','case_relation'],source='TVA_NO',tier='block')
add('CASE-004-ELLIP-NONHUMAN','case','நான்காம் வேற்றுமைத்தொகை அஃறிணை','Implied dative nonhuman','INSERT_HARD','Nonhuman noun in omitted dative compound',['குறிஞ்சித் தலைவன்','படைத்தளபதி','கூலிப்படை'],needs=['noun_animacy','compound_semantics'],source='TVA_ADD')
add('CASE-004-COMPOUND','case','நான்காம் உருபும் பயனும் உடன்தொக்க தொகை','Dative compound','INSERT_HARD','Dative semantic compound with elided material (most, not all)',['தண்ணீர்க்குடம்'],needs=['compound_semantics'],source='TVA_ADD',note='Most (பெரும்பாலும்), not unconditional.')
add('CASE-005-OVERT','case','ஐந்தாம் வேற்றுமை விரி','Ablative expression','NO_INSERT','Explicit இருந்து/நின்று removal expression',['வீட்டிலிருந்து சென்றான்','கூட்டினின்று பறந்தது'],source='TVA_NO',tier='block')
add('CASE-006-ELLIP-HUMAN','case','ஆறாம் வேற்றுமைத்தொகை உயர்திணை','Human genitive compound','NO_INSERT','Human noun possessive relation',['ஆசிரியர் கழகம்','தம்பி துணி','மகளிர் கல்லூரி'],source='TVA_NO',needs=['noun_animacy','compound_semantics'],tier='block')
add('CASE-006-ELLIP-NONHUMAN','case','ஆறாம் வேற்றுமைத்தொகை அஃறிணை','Nonhuman possessive compound','INSERT_HARD','Nonhuman noun possessive compound',['கிளிக்கூண்டு','புலிக்குட்டி'],source='TVA_ADD',needs=['noun_animacy','compound_semantics'])
add('CASE-006-OVERT','case','உடைமைக் குறி','Explicit possessive','NO_INSERT','Possessive -அது/உடைய in grammatical phrase',['எனது புத்தகம்','என்னுடைய புத்தகம்'],source='TVA_NO',tier='block')
add('CASE-007-COMPOUND','case','ஏழாம் உருபும் பயனும் உடன்தொக்க தொகை','Locative compound','INSERT_HARD','Locative compound with implicit location word',['மலைப்பாம்பு','சென்னைக் கல்லூரி','மதுரைக்கோயில்'],source='TVA_ADD',needs=['compound_semantics'])
add('CASE-ALL-SCOPE','case','வேற்றுமை விதிகள் விதிவிலக்கு','Case scope','REVIEW','Six non-nominative/non-vocative cases have different overt, elliptical, and compound forms',source=['TVA_CASE','TVA_ADD','TVA_NO'],needs=['case_relation'])
# vallinam on
for id,category,ex,src in [
('HARD-DEM','Demonstrative adjective',['அந்தப் பையன்','இந்தக் குழந்தை','எந்தப் பொருள்'],'TVA_ADD'),
('HARD-LOCADV','Locative/interrogative adverb',['அங்குக் கண்டான்','எங்குப் போனான்'],'TVA_ADD'),
('HARD-MANNER','Manner adverb',['அப்படிச் சொல்','இப்படிப் பேசு','எப்படிச் சொல்வான்'],'TVA_ADD'),
('HARD-PREFIX','Single-letter demonstrative prefix',['அக்காலம்','எத்திசை'],'TVA_ADD'),
('HARD-ONELETTER','One-letter lexical stems',['கைக்குழந்தை','தீப்பெட்டி','பூப்பல்லக்கு','மைப்பேனா'],'TVA_ADD'),
('HARD-INFINITIVE','A-final infinitive',['வரச் சொன்னான்','உண்ணப் போனான்','உட்காரப் பார்த்தான்'],'TVA_ADD'),
('HARD-PART-I','I-final verbal participle',['ஓடிப் போனான்','தேடிப் பார்த்தான்','கூறிச் சென்றான்'],'TVA_ADD'),
('HARD-PART-Y','Y-final verbal participle',['போய்ப் பார்த்தான்'],'TVA_ADD'),
('HARD-PART-HARDU','Vanthodar short-u participle',['கற்றுக் கொடுத்தான்','படித்துக் கொடுத்தான்','வைத்துப் போனான்'],'TVA_ADD'),
('HARD-PART-AG','ஆக/ஆய்/என adverbial participles',['நன்றாகச் சொன்னான்','வருவதாய்க் கூறினார்','வா எனக் கூறினார்'],'TVA_ADD'),
('HARD-PART-QUALITY','Qualitative adverbs',['மெல்லப் பேசினார்','வேகமாகக் கூறினார்','நிறையப் பேசுவான்'],'TVA_ADD'),
('HARD-NEG-ELLIP','Clipped negative relative participles',['செல்லாக் காசு','ஓடாக் குதிரை','தீராச் சிக்கல்','காணாப் பொருள்'],'TVA_ADD'),
('HARD-QUALITY','Quality compounds',['சிவப்புத்துணி','புதுமைப்பெண்','வெள்ளைத்தாள்','மெய்ப்பொருள்'],'TVA_ADD'),
('HARD-SPEC-GEN','Specific+generic noun compound',['மல்லிகைப்பூ'],'TVA_ADD'),
('HARD-SIMILE','Simile/metaphor compounds',['தாமரைப்பாதம்'],'TVA_ADD'),
('HARD-HARDU-NOUN','Vanthodar short-u noun compounds',['பாக்குத்தோப்பு','எட்டுத்தொகை','பத்துப்பாட்டு'],'TVA_ADD'),
('HARD-SOFTU-NOUN','Selected menthodar short-u noun compounds',['குரங்குக் குட்டி','மருந்துச் சீட்டு','பாம்புத்தோல்'],'TVA_ADD')]:
 add(id,'vallinam_add',category,category,'INSERT_HARD' if 'HARD-' in id else 'REVIEW',f'Confirmed {category} grammatical category; second constituent begins with க/ச/த/ப',ex,needs=['syntactic_relation','compound_type'],source=src, tier='review' if id not in ['HARD-DEM','HARD-MANNER','HARD-INFINITIVE'] else 'candidate')
# vallinam protected
no_sets=[
('NO-PRON','Pronouns',['அது சிறிது','இது பெரியது','எது பெரியது','அவை போயின','இவை சிறந்தவை']),
('NO-DEMONSTRATIVE-QUANT','Quantifiers',['அத்தனை செடிகள்','இத்தனை பொருட்கள்','எத்தனை பழங்கள்','அவ்வளவு பேர்','எவ்வாறு பேசினான்']),
('NO-TIME-ADV','Time and reporting adverbs',['அன்று பார்த்தான்','இன்று சென்றான்','என்று சொன்னான்']),
('NO-NUM','Ordinary cardinal numeral',['ஒன்று போதும்','இரண்டு தடவை','மூன்று குழந்தைகள்','நான்கு கால்கள்','ஐந்து சிறுவர்கள்','நூறு பழங்கள்']),
('NO-NUM-ADJ','ஒரு/இரு/அறு/எழு adjectival numbers',['ஒரு புத்தகம்','இரு தடவை','அறுபடைவீடு','எழுகடல்']),
('NO-SUBJECT','Subject-predicate',['கண்ணகி பேசினாள்','கோழி கூவியது','யெகோவா காப்பார்','கர்த்தர் சொன்னார்']),
('NO-VOCATIVE','Vocative',['மகனே கேள்','அண்ணா பாருங்கள்','அரசே பார்']),
('NO-IMPERATIVE','Imperative/optative boundary',['வா கண்ணா','படி பாடத்தை','வாழ்க தலைவா']),
('NO-REL-POS','Positive relative participle',['வந்த பையன்','கேட்ட கேள்வி','ஓடிய குதிரை']),
('NO-ADJECTIVE','Qualifying adjective',['நல்ல பையன்','பெரிய தெரு','புதிய சிந்தனை']),
('NO-REL-NEG','Unclipped negative relative participle',['செல்லாத காசு','காணாத கண்கள்','ஓடாத குதிரை']),
('NO-PART-SOFTU','Menthodar verbal participle',['கண்டு பேசினார்','கொண்டு போனான்','வந்து சென்றான்','தின்று பார்த்தான்']),
('NO-PART-MIDU','Idaithodar verbal participle',['செய்து தந்தான்','கொய்து கொடுத்தாள்']),
('NO-PART-QUALU','U-final qualitative adverb',['நன்கு பேசினான்','நன்று பேசினாய்']),
('NO-PART-PADI','படி/ஆறு adverbs',['சொன்னபடி செய்தான்','கண்டவாறு சொன்னான்','வருமாறு கூறினான்']),
('NO-VERBCOMP','வினைத்தொகை',['சுடுகாடு','நடுகல்','சுடுசோறு','விடுகதை']),
('NO-UMMAI','உம்மைத்தொகை',['இரவு பகல்','யானை குதிரை','நன்மை தீமை']),
('NO-QUESTION','Interrogative ஆ/ஓ',['அவனா தந்தான்','அவளோ கொடுத்தாள்']),
('NO-PALA-SILA','பல/சில',['பல பொருள்','சில பூக்கள்']),
('NO-GEN','Possessive explicit',['எனது புத்தகம்','என்னுடைய புத்தகம்']),
('NO-SANSKRIT','Selected learned loan compounds',['ஆதிபகவன்','தேச பக்தி']),
('NO-SUFFIX-PL','Plural கள் suffix to certain short-u stems',['எழுத்துகள்','வாக்குகள்','வழக்குகள்']),
('NO-SUFFIX-NOM','Nominalizing தல் suffix',['ஆக்குதல்','வாழ்த்துதல்','கூப்புதல்']),
('NO-LEXICAL-AI','Lexical ஐ is not accusative',['அறிக்கை செய்தான்','கொலை செய்தான்']),
('NO-PUNCT','Punctuation/quote boundaries',['அவன் சொன்னான்: “பார்.”']),
('NO-NAMES','Proper names and names of deity',['யெகோவா காப்பார்','கர்த்தர் சொன்னார்']),
]
for id,title,examples in no_sets:
 add(id,'vallinam_no','வல்லினம் மிகா இடம்',title,'NO_INSERT',f'Protected context: {title}; must classify grammatical relation, not suffix alone',examples,needs=['syntactic_relation'],source='TVA_NO' if id not in ('NO-PUNCT','NO-LEXICAL-AI','NO-NAMES') else 'TVA_CLASS',tier='block',note='Bible examples reflect configured house-style testing, not quotations of source text.' if id=='NO-NAMES' else '')
# special lexical compound families
for id,tamil,title,examples,src in [
('LEX-QUAL-LOSS','ஈறுபோதல்','மை drop',['நன்மை + நூல் → நன்னூல்'],'TVA_BOOK'),
('LEX-QUAL-LENGTH','ஆதிநீடல்','Initial vowel lengthening',['பசுமை + இலை → பாசிலை'],'TVA_BOOK'),
('LEX-QUAL-VOW','அடியகரம் ஐயாதல்','Initial a to ai',['பசுமை + தமிழ் → பைந்தமிழ்'],'TVA_BOOK'),
('LEX-QUAL-DOUBLE','தன்னொற்று இரட்டல்','Consonant doubling in quality compound',['வெறுமை + இலை → வெற்றிலை'],'TVA_BOOK'),
('LEX-QUAL-UY','இடைஉகரம் இய்யாதல்','Medial உ to இ',[],'TVA_BOOK'),
('LEX-QUAL-MEI','முன்னின்ற மெய் திரிதல்','Previous consonant transformation',[], 'TVA_BOOK'),
('LEX-QUAL-NASAL','இனமிகல்','Nasal increment',[], 'TVA_BOOK'),
('LEX-QUAL-FLEX','இணையவும்பெறும்','Alternative derivations',[], 'TVA_BOOK'),
('LEX-DIR-NORTH','வடக்கு','North direction compounds',['வடக்கு + கிழக்கு → வடகிழக்கு','வடக்கு + நாடு → வடநாடு'],'TVA_DIR'),
('LEX-DIR-SOUTH','தெற்கு','South direction compounds',['தெற்கு + மேற்கு → தென்மேற்கு','தெற்கு + நாடு → தென்னாடு'],'TVA_DIR'),
('LEX-DIR-WEST','மேற்கு','West direction compounds',['மேற்கு + நாடு → மேல்நாடு'],'TVA_DIR'),
('LEX-DIR-EAST','கிழக்கு','East direction compounds',['கிழக்கு + நாடு → கீழ்நாடு'],'TVA_DIR'),
('LEX-DIR-ALT','திசைப்பெயர் விகற்பம்','Untransformed directional alternatives',['வடக்கு + திசை → வடக்குத் திசை','மேற்கு + திசை → மேற்குத் திசை'],'TVA_DIR'),
('LEX-NUM-1','ஒன்று','One compounds',['ஒன்று + மாடு → ஒரு மாடு','ஒன்று + ஆடு → ஓராடு'],'TVA_NUM1'),
('LEX-NUM-2','இரண்டு','Two compounds',['இரண்டு + கால் → இருகால்','இரண்டு + ஆயிரம் → ஈராயிரம்'],'TVA_NUM2'),
('LEX-NUM-3','மூன்று','Three compounds',['மூன்று + தமிழ் → முத்தமிழ்','மூன்று + உலகு → மூவுலகு'],'TVA_NUM2'),
('LEX-NUM-4','நான்கு','Four compounds',['நான்கு + வகை → நால்வகை','நான்கு + படை → நாற்படை','நான்கு + மணி → நான்மணி'],'TVA_NUM2'),
('LEX-NUM-5','ஐந்து','Five compounds',['ஐந்து + பொறி → ஐம்பொறி'],'TVA_NUM1'),
('LEX-NUM-6','ஆறு','Six compounds',['ஆறு + படைவீடு → அறுபடைவீடு'],'TVA_NUM1'),
('LEX-NUM-7','ஏழு','Seven compounds',['ஏழு + பிறப்பு → எழுபிறப்பு'],'TVA_NUM1'),
('LEX-NUM-8','எட்டு','Eight compounds',['எட்டு + குணம் → எண்குணம்'],'TVA_NUM1'),
('LEX-NUM-9','ஒன்பது','Nine compounds',['ஒன்பது + ஆயிரம் → ஒன்பதினாயிரம்'],'TVA_NUM3'),
('LEX-NUM-10','பத்து','Ten compounds',['பத்து + ஒன்று → பதினொன்று','பத்து + இரண்டு → பன்னிரண்டு','பத்து + பத்து → பதிற்றுப்பத்து'],'TVA_NUM3'),
('LEX-NUM-TENS','பத்துகளின் புணர்ச்சி','Tens',['இரண்டு + பத்து → இருபது','மூன்று + பத்து → முப்பது','ஐந்து + பத்து → ஐம்பது'],'TVA_NUM3'),
('LEX-NUM-REDUP','எண்ணுப்பெயர் இரட்டிப்பு','Numeric reduplication',['ஒன்று + ஒன்று → ஒவ்வொன்று'],'TVA_NUM3'),
('LEX-TREE-MA','மரப்பெயர் மா','Mango compounds',['மா + காய் → மாங்காய்'],'TVA_BOOK'),
('LEX-TREE-PALAA','மரப்பெயர் பலா','Jackfruit compounds',['பலா + காய் → பலாக்காய்'],'TVA_BOOK'),
('LEX-TREE-THENGU','தெங்கு + காய்','Coconut compound',['தெங்கு + காய் → தேங்காய்'],'TVA_DIR')]:
 add(id,'lexical','சிறப்புப் புணர்ச்சி — '+tamil,title,'LEXICON_REVIEW',f'Established lexical derivation for {tamil}; never generate unknown forms',examples,needs=['lemma_id','approved_dictionary'],source=src,layer='classical_and_modern',tier='review',note='Listed as an inventory item, not an open-ended suffix replacement.')
# sariyai
for id,ex in [('SARI-AN','அன்'),('SARI-AAN','ஆன்'),('SARI-IN','இன்'),('SARI-AL','அல்'),('SARI-ATTRU','அற்று'),('SARI-ITTRU','இற்று'),('SARI-ATTHU','அத்து'),('SARI-AM','அம்'),('SARI-THAM','தம்'),('SARI-NAM','நம்'),('SARI-NUM','நும்'),('SARI-E','ஏ'),('SARI-A','அ'),('SARI-U','உ'),('SARI-AI','ஐ'),('SARI-KU','கு'),('SARI-N','ன்')]:
 add(id,'sariyai','சாரியை — '+ex,'Connecting morph '+ex,'REVIEW',f'Classify linking element {ex}; recognize only when morphology supports',needs=['stem_morphology','suffix','grammar_model'],source='TVA_SARI',layer='classical_and_modern',tier='review')
add('SARI-ATTHU-EX','sariyai','அத்துச் சாரியை','Aththu in case inflection','TRANSFORM_COMPOUND','Lexeme மரம் with accusative marker ஐ',['மரம் + ஐ → மரத்தை'],needs=['stem_morphology','case_marker'],source='TVA_SARI')
add('SARI-IN-EX','sariyai','இன் சாரியை','In increment','REVIEW','Recognized paradigm insertion before case suffixes',['பூவிற்கு','பூவுக்கு'],needs=['stem_morphology'],source='TVA_SARI',note='Do not force one of optional accepted forms.')
# policy and enforcement
for id,title,pre,ex in [
('SAFE-001','Unicode NFC','Normalize input for analysis; preserve original bytes for output',[]),
('SAFE-002','Not a whitespace-only task','Never infer grammar from last/first letters alone',[]),
('SAFE-003','Preserve Scripture','No silent changes to USFM Bible text',[]),
('SAFE-004','Protect USFM markers','Treat USFM verse/book/chapter markers, footnotes, xrefs, quotes and punctuation as protected boundaries',[]),
('SAFE-005','Lexical ஐ','Distinguish root-final ஐ from case marker ஐ',['அறிக்கை செய்தான்','கொலை செய்தான்']),
('SAFE-006','Project exception','No added த் before தேவன்-family terms in Bible IRV house style',['அவனை தேவசாயலாக']),
('SAFE-007','Proper divine name spelling','Canonical configured name spelling யெகோவா; not யெஹோவா', ['யெகோவா']),
('SAFE-008','No forced emendation','Human must approve edits to authoritative target',[]),
('SAFE-009','Rule conflict','Specific protection > explicit house override > verified grammatical rule > optional lexical rule',[]),
('SAFE-010','Open findings','Unknown syntax or low confidence creates review-only finding',[]),
('SAFE-011','No forced word join','Do not rewrite independent word spacing from classical compounds',[]),
('SAFE-012','Morphology tokens','Do not treat postpositions/suffixes as lexical independent words without analysis',[]),
('SAFE-013','Source separation','House rules are separate from grammatical facts',[]),
('SAFE-014','Idempotence','Repeated check on unchanged text emits same findings; applying approved fix is idempotent',[]),
('SAFE-015','Audit trail','Log context, exact text spans, rule id, evidence, reviewer and reason, not just corrected form',[])]:
 add(id,'safety','பாதுகாப்பு விதி',title,'PROTECT',pre,ex,source='TVA_CLASS',layer='house' if id in ('SAFE-003','SAFE-006','SAFE-007','SAFE-008') else 'engineering',tier='block')

# Additional classical special cases and scope exceptions verified in TVA texts
add('SPECIAL-MA-QUALITY','lexical','மா உரிச்சொல்','Maa as intensifier differs from tree noun','LEXICON_REVIEW','மா is an uriccol (intensifier), not tree noun; takes distinctive ய் derivation',['மா + இரு → மாயிரு'],source='TVA_PURAN',needs=['lexical_sense'],layer='classical',note='Contrast noun மா + இலை → மாவிலை.')
add('SPECIAL-MA-NOUN','lexical','மா பெயர்ச்சொல்','Maa noun takes ordinary வ் glide','LEXICON_REVIEW','மா means mango tree or animal; vowel onset',['மா + இலை → மாவிலை'],source='TVA_PURAN',needs=['lexical_sense'],layer='classical_and_modern')
add('SPECIAL-PALA-SILA','lexical','பல/சில இரட்டிப்பு','Several accepted reduplication forms','ALLOW_VARIANTS','Reduplicated பல+பல or சில+சில',['பலபல','பலப்பல','சிலசில','சிலச்சில'],source='TVA_AKAR',needs=['lexical_paradigm'],layer='classical_and_modern')
add('SPECIAL-MUTTRU-ODU','short_u','ஒடு உருபு','Instrumental/comitative ஒடு suffix','NO_INSERT','Comitative suffix ஒடு followed by predicate',['இராமனொடு சென்றான்'],source='TVA_UKAR',needs=['case_marker'],tier='block')
add('SPECIAL-SARI-PROTECT','sariyai','சாரியைகள் இடைச்சொற்கள்','Sariyai is not the same as a lexical final ன்','PROTECT','Do not apply lexical final ன் → ற் change to a grammatical சாரியை',['ஒன்றன் கூட்டம்','வண்டின்கால்'],source='TVA_PURAN',tier='block',needs=['morphological_segmentation'])
add('SPECIAL-ALLAM','sariyai','எல்லாம் எல்லாரும் எல்லீரும்','Special quantifier case forms','REVIEW','Quantifier/pronominal paradigms use special சாரியை placement',source='TVA_URUPU2',needs=['pronoun_paradigm'],layer='classical_and_modern')
add('SPECIAL-PRON-CASE','sariyai','தான் தாம் நாம்','Pronoun case inflection','REVIEW','Pronouns change stems and linkers before case suffixes',['தாம் + ஐ → தம்மை','நாம் + ஐ → நம்மை'],source='TVA_URUPU2',needs=['pronoun_paradigm'])
add('SPECIAL-AH','sariyai','அஃது சுட்டுப்பெயர்','Aytham-containing pronoun case forms','REVIEW','Special case inflection of அஃது and related demonstratives',source='TVA_URUPU2',needs=['pronoun_paradigm'],layer='classical')
add('SPECIAL-IRU','sariyai','ஆ மா கோ ஒரெழுத்துப் பெயர்கள்','One-letter stems in case inflection','REVIEW','Case endings of special single-letter nouns',source='TVA_URUPU2',needs=['stem_morphology'])

assert len({r['id'] for r in rules})==len(rules)
out={'schema_version':'1.0.0','catalog_version':'2026-11-13','language':'ta','description':'Conservative, context-aware Tamil punarchi QA rule inventory, not a standalone regex correction engine','sources':sources,'rule_count':len(rules),'rules':rules}
(p/'rules.json').write_text(json.dumps(out,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('rules=',len(rules),Counter(r['category'] for r in rules))
