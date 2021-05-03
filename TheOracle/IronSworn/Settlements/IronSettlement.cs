﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.SettlementGenerator;

namespace TheOracle.IronSworn.Settlements
{
    public class IronSettlement : ISettlement
    {
        private IServiceProvider Services;
        private OracleService oracles;
        private Emoji troubleEmoji = new Emoji("🔥");

        public List<string> SettlementTrouble { get; set; } = new List<string>();

        public IronSettlement(IServiceProvider services, ulong channelId)
        {
            Services = services;
            oracles = services.GetRequiredService<OracleService>();
            var hooks = services.GetRequiredService<HookedEvents>();
            if (!hooks.IronSettlmentReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent trouble = new ReactionEventBuilder().WithEmote(troubleEmoji).WithEvent(TroubleReactionHandler).Build();
                reactionService.reactionList.Add(trouble);

                hooks.IronSettlmentReactions = true;
            }
        }

        public string Name { get; set; }
        public string IconUrl { get; private set; }
        public object LocationDesc { get; private set; }

        public async Task AfterMessageCreated(IUserMessage msg)
        {
            await msg.AddReactionAsync(troubleEmoji);
            return;
        }

        public ISettlement FromEmbed(IEmbed embed)
        {
            Name = embed.Title.Replace("__", string.Empty);
            IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : null;
            LocationDesc = embed.Description.Replace(SettlementResources.Settlement, string.Empty).Trim();

            foreach (var trouble in embed.Fields.Where(fld => fld.Name == SettlementResources.SettlementTrouble))
            {
                SettlementTrouble.Add(trouble.Value);
            }

            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithThumbnailUrl(IconUrl)
                .WithDescription($"{LocationDesc} {SettlementResources.Settlement}")
                .WithFooter(GameName.Ironsworn.ToString())
                ;

            foreach (var trouble in SettlementTrouble)
            {
                embedBuilder.AddField(SettlementResources.SettlementTrouble, trouble);
            }

            return embedBuilder;
        }

        public ISettlement SetupFromUserOptions(string options)
        {
            OracleRoller roller = new OracleRoller(Services, GameName.Ironsworn);

            Name = (options.Length > 0) ? options : roller.BuildRollResults("Settlement Name").RollResultList.Last().Result.Description;
            
            LocationDesc = oracles.RandomOracleResult("Location Descriptors", Services, GameName.Ironsworn);

            return this;
        }

        private bool IsIronSettlementPost(IUserMessage message)
        {
            var embed = message?.Embeds?.FirstOrDefault();
            if (embed == default) return false;

            if (!embed.Description.Contains(SettlementResources.Settlement)) return false;

            if (embed.Footer.HasValue && embed.Footer.Value.Text.Equals(GameName.Ironsworn.ToString())) return true;

            return false;
        }

        private async Task TroubleReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronSettlementPost(message)) return;

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return;

            var settlement = new IronSettlement(Services, channel.Id).FromEmbed(settlmentEmbed) as IronSettlement;

            settlement.RevealTrouble();

            await message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        private void RevealTrouble()
        {
            SettlementTrouble.Add(oracles.RandomOracleResult("Settlement Trouble", Services, GameName.Ironsworn));
        }
    }
}