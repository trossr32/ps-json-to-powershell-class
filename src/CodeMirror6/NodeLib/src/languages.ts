import type { Extension } from "@codemirror/state";
import { css } from "@codemirror/lang-css";
import { html } from "@codemirror/lang-html";
import { javascript } from "@codemirror/lang-javascript";
import { json } from "@codemirror/lang-json";
import { markdown } from "@codemirror/lang-markdown";
import { python } from "@codemirror/lang-python";
import { sql } from "@codemirror/lang-sql";
import { rust } from "@codemirror/lang-rust";
import { xml } from "@codemirror/lang-xml";

/* Legacy languages */
import { powerShell } from "@codemirror/legacy-modes/mode/powershell";
import { shader, cpp, c, dart, java, kotlin } from "@codemirror/legacy-modes/mode/clike";
import { StreamLanguage } from "@codemirror/language";

export function codeMirrorLanguageExtension(language?: string): Extension | undefined {
    let lang = language || "";
    lang = lang.toLowerCase();

    switch (lang) {
        case "javascript":
        case "js":
            return javascript();

        case "typescript":
        case "ts":
            return javascript({ typescript: true });

        case "jsx":
            return javascript({ jsx: true });

        case "tsx":
            return javascript({ typescript: true, jsx: true });

        case "python":
        case "py":
            return python();

        case "css":
            return css();

        case "html":
            return html();

        case "markdown":
        case "md":
            return markdown();

        case "xml":
            return xml();

        case "json":
            return json();

        case "sql":
            return sql();

        case "rust":
            return rust();

        case "powershell":
        case "ps":
        case "ps1":
            return StreamLanguage.define(powerShell);

        case "shader":
        case "glsl":
        case "opengl":
            return StreamLanguage.define(shader);

        case "cpp":
            return StreamLanguage.define(cpp);

        case "java":
            return StreamLanguage.define(java);

        case "kotlin":
            return StreamLanguage.define(kotlin);

        case "c":
            return StreamLanguage.define(c);

        case "dart":
            return StreamLanguage.define(dart);

        default:
            // Handle any other cases or return a default value
            break;
    }
}