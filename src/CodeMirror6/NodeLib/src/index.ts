import { basicSetup } from "codemirror";
import { EditorView, keymap, placeholder } from "@codemirror/view";
import { EditorState, Compartment } from "@codemirror/state";
import { StreamLanguage } from "@codemirror/language";
import { powerShell } from "@codemirror/legacy-modes/mode/powershell";
import { indentWithTab } from "@codemirror/commands";
import { autocompletion } from "@codemirror/autocomplete";
import { CmInstance } from "./CmInstance";
import { codeMirrorLanguageExtension } from "./languages";
import { basicDark } from "cm6-theme-basic-dark"
import { basicLight } from "cm6-theme-basic-light";

let CMInstances: { [id: string]: CmInstance } = {};

export function initCodeMirror(
    dotnetHelper: any,
    id: string,
    initialText: string,
    placeholderText: string,
    tabulationSize: number,
    languageTarget: string
) {
    var language = new Compartment;
    var tabSize = new Compartment;
    var themeConfig = new Compartment;

    let theme = window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? basicLight : basicDark;

    var state = EditorState.create({
        doc: initialText,
        extensions: [
            basicSetup,
            StreamLanguage.define(powerShell),
            language.of(codeMirrorLanguageExtension(languageTarget)),
            tabSize.of(EditorState.tabSize.of(tabulationSize)),
            keymap.of([indentWithTab]),
            EditorView.updateListener.of(async (update) => {
                if (update.docChanged) {
                    await dotnetHelper.invokeMethodAsync("DocChanged", update.state.doc.toString());
                }
                if (update.focusChanged) {
                    await dotnetHelper.invokeMethodAsync("FocusChanged", update.view.hasFocus);
                    if (!update.view.hasFocus)
                        await dotnetHelper.invokeMethodAsync("DocChanged", update.state.doc.toString());
                }
                if (update.selectionSet) {
                    await dotnetHelper.invokeMethodAsync("SelectionSet", update.state.selection.ranges.map(r => { return { from: r.from, to: r.to } }));
                }
            }),
            placeholder(placeholderText),
            autocompletion(),
            themeConfig.of(theme)
        ]
    });

    var view = new EditorView({
        state,
        parent: document.getElementById(id)
    });

    CMInstances[id] = new CmInstance();

    CMInstances[id].dotNetHelper = dotnetHelper;
    CMInstances[id].state = state;
    CMInstances[id].view = view;
    CMInstances[id].tabSize = tabSize;
    CMInstances[id].language = language;
    CMInstances[id].themeConfig = themeConfig;
}

export function setTabSize(id: string, size: number) {
    CMInstances[id].view.dispatch({
        effects: CMInstances[id].tabSize.reconfigure(EditorState.tabSize.of(size))
    });
}

export function setText(id: string, text: string) {
    const transaction = CMInstances[id].view.state.update({        
        changes: { from: 0, to: CMInstances[id].view.state.doc.length, insert: text }
    });

    CMInstances[id].view.dispatch(transaction);
}

export function dispose(id: string)
{
    CMInstances[id] = undefined;
}