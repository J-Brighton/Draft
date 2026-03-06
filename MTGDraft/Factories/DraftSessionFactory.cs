using MTGDraft.Models;
using MTGDraft.Enums;
using MTGDraft.PackGeneration;

public static class DraftSessionFactory
{
    public static DraftSession Create(Set set, int playerCount)
    {
        var session = new DraftSession
        {
            SetCode = set.Code,
            PlayerCount = playerCount,
            DraftState = DraftState.NotStarted,
            CreatedAt = DateTime.Now,
            Packs = [] // make packs empty initially, instead generate when draft starts
        };

        return session;
    }
}