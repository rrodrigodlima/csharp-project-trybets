using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        decimal betValue = decimal.Parse(BetValue.Replace(",", "."), CultureInfo.InvariantCulture);

        Match foundMatch = _context.Matches.FirstOrDefault(match => match.MatchId == MatchId) ?? throw new Exception("Match not found");
        if (foundMatch.MatchTeamAId != TeamId && foundMatch.MatchTeamBId != TeamId)
        {
            throw new Exception("Team is not in this match");
        }

        if (TeamId == foundMatch.MatchTeamAId)
        {
            foundMatch.MatchTeamAValue += betValue;
        }
        else
        {
            foundMatch.MatchTeamBValue += betValue;
        }

        return foundMatch;
    }
}