using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        User user = _context.Users.FirstOrDefault(x => x.Email == email) ?? throw new Exception("Invalid Token");

        Match match = _context.Matches.FirstOrDefault(m => m.MatchId == betRequest.MatchId) ?? throw new Exception("Match not founded");

        if (match.MatchFinished) throw new Exception("Match finished");

        Team team = _context.Teams.FirstOrDefault(t => t.TeamId == betRequest.TeamId) ?? throw new Exception("Team not founded");

        if (match.MatchTeamAId != team.TeamId && match.MatchTeamBId != team.TeamId) throw new Exception("Team is not in this match");

        Bet bet = new()
        {
            UserId = user.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue
        };

        EntityEntry<Bet> newBet = _context.Bets.Add(bet);
        _context.SaveChanges();

        BetDTOResponse response = new()
        {
            BetId = newBet.Entity.BetId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue,
            MatchDate = match.MatchDate,
            TeamName = team.TeamName,
            Email = email
        };

        return response;
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        Bet bet = _context.Bets
                  .Include(b => b.Team)
                  .Include(b => b.Match)
                  .Include(b => b.User)
                  .FirstOrDefault(b => b.BetId == BetId) ?? throw new Exception("Bet not founded");

        if (bet.User!.Email != email) throw new Exception("Bet view not allowed");

        return new BetDTOResponse
        {
            BetId = bet.BetId,
            MatchId = bet.MatchId,
            TeamId = bet.TeamId,
            BetValue = bet.BetValue,
            MatchDate = bet.Match!.MatchDate,
            TeamName = bet.Team!.TeamName,
            Email = bet.User!.Email
        };
    }
}