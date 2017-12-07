using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dialoges
{
    public class QuestWindow : EditorWindow
    {
        public enum EditorMode
        {
            packs,
            chains
        }

        #region private fields
        private static Vector2 lastMousePosition;
        private static Vector2 paramsScrollPosition = Vector2.zero;
        private static Vector2 chainsScrollPosition = Vector2.zero;
        private static Rect makingPathRect = new Rect(Vector2.one * 0.12345f, Vector2.one * 0.12345f);
        private static bool makingPath = false;
        private static Path startPath;
        private static State menuState;
        private static Path menuPath;
        private static Chain menuChain;
        private static Param menuParam;
        private static StateLink menuStateLink;
        private static Vector2 draggingVector;
        private static State draggingState;
        private static StateLink draggingStateLink;
        private static State debuggingState;
        private static UnityEngine.Object copyBuffer = new UnityEngine.Object();
        private static Texture2D backgroundTexture;
        private static float zoom
        {
            get
            {
                if (game)
                {
                    return game.zoom;
                }
                return 1;
            }
            set
            {
                if (game)
                {
                    game.zoom = value;
                }
            }
        }
        private static Texture2D BackgroundTexture
        {
            get
            {
                if (backgroundTexture == null)
                {
                    backgroundTexture = (Texture2D)Resources.Load("Icons/background") as Texture2D;
                    BackgroundTexture.wrapMode = TextureWrapMode.Repeat;
                }
                return backgroundTexture;
            }
        }
        private static GUISkin QuestCreatorSkin
        {
            get
            {
                return Resources.Load("Skins/QuestEditorSkin") as GUISkin;
            }
        }
        #endregion

        #region public fields
        public static PathGame game;
        public static Chain currentChain;
        public static EditorMode chainEditorMode = EditorMode.packs;
        public State DebuggingState
        {
            set
            {
                debuggingState = value;
                if (!currentChain.states.Contains(debuggingState))
                {
                    if (!game.chains.SelectMany(g => g.states).Contains(debuggingState))
                    {
                        foreach (PathGame pg in Resources.FindObjectsOfTypeAll<PathGame>())
                        {
                            if (pg.chains.SelectMany(g => g.states).Contains(debuggingState))
                            {
                                Init(pg);
                                Repaint();
                            }
                        }
                    }

                    foreach (Chain c in game.chains)
                    {
                        if (c.states.Contains(debuggingState))
                        {
                            currentChain = c;
                            RecalculateWindowsZoomPositions(0);
                            Repaint();
                        }
                    }
                }
            }
        }
        public string GamePath
        {
            get
            {
                return AssetDatabase.GetAssetPath(game.GetInstanceID());
            }
        }
        #endregion

        #region LifeCycle
        public static QuestWindow Init(PathGame editedGame = null)
        {
            game = editedGame;
            return Init();
        }
        static QuestWindow Init()
        {
            QuestWindow window = (QuestWindow)EditorWindow.GetWindow<QuestWindow>("Dialog creator", true, new Type[3] { typeof(Animator), typeof(Console), typeof(SceneView) });
            window.minSize = new Vector2(600, 400);
            window.ShowAuxWindow();
            paramsScrollPosition = Vector2.zero;
            currentChain = null;
            paramsScrollPosition = Vector2.zero;
            chainsScrollPosition = Vector2.zero;
            chainEditorMode = EditorMode.packs;
            makingPathRect = new Rect(Vector2.one * 0.12345f, Vector2.one * 0.12345f);
            makingPath = false;
            startPath = null;
            menuState = null;
            menuPath = null;
            menuChain = null;
            menuParam = null;
            menuStateLink = null;
            draggingVector = Vector2.zero;
            draggingState = null;
            draggingStateLink = null;
            debuggingState = null;
            copyBuffer = null;
            return window;
        }
        void OnGUI()
        {
            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        Repaint();
                        break;
                }

            }

            CopyPasteEvents();
            DeleteEvents();

            EditorMode newChainMode = (EditorMode)Tabs.DrawTabs(new Rect(0, 0, position.width, 30), new string[] { "Dialogs and parameters", "Node tree" }, (int)chainEditorMode);
            if (newChainMode == EditorMode.chains && newChainMode != chainEditorMode)
            {
                if (currentChain == null && game.chains.Count > 0)
                {
                    currentChain = game.chains[0];
                }
                if (currentChain != null)
                {
                    RecalculateWindowsPositions(currentChain.StartState.position.position);
                    RecalculateWindowsZoomPositions(0);
                }
            }

            chainEditorMode = newChainMode;


            switch (chainEditorMode)
            {
                case EditorMode.chains:
                    if (game && currentChain)
                    {
                        DrowChainsWindow();
                    }
                    break;
                case EditorMode.packs:
                    if (game)
                    {
                        DrowPacksWindow();
                    }
                    break;
            }


            if (game)
            {
                EditorUtility.SetDirty(game);
                if (game.Dirty)
                {
                    AssetDatabase.SaveAssets();
                    game.Dirty = false;
                }
            }

        }
        private void OnDestroy()
        {
            if (game)
            {
                AssetDatabase.SaveAssets();
            }
        }
        private void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }
        #endregion

        #region Debug
        public void DebugPathGame(State s)
        {
            if (FindObjectOfType<DialogPlayer>())
            {
                //FindObjectOfType<DialogPlayer>().onStateIn += DebugState;
            }
            DebugState(s);
        }
        void DebugState(State s)
        {
            currentChain = GuidManager.GetChainByState(s);
            chainEditorMode = EditorMode.chains;
            Repaint();
            DebuggingState = s;
        }
        #endregion

        #region Draw
        void DrowChainsWindow()
        {
            if (currentChain == null)
            {
                foreach (Chain c in game.chains)
                {
                    currentChain = c;
                    RecalculateWindowsZoomPositions(0);
                }
                if (currentChain == null)
                {
                    return;
                }
            }

            Rect fieldRect = new Rect(0, 30, position.width, position.height);
            GUI.DrawTextureWithTexCoords(fieldRect, BackgroundTexture, new Rect(0, 0, fieldRect.width / BackgroundTexture.width, fieldRect.height / BackgroundTexture.height));

            if (Event.current.type == EventType.ScrollWheel)
            {
                if (zoom > 0.2f && zoom < 1.5f)
                {
                    RecalculateWindowsZoomPositions(Event.current.delta.y * 0.01f);
                }
                zoom += Event.current.delta.y * 0.01f;
                zoom = Mathf.Clamp(zoom, 0.2f, 1.5f);
                Repaint();
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
            {
                lastMousePosition = Event.current.mousePosition;
            }

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                Vector2 delta = Event.current.mousePosition - lastMousePosition;
                foreach (State s in currentChain.states)
                {
                    s.position = new Rect(s.position.position + delta, s.position.size);
                }
                foreach (StateLink s in currentChain.links)
                {
                    s.position = new Rect(s.position.position + delta, s.position.size);
                }
                lastMousePosition = Event.current.mousePosition;
                Repaint();
            }

            DrawAditional();

            BeginWindows();

            if (makingPath && !makingPathRect.Contains(Event.current.mousePosition))
            {
                Undo.RecordObject(startPath, "set path aim");
                startPath.aimState = null;
            }


            State upperState = null;
            StateLink upperLink = null;

            foreach (State state in currentChain.states)
            {
                if (DrawStateBox(state))
                {
                    upperState = state;
                }
            }

            foreach (StateLink link in currentChain.links)
            {
                if (DrawStateLinkBox(link))
                {
                    upperLink = link;
                }
            }

            if (upperState)
            {
                currentChain.states.Remove(upperState);
                currentChain.states.Insert(currentChain.states.Count, upperState);
            }

            if (upperLink)
            {
                currentChain.links.Remove(upperLink);
                currentChain.links.Insert(currentChain.links.Count, upperLink);
            }
            EndWindows();
            ChainsWindowEvents();
        }
        void DrowPacksWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            ChainListEvents();
            DrawChainsList();
            DrawParamsList();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        void DrawParamsList()
        {
            paramsScrollPosition = GUILayout.BeginScrollView(paramsScrollPosition, false, true, GUILayout.Width(position.width / 2 - 5), GUILayout.Height(position.height - 20));
            GUILayout.BeginVertical();
            Param selectedParam = null;
            if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Param))
            {
                selectedParam = (Param)Selection.activeObject;
            }

            List<string> paramsNames = game.parameters.Select(x => x.name).ToList();
            int index = -1;
            if (game.parameters.Contains(selectedParam))
            {
                index = game.parameters.IndexOf(selectedParam);
            }

            GUILayout.Space(30 * game.parameters.Count);
            int paramIndex = Tabs.DrawTabs(new Rect(0, 0, position.width / 2 - 20, 30 * game.parameters.Count), paramsNames.ToArray(), index, true);
            if (paramIndex >= 0)
            {
                Selection.activeObject = game.parameters[paramIndex];
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        void DrawChainsList()
        {
            chainsScrollPosition = GUILayout.BeginScrollView(chainsScrollPosition, false, true, GUILayout.Width(position.width / 2 - 5), GUILayout.Height(position.height - 20));
            GUILayout.BeginVertical();
            if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Chain))
            {
                if (currentChain != (Chain)Selection.activeObject)
                {
                    currentChain = (Chain)Selection.activeObject;
                    RecalculateWindowsZoomPositions(0);
                }
            }



            List<string> chainNames = game.chains.Select(x => x.name).ToList();
            int index = -1;
            if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Chain) && game.chains.Contains((Chain)Selection.activeObject))
            {
                index = game.chains.IndexOf((Chain)Selection.activeObject);
            }

            GUILayout.Space(30 * game.chains.Count);
            int chainIndex = Tabs.DrawTabs(new Rect(0, 0, position.width / 2 - 20, 30 * game.chains.Count), chainNames.ToArray(), index, true);
            if (chainIndex >= 0)
            {
                Selection.activeObject = game.chains[chainIndex];
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        void DrawNodeCurve(Rect start, Rect end, Color c)
        {
            float force = 1;
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            float distanceY = Mathf.Abs(startPos.y - endPos.y);
            float distanceX = Mathf.Abs(startPos.x - endPos.x);
            Vector3 middlePoint = (startPos + endPos) / 2;

            Vector3 startTan1 = startPos;
            Vector3 endTan2 = endPos;
            Vector3 startTan2 = middlePoint;
            Vector3 endTan1 = middlePoint;

            if (startPos.y > endPos.y)
            {
                startTan1 -= Vector3.down * 150;
                endTan2 -= Vector3.up * 150;
                if (startPos.y > endPos.y)
                {
                    endTan1 += Vector3.up * Mathf.Max(distanceY, 50);
                    startTan2 -= Vector3.up * Mathf.Max(distanceY, 50);
                }
                else
                {
                    endTan1 += Vector3.down * Mathf.Max(distanceY, 50);
                    startTan2 -= Vector3.down * Mathf.Max(distanceY, 50);
                }
            }
            else
            {
                startTan1 -= distanceY * Vector3.down / force / 2;
                endTan2 -= distanceY * Vector3.up / force / 2;
                if (startPos.x > endPos.x)
                {
                    endTan1 += distanceX * Vector3.right / force / 2;
                    startTan2 -= distanceX * Vector3.right / force / 2;
                }
                else
                {
                    endTan1 += distanceX * Vector3.left / force / 2;
                    startTan2 -= distanceX * Vector3.left / force / 2;
                }
            }

            Color shadowCol = new Color(0, 0, 0, 0.06f);

            // Draw a shadow
            for (int i = 0; i < 2; i++)
            {
                Handles.DrawBezier(startPos, middlePoint, startTan1, endTan1, shadowCol, null, (i + 1) * 7);
            }
            Handles.DrawBezier(startPos, middlePoint, startTan1, endTan1, c, null, 3);

            for (int i = 0; i < 2; i++)
            {
                Handles.DrawBezier(middlePoint, endPos, startTan2, endTan2, shadowCol, null, (i + 1) * 7);
            }
            Handles.DrawBezier(middlePoint, endPos, startTan2, endTan2, c, null, 3);
        }
        void DrawAditional()
        {

            if (makingPath)
            {
                Handles.BeginGUI();
                Handles.color = Color.white;
                DrawNodeCurve(makingPathRect, new Rect(Event.current.mousePosition, Vector2.zero), Color.white);

                Handles.EndGUI();
                Repaint();
            }

            foreach (State state in currentChain.states)
            {
                int i = 0;
                foreach (Path path in state.pathes)
                {
                    if (path == null)
                    {
                        state.pathes.Remove(path);
                        continue;
                    }
                    if (path.aimState != null && path.aimStateGuid != -1)
                    {
                        Handles.BeginGUI();
                        Handles.color = Color.red;

                        Rect end = path.aimState.position;
                        if (currentChain.links.Select(x => x.state).Contains(path.aimState))
                        {
                            end = currentChain.links.Find(x => x.state == path.aimState).position;
                        }

                        Rect start = new Rect(state.position.x + state.position.size.x * 0.2f + 16 * zoom * i, state.position.y + state.position.height, 15 * zoom, 15 * zoom);
                        DrawNodeCurve(start, end, Color.gray);
                        Handles.EndGUI();
                    }
                    i++;
                }
            }
        }
        bool DrawStateLinkBox(StateLink state)
        {
            bool moving = false;
            //GUI.Box(_boxPos, state.description);

            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && state.position.Contains(Event.current.mousePosition))
            {
                moving = true;
                Selection.activeObject = state;
                draggingStateLink = state;
                draggingVector = state.position.position - Event.current.mousePosition;
                Repaint();
            }

            if (Event.current.type == EventType.mouseDrag)
            {
                if (draggingStateLink == state)
                {
                    state.position = new Rect(draggingVector + Event.current.mousePosition, state.position.size);
                    Repaint();
                }
            }

            if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
            {
                draggingStateLink = null;
            }


            GUI.backgroundColor = Color.cyan * 0.8f;


            if (Selection.activeObject == state)
            {
                GUI.backgroundColor = GUI.backgroundColor * 1.3f;
            }

            if (state.position.Contains(Event.current.mousePosition) && makingPath == true)
            {
                GUI.backgroundColor = Color.yellow;
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && state.state)
                {
                    Undo.RecordObject(startPath, "set path aim");
                    startPath.aimState = state.state;
                    makingPath = false;
                    Repaint();
                }
            }

            GUIStyle s = new GUIStyle(GUI.skin.box);
            s.fontSize = Mathf.FloorToInt(15 * zoom);
            s.border = new RectOffset(15, 15, 5, 5);
            s.normal.background = (Texture2D)Resources.Load("Icons/button") as Texture2D;
            GUI.Box(state.position, state.name, s);

            Event c = Event.current;

            GUI.backgroundColor = Color.white;
            return moving;
        }
        bool DrawStateBox(State state)
        {
            bool moving = false;


            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && state.position.Contains(Event.current.mousePosition))
            {
                moving = true;
                Selection.activeObject = state;
                draggingState = state;
                draggingVector = state.position.position - Event.current.mousePosition;
                Repaint();
            }

            if (Event.current.type == EventType.mouseDrag)
            {
                if (draggingState == state)
                {
                    state.position = new Rect(draggingVector + Event.current.mousePosition, state.position.size);
                    Repaint();
                }
            }

            if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
            {
                draggingState = null;
            }

            if (currentChain.StartState == state)
            {
                GUI.backgroundColor = Color.green * 0.8f;
            }
            else
            {
                GUI.backgroundColor = Color.white * 0.8f;
            }

            if (Selection.activeObject == state)
            {
                GUI.backgroundColor = GUI.backgroundColor * 1.3f;
            }

            if (debuggingState == state)
            {
                GUI.backgroundColor = Color.red * 0.8f;
            }

            if (state.position.Contains(Event.current.mousePosition) && makingPath == true)
            {
                GUI.backgroundColor = Color.yellow;
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                {
                    Undo.RecordObject(startPath, "set path aim");
                    startPath.aimState = state;
                    makingPath = false;
                    Repaint();
                }
            }
            GUIStyle s = new GUIStyle(GUI.skin.box);
            s.fontSize = Mathf.FloorToInt(15 * zoom);
            s.border = new RectOffset(15, 15, 5, 5);
            s.normal.background = (Texture2D)Resources.Load("Icons/button") as Texture2D;
            GUI.Box(state.position, state.name, s);

            int i = 0;

            Event c = Event.current;

            foreach (Path path in state.pathes)
            {
                Rect r = new Rect(state.position.x + state.position.size.x * 0.2f + 16 * zoom * i, state.position.y + state.position.height, 15 * zoom, 15 * zoom);
                GUI.backgroundColor = Color.white * 0.8f;
                if (Selection.activeObject == path)
                {
                    GUI.backgroundColor = Color.white;
                }

                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.normal.background = (Texture2D)Resources.Load("Icons/path") as Texture2D;

                GUI.Box(r, "", style);

                if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    Selection.activeObject = path;
                    Repaint();
                }

                if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
                {
                    makingPath = true;
                    makingPathRect = r;
                    startPath = path;
                }

                if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
                {
                    if (makingPath)
                    {
                        Selection.activeObject = path;
                    }
                    makingPath = false;
                }
                i++;
            }

            GUI.backgroundColor = Color.white;
            return moving;
        }
        #endregion

        #region Create
        private Path CreatePath(State s)
        {
            Undo.RecordObject(s, "path creattion");
            Path newPath = s.AddPath();
            s.position = new Rect(s.position.position, new Vector2(Mathf.Max(208, +s.position.size.x * 0.4f + 16 * s.pathes.Count), 30) * zoom);
            Undo.RegisterCreatedObjectUndo(newPath, "param creation");
            Selection.activeObject = newPath;
            AssetDatabase.AddObjectToAsset(newPath, AssetDatabase.GetAssetPath(game));
            return newPath;
        }
        private StateLink CreateStateLink(Chain c)
        {
            Undo.RecordObject(c, "state link creattion");
            StateLink newStateLink = c.AddStateLink();
            Undo.RegisterCreatedObjectUndo(newStateLink, "param creation");
            AssetDatabase.AddObjectToAsset(newStateLink, AssetDatabase.GetAssetPath(game));
            newStateLink.position = new Rect(lastMousePosition.x - newStateLink.position.width / 2, lastMousePosition.y - newStateLink.position.height / 2, newStateLink.position.width, newStateLink.position.height);
            Selection.activeObject = newStateLink;
            return newStateLink;
        }
        private State CreateState(Chain c)
        {
            Undo.RecordObject(c, "state creattion");
            State newState = c.AddState();
            Undo.RegisterCreatedObjectUndo(newState, "state creation");
            AssetDatabase.AddObjectToAsset(newState, AssetDatabase.GetAssetPath(game));
            newState.position = new Rect(lastMousePosition.x - newState.position.width / 2, lastMousePosition.y - newState.position.height / 2, newState.position.width, newState.position.height);
            Selection.activeObject = newState;
            return newState;
        }
        private Param CreateParam()
        {
            Undo.RecordObject(game, "param creattion");
            Param newParam = CreateInstance<Param>();
            Undo.RegisterCreatedObjectUndo(newParam, "param creation");
            game.parameters.Add(newParam);
            newParam.Game = game;
            newParam.paramName = "new param";
            newParam.paramGUID = GuidManager.GetItemGUID();
            AssetDatabase.AddObjectToAsset(newParam, AssetDatabase.GetAssetPath(game));
            game.Dirty = true;
            Selection.activeObject = newParam;
            return newParam;
        }
        private Chain CreateChain(bool withStartState = true)
        {
            Undo.RecordObject(game, "chain creation");
            Chain newChain = CreateInstance<Chain>();
            Undo.RegisterCreatedObjectUndo(newChain, "chain creation");
            AssetDatabase.AddObjectToAsset(newChain, AssetDatabase.GetAssetPath(game));
            newChain.Init(game);
            if (withStartState)
            {
                newChain.StartState = newChain.AddState();
                AssetDatabase.AddObjectToAsset(newChain.StartState, AssetDatabase.GetAssetPath(newChain));
            }
            Selection.activeObject = newChain;
            Repaint();
            return newChain;
        }
        #endregion

        #region Delete
        private void RemoveParam(Param deletingParam)
        {
            Undo.RecordObject(game, "removing param");
            if (copyBuffer == deletingParam)
            {
                copyBuffer = null;
            }
            game.parameters.Remove(deletingParam);
            Undo.DestroyObjectImmediate(deletingParam);
            game.Dirty = true;
            Repaint();
        }
        private void RemoveChain(Chain deletingChain)
        {
            if (copyBuffer == deletingChain)
            {
                copyBuffer = null;
            }

            for (int i = 0; i < deletingChain.links.Count; i++)
            {
                RemoveStateLink(deletingChain, deletingChain.links[i]);
            }


            for (int i = 0; i < deletingChain.states.Count; i++)
            {
                RemoveState(deletingChain, deletingChain.states[i], false);
            }

            game.chains.Remove(deletingChain);
            Undo.DestroyObjectImmediate(deletingChain);
            game.Dirty = true;
            Repaint();
        }
        private void RemovePath(State s, Path p)
        {
            Undo.RecordObject(s, "remove path");
            if (copyBuffer == p)
            {
                copyBuffer = null;
            }

            s.RemovePath(p);

            Undo.DestroyObjectImmediate(p);
            s.position = new Rect(s.position.position, new Vector2(Mathf.Max(208, +s.position.size.x / zoom * 0.4f + 16 * s.pathes.Count), 30) * zoom);
            Repaint();
            return;
        }
        private void RemoveState(Chain chain, State state, bool WithCreatingStart = true)
        {
            Undo.RecordObject(chain, "remove state");

            if (copyBuffer == state)
            {
                copyBuffer = null;
            }
            foreach (State s in chain.states)
            {
                foreach (Path p in s.pathes)
                {
                    if (p.aimState == state)
                    {
                        p.aimState = null;
                    }
                }
            }

            if (state == chain.StartState && WithCreatingStart)
            {
                if (chain.states.Count == 1)
                {
                    chain.StartState = chain.AddState();
                    AssetDatabase.AddObjectToAsset(chain.StartState, AssetDatabase.GetAssetPath(game));
                }
                else
                {
                    foreach (State s in chain.states)
                    {
                        if (s != state)
                        {
                            chain.StartState = s;
                            break;
                        }
                    }
                }
            }
            int pc = state.pathes.Count;
            for (int i = pc - 1; i >= 0; i--)
            {
                RemovePath(state, state.pathes[i]);
            }
            chain.RemoveState(state);
            Undo.DestroyObjectImmediate(state);
            game.Dirty = true;
            Repaint();
        }
        private void RemoveStateLink(Chain chain, StateLink link)
        {
            Undo.RecordObject(chain, "remove state Link");
            if (copyBuffer == link)
            {
                copyBuffer = null;
            }
            foreach (State s in chain.states)
            {
                foreach (Path p in s.pathes)
                {
                    if (p.aimState == link.state)
                    {
                        p.aimState = null;
                    }
                }
            }
            chain.RemoveStateLink(link);
            Undo.DestroyObjectImmediate(link);
            game.Dirty = true;
            Repaint();
        }
        #endregion

        #region Paste
        private Chain PasteChain(Chain copied)
        {
            Chain c = CreateChain(false);
            PasteChainValues(copied, c);
            return c;
        }
        private Param PasteParam(Param copied)
        {
            Param p = CreateParam();
            PasteParamValues(copied, p);
            return p;
        }
        private StateLink PasteStateLink(Chain chain, StateLink copied)
        {
            StateLink s = CreateStateLink(chain);
            PasteStateLinkValues(copied, s);
            return s;
        }
        private State PasteState(Chain chain, State copied, bool pastePosition = false)
        {
            State s = CreateState(chain);
            PasteStateValues(copied, s, pastePosition);
            if (!pastePosition)
            {
                s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
            }
            return s;
        }
        private Path PastePath(State aimState, Path copied)
        {
            Path pastedPath = CreatePath(aimState);
            PastePathValues(copied, pastedPath);
            return pastedPath;
        }
        #endregion

        #region PasteValues
        private void PasteStateLinkValues(StateLink from, StateLink to, bool pastePosition = false)
        {
            Rect p = to.position;
            EditorUtility.CopySerialized(from, to);
            if (!pastePosition)
            {
                to.position = p;
            }
            Selection.activeObject = to;
            Repaint();
        }
        private void PasteStateValues(State from, State to, bool pastePosition = false)
        {
            Rect position = to.position;
            List<Path> deletingPathes = new List<Path>(to.pathes);
            foreach (Path p in deletingPathes)
            {
                RemovePath(to, p);
            }

            EditorUtility.CopySerialized(from, to);
            List<Path> pathes = new List<Path>(from.pathes);

            to.pathes.Clear();
            foreach (Path p in pathes)
            {
                Path newPath = CreatePath(to);
                PastePathValues(p, newPath);
            }

            if (!pastePosition)
            {
                to.position = position;
            }

            if (from.Chain.StartState == from)
            {
                to.Chain.StartState = to;
            }
            Selection.activeObject = to;
            Repaint();
        }
        private void PastePathValues(Path from, Path to)
        {
            EditorUtility.CopySerialized(from, to);


            if (!currentChain.states.Contains(from.aimState) && to.aimState != null)
            {
                //to.aimState = GuidManager.GetChainByPath(to).states[GuidManager.GetChainByState(from.aimState).states.IndexOf(from.aimState)];

                StateLink sl = currentChain.links.Find(l => l.state);
                if (!sl)
                {
                    sl = CreateStateLink(currentChain);
                    sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
                    sl.chain = GuidManager.GetChainByState(to.aimState);
                    sl.state = to.aimState;
                }
            }
            Selection.activeObject = to;
            Repaint();
        }
        private void PasteChainValues(Chain from, Chain to)
        {
            List<StateLink> removingLinks = to.links;
            List<State> removingStates = to.states;
            foreach (StateLink sl in removingLinks)
            {
                RemoveStateLink(to, sl);
            }

            foreach (State s in removingStates)
            {
                RemoveState(to, s, false);
            }

            EditorUtility.CopySerialized(from, to);

            to.states.Clear();

            Dictionary<State, State> fromToList = new Dictionary<State, State>();

            foreach (State s in from.states)
            {
                fromToList.Add(s, PasteState(to, s, true));
            }

            to.links.Clear();
            foreach (StateLink s in from.links)
            {
                PasteStateLink(to, s);
            }
            foreach (State s in to.states)
            {
                foreach (Path p in s.pathes)
                {
                    if (p.aimState != null && fromToList.ContainsKey(p.aimState))
                    {
                        if (!to.links.Find(l => l.state == p.aimState))
                        {
                            p.aimState = fromToList[p.aimState];
                        }
                    }

                }
            }
            Repaint();
        }
        private void PasteParamValues(Param from, Param to)
        {
            Undo.RecordObject(to, "paste param values");
            EditorUtility.CopySerialized(from, to);
            Selection.activeObject = to;
            Repaint();
        }
        #endregion

        #region MenuEvents
        private void EditChain()
        {
            currentChain = menuChain;
            RecalculateWindowsZoomPositions(0);
            chainEditorMode = EditorMode.chains;
        }
        private void MakeStart()
        {
            Undo.RecordObject(currentChain, "set start state");
            currentChain.StartState = menuState;
        }
        private void CreateNewParam()
        {
            CreateParam();
        }
        private void CreateNewPath()
        {
            CreatePath(menuState);
        }
        private void CreateNewState()
        {
            CreateState(currentChain);
        }
        private void CreateNewStateLink()
        {
            CreateStateLink(currentChain);
        }
        private void CreateNewChain()
        {
            CreateChain();
        }
        private void DeleteState()
        {
            RemoveState(currentChain, menuState);
        }
        private void DeleteStateLink()
        {
            RemoveStateLink(currentChain, menuStateLink);
        }
        private void DeletePath()
        {
            foreach (State s in currentChain.states)
            {
                if (s.pathes.Contains(menuPath))
                {
                    RemovePath(s, menuPath);
                }
            }
        }
        private void DeleteChain()
        {
            RemoveChain(menuChain);
        }
        private void DeleteParam()
        {
            RemoveParam(menuParam);
        }
        private void CopyChain()
        {
            copyBuffer = menuChain;
            Debug.Log(menuChain.states.Count);
        }
        private void CopyParam()
        {
            copyBuffer = menuParam;
        }
        private void CopyPath()
        {
            copyBuffer = menuPath;
        }
        private void CopyState()
        {
            copyBuffer = menuState;
        }
        private void CopyStateLink()
        {
            copyBuffer = menuStateLink;
        }
        private void PastePath()
        {
            PastePath(menuState, (Path)copyBuffer);
        }
        private void PasteChain()
        {
            PasteChain((Chain)copyBuffer);
        }
        private void PasteParam()
        {
            PasteParam((Param)copyBuffer);
        }
        private void PasteState()
        {
            PasteState(currentChain, (State)copyBuffer);
        }
        private void PasteStateLink()
        {
            PasteStateLink(currentChain, (StateLink)copyBuffer);
        }
        private void PasteChainValues()
        {
            Undo.RecordObject(menuChain, "paste chain values");
            PasteChainValues((Chain)copyBuffer, menuChain);
        }
        private void PasteParamValues()
        {
            PasteParamValues((Param)copyBuffer, menuParam);
        }
        private void PasteStateLinkValues()
        {
            Undo.RecordObject(menuStateLink, "paste link values");
            PasteStateLinkValues((StateLink)copyBuffer, menuStateLink);
        }
        private void PasteStateValues()
        {
            Undo.RecordObject(menuState, "paste state values");
            PasteStateValues((State)copyBuffer, menuState);
        }
        private void PastePathValues()
        {
            Undo.RecordObject(menuPath, "paste path values");
            PastePathValues((Path)copyBuffer, menuPath);
        }
        #endregion

        #region Tools
        private void RecalculateWindowsPositions(Vector2 center)
        {
            if (currentChain != null)
            {
                Vector2 delta;

                delta = position.center - center;

                foreach (State s in currentChain.states)
                {
                    s.position = new Rect(s.position.position + delta, s.position.size);
                }
                foreach (StateLink s in currentChain.links)
                {
                    s.position = new Rect(s.position.position + delta, s.position.size);
                }
            }
        }
        private void RecalculateWindowsZoomPositions(float force)
        {
            if (currentChain != null)
            {
                foreach (State s in currentChain.states)
                {
                    s.position = new Rect((s.position.position - Event.current.mousePosition) * (1 + force) + Event.current.mousePosition, new Vector2(Mathf.Max(208, +s.position.size.x / zoom * 0.4f + 16 * s.pathes.Count), 30) * zoom);
                }
                foreach (StateLink s in currentChain.links)
                {
                    s.position = new Rect((s.position.position - Event.current.mousePosition) * (1 + force) + Event.current.mousePosition, new Vector2(208, 30) * zoom);
                }
            }
        }
        private void DeleteEvents()
        {
            if (Event.current.isKey && (Event.current.keyCode == KeyCode.Delete) || (Event.current.command && Event.current.keyCode == KeyCode.Backspace) && Selection.activeObject)
            {
                if (!Selection.activeObject)
                {
                    return;
                }

                if (Selection.activeObject.GetType() == typeof(State))
                {
                    RemoveState(currentChain, (State)Selection.activeObject);
                    return;
                }
                if (Selection.activeObject.GetType() == typeof(StateLink))
                {
                    RemoveStateLink(currentChain, (StateLink)Selection.activeObject);
                    return;
                }
                if (Selection.activeObject.GetType() == typeof(Path))
                {
                    foreach (State s in currentChain.states)
                    {
                        if (s.pathes.Contains((Path)Selection.activeObject))
                        {
                            RemovePath(s, (Path)Selection.activeObject);
                        }
                    }
                    return;
                }
                if (Selection.activeObject.GetType() == typeof(Param))
                {
                    RemoveParam((Param)Selection.activeObject);
                    return;
                }
                if (Selection.activeObject.GetType() == typeof(Chain))
                {
                    RemoveChain((Chain)Selection.activeObject);
                }
            }
        }
        private void CopyPasteEvents()
        {
            if (Event.current.control && Event.current.isKey)
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    copyBuffer = Selection.activeObject;
                }

                if (chainEditorMode == EditorMode.chains)
                {
                    if (copyBuffer && copyBuffer.GetType() == typeof(State))
                    {
                        if (Event.current.keyCode == KeyCode.V)
                        {
                            lastMousePosition = Event.current.mousePosition;
                            PasteState();
                        }
                    }

                    if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
                    {
                        if (Event.current.keyCode == KeyCode.V)
                        {
                            lastMousePosition = Event.current.mousePosition;
                            PasteStateLink();
                        }
                    }

                    if (copyBuffer && copyBuffer.GetType() == typeof(Path))
                    {
                        if (Event.current.keyCode == KeyCode.V)
                        {
                            if (Selection.activeObject.GetType() == typeof(State))
                            {
                                menuState = (State)Selection.activeObject;
                                PastePath();
                            }
                        }
                    }
                }

                if (chainEditorMode == EditorMode.packs)
                {
                    if (copyBuffer && copyBuffer.GetType() == typeof(Param))
                    {
                        if (Event.current.keyCode == KeyCode.V)
                        {
                            PasteParam((Param)copyBuffer);
                        }
                    }

                    if (copyBuffer && copyBuffer.GetType() == typeof(Chain))
                    {
                        if (Event.current.keyCode == KeyCode.V)
                        {
                            PasteChain((Chain)copyBuffer);
                        }
                    }
                }
            }
        }
        private void ChainListEvents()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < game.chains.Count; i++)
                {
                    if (new Rect(5, 30 * (i + 1), -30 + position.width / 2, (30 * game.chains.Count) / game.chains.Count).Contains(Event.current.mousePosition))
                    {
                        menuChain = game.chains[i];
                        menu.AddItem(new GUIContent("Add chain"), false, CreateNewChain);
                        menu.AddItem(new GUIContent("Edit"), false, EditChain);
                        menu.AddItem(new GUIContent("Remove"), false, DeleteChain);
                        menu.AddItem(new GUIContent("Copy"), false, CopyChain);
                        menu.AddItem(new GUIContent("Edit"), false, EditChain);
                        if (copyBuffer && copyBuffer.GetType() == typeof(Chain))
                        {
                            //menu.AddItem(new GUIContent("Paste values"), false, PasteChainValues);
                            menu.AddItem(new GUIContent("Paste chain"), false, PasteChain);
                        }
                        menu.ShowAsContext();
                        return;
                    }
                }

                for (int i = 0; i < game.parameters.Count; i++)
                {
                    if (new Rect(position.width / 2, 30 * (1 + i), -5 + position.width / 2 - 20, (30 * game.parameters.Count) / game.parameters.Count).Contains(Event.current.mousePosition))
                    {
                        menuParam = game.parameters[i];
                        menu.AddItem(new GUIContent("Add param"), false, CreateNewParam);
                        menu.AddItem(new GUIContent("Remove"), false, DeleteParam);
                        menu.AddItem(new GUIContent("Copy"), false, CopyParam);
                        if (copyBuffer && copyBuffer.GetType() == typeof(Param))
                        {
                            //menu.AddItem(new GUIContent("Paste values"), false, PasteParamValues);
                            menu.AddItem(new GUIContent("Paste param"), false, PasteParam);
                        }
                        menu.ShowAsContext();

                        return;
                    }
                }

                menu.AddItem(new GUIContent("Add chain"), false, CreateNewChain);
                menu.AddItem(new GUIContent("Add param"), false, CreateNewParam);

                if (copyBuffer && copyBuffer.GetType() == typeof(Chain))
                {
                    menu.AddItem(new GUIContent("Paste chain"), false, PasteChain);

                }
                if (copyBuffer && copyBuffer.GetType() == typeof(Param))
                {
                    menu.AddItem(new GUIContent("Paste param"), false, PasteParam);
                }
                menu.ShowAsContext();
                return;


            }
        }
        private void ChainsWindowEvents()
        {
            Event evt = Event.current;

            if (evt.button == 1 && evt.type == EventType.MouseDown)
            {
                State onState = null;
                Path onPath = null;
                StateLink onStateLink = null;
                foreach (State s in currentChain.states)
                {
                    int i = 0;
                    foreach (Path p in s.pathes)
                    {
                        if (new Rect(s.position.x + 16 * zoom * i, s.position.y + s.position.height, 15 * zoom, 15 * zoom).Contains(evt.mousePosition))
                        {
                            onPath = p;
                        }
                        i++;
                    }

                    if (s.position.Contains(evt.mousePosition))
                    {
                        onState = s;
                    }
                }

                foreach (StateLink s in currentChain.links)
                {
                    if (s.position.Contains(evt.mousePosition))
                    {
                        onStateLink = s;
                    }
                }
                if (onPath)
                {
                    menuPath = onPath;
                    lastMousePosition = Event.current.mousePosition;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Remove"), false, DeletePath);
                    menu.AddItem(new GUIContent("Copy"), false, CopyPath);
                    if (copyBuffer && copyBuffer.GetType() == typeof(Path))
                    {
                        menu.AddItem(new GUIContent("Paste values"), false, PastePathValues);
                    }
                    menu.ShowAsContext();
                }
                else if (onState)
                {
                    menuState = onState;
                    lastMousePosition = Event.current.mousePosition;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Remove state"), false, DeleteState);
                    menu.AddItem(new GUIContent("Add path"), false, CreateNewPath);
                    menu.AddItem(new GUIContent("Make start"), false, MakeStart);
                    menu.AddItem(new GUIContent("Copy"), false, CopyState);
                    if (copyBuffer && copyBuffer.GetType() == typeof(Path))
                    {
                        menu.AddItem(new GUIContent("Paste path"), false, PastePath);
                    }
                    if (copyBuffer && copyBuffer.GetType() == typeof(State))
                    {
                        //menu.AddItem(new GUIContent("Paste values"), false, PasteStateValues);
                    }
                    menu.ShowAsContext();
                }
                else if (onStateLink)
                {
                    menuStateLink = onStateLink;
                    lastMousePosition = Event.current.mousePosition;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Remove"), false, DeleteStateLink);
                    menu.AddItem(new GUIContent("Copy"), false, CopyStateLink);
                    if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
                    {
                        menu.AddItem(new GUIContent("Paste values"), false, PasteStateLinkValues);
                    }
                    menu.ShowAsContext();
                }

                else
                {
                    lastMousePosition = evt.mousePosition;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add state"), false, CreateNewState);
                    menu.AddItem(new GUIContent("Add state link"), false, CreateNewStateLink);
                    if (copyBuffer && copyBuffer.GetType() == typeof(State))
                    {
                        menu.AddItem(new GUIContent("Paste state"), false, PasteState);
                    }
                    if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
                    {
                        menu.AddItem(new GUIContent("Paste state link"), false, PasteStateLink);
                    }
                    menu.ShowAsContext();
                }
            }

            if (evt.button == 0 && evt.type == EventType.MouseUp)
            {
                makingPath = false;
            }
        }
        private void UndoRedoPerformed()
        {
            AssetDatabase.SaveAssets();
            game.Dirty = false;
            Repaint();
        }
        #endregion
    }
}