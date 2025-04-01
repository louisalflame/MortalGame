using System;
using System.Collections.Generic;
using UnityEngine;

public interface IReactionSession
{
    void Update(GameStatus gameStatus, GameContext context);
}

[Serializable]
public abstract class ReactionSession : IReactionSession
{
    public string Id;

    public Dictionary<string, ISessionValue> Values;

    public abstract void Update(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class WholeGameSession : ReactionSession
{
    public override void Update(GameStatus gameStatus, GameContext context) { }
}

[Serializable]
public class WhileTurnSession : ReactionSession
{
    public override void Update(GameStatus gameStatus, GameContext context) { }
}

[Serializable]
public class TurnSession : ReactionSession
{
    public override void Update(GameStatus gameStatus, GameContext context) { }
}

[Serializable]
public class CardSession : ReactionSession
{
    public override void Update(GameStatus gameStatus, GameContext context) { }
}
