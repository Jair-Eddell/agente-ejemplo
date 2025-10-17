# ...existing code...
from pathlib import Path
import re
import json
from datetime import datetime
import subprocess

class DocumentationAgent:
    def __init__(self):
        self.base_path = Path.cwd()
        self.output_path = self.base_path / "agent_test_output"
        self.output_path.mkdir(exist_ok=True)

    def parse_code_files(self):
        """Parse all code files and extract signatures"""
        signatures = []
        for ext in ['.cs', '.py', '.js']:
            for file in self.base_path.rglob(f'*{ext}'):
                if '.git' in str(file):
                    continue
                try:
                    print(f"Parsing {file}")
                    content = file.read_text(encoding='utf-8', errors='ignore')
                    if ext == '.cs':
                        signatures.extend(self._parse_csharp(file, content))
                    elif ext == '.py':
                        signatures.extend(self._parse_python(file, content))
                    elif ext == '.js':
                        signatures.extend(self._parse_javascript(file, content))
                except Exception as e:
                    print(f"Error parsing {file}: {e}")
        print(f"Total signatures found: {len(signatures)}")
        return signatures

    def _parse_csharp(self, file, content):
        """Extract C# classes and methods"""
        relative_path = file.relative_to(self.base_path)
        signatures = []
        
        # Parse classes
        class_matches = re.finditer(r'\bclass\s+([A-Za-z_][A-Za-z0-9_]*)', content)
        for match in class_matches:
            signatures.append({
                'file': str(relative_path),
                'type': 'class',
                'name': match.group(1),
                'language': 'C#'
            })

        # Parse methods (simple)
        method_matches = re.finditer(r'\b(?:public|private|protected|internal|static|virtual|override)\b[^{;()]*\b([A-Za-z_][A-Za-z0-9_]*)\s*\(', content)
        for match in method_matches:
            signatures.append({
                'file': str(relative_path),
                'type': 'method',
                'name': match.group(1),
                'language': 'C#'
            })
        
        return signatures

    # --- NUEVOS: parsers para Python y JS ---
    def _parse_python(self, file, content):
        relative_path = file.relative_to(self.base_path)
        signatures = []
        for m in re.finditer(r'^(?:async\s+)?(?:def|class)\s+([A-Za-z_][A-Za-z0-9_]*)', content, flags=re.M):
            kind = 'python_function' if content[m.start():m.start()+10].strip().startswith('def') else 'python_class'
            signatures.append({
                'file': str(relative_path),
                'type': kind,
                'name': m.group(1),
                'language': 'Python'
            })
        return signatures

    def _parse_javascript(self, file, content):
        relative_path = file.relative_to(self.base_path)
        signatures = []
        # function declarations and exported functions
        for a,b in re.findall(r'function\s+([A-Za-z_][A-Za-z0-9_]*)|export\s+function\s+([A-Za-z_][A-Za-z0-9_]*)', content):
            name = a or b
            if name:
                signatures.append({
                    'file': str(relative_path),
                    'type': 'javascript_function',
                    'name': name,
                    'language': 'JavaScript'
                })
        # arrow function exports (basic)
        for m in re.finditer(r'export\s+const\s+([A-Za-z_][A-Za-z0-9_]*)\s*=', content):
            signatures.append({
                'file': str(relative_path),
                'type': 'javascript_function',
                'name': m.group(1),
                'language': 'JavaScript'
            })
        return signatures
    # --- FIN parsers ---

    def get_git_changes(self):
        """Get recent git commits"""
        try:
            cmd = ['git', 'log', '--pretty=format:%H|%an|%ad|%s', '--date=iso', '-n', '50']
            result = subprocess.run(cmd, capture_output=True, text=True, cwd=self.base_path)
            if result.returncode != 0:
                print("git log failed:", result.stderr)
                return []
            commits = []
            for line in result.stdout.splitlines():
                parts = line.split('|', 3)
                if len(parts) < 4:
                    # línea inesperada: ignorar
                    continue
                hash, author, date, msg = parts
                commits.append({
                    'hash': hash[:7],
                    'author': author,
                    'date': date,
                    'message': msg
                })
            print(f"Commits found: {len(commits)}")
            return commits
        except Exception as e:
            print(f"Error getting git history: {e}")
            return []

    def generate_documentation(self):
        """Generate all documentation artifacts"""
        signatures = self.parse_code_files()
        commits = self.get_git_changes()
        
        # Generate TDD
        tdd_content = ['<?xml version="1.0" encoding="UTF-8"?>', '<TDD>']
        for sig in signatures:
            tdd_content.append(f'  <api file="{sig["file"]}" type="{sig["type"]}" name="{sig["name"]}" language="{sig["language"]}" />')
        tdd_content.append('</TDD>')
        (self.output_path / 'TDD.xml').write_text('\n'.join(tdd_content))

        # Generate Changelog
        changelog = [
            '# Changelog',
            f'Generated: {datetime.now().isoformat()}Z\n',
            '## Recent Changes\n'
        ]
        for commit in commits:
            changelog.append(f"- [{commit['hash']}] {commit['date']} - {commit['message']} (by {commit['author']})")
        (self.output_path / 'CHANGELOG.md').write_text('\n'.join(changelog))

        # Generate Release Notes
        release_notes = {
            'generated_at': datetime.now().isoformat(),
            'version': '1.0.0',
            'signatures_found': len(signatures),
            'commits_analyzed': len(commits),
            'api_changes': signatures,
            'recent_commits': commits
        }
        (self.output_path / 'release_notes.json').write_text(
            json.dumps(release_notes, indent=2)
        )

if __name__ == '__main__':
    agent = DocumentationAgent()
    agent.generate_documentation()
    print("Documentation generated successfully in ./agent_test_output/")
# ...existing code...