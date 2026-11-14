"""Regression checks for the standalone evaluator; uses synthetic USFM only.

Run after evaluate-reference.ps1 has compiled artifacts/build/ReferenceBenchmark.exe.
"""
import json
import subprocess
import uuid
from pathlib import Path

root = Path(__file__).resolve().parents[1]
codes = ('GEN EXO LEV NUM DEU JOS JDG RUT 1SA 2SA 1KI 2KI 1CH 2CH EZR NEH EST '
         'JOB PSA PRO ECC SNG ISA JER LAM EZK DAN HOS JOL AMO OBA JON MIC NAM HAB '
         'ZEP HAG ZEC MAL MAT MRK LUK JHN ACT ROM 1CO 2CO GAL EPH PHP COL 1TH 2TH '
         '1TI 2TI TIT PHM HEB JAS 1PE 2PE 1JN 2JN 3JN JUD REV').split()
build = root / 'artifacts/build'
def main():
    work = root / 'artifacts' / ('reference-identity-' + uuid.uuid4().hex)
    work.mkdir()
    source = work / 'source'
    source.mkdir()
    for code in codes:
        # Deliberately misleading prefixes: never use filenames for book identity.
        (source / f'99-{code}.usfm').write_text(f'\\id {code}\n\\c 1\n\\v 1 Example.\n', encoding='utf-8')

    def run(stage, label):
        return subprocess.run([str(build / 'ReferenceBenchmark.exe'), str(source),
                               str(work / label), stage, str(build / 'rules.json'),
                               'Synthetic identity regression'], capture_output=True, text=True)

    for stage, expected in [('development', 4), ('holdout', 62)]:
        result = run(stage, stage)
        assert result.returncode == 0, result.stderr
        report = json.loads(result.stdout)
        assert report['books'] == expected and report['chapters'] == expected
        assert report['allInputFilesUnchanged']
        assert report['reference'] == 'Synthetic identity regression'
    (source / '99-REV.usfm').write_text('\\id GEN\n\\c 1\n\\v 1 Example.\n', encoding='utf-8')
    result = run('development', 'duplicate')
    assert result.returncode != 0 and 'duplicate USFM book identity' in result.stderr
    (source / '99-REV.usfm').unlink()
    result = run('holdout', 'missing')
    assert result.returncode != 0 and 'all 66 canonical' in result.stderr
    (source / '99-REV.usfm').write_text('\\id XYZ\n\\c 1\n\\v 1 Example.\n', encoding='utf-8')
    result = run('development', 'unknown')
    assert result.returncode != 0 and 'Invalid or duplicate USFM book identity' in result.stderr
if __name__ == '__main__':
    main()
    print('Reference identity: both canonical splits, source preservation, duplicate/missing/unknown refusal passed.')
