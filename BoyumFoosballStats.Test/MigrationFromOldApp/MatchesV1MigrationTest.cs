﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using BoyumFoosballStats.Cosmos.Services;
using BoyumFoosballStats.Services;
using BoyumFoosballStats.Shared.Extensions;
using BoyumFoosballStats.Shared.Models;
using CosmosDb.Model;
using Microsoft.Extensions.Options;
using Moserware.Skills;
using Newtonsoft.Json;
using Xunit;

namespace BoyumFoosballStats.Test.MigrationFromOldApp;

public class MatchesV1MigrationTest
{
    [Fact]
    public async void MigrateMatchesToFirestore()
    {
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var cosmosSettings = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var cosmosOptions = Options.Create(cosmosSettings);
        var blobSettings = new BlobStorageOptions
        {
            BlobUrl = secretClient.GetSecret("BlobStorageConnectionString").Value.Value,
            ContainerName = "BoyumFoosballStats"
        };
        var blobOptions = Options.Create(blobSettings);
        var blobHelper = new AzureBlobStorageHelper(blobOptions);
        var matches = await blobHelper.GetEntriesAsync<Match>("matches.json");
        var matchController = new DebugMatchCrudService(cosmosOptions);
        var playerController = new DebugPlayerCrudService(cosmosOptions);
        var players = await playerController.GetAllAsync();

        if (matches != null)
        {
            foreach (var oldMatch in matches)
            {
                var blackAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Attacker!);
                var blackDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Defender!);
                var greyAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Attacker!);
                var greyDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Defender!);
                var migratedMatch = new Shared.DbModels.Match
                {
                    ScoreGrey = oldMatch.ScoreGray,
                    ScoreBlack = oldMatch.ScoreBlack,
                    BlackAttackerPlayer = blackAttackerPlayer,
                    BlackDefenderPlayer = blackDefenderPlayer,
                    GreyAttackerPlayer = greyAttackerPlayer,
                    GreyDefenderPlayer = greyDefenderPlayer,
                    MatchDate = oldMatch.MatchDate.ToUniversalTime(),
                    LegacyMatchId = oldMatch.Id
                };
                migratedMatch.IncrementMatchesPlayed();
                migratedMatch.UpdateTrueSkill();
                await matchController.CreateOrUpdateAsync(migratedMatch);
            }
        }

        foreach (var player in players)
        {
            await playerController.CreateOrUpdateAsync(player);
        }

        var test = matches;
    }

    [Fact]
    public async void RecalculateTrueSkillForAllMatches()
    {
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var cosmosSettings = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var cosmosOptions = Options.Create(cosmosSettings);
        var matchController = new MatchCrudService(cosmosOptions);
        var playerController = new PlayerCrudService(cosmosOptions);
        var matches = await matchController.GetAllAsync();
        var origPlayers = await playerController.GetAllAsync();
        var players = await playerController.GetAllAsync();
        foreach (var player in players)
        {
            var gameInfo = GameInfo.DefaultGameInfo;
            player.TrueSkillRating = new TrueSkillRating(gameInfo.DefaultRating);
            player.TrueSkillRatingAttacker = new TrueSkillRating(gameInfo.DefaultRating);
            player.TrueSkillRatingDefender = new TrueSkillRating(gameInfo.DefaultRating);
            player.SeasonalTrueSkill = new Dictionary<string, TrueSkillRatings?>();
            player.MatchesPlayed = 0;
            player.MatchesPlayedAttacker = 0;
            player.MatchesPlayedDefender = 0;
        }

        foreach (var match in matches)
        {
            match.BlackAttackerPlayer = players.Single(x => x.Id == match.BlackAttackerPlayer.Id);
            match.BlackDefenderPlayer = players.Single(x => x.Id == match.BlackDefenderPlayer.Id);
            match.GreyAttackerPlayer = players.Single(x => x.Id == match.GreyAttackerPlayer.Id);
            match.GreyDefenderPlayer = players.Single(x => x.Id == match.GreyDefenderPlayer.Id);
            match.IncrementMatchesPlayed();
            match.UpdateTrueSkill();
            await playerController.CreateOrUpdateAsync(new List<BoyumFoosballStats.Shared.DbModels.Player>
            {
                match.BlackDefenderPlayer!, match.BlackAttackerPlayer!, match.GreyAttackerPlayer!, match.GreyDefenderPlayer!
            });
        }

        var updatedPlayers = await playerController.GetAllAsync();
        foreach (var updatedPlayer in updatedPlayers)
        {
            var origPlayer = origPlayers.Single(x => x.Id == updatedPlayer.Id);
            if (updatedPlayer.TrueSkillRating.Mean != origPlayer.TrueSkillRating.Mean)
            {
                var test = 1;
            }

            if (updatedPlayer.MatchesPlayed != origPlayer.MatchesPlayed)
            {
                var test = 1;
            }
        }
        
    }
}

public class Match
{
    public Match()
    {
        Black = new Team(TableSide.Black);
        Gray = new Team(TableSide.Grey);
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }

    public Team Black { get; set; }
    public Team Gray { get; set; }

    public int ScoreBlack { get; set; }

    public int ScoreGray { get; set; }

    public DateTime MatchDate { get; set; }
}

public class Team : TeamBase
{
    public Team(TableSide side)
    {
        Side = side;
    }

    public TableSide Side { get; set; }
}

public class TeamBase
{
    public Player? Attacker { get; set; }

    public Player? Defender { get; set; }

    [JsonIgnore] public List<Player?> Players => new List<Player?>() { Attacker, Defender };

    [JsonIgnore] public string TeamIdentifier => $"{Attacker} {Defender}";
    [JsonIgnore] public string TeamIdentifierSwapped => $"{Defender} {Attacker}";
}

public enum TableSide
{
    Black = 0,
    Grey = 1
}