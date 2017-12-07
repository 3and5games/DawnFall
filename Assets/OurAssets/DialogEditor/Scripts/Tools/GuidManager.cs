using System;
using System.Collections.Generic;
using UnityEngine;

    namespace Dialoges
    {
        public class GuidManager
        {
            private static List<PathGame> games;
            private static List<PathGame> Games
            {
                get
                {
                    if (games == null)
                    {
                        games = new List<PathGame>();
                        foreach (PathGame pg in Resources.FindObjectsOfTypeAll<PathGame>())
                        {
                            games.Add(pg);
                        }
                    }
                    return games;
                }
            }


            public static int GetItemGUID()
            {
                int r = UnityEngine.Random.Range(0, Int32.MaxValue);


                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Param p in inspectedgame.parameters)
                    {
                        if (p.paramGUID == r)
                        {
                            return GetItemGUID();
                        }
                    }
                }
                return r;
            }
            public static PathGame GetGameByParam(Param param)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Param p in inspectedgame.parameters)
                    {
                        if (p.paramGUID == param.paramGUID)
                        {
                            return inspectedgame;
                        }
                    }
                }
                return null;
            }

            public static int GetStateGuid()
            {
                int r = UnityEngine.Random.Range(0, Int32.MaxValue);

                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Chain c in inspectedgame.chains)
                    {
                        foreach (State s in c.states)
                        {
                            if (s.guid == r)
                            {
                                return GetStateGuid();
                            }
                        }
                    }
                }
                return r;
            }

            public static PathGame GetGameByState(State state)
            {
                return GetGameByChain(GetChainByState(state));
            }

            public static State GetStateByGuid(int guid)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Chain c in inspectedgame.chains)
                    {
                        foreach (State s in c.states)
                        {
                            if (s.Guid == guid)
                            {
                                return s;
                            }
                        }
                    }
                }
                return null;
            }

            public static PathGame GetGameByChain(Chain personChain)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Chain c in inspectedgame.chains)
                    {
                        if (personChain == c)
                        {
                            return inspectedgame;
                        }
                    }
                }
                return null;
            }
            public static PathGame GetGameByPath(Path p)
            {
                foreach (PathGame game in Games)
                {
                    foreach (Chain chain in game.chains)
                    {
                        foreach (State s in chain.states)
                        {
                            if (s.pathes.Contains(p))
                            {
                                return game;
                            }
                        }
                    }
                }
                return null;
            }


            public static Param GetItemByGuid(int aimParamGuid)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Param p in inspectedgame.parameters)
                    {
                        if (p.paramGUID == aimParamGuid)
                        {
                            return p;
                        }
                    }
                }
                return null;
            }
            public static Chain GetChainByState(State state)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Chain c in inspectedgame.chains)
                    {
                        foreach (State s in c.states)
                        {
                            if (state == s)
                            {
                                return c;
                            }
                        }
                    }
                }
                return null;
            }
            public static State GetStateByPath(Path activeObject)
            {
                foreach (PathGame inspectedgame in Games)
                {
                    foreach (Chain c in inspectedgame.chains)
                    {
                        foreach (State s in c.states)
                        {
                            foreach (Path p in s.pathes)
                            {
                                if (p == activeObject)
                                {
                                    return s;
                                }
                            }
                        }
                    }
                }
                return null;
            }

            public static Chain GetChainByPath(Path path)
            {
                foreach (PathGame game in games)
                {
                    foreach (Chain c in game.chains)
                    {
                        if (c.states.Contains(GetStateByPath(path)))
                        {
                            return c;
                        }
                    }
                }
                return null;
            }
        }
    }