﻿using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Creatures
{
    public enum CreatureEnvironment
    {
        None,
        Space,
        Interior,
        Land,
        Liquid,
        Air
    }

    public class Creature
    {
        private IServiceProvider serviceProvider;
        private ulong channelId;

        public List<string> BasicForm { get; set; } = new List<string>();
        public CreatureEnvironment Environment { get; set; }
        public string EncounteredBehavior { get; set; }
        public List<string> FirstLook { get; set; } = new List<string>();
        public List<string> RevealedAspectsList { get; set; } = new List<string>();
        public string IconUrl { get; private set; }
        public string Scale { get; set; }

        public Creature(IServiceProvider serviceProvider, ulong channelId)
        {
            this.serviceProvider = serviceProvider;
            this.channelId = channelId;
        }

        public static Creature GenerateNewCreature(IServiceProvider serviceProvider, ulong ChannelId, CreatureEnvironment environment)
        {
            var creature = new Creature(serviceProvider, ChannelId);
            OracleService oracles = serviceProvider.GetRequiredService<OracleService>();
            Random rnd = BotRandom.Instance;

            if (environment == CreatureEnvironment.None) Enum.TryParse(oracles.RandomRow("Creature Environment", GameName.Starforged, rnd).Description, out environment);

            creature.BasicForm.AddRandomOracleRow($"Basic Form {environment}", GameName.Starforged, serviceProvider);
            creature.Environment = environment;
            creature.EncounteredBehavior = oracles.RandomOracleResult("Creature Behavior", serviceProvider, GameName.Starforged, rnd);

            int firstLookCount = 2; // max as of sept 3 2021
            for (int i = 0; i < firstLookCount; i++)
            {
                creature.FirstLook.AddRandomOracleRow("Creature First Look", GameName.Starforged, serviceProvider, ChannelId, rnd);
            }

            creature.Scale = oracles.RandomOracleResult($"Creature Scale", serviceProvider, GameName.Starforged, rnd);

            return creature;
        }

        public static Creature FromEmbed(IEmbed embed, IServiceProvider serviceProvider, ulong ChannelId)
        {
            var creature = new Creature(serviceProvider, ChannelId);
            creature.Environment = StarforgedUtilites.GetAnyEnvironment(embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.Environment).Value);
            creature.Scale = embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.Scale).Value;
            creature.BasicForm = embed.Fields.Where(fld => fld.Name == CreatureResources.BasicForm || fld.Name == "BasicForm").Select(e => e.Value).ToList(); //todo: BasicForm is for an old typo, and can be removed in the future
            creature.FirstLook = embed.Fields.Where(fld => fld.Name == CreatureResources.FirstLook).Select(fld => fld.Value).ToList();
            creature.EncounteredBehavior = embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.EncounteredBehavior).Value;
            creature.RevealedAspectsList = embed.Fields.Where(fld => fld.Name == CreatureResources.RevealedAspect).Select(fld => fld.Value).ToList();
            creature.IconUrl = embed.Thumbnail?.Url;

            return creature;
        }

        public Creature AddRandomAspect()
        {
            this.RevealedAspectsList.AddRandomOracleRow("Creature Revealed Aspect", GameName.Starforged, serviceProvider, channelId);
            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            var builder = new EmbedBuilder().WithTitle(CreatureResources.CreatureTitle);
            builder.WithThumbnailUrl(IconUrl);

            builder.AddField(CreatureResources.Environment, Environment.ToString(), true);
            builder.AddField(CreatureResources.Scale, Scale, true);
            foreach (var form in BasicForm) builder.AddField(CreatureResources.BasicForm, form, true);
            foreach (var s in FirstLook) builder.AddField(CreatureResources.FirstLook, s, true);
            builder.AddField(CreatureResources.EncounteredBehavior, EncounteredBehavior, true);
            for (int i = 0; i < RevealedAspectsList.Count; i++) builder.AddField(CreatureResources.RevealedAspect, RevealedAspectsList[i], true);

            return builder;
        }
    }
}