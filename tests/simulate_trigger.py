import os
import re
import json
from pathlib import Path
from datetime import datetime

# Ruta base (ajusta si tu workspace está en otra ubicación)
BASE = Path(r"c:\Users\SPARTAN PC\Desktop\curso\orquestador mini")
SRC = BASE / "sample_repo" / "src"
OUT = BASE / "agent_test_output"
SRC.mkdir(parents=True, exist_ok=True)
OUT.mkdir(parents=True, exist_ok=True)

# --- Crear archivos de ejemplo ---
api_py = SRC / "api.py"
api_py.write_text("""\
\"\"\"Módulo de ejemplo - API\"\"\"
def get_user(user_id):
    \"\"\"Devuelve datos de usuario\"\"\"
    pass

class OrderService:
    def create_order(data):
        \"\"\"Crea una orden\"\"\"
        pass
""")

utils_js = SRC / "utils.js"
utils_js.write_text("""\
// Módulo JS de ejemplo
function formatDate(d) {
  return d.toISOString();
}

export function computeTotal(items) {
  // calcula total
}
""")

# --- Simular commits desde último release ---
commits = [
    {"id": "c1a2", "author": "dev1", "message": "feat(api): add get_user", "files": ["src/api.py"], "timestamp": "2025-10-16T10:00:00"},
    {"id": "c1a3", "author": "dev2", "message": "refactor(order): OrderService.create_order", "files": ["src/api.py"], "timestamp": "2025-10-16T11:00:00"},
    {"id": "c1a4", "author": "dev3", "message": "feat(ui): add formatting", "files": ["src/utils.js"], "timestamp": "2025-10-16T12:00:00"},
]

# --- Parser simple para extraer API signatures ---
def parse_py(path):
    text = path.read_text(encoding="utf-8")
    funcs = re.findall(r'^(def|class)\\s+([A-Za-z_0-9]+)', text, flags=re.M)
    signatures = []
    for kind, name in funcs:
        signatures.append({"file": str(path.relative_to(BASE)), "kind": kind, "name": name})
    return signatures

def parse_js(path):
    text = path.read_text(encoding="utf-8")
    funcs = re.findall(r'function\\s+([A-Za-z_0-9]+)|export\\s+function\\s+([A-Za-z_0-9]+)', text)
    signatures = []
    for f1, f2 in funcs:
        name = f1 or f2
        if name:
            signatures.append({"file": str(path.relative_to(BASE)), "kind": "function", "name": name})
    return signatures

signatures = []
for f in SRC.glob("**/*"):
    if f.suffix == ".py":
        signatures += parse_py(f)
    if f.suffix == ".js":
        signatures += parse_js(f)

# --- Generar TDD (XML simple) ---
tdd_path = OUT / "TDD_actualizado.xml"
tdd_entries = []
for s in signatures:
    tdd_entries.append(f'  <api file="{s["file"]}" kind="{s["kind"]}" name="{s["name"]}" />')
tdd_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<TDD>\n" + "\n".join(tdd_entries) + "\n</TDD>\n"
tdd_path.write_text(tdd_xml, encoding="utf-8")

# --- Generar Manual de Integración (XML simple) ---
manual_path = OUT / "Manual_Integracion.xml"
manual_entries = []
for s in signatures:
    manual_entries.append(f'  <endpoint file="{s["file"]}" name="{s["name"]}"><description>Autogenerado</description></endpoint>')
manual_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Manual>\n" + "\n".join(manual_entries) + "\n</Manual>\n"
manual_path.write_text(manual_xml, encoding="utf-8")

# --- Generar Changelog.md (cronológico) ---
changelog_path = OUT / "CHANGELOG.md"
lines = ["# Changelog\n", f"Generado: {datetime.utcnow().isoformat()}Z\n"]
for c in sorted(commits, key=lambda x: x["timestamp"]):
    lines.append(f"- {c['timestamp']} [{c['id']}] {c['author']}: {c['message']} (files: {', '.join(c['files'])})")
changelog_path.write_text("\n".join(lines), encoding="utf-8")

# --- Generar Release Notes (JSON) ---
release = {
    "generated_at": datetime.utcnow().isoformat() + "Z",
    "commit_count": len(commits),
    "commits": commits,
    "api_changes": [s for s in signatures],
}
release_path = OUT / "release_notes.json"
release_path.write_text(json.dumps(release, indent=2, ensure_ascii=False), encoding="utf-8")

# --- Resultado resumen ---
print("Simulación completada.")
print(f"Archivos creados en: {OUT}")
print("Salidas:")
for p in OUT.iterdir():
    print(" -", p.name)
# Código de salida 0 indica éxito