import json, pathlib
p=pathlib.Path('/mnt/data/tamil_punarchi_agent_package')
cases=[]
def tc(id,text,expected,rule,features=None,expected_action='suggest',must_not_change=False,details=''):
 cases.append({'id':id,'input_text':text,'expected_text_if_approved':expected,'expected_rule_ids':[rule],
               'parser_annotations':features or {},'expected_action':expected_action,
               'must_not_change_automatically':True,'must_not_suggest_insertion':must_not_change,'notes':details})
# Must remain unchanged: protection/regressions
negative=[
('A001','அறிக்கை செய்தான்','NO-LEXICAL-AI','noun_verb'),
('A002','கொலை செய்தான்','NO-LEXICAL-AI','noun_verb'),
('A003','அது சிறிது','NO-PRON','pronoun_predicate'),
('A004','இது பெரியது','NO-PRON','pronoun_predicate'),
('A005','எது பெரியது','NO-PRON','pronoun_predicate'),
('A006','அவை போயின','NO-PRON','pronoun_predicate'),
('A007','அத்தனை செடிகள்','NO-DEMONSTRATIVE-QUANT','quantifier_noun'),
('A008','எத்தனை பழங்கள்','NO-DEMONSTRATIVE-QUANT','quantifier_noun'),
('A009','அவ்வளவு பேர்','NO-DEMONSTRATIVE-QUANT','quantifier_noun'),
('A010','ஒன்று போதும்','NO-NUM','numeral_predicate'),
('A011','இரண்டு தடவை','NO-NUM','numeral_noun'),
('A012','ஒரு புத்தகம்','NO-NUM-ADJ','numeral_adjective'),
('A013','கோழி கூவியது','NO-SUBJECT','subject_predicate'),
('A014','யெகோவா காப்பார்','NO-SUBJECT','subject_predicate'),
('A015','கர்த்தர் சொன்னார்','NO-SUBJECT','subject_predicate'),
('A016','மகனே கேள்','NO-VOCATIVE','vocative'),
('A017','அண்ணா பாருங்கள்','NO-VOCATIVE','vocative'),
('A018','வந்த பையன்','NO-REL-POS','relative_participle'),
('A019','ஓடிய குதிரை','NO-REL-POS','relative_participle'),
('A020','நல்ல பையன்','NO-ADJECTIVE','adjective'),
('A021','பெரிய தெரு','NO-ADJECTIVE','adjective'),
('A022','ஓடாத குதிரை','NO-REL-NEG','negative_relative_participle'),
('A023','காணாத கண்கள்','NO-REL-NEG','negative_relative_participle'),
('A024','கண்டு பேசினார்','NO-PART-SOFTU','menthodar_vinaiyecham'),
('A025','வந்து சென்றான்','NO-PART-SOFTU','menthodar_vinaiyecham'),
('A026','செய்து தந்தான்','NO-PART-MIDU','idaithodar_vinaiyecham'),
('A027','நன்கு பேசினான்','NO-PART-QUALU','qualitative_u_adverb'),
('A028','சொன்னபடி செய்தான்','NO-PART-PADI','padi_adverb'),
('A029','சுடுகாடு','NO-VERBCOMP','verbal_compound'),
('A030','சுடுசோறு','NO-VERBCOMP','verbal_compound'),
('A031','நன்மை தீமை','NO-UMMAI','ummai_compound'),
('A032','அவனா தந்தான்','NO-QUESTION','interrogative'),
('A033','பல பொருள்','NO-PALA-SILA','quantifier'),
('A034','எனது புத்தகம்','NO-GEN','possessive'),
('A035','என்னுடைய புத்தகம்','NO-GEN','possessive'),
('A036','அவனை தேவசாயலாக','SAFE-006','house_divine_exception'),
('A037','மரம் வளர்ந்தது','M-004','subject_predicate'),
('A038','கொல்லும் யானை','M-004','relative_participle'),
('A039','நகை செய்தான்','CASE-002-ELLIP','accusative_ellipsis'),
('A040','தமிழ் படித்தான்','CASE-002-ELLIP','accusative_ellipsis'),
('A041','வீட்டிலிருந்து சென்றான்','CASE-005-OVERT','ablative'),
('A042','ஆசிரியர் கழகம்','CASE-006-ELLIP-HUMAN','human_genitive_compound'),
('A043','பொன்னி கணவன்','CASE-004-ELLIP-HUMAN','human_dative_compound'),
('A044','இரவு பகல்','NO-UMMAI','ummai_compound'),
('A045','எழுத்துகள்','NO-SUFFIX-PL','plural_suffix'),
('A046','பூவுக்கு','SARI-IN-EX','accepted_variant'),
('A047','பூவிற்கு','SARI-IN-EX','accepted_variant'),
('A048','குளக்கரை','M-005','accepted_variant'),
('A049','குளங்கரை','M-005','accepted_variant'),
]
for id,raw,rule,relation in negative:
 tc(id,raw,raw,rule,{'syntactic_relation':relation},expected_action='none',must_not_change=True)
# Candidate: flag, never mutate Bible text automatically
positive=[
('B001','கதையை படித்தான்','கதையைப் படித்தான்','CASE-002-OVERT','explicit_accusative'),
('B002','மலரை பறித்தான்','மலரைப் பறித்தான்','CASE-002-OVERT','explicit_accusative'),
('B003','கதவை தட்டு','கதவைத் தட்டு','CASE-002-OVERT','explicit_accusative'),
('B004','அவரை பார்த்தான்','அவரைப் பார்த்தான்','CASE-002-OVERT','explicit_accusative'),
('B005','ஊருக்கு போனான்','ஊருக்குப் போனான்','CASE-004-OVERT','explicit_dative'),
('B006','எனக்கு கொடு','எனக்குக் கொடு','CASE-004-OVERT','explicit_dative'),
('B007','அந்த பையன்','அந்தப் பையன்','HARD-DEM','demonstrative_adjective'),
('B008','இந்த குழந்தை','இந்தக் குழந்தை','HARD-DEM','demonstrative_adjective'),
('B009','எந்த பொருள்','எந்தப் பொருள்','HARD-DEM','interrogative_adjective'),
('B010','அப்படிச் சொல்','அப்படிச் சொல்','HARD-MANNER','manner_adverb'),
('B011','வர சொன்னான்','வரச் சொன்னான்','HARD-INFINITIVE','infinitive'),
('B012','உண்ண போனான்','உண்ணப் போனான்','HARD-INFINITIVE','infinitive'),
('B013','ஓடி போனான்','ஓடிப் போனான்','HARD-PART-I','i_ending_participle'),
('B014','தேடி பார்த்தான்','தேடிப் பார்த்தான்','HARD-PART-I','i_ending_participle'),
('B015','போய் பார்த்தான்','போய்ப் பார்த்தான்','HARD-PART-Y','y_ending_participle'),
('B016','படித்து கொடுத்தான்','படித்துக் கொடுத்தான்','HARD-PART-HARDU','vanthodar_participle'),
('B017','கற்று கொடுத்தான்','கற்றுக் கொடுத்தான்','HARD-PART-HARDU','vanthodar_participle'),
('B018','நன்றாக சொன்னான்','நன்றாகச் சொன்னான்','HARD-PART-AG','aaga_adverb'),
('B019','ஓடா குதிரை','ஓடாக் குதிரை','HARD-NEG-ELLIP','clipped_negative_relative_participle'),
('B020','காணா பொருள்','காணாப் பொருள்','HARD-NEG-ELLIP','clipped_negative_relative_participle'),
('B021','செல்லா காசு','செல்லாக் காசு','HARD-NEG-ELLIP','clipped_negative_relative_participle'),
('B022','அறிக்கையை சமர்ப்பித்தான்','அறிக்கையைச் சமர்ப்பித்தான்','CASE-002-OVERT','explicit_accusative'),
('B023','கொலையை செய்தான்','கொலையைச் செய்தான்','CASE-002-OVERT','explicit_accusative'),
('B024','கர்த்தருக்கு சொன்னான்','கர்த்தருக்குச் சொன்னான்','CASE-004-OVERT','explicit_dative'),
]
for id,raw,expected,rule,rel in positive:
 tc(id,raw,expected,rule,{'syntactic_relation':rel,'case_marker':{'explicit_accusative':'ஐ','explicit_dative':'கு'}.get(rel)},'suggest',False,
    'Suggest only when parser annotations satisfy rule; never claim high confidence from suffix alone.')
# exact compound derivations: use lexicon not a free text correction
morph=[
('C001','மணி','ஓசை','மணியோசை','GLIDE-001'),
('C002','வாழை','இலை','வாழையிலை','GLIDE-003'),
('C003','பூ','இதழ்','பூவிதழ்','GLIDE-005'),
('C004','வண்டு','இனம்','வண்டினம்','U-007'),
('C005','மரம்','கிளை','மரக்கிளை','M-001'),
('C006','மரம்','வேர்','மரவேர்','M-003'),
('C007','கல்','அழகு','கல்லழகு','C-002'),
('C008','நல்','நூல்','நன்னூல்','LR-003'),
('C009','வடக்கு','கிழக்கு','வடகிழக்கு','LEX-DIR-NORTH'),
('C010','தெற்கு','மேற்கு','தென்மேற்கு','LEX-DIR-SOUTH'),
('C011','மேற்கு','நாடு','மேல்நாடு','LEX-DIR-WEST'),
('C012','கிழக்கு','நாடு','கீழ்நாடு','LEX-DIR-EAST'),
('C013','மூன்று','தமிழ்','முத்தமிழ்','LEX-NUM-3'),
('C014','நான்கு','படை','நாற்படை','LEX-NUM-4'),
('C015','பத்து','இரண்டு','பன்னிரண்டு','LEX-NUM-10'),
('C016','மா','காய்','மாங்காய்','LEX-TREE-MA'),
('C017','தெங்கு','காய்','தேங்காய்','LEX-TREE-THENGU'),
('C018','அகம்','கை','அங்கை','M-006'),
]
for id,left,right,expected,rule in morph:
 cases.append({'id':id,'operation':'join_approved_compound','left':left,'right':right,'expected_compound':expected,'expected_rule_ids':[rule],
 'parser_annotations':{'boundary_is_compound_or_suffix':True,'lexicon_approved':True},'expected_action':'review_only','must_not_change_automatically':True,'notes':'Use as lexicon and morphological analysis regression, not free-word joining.'})
# USFM and structural edge cases
for id,inp,description in [
('D001',r'\v 1 கர்த்தர் சொன்னார்.','Preserve verse marker and wording'),
('D002',r'\v 2 அவனை தேவசாயலாக படைத்தார்.','House style: protect divine family boundary; other boundaries independently reviewable'),
('D003',r'\v 3 அவர் சொன்னார்: “வா, மகனே.”','Do not insert across quotation/punctuation'),
('D004',r'\v 4 அவரை \f + \ft அடிக்குறிப்பு \f* பார்த்தான்.','Footnote creates nonadjacent source boundary'),
('D005',r'\v 5 \w கர்த்தர்|lemma="x"\w* சொன்னார்.','Preserve word attribute marker'),
('D006','யெகோவா காப்பார்','Unicode and divine-name spelling preserved'),
('D007','அவனை தேவசாயலாக','House style no inserted த்'),
('D008','இது புத்தகம். அது சிறந்தது.','Two independent sentences'),
]:
 cases.append({'id':id,'operation':'roundtrip_guard','input_text':inp,'expected_output_bytes_identical':True,
 'expected_action':'none','must_not_change_automatically':True,'notes':description})
(p/'tests.jsonl').write_text(''.join(json.dumps(c,ensure_ascii=False)+'\n' for c in cases),encoding='utf-8')
(p/'test_summary.json').write_text(json.dumps({'total_tests':len(cases),'protected_tests':len(negative),'suggestion_tests':len(positive),'morphology_tests':len(morph),'structural_tests':8},indent=2)+'\n')
print('tests',len(cases))
